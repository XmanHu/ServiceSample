
namespace Client
{
    using System;
    using Microsoft.Telepathy.Session;
    using CalcService;
    using System.Text;
    using System.Collections.Generic;

    class Program
    {
        private static string serviceName = "CalcService";

        private static string server = "localhost";

        private static int requestNum = 500;

        private static readonly Random rnd = new Random();

        private static Dictionary<Guid, AddRequest> dic = new Dictionary<Guid, AddRequest>();

        static void Main(string[] args)
        {
            WriteLog("Start Client");

            //Set headnode if not localhost
            if (args.Length > 0)
            {
                server = args[0];
            }

            //Create requests
            for (int i = 0; i < requestNum; i++)
            {
                var guid = Guid.NewGuid();
                dic.Add(guid, new AddRequest(rnd.Next(1, 100), rnd.Next(1, 100)));
            }

            //Set Session Start Info
            SessionStartInfo startInfo = new SessionStartInfo(server, serviceName);
            startInfo.SessionResourceUnitType = SessionUnitType.Node;
            startInfo.Secure = false;

            //Create Session
            using (Session session = Session.CreateSession(startInfo))
            {
                WriteLog("Session " + session.Id + " is created.");

                string guid = Guid.NewGuid().ToString();

                //Create Client
                using (BrokerClient<ICalcService> client = new BrokerClient<ICalcService>(guid, session))
                {
                    //send requests
                    foreach (var item in dic)
                    {
                        client.SendRequest(item.Value, new System.Xml.UniqueId(item.Key));
                    }

                    client.EndRequests();
                    WriteLog("All requests have been sent.");
                    
                    //get response
                    foreach (var response in client.GetResponses<AddResponse>())
                    {
                        AddRequest request = dic[response.RequestMessageId];
                        WriteLog(request.a + " + " + request.b + " = " +response.Result.AddResult);
                    }

                    WriteLog("All responses have been received.");

                    client.Close();
                }
                //close session
                session.Close();
                WriteLog("Session closed.");
            }
        }

        static void WriteLog(string body)
        {
            StringBuilder sb = new StringBuilder(string.Format("[{0:HH:mm:ss.fff}] ", DateTime.Now));
            sb.Append(body);
            Console.WriteLine(sb);
            Console.WriteLine();
        }
    }
}
