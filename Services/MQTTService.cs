using System;
using System.Collections;
using System.Collections.Generic;
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
    public class ChargerInfo {
        public float fCurrent;
        public float[] fMeanCurrent;


    }
    public class MQTTService: BackgroundService
    {
        //private readonly DbContextOptions<ApplicationDbContext> _dbContextOptions;
        private readonly IServiceProvider _serviceProvider;
        private ApplicationDbContext dbContext;
        private List<Configuration> appConfig;
        public ILogger Logger { get; }
        public MqttFactory Factory { get; }
        public IMqttClient MqttClnt {get; }

        public IMqttClientOptions options;
        public float[] fMeanCurrent;
        public float fMaxCurrent;
        public float fChargeCurrent;
        public ChargerInfo 

        //automatically passes the logger factory in to the constructor via dependency injection
        public MQTTService(ILoggerFactory loggerFactory, IServiceProvider serviceProvider)
        {
            //Get serviceprovider so we later can connect to databasemodel from it.
            _serviceProvider=serviceProvider;
            fMeanCurrent=new float[4];
            
            string sBrokerURL, sBrokerUser, sBrokerPasswd="";

            Factory=new MqttFactory();

            MqttClnt=Factory.CreateMqttClient();

            Logger = loggerFactory?.CreateLogger("MQTTSvc");
            if(Logger == null)
            {
                throw new ArgumentNullException(nameof(loggerFactory));
            }

            using(var scope = _serviceProvider.CreateScope())
            {
                dbContext = scope.ServiceProvider.GetService<ApplicationDbContext>();

                //Read all of the config
                appConfig=dbContext.Configurations.ToList();
                
                sBrokerURL=GetConfigString("BrokerURL");
                sBrokerUser=GetConfigString("BrokerUser");
                sBrokerPasswd=GetConfigString("BrokerPasswd");
                try {
                    fMaxCurrent=dbContext.Meters.First(c=>c.Name.Equals("FredriksMätare")).MaxCurrent;
                } catch(Exception e) {
                    Logger.LogWarning(e.Message);
                    fMaxCurrent=-1;
                }
                Logger.LogInformation($"BrokerURL:{sBrokerURL}");
            }

            for(int i=1;i<4;i++) {
                fMeanCurrent[i]=0;
            }

            //MqttNetGlobalLogger.LogMessagePublished += OnTraceMessagePublished;
            options = new MqttClientOptionsBuilder()
            .WithClientId(Guid.NewGuid().ToString())
            .WithTcpServer(sBrokerURL)
            .WithCredentials(sBrokerUser, sBrokerPasswd)
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
                if(e.ApplicationMessage.Topic.Contains("EVCharger/status/current")) {
                    fChargeCurrent=ToFloat(e.ApplicationMessage.Payload);
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
                    fMeanCurrent[iPhase]=(2 * fMeanCurrent[iPhase]+fCurrent)/3;
                    Logger.LogInformation($"Phase: {iPhase}; Current: {fCurrent}; Mean Current: {fMeanCurrent[iPhase]}");
                    float fNewChargeCurrent;
                    //Calculate new value
                    if(fCurrent>fMaxCurrent) {
                        fNewChargeCurrent=fChargeCurrent-(fCurrent-fMaxCurrent);
                        Logger.LogInformation($"Holy Moses, {fCurrent} is too much power!");
                    
                        //Round down
                        fNewChargeCurrent=(int)fNewChargeCurrent;
                        if(fNewChargeCurrent<2) {
                            fNewChargeCurrent=0;
                        }
                        if(fChargeCurrent!=fNewChargeCurrent) {
                            string sNewChargeCurrent=fNewChargeCurrent.ToString();
                            Logger.LogInformation($"Adjusting down to {sNewChargeCurrent}");
                            MqttClnt.PublishAsync("TEVCharger/set/current",
                                        sNewChargeCurrent,
                                        MqttQualityOfServiceLevel.AtLeastOnce,
                                        false);
                        }
                    }
                }
            });

            Logger.LogInformation("MQTTService created");
        }

        private string GetConfigString(string sKey) {
            try {
                var result=appConfig.First(c=>c.Key.Equals(sKey));
                return result.Value;
            } catch (Exception e) {
                return e.Message;
            }
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
            Logger.LogInformation($"Connection result: {result.ResponseInformation}");

            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(30000, stoppingToken);


                //Check if we can increase the charge current
                float fMaxMeanCurrent=0;
                for(int i=1;i<4;i++) {
                    if(fMeanCurrent[i]>fMaxMeanCurrent) {
                        fMaxMeanCurrent=fMeanCurrent[i];
                    }
                }
                if(fMaxMeanCurrent<fMaxCurrent) {
                    float fNewChargeCurrent=fMaxCurrent-fMaxMeanCurrent;
                    fNewChargeCurrent=(int)fNewChargeCurrent;
                    if(fNewChargeCurrent<2) {
                        fNewChargeCurrent=0;
                    }
                    if(fChargeCurrent!=fNewChargeCurrent) {
                        string sNewChargeCurrent=fNewChargeCurrent.ToString();
                        Logger.LogInformation($"Adjusting up to {sNewChargeCurrent}");
                        await MqttClnt.PublishAsync("TEVCharger/set/current",
                                    sNewChargeCurrent,
                                    MqttQualityOfServiceLevel.AtLeastOnce,
                                    false);
                    }
                }

                Console.WriteLine("Background svc looping");
            }

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

        private class List<T1, T2, T3, T4>
        {
        }
    }
}