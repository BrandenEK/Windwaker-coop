using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace Windwaker.Multiplayer.Server
{
    internal class Room
    {
        public string Id => _id;
        public string Game => _game;
        public string Password => _password;
        //public ushort Port => (ushort)gameServer.Port;

        public ReadOnlyDictionary<string, string> AllPlayers => connectedPlayers.AsReadOnly();

        public Room(string ipPort, string id, string game, string password)
        {
            _id = id;
            _game = game;
            _password = password;
            //gameServer = new WindwakerServer(this);
            //gameServer.Start(ipPort);
        }

        public void QueuePlayer(string ip, string name)
        {
            if (!queuedPlayers.ContainsKey(ip))
            {
                Console.WriteLine($"{_id}: Adding player {ip}");
                queuedPlayers.Add(ip, name);
            }
        }

        public bool AddPlayer(string ip)
        {
            if (queuedPlayers.TryGetValue(ip, out string name))
            {
                Console.WriteLine($"{_id}: Connecting registered player {ip}");
                connectedPlayers.Add(ip, name);
                queuedPlayers.Remove(ip);
                return true;
            }

            Console.WriteLine($"{_id}: Disconnecting unregistered player {ip}");
            return false;
        }

        public void RemovePlayer(string ip)
        {
            Console.WriteLine($"{_id}: Removing player {ip}");
            connectedPlayers.Remove(ip);
            queuedPlayers.Remove(ip);
        }

        public bool IsIpTaken(string ip)
        {
            foreach (string playerIp in connectedPlayers.Keys)
            {
                if (ip == playerIp)
                    return true;
            }
            return false;
        }

        public bool IsNameTaken(string name)
        {
            foreach (string playerName in connectedPlayers.Values)
            {
                if (name == playerName)
                    return true;
            }
            return false;
        }


        //private readonly IServer gameServer;

        private readonly Dictionary<string, string> connectedPlayers = new();
        private readonly Dictionary<string, string> queuedPlayers = new();

        private readonly string _id;
        private readonly string _game;
        private readonly string _password;
    }
}
