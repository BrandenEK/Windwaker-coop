using SuperSimpleTcp;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace Windwaker.Multiplayer.Server
{
    internal class Server
    {
        private SimpleTcpServer _server;

        private readonly Dictionary<string, PlayerData> _connectedPlayers = new();

        public bool IsListening => _server != null && _server.IsListening;

        /// <summary>
        /// Attempts to start the server at the specified ip port.  
        /// </summary>
        public bool Start(string ip, int port)
        {
            try
            {
                _connectedPlayers.Clear();
                _server = new SimpleTcpServer(ip, port);

                _server.Events.ClientConnected += OnClientConnected;
                _server.Events.ClientDisconnected += OnClientDisconnected;
                _server.Events.DataReceived += Receive;

                _server.Start();
            }
            catch (Exception e) when (e is SocketException || e is TimeoutException)
            {
                ServerForm.Log($"Failed to start server at {ip}:{port}");
                return false;
            }

            ServerForm.Log($"Started server at {ip}:{port}");
            return true;
        }

        /// <summary>
        /// Stops the server
        /// </summary>
        public void Stop()
        {
            _connectedPlayers.Clear(); // Maybe remove this once I have a button to test
            _server?.Stop();
            _server.Dispose();
            _server = null;
        }

        /// <summary>
        /// Disconnects the specified client ip from the server
        /// </summary>
        public void DisconnectClient(string ipPort)
        {
            _server?.DisconnectClient(ipPort);
        }

        /// <summary>
        /// Called whenever a client connects to the server
        /// </summary>
        private void OnClientConnected(object sender, ConnectionEventArgs e)
        {
            //ServerForm.Log("Received connection to server");
        }

        /// <summary>
        /// Called whenever a client disconnects from the server
        /// </summary>
        private void OnClientDisconnected(object sender, ConnectionEventArgs e)
        {
            //ServerForm.Log("Client disconnected");
            _connectedPlayers.Remove(e.IpPort);
            ServerForm.UpdatePlayerGrid(_connectedPlayers.Values, _connectedPlayers.Count);
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
                ServerForm.Log($"*** Couldn't send data to {ip} ***");
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
                ServerForm.Log("*** Received data was formatted incorrectly ***");
        }

        // Intro

        public void SendIntro(string playerIp, byte response)
        {
            Send(playerIp, new byte[] { response }, NetworkType.Intro);
        }

        private void ReceiveIntro(string playerIp, byte[] message)
        {
            string player = DeserializeString(message, 0, out byte playerLength);
            string game = DeserializeString(message, 0 + playerLength, out byte gameLength);
            string password = DeserializeString(message, 0 + playerLength + gameLength, out _);

            // Ensure the password is correct
            if (!string.IsNullOrEmpty(ServerForm.Settings.ValidPassword) && password != ServerForm.Settings.ValidPassword)
            {
                ServerForm.Log("Player connection rejected: Incorrect password");
                SendIntro(playerIp, 101);
                return;
            }

            // Ensure the game is correct
            if (ServerForm.Settings.ValidGameName != game)
            {
                ServerForm.Log("Player connection rejected: Incorrect game");
                SendIntro(playerIp, 102);
                return;
            }

            // Ensure that the room doesn't already have the max number of players
            if (_connectedPlayers.Count >= ServerForm.Settings.ValidMaxPlayers)
            {
                ServerForm.Log("Player connection rejected: Player limit reached");
                SendIntro(playerIp, 103);
                return;
            }

            // Ensure there are no duplicate ips
            if (_connectedPlayers.ContainsKey(playerIp))
            {
                ServerForm.Log("Player connection rejected: Duplicate ip address");
                SendIntro(playerIp, 104);
                return;
            }

            // Ensure there are no duplicate names
            foreach (var data in _connectedPlayers.Values)
            {
                if (data.Name == player)
                {
                    ServerForm.Log("Player connection rejected: Duplicate name");
                    SendIntro(playerIp, 105);
                    return;
                }
            }

            // Send acceptance response
            _connectedPlayers.Add(playerIp, new PlayerData(player));
            ServerForm.UpdatePlayerGrid(_connectedPlayers.Values, _connectedPlayers.Count);
            SendIntro(playerIp, 200);
        }

        // Scene

        public void SendScene(string playerIp, byte scene)
        {

        }

        private void ReceiveScene(string playerIp, byte[] message)
        {
            byte scene = message[0];
            _connectedPlayers[playerIp].UpdateScene(scene);
            ServerForm.UpdatePlayerGrid(_connectedPlayers.Values, _connectedPlayers.Count);
            ServerForm.Log("Received new scene: " + _connectedPlayers[playerIp].CurrentSceneName);
        }

        // Progress

        public void SendProgress(string playerIp)
        {

        }

        private void ReceiveProgress(string playerIp, byte[] message)
        {

        }

        // Helpers

        private byte[] SerializeString(string str)
        {
            byte[] stringBytes = Encoding.UTF8.GetBytes(str);
            byte[] outputBytes = new byte[str.Length + 1];

            outputBytes[0] = (byte)str.Length;
            for (int i = 0; i < stringBytes.Length; i++)
            {
                outputBytes[i + 1] = stringBytes[i];
            }

            return outputBytes;
        }

        private string DeserializeString(byte[] bytes, int start, out byte length)
        {
            length = (byte)(bytes[start] + 1);

            return length == 1 ? null : Encoding.UTF8.GetString(bytes, start + 1, length - 1);
        }
    }
}
