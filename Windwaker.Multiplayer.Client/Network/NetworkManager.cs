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

        public void Initialize()
        {

        }

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
                ClientForm.Log($"Failed to connect to {ip}:{port}");
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
        private void OnServerConnected(object sender, ConnectionEventArgs e) => OnConnect?.Invoke();

        /// <summary>
        /// Called whenever the client disconnects from the server
        /// </summary>
        private void OnServerDisconnected(object sender, ConnectionEventArgs e) => OnDisconnect?.Invoke();

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
                ClientForm.Log("*** Received data was formatted incorrectly ***");
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
                OnIntroValidated?.Invoke();
            }
            else
            {
                switch (response)
                {
                    case 101: ClientForm.Log("Connection refused: Incorrect password"); break;
                    case 102: ClientForm.Log("Connection refused: Incorrect game"); break;
                    case 103: ClientForm.Log("Connection refused: Player limit reached"); break;
                    case 104: ClientForm.Log("Connection refused: Duplicate ip address"); break;
                    case 105: ClientForm.Log("Connection refused: Duplicate name"); break;
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

        public void SendProgress(ProgressType progressType, string progressId, byte progressValue)
        {
            var bytes = new List<byte>();

            bytes.Add((byte)progressType);
            bytes.Add(progressValue);
            bytes.AddRange(Encoding.UTF8.GetBytes(progressId));

            Send(bytes.ToArray(), NetworkType.Progress);
        }

        private void ReceiveProgress(byte[] message)
        {
            string playerName = DeserializeString(message, 0, out byte nameLength);
            ProgressType progressType = (ProgressType)message[nameLength];
            byte progressValue = message[nameLength + 1];
            string progressId = Encoding.UTF8.GetString(message, nameLength + 2, message.Length - nameLength - 2);

            OnReceiveProgress?.Invoke(playerName, new ProgressUpdate(progressType, progressId, progressValue));
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

        public delegate void ValidateIntroEvent();
        public event ValidateIntroEvent OnIntroValidated;

        public delegate void ReceiveProgressEvent(string player, ProgressUpdate progress);
        public event ReceiveProgressEvent OnReceiveProgress;

        public delegate void ConnectionEvent();
        public event ConnectionEvent OnConnect;

        public delegate void DisconnectionEvent();
        public event DisconnectionEvent OnDisconnect;
    }
}
