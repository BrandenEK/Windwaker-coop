using SuperSimpleTcp;
using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace Windwaker.Multiplayer.Server
{
    internal abstract class AbstractServer<T> : IServer where T : Enum
    {
        public int Port => _server == null ? 0 : _server.Port;

        /// <summary>
        /// Initializes the server with the given receive methods
        /// </summary>
        protected void Initialize(Dictionary<T, Action<string, byte[]>> receivers)
        {
            _receivers = receivers;
        }

        /// <summary>
        /// Attempts to start the server at the specified ip port.  
        /// </summary>
        public bool Start(string ipPort)
        {
            try
            {
                _server = new SimpleTcpServer(ipPort);

                _server.Events.ClientConnected += OnClientConnected;
                _server.Events.ClientDisconnected += OnClientDisconnected;
                _server.Events.DataReceived += Receive;

                _server.Start();
            }
            catch (Exception e) when (e is SocketException || e is TimeoutException)
            {
                Console.WriteLine($"Failed to start server at {ipPort}");
                return false;
            }

            Console.WriteLine($"Started server at {ipPort}");
            return true;
        }

        /// <summary>
        /// Stops the server
        /// </summary>
        public void Stop()
        {
            if (_server != null)
            {
                _server.Stop();
                _server = null;
            }
        }

        /// <summary>
        /// Disconnects the specified client ip from the server
        /// </summary>
        public void DisconnectClient(string ipPort)
        {
            if (_server != null )
            {
                _server.DisconnectClient(ipPort);
            }
        }

        /// <summary>
        /// Sends a message to a client at the specified ip port
        /// </summary>
        protected void Send(string ip, byte[] message, T type)
        {
            if (message == null || message.Length == 0)
                return;

            var list = new List<byte>();
            list.AddRange(BitConverter.GetBytes((ushort)message.Length));
            list.Add((byte)(object)type);
            list.AddRange(message);

            try
            {
                _server.Send(ip, list.ToArray());
            }
            catch (Exception)
            {
                Console.WriteLine($"*** Couldn't send data to {ip} ***");
            }
        }

        /// <summary>
        /// Receives a message from a client and calls the specific receive method to process it
        /// </summary>
        private void Receive(object sender, DataReceivedEventArgs e)
        {
            byte[] data = e.Data.ToArray();
            int startIdx = 0;

            while (startIdx < data.Length - 3)
            {
                ushort length = BitConverter.ToUInt16(data, startIdx);
                T type = (T)Enum.Parse(typeof(T), data[startIdx + 2].ToString());
                byte[] message = data[(startIdx + 3)..(startIdx + 3 + length)];

                _receivers[type](e.IpPort, message);
                startIdx += 3 + length;
            }

            if (startIdx != data.Length)
                Console.WriteLine("*** Received data was formatted incorrectly ***");
        }

        /// <summary>
        /// Called whenever a client connects to the server
        /// </summary>
        protected virtual void ClientConnected(string clientIp)
        {

        }
        private void OnClientConnected(object sender, ConnectionEventArgs e) => ClientConnected(e.IpPort);

        /// <summary>
        /// Called whenever a client disconnects from the server
        /// </summary>
        protected virtual void ClientDisconnected(string clientIp)
        {

        }
        private void OnClientDisconnected(object sender, ConnectionEventArgs e) => ClientDisconnected(e.IpPort);

        private SimpleTcpServer _server;

        private Dictionary<T, Action<string, byte[]>> _receivers;

    }

    internal enum AbstractType
    {
        None = 0,
    }
}
