using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SuperSimpleTcp;

namespace Windwaker.Multiplayer.Server
{
    public class Server
    {
        private SimpleTcpServer _server;

        public Server()
        {
            _server = new SimpleTcpServer("192.168.1.166", Core.ServerSettings.port);

            _server.Events.ClientConnected += OnClientConnected;
            _server.Events.ClientDisconnected += OnClientDisconnected;
            _server.Events.DataReceived += OnDataReceived;

            _server.Start();
            Console.WriteLine("Started server at 192.168.1.166:" + Core.ServerSettings.port);
        }

        private void OnDataReceived(object sender, DataReceivedEventArgs e)
        {
            Console.WriteLine("Data amount: " + e.Data.Count);
        }

        private void OnClientDisconnected(object sender, ConnectionEventArgs e)
        {
            Console.WriteLine("Client connected: " + e.IpPort);
        }

        private void OnClientConnected(object sender, ConnectionEventArgs e)
        {
            Console.WriteLine("Client disconnected: " + e.IpPort);
        }
    }
}
