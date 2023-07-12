using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SuperSimpleTcp;
using System.Net.Sockets;

namespace Windwaker.Multiplayer.Server
{
    internal class Server
    {
        private SimpleTcpServer _server;

        public bool Connect(string ipPort)
        {
            try
            {
                _server = new SimpleTcpServer(ipPort);

                _server.Events.ClientConnected += OnClientConnected;
                _server.Events.ClientDisconnected += OnClientDisconnected;
                _server.Events.DataReceived += OnDataReceived;

                _server.Start();
            }
            catch (Exception e) when (e is SocketException || e is TimeoutException)
            {
                Console.WriteLine($"Failed to start server at {ipPort}");
                return false;
            }

            Console.WriteLine($"Successfully started server at {ipPort}");
            return true;
        }

        public void Disconnect()
        {
            if (_server != null)
            {
                _server.Stop();
                _server = null;
            }
        }

        private void OnClientConnected(object sender, ConnectionEventArgs e)
        {
            Console.WriteLine("Client connected: " + e.IpPort);
        }

        private void OnClientDisconnected(object sender, ConnectionEventArgs e)
        {
            Console.WriteLine("Client disconnected: " + e.IpPort);
        }

        private void OnDataReceived(object sender, DataReceivedEventArgs e)
        {
            Console.WriteLine("Data amount: " + e.Data.Count);
        }
    }
}
