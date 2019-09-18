using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EnergyApp.Data;
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
        private ApplicationDbContext dbContext;
        private List<Configuration> appConfig;
        public ILogger Logger { get; }
        public MqttFactory Factory { get; }
        public IMqttClient MqttClnt {get; }
        
        public IMqttClientOptions options;
        
        public CultureInfo culture;
        private string sTestPrefix {get;set;}

        //automatically passes the logger factory in to the constructor via dependency injection
        public MQTTService(ILoggerFactory loggerFactory, IServiceProvider serviceProvider)
        {
            //Get serviceprovider so we later can connect to databasemodel from it.
            _serviceProvider=serviceProvider;
            
            culture=CultureInfo.CreateSpecificCulture("sv-SE");

            //fMeanCurrent=new float[4];
            bcLookup=new Dictionary<string, BaseCache>();     
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
                appConfig=dbContext.Configuration.ToList();
                
                sBrokerURL=GetConfigString("BrokerURL");
                sBrokerUser=GetConfigString("BrokerUser");
                sBrokerPasswd=GetConfigString("BrokerPasswd");
                sTestPrefix=GetConfigString("TestPrefix");

                try {
                    var assignments=dbContext.CMPAssignments
                        .Include(m=>m.Meter)
                        .Include(p=>p.Partner)
                        .Include(c=>c.Charger)
                        .AsNoTracking();
                            
                    foreach (var assignItem in assignments)
                    {   
                        var meitem=assignItem.Meter;
                        var paitem=assignItem.Partner;
                        var chitem=assignItem.Charger;
                        
                        try
                        {
                            MeterCache meterCache=new MeterCache();
                            meterCache.sName=meitem.Name;
                            meterCache.sCustomerID=paitem.UserReference;
                            meterCache.fMaxCurrent=meitem.MaxCurrent;
                            bcLookup.Add(meitem.Name, meterCache);

                            ChargerCache chargerCache=new ChargerCache();
                            chargerCache.sCustomerID=paitem.UserReference;
                            chargerCache.fMaxCurrent=chitem.MaxCurrent;
                            chargerCache.bcParent=meterCache;
                            chargerCache.sName=chitem.Name;
                            chargerCache.iPhases=7;
                            bcLookup.Add(chitem.Name, chargerCache);
                        }
                        catch (System.Exception e)
                        {
                            Logger.LogError(e.Message);
                        }
                    }
                } catch(Exception e) {
                    Logger.LogWarning(e.Message);
                    //fMaxCurrent=-1;
                    Logger.LogInformation($"MaxCurrent error: {e.Message}");
                }
                Logger.LogInformation($"BrokerURL:{sBrokerURL}");
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
                Console.WriteLine("### CONNECTED TO SERVER ###");
                // Subscribe to topics
                foreach(string Name in bcLookup.Keys) {
                    if(Name.Contains("EVC")) {
                        await MqttClnt.SubscribeAsync(new TopicFilterBuilder().WithTopic($"{Name}/#").Build());
                    }
                }
                for (int i = 1; i < 4; i++)
                {
                    await MqttClnt.SubscribeAsync(new TopicFilterBuilder().WithTopic($"+/status/current_l{i}/#").Build());
                }

                Console.WriteLine("### SUBSCRIBED to ###");
            });
            #endregion

            MqttClnt.UseDisconnectedHandler(async e =>
            {
                Console.WriteLine("### DISCONNECTED FROM SERVER ###");
                await Task.Delay(TimeSpan.FromSeconds(5));
                try
                {
                    await MqttClnt.ConnectAsync(options);
                }
                catch
                {
                    Console.WriteLine("### RECONNECTING FAILED ###");
                }
            });

            MqttClnt.UseApplicationMessageReceivedHandler(async e =>
            {
                Stopwatch sw = new Stopwatch();

                sw.Start();

                string logstr=$"{DateTime.Now} {e.ApplicationMessage.Topic} {Encoding.UTF8.GetString(e.ApplicationMessage.Payload)}";
                Logger.LogInformation(logstr);

                //Find charger from chargername or meter from metername
                string[] topic=e.ApplicationMessage.Topic.Split('/');

                BaseCache mc;
                try
                {
                    mc=bcLookup[topic[0]];
                }
                catch (System.Exception)
                {
                    Logger.LogInformation($"Cannot find {topic[0]}");
                    mc=null;
                }
                //TODO What happens if not found

                //Check current from the highest meter to the charger
                if(mc is MeterCache) {
                    Logger.LogInformation($"Found mCache:{sw.ElapsedMilliseconds} ms");
                    MeterCache mCache=(MeterCache)mc;
                    if(e.ApplicationMessage.Topic.Contains("current_l")) {
                        //Get info in temp vars
                        var fCurrent=ToFloat(e.ApplicationMessage.Payload);
                        int iPhase=Int16.Parse(e.ApplicationMessage.Topic.Substring(e.ApplicationMessage.Topic.Length-1));
                        
                        //Store in cache
                        mCache.fMeterCurrent[iPhase]=fCurrent;
                        mCache.fMeanCurrent[iPhase]=(2 * mCache.fMeanCurrent[iPhase]+fCurrent)/3;
                        Logger.LogInformation($"Phase: {iPhase}; Current: {fCurrent}; Mean Current: {mCache.fMeanCurrent[iPhase]}");
                        float fNewChargeCurrent;
                        //Calculate new value
                        if(fCurrent>mCache.fMaxCurrent) {
                            Logger.LogInformation($"Overload meter:{sw.ElapsedMilliseconds} ms");
                            //What's the overcurrent?
                            float fOverCurrent=fCurrent-mCache.fMaxCurrent;
                            Logger.LogInformation($"Holy Moses, {fCurrent} is {fOverCurrent}A too much!");
                            
                            var chargers=mCache.cChildren.Select(m => m.GetType().Equals("Charger"));

                            //Set new current
                            //Number of chargers to even out on
                            int iChargers=mCache.cChildren.Count;

                            fNewChargeCurrent=(mCache.fMaxCurrent-fOverCurrent)/iChargers;

                            foreach(ChargerCache cc in mCache.cChildren) {
                                await AdjustCurrent(fNewChargeCurrent, cc);
                            }
                            Logger.LogInformation($"Adjusted current:{sw.ElapsedMilliseconds} ms");
                        } 
                        else
                        {
                            //Loop through chargers and outlets
                            /* 
                            foreach (var charger in mCache.cChildren)
                            {
                                if(fCurrent>charger.fMaxCurrent) {
                                    float fOverCurrent=fCurrent-mCache.fMaxCurrent;
                                    Logger.LogInformation($"Holy Charger, {fCurrent} is {fOverCurrent}A too much!");
                                    fNewChargeCurrent=(charger.fMaxCurrent-fOverCurrent);
                                    await AdjustCurrent(fNewChargeCurrent, charger);
                                }
                            }
                            */
                        }
                    }
                }
                else if(mc is ChargerCache) {
                    var cCache=(ChargerCache)mc;
                    if(e.ApplicationMessage.Topic.Contains("/status/current")) {
                        cCache.fCurrentSet=ToFloat(e.ApplicationMessage.Payload);
                        Logger.LogInformation($"Got charger current:{cCache.fCurrentSet}, {sw.ElapsedMilliseconds} ms");
                    }
                    else if(e.ApplicationMessage.Topic.Contains("/status/max_current"))
                    {
                        cCache.fMaxCurrent=ToFloat(e.ApplicationMessage.Payload);
                        Logger.LogInformation($"Got charger maxcurrent:{cCache.fMaxCurrent}, {sw.ElapsedMilliseconds} ms");
                    }
                    else if(e.ApplicationMessage.Topic.Contains("/status/charging"))
                    {
                        cCache.bConnected=ToBool(e.ApplicationMessage.Payload);
                        Logger.LogInformation($"Got charger charging:{cCache.bConnected}, {sw.ElapsedMilliseconds} ms");
                    }
                }

                if(e.ApplicationMessage.Topic.Contains("current1d")) {
                    //Save PNG to file
                    
                    Stream s=new FileStream("1d.png",FileMode.Create);
                    var bw=new BinaryWriter(s);
                    bw.Write(e.ApplicationMessage.Payload);
                    //bw.Flush();
                    bw.Close();
                }
                Logger.LogInformation($"Receive end:{sw.ElapsedMilliseconds} ms");
                sw.Stop();
            });

            Logger.LogInformation("MQTTService created");
        }
        private async Task AdjustCurrent(float fNewChargeCurrent, ChargerCache cCache) {
            //Round down
            fNewChargeCurrent=(int)fNewChargeCurrent;
            if(fNewChargeCurrent<2) {
                fNewChargeCurrent=0;
            }
            
            string sNewChargeCurrent=fNewChargeCurrent.ToString();

            if(cCache.fCurrentSet!=fNewChargeCurrent) {
                Logger.LogInformation($"Adjusting to {sNewChargeCurrent}");
                await MqttClnt.PublishAsync($"{cCache.sName}/set/current",
                            sNewChargeCurrent,
                            MqttQualityOfServiceLevel.AtLeastOnce,
                            false);
            }

            //TODO Store in database
        }
        private string GetConfigString(string sKey) {
            try {
                var result=appConfig.First(c=>c.Key.Equals(sKey));
                return result.Value;
            } catch (Exception e) {
                return e.Message;
            }
        }
        private bool ToBool(byte[] bArray) {
            string s=System.Text.Encoding.UTF8.GetString(bArray);

            return s.Equals('1');
        }
        private float ToFloat(byte[] bArray) {
            float f=0;
            string s=System.Text.Encoding.UTF8.GetString(bArray);
            //Adjust to sv-SE Culture? Or Invariant
            try
            {
                f=float.Parse(s,System.Globalization.NumberStyles.Float,CultureInfo.InvariantCulture);
            }
            catch (System.Exception)
            {
                Logger.LogError($"Unable to parse {s}");
            }
            return f;
        }
        private void OnTraceMessagePublished(object sender, MqttNetLogMessagePublishedEventArgs e)
        {
            //Logger.LogTrace(e.TraceMessage.Message);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using(var scope = _serviceProvider.CreateScope())
            {
                dbContext = scope.ServiceProvider.GetService<ApplicationDbContext>();

                var test=dbContext.Meters.AsQueryable();
                
            }

            Logger.LogInformation("Background thread started");

            var result = await MqttClnt.ConnectAsync(options);
            Logger.LogInformation($"Connection result: {result.ResponseInformation}");

            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(30000, stoppingToken);

                //Check if we can increase the charge current
                foreach(BaseCache cCache in bcLookup.Values) {
                    if(cCache is ChargerCache) {
                        ChargerCache cc = (ChargerCache)cCache;

                        //TODO Look per phase for possible current adjustments
                        string sNewChargeCurrent = cc.GetNewChargeCurrent();

                        if(cc.bConnected) {
                            if(sNewChargeCurrent != null) {
                                Logger.LogInformation($"Adjusting up to {sNewChargeCurrent}");
                                await MqttClnt.PublishAsync($"{sTestPrefix}{cc.sName}/set/current",
                                            sNewChargeCurrent,
                                            MqttQualityOfServiceLevel.AtLeastOnce,
                                            false);
                                
                                //Get info in temp vars
                                //var fCurrent=ToFloat(e.ApplicationMessage.Payload);
                                //int iPhase=Int16.Parse(e.ApplicationMessage.Topic.Substring(e.ApplicationMessage.Topic.Length-1));
                            }
                        } 
                        else
                        {
                            Logger.LogInformation($"Could turn {cc.sName} to {sNewChargeCurrent}A");
                        }
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
        private Dictionary<string, BaseCache> bcLookup {get; set;}

        public BaseCache GetBaseCache(string sKey) {
            return bcLookup[sKey];
        }
    }

    public class BaseCache {
        public BaseCache() {
            cChildren=new List<ChargerCache>();
        }
        public string sName;
        public string sCustomerID;
        public float fMaxCurrent;
        public BaseCache bcParent;
        public ICollection<ChargerCache> cChildren;
        public ICollection<MeterCache> mChildren;
    }
    public enum CacheType {ChargerType, MeterType};
    public class MeterCache :BaseCache {
        public MeterCache() {
            fMeanCurrent=new float[4];
            fMeterCurrent=new float[4];
        }
        public float[] fMeanCurrent;
        public float[] fMeterCurrent;
        //public int iPhase;
        public bool bCurrentAdd;

        /**Return max meancurrent for the incoming phases 
        iPhases = phases in binary form */
        public float GetMaxPhaseAddCurrent(int iPhases) {
            float fMax=0;
            int iPhase=3;
            
            for (int iPowTwo = 4; iPowTwo > 0 ; iPowTwo/=2)
            {
                if(iPhases>=iPowTwo) {
                    //Valid phase - use in calculation
                    if(fMeanCurrent[iPhase]>fMax) {
                        fMax=fMeanCurrent[iPhase];
                    }
                    iPhase--;
                    iPhases-=iPowTwo;
                }
            }
            return fMaxCurrent-fMax;
        }
    }

    public class ChargerCache :BaseCache {
        public float fCurrentSet;
        public bool bConnected;
        public int iPhases=0;
        public string GetNewChargeCurrent()
        {
            //Get max current that meter could handle
            float fCurrentMaxAdd=((MeterCache)bcParent).GetMaxPhaseAddCurrent(iPhases);

            //Can the charger handle that too?
            //TODO Get this from Outlet instead

            //Current to increase with
            float fNewChargeCurrent=fCurrentSet+fCurrentMaxAdd;
            if(fNewChargeCurrent>fMaxCurrent) {
                fNewChargeCurrent=fMaxCurrent;
            }

            //Max current to increase with
            return fNewChargeCurrent.ToString("0");
        }
    }
}