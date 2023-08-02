using SuperSimpleTcp;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using Windwaker.Multiplayer.Client.Progress;

namespace Windwaker.Multiplayer.Client.Network
{
    internal class NetworkManager
    {
        private SimpleTcpClient _client;

        public bool IsConnected => _client != null && _client.IsConnected;

        /// <summary>
        /// Attempts to connect to a server at the specified ip port
        /// </summary>
        public bool Connect(string ip, int port)
        {
            try
            {
                _client = new SimpleTcpClient(ip, port);

                _client.Events.Connected += OnServerConnected;
                _client.Events.Disconnected += OnServerDisconnected;
                _client.Events.DataReceived += Receive;

                _client.Connect();
            }
            catch (Exception e) when (e is SocketException || e is TimeoutException)
            {
                Core.UIManager.LogError($"Failed to connect to {ip}:{port}");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Disconnects from the current server
        /// </summary>
        public void Disconnect()
        {
            _client?.Disconnect();
            _client = null;
        }

        /// <summary>
        /// Called whenever the client connects to the server
        /// </summary>
        private void OnServerConnected(object sender, ConnectionEventArgs e)
        {
            Core.UIManager.Log($"Established connection with server");
            Core.NetworkManager.SendIntro(ClientForm.Settings.ValidPlayerName, ClientForm.Settings.ValidGameName, ClientForm.Settings.ValidPassword);
        }

        /// <summary>
        /// Called whenever the client disconnects from the server
        /// </summary>
        private void OnServerDisconnected(object sender, ConnectionEventArgs e)
        {
            Core.UIManager.Log($"Lost connection with server");
            Core.UIManager.UpdateButtonText();
            Core.ProgressManager.ResetProgress();
            Core.DolphinManager.StopLoop();
        }

        /// <summary>
        /// Sends a message to the server
        /// </summary>
        private void Send(byte[] message, NetworkType type)
        {
            if (message == null || message.Length == 0 || !IsConnected)
                return;

            List<byte> bytes = new();
            bytes.AddRange(BitConverter.GetBytes((ushort)message.Length));
            bytes.Add((byte)type);
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
                NetworkType type = (NetworkType)data[startIdx + 2];
                byte[] message = data[(startIdx + 3)..(startIdx + 3 + length)];

                switch (type)
                {
                    case NetworkType.Intro: ReceiveIntro(message); break;
                    case NetworkType.Scene: ReceiveScene(message); break;
                    case NetworkType.Progress: ReceiveProgress(message); break;
                }
                startIdx += 3 + length;
            }

            if (startIdx != data.Length)
                Core.UIManager.LogError("*** Received data was formatted incorrectly ***");
        }

        // Intro

        public void SendIntro(string player, string game, string password)
        {
            var bytes = new List<byte>();

            bytes.AddRange(SerializeString(player));
            bytes.AddRange(SerializeString(game));
            bytes.AddRange(SerializeString(password));

            Send(bytes.ToArray(), NetworkType.Intro);
        }

        private void ReceiveIntro(byte[] message)
        {
            byte response = message[0];

            if (response == 200)
            {
                Core.UIManager.Log($"Connection to server was approved");
                Core.UIManager.UpdateButtonText();
                Core.DolphinManager.StartLoop();
            }
            else
            {
                switch (response)
                {
                    case 101: Core.UIManager.LogWarning("Connection refused: Incorrect password"); break;
                    case 102: Core.UIManager.LogWarning("Connection refused: Incorrect game"); break;
                    case 103: Core.UIManager.LogWarning("Connection refused: Player limit reached"); break;
                    case 104: Core.UIManager.LogWarning("Connection refused: Duplicate ip address"); break;
                    case 105: Core.UIManager.LogWarning("Connection refused: Duplicate name"); break;
                }
                Disconnect();
            }
        }

        // Scene

        public void SendScene(byte scene)
        {
            Send(new byte[] { scene }, NetworkType.Scene);
        }

        private void ReceiveScene(byte[] message)
        {

        }

        // Progress

        public void SendProgress(ProgressUpdate progress)
        {
            var bytes = new List<byte>();

            bytes.Add((byte)progress.type);
            bytes.Add(progress.value);
            bytes.AddRange(Encoding.UTF8.GetBytes(progress.id));

            Send(bytes.ToArray(), NetworkType.Progress);
        }

        private void ReceiveProgress(byte[] message)
        {
            string playerName = DeserializeString(message, 0, out byte nameLength);
            ProgressType progressType = (ProgressType)message[nameLength];
            byte progressValue = message[nameLength + 1];
            string progressId = Encoding.UTF8.GetString(message, nameLength + 2, message.Length - nameLength - 2);

            // Queue it instead to be processed right after read loop
            var progress = new ProgressUpdate(progressType, progressId, progressValue);
            Core.ProgressManager.ReceiveProgress(playerName, progress);
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
