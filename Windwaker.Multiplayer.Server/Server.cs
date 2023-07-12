using SuperSimpleTcp;
using System;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Windwaker.Multiplayer.Server
{
    internal class Server
    {
        /// <summary>
        /// The tcp server that handles sending messages to the client
        /// </summary>
        private SimpleTcpServer _server;

        /// <summary>
        /// Starts the server at the specified ip port.  
        /// Is called on application start and should never be disconnected
        /// </summary>
        public bool Start(string ipPort)
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

        /// <summary>
        /// Called whenever a client connects to the server
        /// ???
        /// </summary>
        private void OnClientConnected(object sender, ConnectionEventArgs e)
        {
            Console.WriteLine("Client connected: " + e.IpPort);
        }

        /// <summary>
        /// Called whenever a client disconnects from the server
        /// ???
        /// </summary>
        private void OnClientDisconnected(object sender, ConnectionEventArgs e)
        {
            Console.WriteLine("Client disconnected: " + e.IpPort);
        }

        /// <summary>
        /// Called whenever data is received from a client
        /// ???
        /// </summary>
        private void OnDataReceived(object sender, DataReceivedEventArgs e)
        {
            Console.WriteLine("Data amount: " + e.Data.Count);
        }
    }
}
