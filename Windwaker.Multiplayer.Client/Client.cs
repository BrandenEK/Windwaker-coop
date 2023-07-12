using SuperSimpleTcp;
using System;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Windwaker.Multiplayer.Client
{
    internal class Client
    {
        private SimpleTcpClient _client;

        public bool IsConnected => _client != null && _client.IsConnected;

        /// <summary>
        /// Attempts to connect to a server at the specified ip port
        /// </summary>
        public bool Connect(string ipPort)
        {
            try
            {
                _client = new SimpleTcpClient(ipPort);

                _client.Events.Connected += OnServerConnected;
                _client.Events.Disconnected += OnServerDisconnected;
                _client.Events.DataReceived += OnDataReceived;

                _client.Connect();
            }
            catch (Exception e) when (e is SocketException || e is TimeoutException)
            {
                MainForm.Log($"Failed to connect to {ipPort}");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Disconnects from the current server
        /// </summary>
        public void Disconnect()
        {
            if (_client != null)
            {
                _client.Disconnect();
                _client = null;
            }
        }

        /// <summary>
        /// Called whenever the client connects to the server
        /// ???
        /// </summary>
        private void OnServerConnected(object sender, ConnectionEventArgs e)
        {
            MainForm.Log($"Connected to {e.IpPort}");
            MainForm.UpdateUI();
        }

        /// <summary>
        /// Called whenever the client disconnects from the server
        /// ???
        /// </summary>
        private void OnServerDisconnected(object sender, ConnectionEventArgs e)
        {
            MainForm.Log($"Disconnected from {e.IpPort}");
            MainForm.UpdateUI();
        }

        /// <summary>
        /// Called whenever data is received from the server
        /// ???
        /// </summary>
        private void OnDataReceived(object sender, DataReceivedEventArgs e)
        {
            MainForm.Log("Data count: " + e.Data.Count);
        }
    }
}
