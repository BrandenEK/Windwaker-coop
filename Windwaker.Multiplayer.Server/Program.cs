using System;
using Windwaker.Multiplayer.Server.Logging;

namespace Windwaker.Multiplayer.Server
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ILogger logger = new ConsoleLogger();
            var properties = new ServerProperties(8989, string.Empty, 10); // Change with args

            //IServer server = new NetworkServer(properties, logger);
            //server.Start();

            Console.ReadKey();
        }
    }
}