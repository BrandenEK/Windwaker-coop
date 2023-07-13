using SuperSimpleTcp;
using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace Windwaker.Multiplayer.Client
{
    internal abstract class AbstractClient<T> where T : Enum
    {
        private SimpleTcpClient _client;

        private Dictionary<T, Action<byte[]>> _receivers;

        public bool IsConnected => _client != null && _client.IsConnected;

        /// <summary>
        /// Initializes the client with the given receive methods
        /// </summary>
        protected void Initialize(Dictionary<T, Action<byte[]>> receivers)
        {
            _receivers = receivers;
        }

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
                _client.Events.DataReceived += Receive;

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
        /// Sends a message to the server
        /// </summary>
        protected void Send(byte[] message, T type)
        {
            if (message == null || message.Length == 0 || !IsConnected)
                return;

            List<byte> bytes = new();
            bytes.AddRange(BitConverter.GetBytes((ushort)message.Length));
            bytes.Add((byte)((object)type));
            bytes.AddRange(message);

            try
            {
                _client.Send(bytes.ToArray());
            }
            catch (Exception)
            {
                Console.WriteLine($"*** Couldn't send data to server ***");
            }
        }

        /// <summary>
        /// Receives a message from the server and calls the specific receive method to process it
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

                _receivers[type](message);
                startIdx += 3 + length;
            }

            if (startIdx != data.Length)
                MainForm.Log("*** Received data was formatted incorrectly ***");
        }

        /// <summary>
        /// Called whenever the client connects to the server
        /// </summary>
        protected virtual void ServerConnected(string serverIp)
        {

        }
        private void OnServerConnected(object sender, ConnectionEventArgs e) => ServerConnected(e.IpPort);

        /// <summary>
        /// Called whenever the client disconnects from the server
        /// </summary>
        protected virtual void ServerDisconnected(string serverIp)
        {

        }
        private void OnServerDisconnected(object sender, ConnectionEventArgs e) => ServerDisconnected(e.IpPort);
    }

    internal enum AbstractType
    {
        None = 0,
    }
}
