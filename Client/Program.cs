
namespace Client
{
    using System;
    using Microsoft.Telepathy.Session;
    using CalcService;

    class Program
    {
        private static string serviceName = "CalcService";

        private static string server = "localhost";

        static void Main(string[] args)
        {
            Console.WriteLine(string.Format("[{0:HH:mm:ss.fff}] ", DateTime.Now) + "Start Client\n");

            if (args.Length > 0)
            {
                server = args[0];
            }

            SessionStartInfo startInfo = new SessionStartInfo(server, serviceName);
            startInfo.SessionResourceUnitType = SessionUnitType.Node;
            startInfo.Secure = false;


            using (Session session = Session.CreateSession(startInfo))
            {
                Console.WriteLine(string.Format("[{0:HH:mm:ss.fff}] ", DateTime.Now) + "Session " + session.Id + " is created.\n");

                string guid = Guid.NewGuid().ToString();

                using (BrokerClient<ICalcService> client = new BrokerClient<ICalcService>(guid, session))
                {
                    client.SendRequest(new AddRequest(1.2, 2.3));
                    client.EndRequests();

                    Console.WriteLine(string.Format("[{0:HH:mm:ss.fff}] ", DateTime.Now) + "Request sent. 1.2 + 2.3 = \n");

                    foreach (var response in client.GetResponses<AddResponse>())
                    {
                        Console.WriteLine(string.Format("[{0:HH:mm:ss.fff}] ", DateTime.Now) + "1.2 + 2.3 = " + response.Result.AddResult + "\n");
                    }

                    client.Close();
                }

                session.Close();
                Console.WriteLine(string.Format("[{0:HH:mm:ss.fff}] ", DateTime.Now) + "Session closed.\n");
            }

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }
    }
}
