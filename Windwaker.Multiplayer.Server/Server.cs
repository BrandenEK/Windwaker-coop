using SuperSimpleTcp;
using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace Windwaker.Multiplayer.Server
{
    internal class Server
    {
        private SimpleTcpServer _server;

        private readonly Dictionary<string, PlayerData> _connectedPlayers = new();

        /// <summary>
        /// Attempts to start the server at the specified ip port.  
        /// </summary>
        public bool Start(string ipPort)
        {
            try
            {
                _connectedPlayers.Clear();
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
            _connectedPlayers.Clear(); // Maybe remove this once I have a button to test
            _server?.Stop();
            _server = null;
        }

        /// <summary>
        /// Disconnects the specified client ip from the server
        /// </summary>
        public void DisconnectClient(string ipPort)
        {
            _server?.DisconnectClient(ipPort);
        }

        private void DisplayPlayers()
        {
            Console.WriteLine("Connected players:");
            foreach (var player in _connectedPlayers)
            {
                Console.WriteLine($"{player.Key}: {player.Value?.Name ?? "Unknown"}");
            }
            Console.WriteLine();
        }

        /// <summary>
        /// Called whenever a client connects to the server
        /// </summary>
        private void OnClientConnected(object sender, ConnectionEventArgs e)
        {
            Console.WriteLine("Received connection to server");
            if (!_connectedPlayers.ContainsKey(e.IpPort))
                _connectedPlayers.Add(e.IpPort, null);
            DisplayPlayers();
        }

        /// <summary>
        /// Called whenever a client disconnects from the server
        /// </summary>
        private void OnClientDisconnected(object sender, ConnectionEventArgs e)
        {
            Console.WriteLine("Client disconnected");
            _connectedPlayers.Remove(e.IpPort);
            DisplayPlayers();
        }

        /// <summary>
        /// Sends a message to a client at the specified ip port
        /// </summary>
        private void Send(string ip, byte[] message, NetworkType type)
        {
            if (message == null || message.Length == 0)
                return;

            var bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes((ushort)message.Length));
            bytes.Add((byte)type);
            bytes.AddRange(message);

            try
            {
                _server.Send(ip, bytes.ToArray());
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
                NetworkType type = (NetworkType)data[startIdx + 2];
                byte[] message = data[(startIdx + 3)..(startIdx + 3 + length)];

                switch (type)
                {
                    case NetworkType.Intro: ReceiveIntro(e.IpPort, message); break;
                    case NetworkType.Scene: ReceiveScene(e.IpPort, message); break;
                    case NetworkType.Progress: ReceiveProgress(e.IpPort, message); break;
                }
                startIdx += 3 + length;
            }

            if (startIdx != data.Length)
                Console.WriteLine("*** Received data was formatted incorrectly ***");
        }

        // Intro

        public void SendIntro(string playerIp, byte response)
        {
            Send(playerIp, new byte[] { response }, NetworkType.Intro);
            DisplayPlayers();
        }

        private void ReceiveIntro(string playerIp, byte[] message)
        {
            string player = "Test player";
            string game = "Windwaker";
            string password = null;

            // Validate information

            if (_connectedPlayers.ContainsKey(playerIp))
            {
                _connectedPlayers[playerIp] = new PlayerData(player);
                SendIntro(playerIp, 200);
            }
            else
            {
                Console.WriteLine("Received data from disconnected player??");
            }
        }

        // Scene

        public void SendScene(string playerIp, string scene)
        {

        }

        private void ReceiveScene(string playerIp, byte[] message)
        {

        }

        // Progress

        public void SendProgress(string playerIp)
        {

        }

        private void ReceiveProgress(string playerIp, byte[] message)
        {

        }
    }
}
