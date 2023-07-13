using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Windwaker.Multiplayer.Server
{
    internal class Room
    {
        public string Game => _game;
        public string PassWord => _password;

        public void AllowPlayer(string playerIp) => allowedIps.Add(playerIp);

        public Room(string ipPort, string game, string password)
        {
            _game = game;
            _password = password;
            gameServer = new WindwakerServer();
            gameServer.Start(ipPort);
            Console.WriteLine("Created new room for " + _game);
        }

        private readonly IServer gameServer;

        private readonly List<string> allowedIps = new();

        private readonly string _game;
        private readonly string _password;
    }
}
