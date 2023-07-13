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
        public string Password => _password;
        public ushort Port => (ushort)gameServer.Port;

        public void AllowPlayer(string playerIp) => allowedIps.Add(playerIp);

        // Not implemented yet
        public bool IsNameTaken(string name) => false;
        public bool IsIpTaken(string ip) => false;
        public int PlayerCount => 0;

        public Room(string ipPort, string game, string password)
        {
            _game = game;
            _password = password;
            gameServer = new WindwakerServer();
            gameServer.Start(ipPort);
        }

        private readonly IServer gameServer;

        private readonly List<string> allowedIps = new();

        private readonly string _game;
        private readonly string _password;
    }
}
