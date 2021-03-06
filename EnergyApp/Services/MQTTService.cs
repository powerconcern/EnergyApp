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
        private readonly ILogger _log;
        private ApplicationDbContext dbContext;
        private List<Configuration> appConfig;
        public MqttFactory Factory { get; }
        public IMqttClient MqttClnt {get; }
        
        public IMqttClientOptions options;
        
        public CultureInfo culture;
        private string sTestPrefix {get;set;}

        //Automatically passes the logger factory in to the constructor via dependency injection
        public MQTTService(ILogger<MQTTService> log, IServiceProvider serviceProvider)
        {
            //Get serviceprovider so we later can connect to databasemodel from it.
            _serviceProvider=serviceProvider;
            _log=log;
            
            //log=_serviceProvider.GetRequiredService()
            
            culture=CultureInfo.CreateSpecificCulture("sv-SE");

            //fMeanCurrent=new float[4];
            bcLookup=new Dictionary<string, BaseCache>();     
            string sBrokerURL, sBrokerUser, sBrokerPasswd="";

            Factory=new MqttFactory();

            MqttClnt=Factory.CreateMqttClient();
/* 
            log = loggerFactory?.CreateLogger("MQTTSvc");
            if(log == null)
            {
                throw new ArgumentNullException(nameof(loggerFactory));
            }
*/
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
                            meterCache.cChildren.Add(chargerCache);
                            bcLookup.Add(chitem.Name, chargerCache);
                        }
                        catch (System.Exception e)
                        {
                            _log.LogError(e.Message);
                        }
                    }

                } catch(Exception e) {
                    _log.LogWarning(e.Message);
                    //fMaxCurrent=-1;
                    LogInformation($"MaxCurrent error: {e.Message}");
                }
                LogInformation($"BrokerURL:{sBrokerURL}");
            }

            //MqttNetGlobalLogger.LogMessagePublished += OnTraceMessagePublished;
            options = new MqttClientOptionsBuilder()
            .WithClientId("EnergyApp")
            .WithTcpServer(sBrokerURL)
            .WithCredentials(sBrokerUser, sBrokerPasswd)
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

                Console.WriteLine("### SUBSCRIBED ###");
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

            MqttClnt.UseApplicationMessageReceivedHandler(e =>
            {
                LogInformation($"IncomingMsg;{e.ApplicationMessage.Topic};{Encoding.UTF8.GetString(e.ApplicationMessage.Payload)}");

                //Find charger from chargername or meter from metername
                string[] topic = e.ApplicationMessage.Topic.Split('/');

                BaseCache mc;
                try
                {
                    mc = bcLookup[topic[0]];
                }
                catch (System.Exception)
                {
                    LogInformation($"Cannot find {topic[0]}");
                    mc = null;
                }
                //TODO What happens if not found

                //Check current from the highest meter to the charger
                if (mc is MeterCache)
                {
                    MeterCache mCache = (MeterCache)mc;
                    if (e.ApplicationMessage.Topic.Contains("current_l"))
                    {
                        //Get info in temp vars
                        var fCurrent = ToFloat(e.ApplicationMessage.Payload);
                        int iPhase = Int16.Parse(e.ApplicationMessage.Topic.Substring(e.ApplicationMessage.Topic.Length - 1));

                        //Store in cache
                        mCache.fMeterCurrent[iPhase] = fCurrent;
                        mCache.fMeanCurrent[iPhase] = (2 * mCache.fMeanCurrent[iPhase] + fCurrent) / 3;
                        LogInformation($"Phase: {iPhase}; Current: {fCurrent}; Mean Current: {mCache.fMeanCurrent[iPhase]}");
                        float fSuggestedCurrentChange;
                        //Calculate new value
                        if (fCurrent > mCache.fMaxCurrent)
                        {
                            //What's the overcurrent?
                            float fOverCurrent = fCurrent - mCache.fMaxCurrent;
                            LogInformation($"Holy Moses, {fCurrent} is {fOverCurrent}A too much!");

                            //Check all chargers that is connected
                            var connChargers = mCache.cChildren.Where(m => m.bConnected);

                            //Set new current
                            //Number of chargers to even out on
                            int iChargers = connChargers.Count();

                            fSuggestedCurrentChange = (-fOverCurrent) / iChargers;

                            foreach (ChargerCache cc in connChargers)
                            {

                                //TODO Handle the case where one charger uses less current than given.
                                //TODO Distribute to the others.

                                float fNewCurrent = cc.AdjustNewChargeCurrent(fSuggestedCurrentChange);

                                //TODO Might hang with await
                                PostAdjustedCurrent(fNewCurrent, cc);
                            }
                            LogInformation($"New charging current for {mCache.sName}");
                        }
                        else
                        {
                            //Loop through chargers and outlets
                            /* 
                            foreach (var charger in mCache.cChildren)
                            {
                                if(fCurrent>charger.fMaxCurrent) {
                                    float fOverCurrent=fCurrent-mCache.fMaxCurrent;
                                    _logger.LogInformation($"Holy Charger, {fCurrent} is {fOverCurrent}A too much!");
                                    fNewChargeCurrent=(charger.fMaxCurrent-fOverCurrent);
                                    await AdjustCurrent(fNewChargeCurrent, charger);
                                }
                            }
                            */
                        }
                    }
                }
                else if (mc is ChargerCache)
                {
                    var cCache = (ChargerCache)mc;
                    if (e.ApplicationMessage.Topic.Contains("/status/current"))
                    {
                        cCache.fCurrentSet = ToFloat(e.ApplicationMessage.Payload);
                        LogInformation($"Got charger current:{cCache.fCurrentSet}");
                    }
                    else if (e.ApplicationMessage.Topic.Contains("/status/max_current"))
                    {
                        cCache.fMaxCurrent = ToFloat(e.ApplicationMessage.Payload);
                        LogInformation($"Got charger maxcurrent:{cCache.fMaxCurrent}");
                    }
                    else if (e.ApplicationMessage.Topic.Contains("/status/charging"))
                    {
                        cCache.bConnected = ToBool(e.ApplicationMessage.Payload);
                        LogInformation($"Got charger charging:{cCache.bConnected}");

                        using(var scope = _serviceProvider.CreateScope())
                        {
                            dbContext = scope.ServiceProvider.GetService<ApplicationDbContext>();

                            var chSession=new ChargeSession();

                            chSession.ChargerID=1;
                            chSession.ID=1;
                            chSession.OutletID=1;
                            chSession.Kwh=10;
   
                            dbContext.ChargeSession.Add(chSession);
                            dbContext.SaveChanges();
                           
                        }
                    }
                }

                if (e.ApplicationMessage.Topic.Contains("current1d"))
                {
                    //Save PNG to file

                    Stream s = new FileStream("1d.png", FileMode.Create);
                    var bw = new BinaryWriter(s);
                    bw.Write(e.ApplicationMessage.Payload);
                    //bw.Flush();
                    bw.Close();
                }
                LogInformation($"Receive end");

            });

            LogInformation("MQTTService created");
        }

        private static Stopwatch NewMethod()
        {
            return new Stopwatch();
        }

        private void LogInformation(string sLogInfo) {
            _log.LogInformation($"{DateTime.Now.ToString("MM-ddTHH:mm:ss.FFF",culture)};{sLogInfo}");
        }
        private async Task PostAdjustedCurrent(float fNewChargeCurrent, ChargerCache cCache) {

            string sNewChargeCurrent=fNewChargeCurrent.ToString();

            if(cCache.fCurrentSet!=fNewChargeCurrent) {
                LogInformation($"Adjusting {sTestPrefix}{cCache.sName} to {sNewChargeCurrent}A");
                await MqttClnt.PublishAsync($"{sTestPrefix}{cCache.sName}/set/current",
                            sNewChargeCurrent,
                            MqttQualityOfServiceLevel.AtLeastOnce,
                            false);
                LogInformation($"Adjusted {sTestPrefix}{cCache.sName} to {sNewChargeCurrent}A");
            } else {
                LogInformation($"{sTestPrefix}{cCache.sName} already at {sNewChargeCurrent}A");
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

            return s.Equals("1");
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
                _log.LogError($"Unable to parse {s}");
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

            LogInformation("Background thread started");

            var result = await MqttClnt.ConnectAsync(options);
            LogInformation($"Connection result: {result.ResultCode}");

            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(30000, stoppingToken);
                
                //Check if we can increase the charge current
                //Loop through meters
                foreach(BaseCache cCache in bcLookup.Values) {
                    if(cCache is MeterCache) {
                        MeterCache mc = (MeterCache)cCache;
                        
                        //Get max current we can increase with
                        //Checks all phases
                        LogInformation($"{mc.sName}, checking possible currentChange");
                        //TODO Check one phase at a time
                        float fSuggestedCurrentChange=mc.GetMaxPhaseAddCurrent(7);

                        //Check all chargers that is connected
                        var connChargers=mc.cChildren.Where(m => m.bConnected);

                        //Set new current
                        //Number of chargers to even out on
                        int iChargers=connChargers.Count();

                        LogInformation($"{mc.sName}, possible currentChange for {iChargers} chargers: {fSuggestedCurrentChange}A");

                        fSuggestedCurrentChange=fSuggestedCurrentChange/iChargers;

                        foreach(ChargerCache cc in connChargers) {

                            //TODO Handle the case where one charger uses less current than given.
                            //TODO Distribute to the others.

                            float fNewCurrent=cc.AdjustNewChargeCurrent(fSuggestedCurrentChange);
                            await PostAdjustedCurrent(fNewCurrent, cc);
                        }
                    }
                }
                LogInformation($"Background svc loop end");
            }

            stoppingToken.Register(() => LogInformation($" MQTTSvc background task is stopping."));


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
                string _logstr=$"{DateTime.Now} {e.ApplicationMessage.Topic} \t {Encoding.UTF8.GetString(e.ApplicationMessage.Payload)}";
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
            //TODO reread structure if not found
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
        /// <summary>
        /// Calculates new chargercurrent
        /// </summary>
        /// <returns></returns>
        public string GetNewChargeCurrent()
        {
            //Get max current that meter could handle
            float fCurrentMaxAdd=((MeterCache)bcParent).GetMaxPhaseAddCurrent(iPhases);

            //Can the charger handle that too?
            //TODO Get this from Outlet instead. Or don't - maybe not necessary

            //Current to increase with
            float fSuggestedCurrentChange=fCurrentMaxAdd-fCurrentSet;
            float fNewChargeCurrent=AdjustNewChargeCurrent(fSuggestedCurrentChange);

            //Max current to increase with
            return fNewChargeCurrent.ToString("0");
        }
        /// <summary>
        /// Adjusts for max and min current
        /// </summary>
        /// <param name="fSuggestedCurrentChange"></param>
        /// <returns></returns>
        public float AdjustNewChargeCurrent(float fSuggestedCurrentChange) {
            float fNewIncomingCurrent=fCurrentSet+fSuggestedCurrentChange;

            if(fNewIncomingCurrent>fMaxCurrent) {
                fNewIncomingCurrent=fMaxCurrent;
            }
            if(fNewIncomingCurrent<2) {
                fNewIncomingCurrent=0;
            }
            //Round down
            return (int)fNewIncomingCurrent;
        }
    }
}