using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EnergyApp.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;
using MQTTnet.Diagnostics;
using MQTTnet.Protocol;

namespace powerconcern.mqtt.services
{
    public class MQTTService: BackgroundService
    {
        //private readonly DbContextOptions<ApplicationDbContext> _dbContextOptions;
        private readonly IServiceProvider _serviceProvider;

        public ILogger Logger { get; }
        public MqttFactory Factory { get; }

        public IMqttClient MqttClnt {get; }
        
        public IMqttClientOptions options;
        public float[] fMeanCurrent;
        public float fMaxCurrent=16;

        //automatically passes the logger factory in to the constructor via dependency injection
        public MQTTService(ILoggerFactory loggerFactory, IServiceProvider serviceProvider)
        {
            //Get serviceprovider so we later can connect to databasemodel from it.
            _serviceProvider=serviceProvider;
            fMeanCurrent=new float[4];
            
            string sBrokerURL="";

            Factory=new MqttFactory();

            MqttClnt=Factory.CreateMqttClient();

            Logger = loggerFactory?.CreateLogger("MQTTSvc");
            if(Logger == null)
            {
                throw new ArgumentNullException(nameof(loggerFactory));
            }

            using(var scope = _serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetService<ApplicationDbContext>();

                //var test=dbContext.Configurations.Select(c=>c.Key).ToList();
                sBrokerURL=dbContext.Configurations.First(c=>c.Key.Equals("BrokerURL")).Value;
                Logger.LogInformation($"BrokerURL:{sBrokerURL}");
            }

            MqttNetGlobalLogger.LogMessagePublished += OnTraceMessagePublished;
            options = new MqttClientOptionsBuilder()
            .WithClientId(Guid.NewGuid().ToString())
            .WithTcpServer(sBrokerURL)
            .WithCredentials("fredrik:fredrik", "aivUGL6no")
            .WithCleanSession()
            .Build();

            //var result = MqttClnt.ConnectAsync(options);
            #region UseConnectedHandler
            MqttClnt.UseConnectedHandler(async e =>
            {
                Console.WriteLine("### CONNECTED WITH SERVER ###");

                // Subscribe to a topic
                await MqttClnt.SubscribeAsync(new TopicFilterBuilder().WithTopic("CurrentMeter/#").Build());
                await MqttClnt.SubscribeAsync(new TopicFilterBuilder().WithTopic("TEVCharger/#").Build());

                Console.WriteLine("### SUBSCRIBED ###");
            });
            #endregion

            MqttClnt.UseDisconnectedHandler(async e =>
            {
                Console.WriteLine("### DISCONNECTED FROM SERVER ###");
                await Task.Delay(TimeSpan.FromSeconds(5));
                if(fMeanCurrent[1]>0) {
                    try
                    {
                        await MqttClnt.ConnectAsync(options);
                    }
                    catch
                    {
                        Console.WriteLine("### RECONNECTING FAILED ###");
                    }
                }
            });

            MqttClnt.UseApplicationMessageReceivedHandler(e =>
            {
                //Console.WriteLine("### RECEIVED APPLICATION MESSAGE ###");
                
                string logstr=$"{DateTime.Now} {e.ApplicationMessage.Topic} \t {Encoding.UTF8.GetString(e.ApplicationMessage.Payload)}";
                //Logger.LogInformation(logstr);
                Console.WriteLine(logstr);
                if(e.ApplicationMessage.Topic.Contains("TEVCharger/status/current")) {
                    
                }
                if(e.ApplicationMessage.Topic.Contains("current1d")) {
                    //Save PNG to file
                    
                    Stream s=new FileStream("1d.png",FileMode.Create);
                    var bw=new BinaryWriter(s);
                    bw.Write(e.ApplicationMessage.Payload);
                    //bw.Flush();
                    bw.Close();
                }
                if(e.ApplicationMessage.Topic.Contains("current_l")) {
                    float fCurrent=ToFloat(e.ApplicationMessage.Payload);
                    int iPhase=Int16.Parse(e.ApplicationMessage.Topic.Substring(e.ApplicationMessage.Topic.Length-1));
                    fMeanCurrent[iPhase]=(4 * fMeanCurrent[iPhase]+fCurrent)/5;
                    Logger.LogInformation($"Phase: {iPhase}; Current: {fCurrent}; Mean Current: {fMeanCurrent[iPhase]}");

                    if(fMeanCurrent[iPhase]>fMaxCurrent) {
                        float fNewChargeCurrent=fMeanCurrent[iPhase]-fMaxCurrent;
                        string sNewChargeCurrent=fNewChargeCurrent.ToString();
                        Logger.LogInformation($"Holy Moses, too much power! Adjusting to {sNewChargeCurrent}");
                        MqttClnt.PublishAsync("TestCharger/set/current",
                                    sNewChargeCurrent,
                                    MqttQualityOfServiceLevel.AtLeastOnce,
                                    false);
                    }
                }
            });

            Logger.LogInformation("MQTTService created");
        }

        private float ToFloat(byte[] bArray) {
            string s=System.Text.Encoding.UTF8.GetString(bArray);
            float f=float.Parse(s.Replace('.',','));
            return f;
        }
        private void OnTraceMessagePublished(object sender, MqttNetLogMessagePublishedEventArgs e)
        {
            //Logger.LogTrace(e.TraceMessage.Message);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Logger.LogInformation("Background thread started");


            var result = await MqttClnt.ConnectAsync(options);
            /*
            var ctx = ServiceProvider.GetService<ApplicationDbContext>();
            using (var ctx = ServiceProvider.GetService<ApplicationDbContext>())
            {
                var test=ctx.Configurations.Select(c=>c.Key).ToList();
            }
            using (var ctx = new ApplicationDbContext(_dbContextOptions))
            {
                var test=ctx.Configurations.Select(c=>c.Key).ToList();
            }
 */

            Logger.LogInformation($"Connection result: {result.ResponseInformation}");

            stoppingToken.Register(() => Logger.LogDebug($" MQTTSvc background task is stopping."));


/* Python rules
    client.subscribe("EVCharger/#")
    client.subscribe("CurrentMeter/#")


    if msg.topic == "EVCharger/status/current":
        charge_current = float(msg.payload)
        print("Charge current %.1f A" % charge_current)
    if msg.topic == "CurrentMeter/status/current":
        current = float(msg.payload)
        mean_current = (9*mean_current + current) / 10
        print("Current %.1f (mean %.1f)" % (current, mean_current))
        if mean_current > max_current:
            new_charge_current = charge_current - (mean_current-max_current)
            client.publish("EVCharger/set/current", payload=str(int(new_charge_current)), qos=0, retain=False)
            if new_charge_current < 2:
                client.publish("EVCharger/set/enable", payload=str(0), qos=0, retain=False)
        else:
            client.publish("EVCharger/set/current", payload=str(int(max_current)), qos=0, retain=False)
            client.publish("EVCharger/set/enable", payload=str(1), qos=0, retain=False)
 */
            //MqttClient client=(MqttClient)MqttClnt;

            //result = await MqttClnt.ConnectAsync(options);
            
/*            async Task Handler1(MqttApplicationMessageReceivedEventArgs e) 
            { 
//                await client1.PublishAsync($"reply/{eventArgs.ApplicationMessage.Topic}");
                string logstr=$"{DateTime.Now} {e.ApplicationMessage.Topic} \t {Encoding.UTF8.GetString(e.ApplicationMessage.Payload)}";
                //Logger.LogInformation(logstr);
                Console.WriteLine(logstr);
            } 
            
            client.UseApplicationMessageReceivedHandler(Handler1);
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken);

                Console.WriteLine("Background svc looping");

            }
*/

            Console.WriteLine("Background svc is stopping.");
            
            //return Task.CompletedTask;
        }
    }
}