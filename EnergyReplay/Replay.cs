using System;
using System.Text;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Globalization;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;
using MQTTnet.Protocol;

namespace Energy
{
    class Replay
    {
        static void Main(string[] args)
        {
            bool IsConnected=false;
            bool isDebug=false;

            //Find replay file
            string sFile="Replay0909.txt";
            if(args[0] != null) {
                sFile=args[0];
                Console.WriteLine($"Reading from {sFile}");
            }
            string sQualifier="2019-";
            if(args[1] != null) {
                sQualifier=args[1];
                Console.WriteLine($"Qualifier {sQualifier}");
            }
            if(args[2] != null) {
                if(args[1].Contains("debug") {
                    isDebug=true;
                }
                Console.WriteLine($"Debugging, no pause");
            }


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
                Console.WriteLine("### CONNECTED TO SERVER ###");
                IsConnected=true;
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
                IsConnected=false;
                await Task.Delay(TimeSpan.FromSeconds(5));
                try
                {
                    await MqttClnt.ConnectAsync(options);
                    IsConnected=true;
                }
                catch
                {
                    Console.WriteLine("### RECONNECTING FAILED ###");
                }
            });


            var result = MqttClnt.ConnectAsync(options);
            
            
            Console.WriteLine($"Replaying from {sFile}");

            MessageHandler msgHandler=new MessageHandler(sQualifier);

            foreach (string line in File.ReadLines(sFile, Encoding.UTF8))
            {
                if(msgHandler.HandleRow(line)) {
                    if(msgHandler.NextSection()) {
                        if(isDebug) {
                            Thread.Sleep(5000);
                        } else {
                            Console.WriteLine("Press key for next topic post");
                            Console.Read();
                        }
                    }
                    string sNewTopic="Test"+msgHandler.Topic;

                    //Check if connected
                    while (!IsConnected)
                    {
                        Console.WriteLine("Waiting for connection");
                        //Task.Delay(TimeSpan.FromSeconds(5));
                        Thread.Sleep(5000);
                    }
                    MqttClnt.PublishAsync(sNewTopic,
                    msgHandler.Value,
                    MqttQualityOfServiceLevel.AtLeastOnce,
                    false);
                    Console.WriteLine($"Sent {msgHandler.Value} to {sNewTopic}");
                }
            }
        }
    }

 
    //Class to handle messages for MQTT topics in replayfile
    public class MessageHandler {
        private DateTime LastPostDTM {get;set;}
        private DateTime PostDTM {get;set;}
        private string[] sTopicParts;
        private string LastClient {get;set;}
        private string Client {get;set;}
        public string Topic {get;set;}
/*         
            Not needed as long as we only prefix Test

            public string NewTopic {
            get {
                StringBuilder stringBuilder=new StringBuilder(sPrefix);

                for (int i = 0; i < sTopicParts.Length; i++)
                {
                    stringBuilder.Append(sTopicParts[i]);
                }
                return stringBuilder.ToString();
            }
            set {
            }
        }
 */
        public string Value {get;set;}
        private string Qualifier {get;set;}

        public MessageHandler(string qualf) {
            Qualifier=qualf;
        }

        public bool HandleRow(string line) {
            bool bHandled=false;
            if(line.Contains(Qualifier)) {
                //Set Topic and Value
                string[] tmpLine=line.Trim().Split(" ");
                if(tmpLine[2].Contains("Test")) {
                    Console.WriteLine($"Filtering out {line}");
                    bHandled=false;
                } else {
                    LastClient=Client;
                    LastPostDTM=PostDTM;
                    if(tmpLine[0].Contains('/')) {
                        //US dateformat
                        PostDTM=DateTime.Parse($"{tmpLine[0]} {tmpLine[1]} {tmpLine[2].Substring(0,2)}", CultureInfo.InvariantCulture);
                        Topic=tmpLine[3];
                    } else {
                        PostDTM=DateTime.Parse($"{tmpLine[0]} {tmpLine[1].Substring(0,8)}", CultureInfo.InvariantCulture);
                        Topic=tmpLine[2];
                    }
                    sTopicParts=Topic.Split("/");
                    Client=sTopicParts[0];
                    Value=tmpLine[tmpLine.Length-1];
                    if(LastClient is null) {
                        LastClient=Client;
                        LastPostDTM=PostDTM;
                    }
                    bHandled=true;
                }
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
            if(bNextSection || SecondsSinceLastPost()>3) {
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
