using System;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Globalization;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;

namespace Energy
{
    class Replay
    {
        static void Main(string[] args)
        {
            //public MqttFactory Factory;
            //public IMqttClient MqttClnt {get; }
            //public IMqttClientOptions options;
            //Find replay file
            string sFile="Replay.txt";
            string sBrokerURL="mqtt.symlink.se";
            string sBrokerUser="johsim:johsim";
            string sBrokerPasswd="UlCoPGgk";

            MqttFactory Factory=new MqttFactory();

            IMqttClient MqttClnt=Factory.CreateMqttClient();

            IMqttClientOptions options = new MqttClientOptionsBuilder()
            .WithClientId(Guid.NewGuid().ToString())
            .WithTcpServer(sBrokerURL)
            .WithCredentials(sBrokerUser, sBrokerPasswd)
            .WithCleanSession()
            .Build();

            //var result = MqttClnt.ConnectAsync(options);
            #region UseConnectedHandler
            MqttClnt.UseConnectedHandler(e =>
            {
                Console.WriteLine("### CONNECTED WITH SERVER ###");
                // Subscribe to topics
//                foreach(string Name in bcLookup.Keys) {
  //                  await MqttClnt.SubscribeAsync(new TopicFilterBuilder().WithTopic($"{Name}/#").Build());
    //            }
                //await MqttClnt.SubscribeAsync(new TopicFilterBuilder().WithTopic("+/status/#").Build());
                //await MqttClnt.SubscribeAsync(new TopicFilterBuilder().WithTopic("+/set/#").Build());

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

            var result = MqttClnt.ConnectAsync(options);

            Console.WriteLine($"Replaying from {sFile}");

            MessageHandler msgHandler=new MessageHandler("2019-");

            foreach (string line in File.ReadLines(sFile, Encoding.UTF8))
            {
                if(msgHandler.HandleRow(line)) {
                    if(msgHandler.NextSection()) {
                        Console.WriteLine("Press key for next topic post");
                        Console.Read();
                    }
                }

            }
        }
    }

 
    //Class to handle messages for MQTT topics in replayfile
    public class MessageHandler {
        private DateTime LastPostDTM {get;set;}
        private DateTime PostDTM {get;set;}
        private string LastClient {get;set;}
        private string Client {get;set;}
        private string Topic {get;set;}
        private string Value {get;set;}
        private string Qualifier {get;set;}

        public MessageHandler(string qualf) {
            Qualifier=qualf;
        }

        public bool HandleRow(string line) {
            bool bHandled=false;
            if(line.Contains(Qualifier)){
                //Set Topic and Value
                LastClient=Client;
                LastPostDTM=PostDTM;
                string[] tmpLine=line.Trim().Split(" ");
                string[] tmpTopic=tmpLine[2].Split("/");
                Client=tmpTopic[0];
                Value=tmpLine[tmpLine.Length-1];
                PostDTM=DateTime.Parse($"{tmpLine[0]} {tmpLine[1].Substring(0,8)}", CultureInfo.InvariantCulture);
                if(LastClient is null) {
                    LastClient=Client;
                    LastPostDTM=PostDTM;
                }
                bHandled=true;
                Console.WriteLine(line);
            }
            return bHandled;
        }

        public bool NextSection() {
            bool bNextSection=false;
            if(LastClient is null) {
                bNextSection=true;
            } else {
                bNextSection=!Client.Equals(LastClient);
            }
            
            //Calculate time since last topic post 
            if(bNextSection || SecondsSinceLastPost()>5) {
                return true;
            } else {
                return false;
            }
        }

        public double SecondsSinceLastPost() {
            return (PostDTM - LastPostDTM).TotalSeconds;
        }
    }
}
