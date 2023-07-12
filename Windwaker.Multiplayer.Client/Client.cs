using SuperSimpleTcp;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Windows.Forms;

namespace Windwaker.Multiplayer.Client
{
    internal class Client
    {
        private SimpleTcpClient _client;

        public bool IsConnected => _client != null && _client.IsConnected;

        private readonly List<byte> sendingQueue = new();
        private readonly List<byte> receivingQueue = new();
        private static readonly object datalock = new();

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
        /// Adds a message to the sending queue to be sent soon
        /// </summary>
        private void QueueMesssage(byte[] data, NetworkType type)
        {
            if (data == null || data.Length == 0 || !IsConnected)
                return;

            List<byte> bytes = new();
            bytes.AddRange(BitConverter.GetBytes((ushort)data.Length));
            bytes.Add((byte)type);
            bytes.AddRange(data);

            sendingQueue.AddRange(bytes);

            //temp
            SendQueue();
        }

        /// <summary>
        /// Sends eveything in the sending queue to the server/>
        /// </summary>
        public void SendQueue()
        {
            if (sendingQueue.Count == 0 || !IsConnected)
                return;

            byte[] message = sendingQueue.ToArray();
            sendingQueue.Clear();
            _client.Send(message);
        }

        /// <summary>
        /// Adds a message to the receiving queue to be processed soon
        /// </summary>
        private void OnDataReceived(object sender, DataReceivedEventArgs e)
        {
            lock (datalock)
            {
                receivingQueue.AddRange(e.Data);
            }

            //temp
            ProcessQueue();
        }

        /// <summary>
        /// Converts everything in the receiving queue into a type and message and then processes it
        /// </summary>
        public void ProcessQueue()
        {
            lock (datalock)
            {
                if (receivingQueue.Count == 0)
                    return;

                byte[] data = receivingQueue.ToArray();
                receivingQueue.Clear();
                int startIdx = 0;

                while (startIdx < data.Length - 3)
                {
                    ushort length = BitConverter.ToUInt16(data, startIdx);
                    NetworkType type = (NetworkType)data[startIdx + 2];
                    byte[] message = data[(startIdx + 3)..(startIdx + 3 + length)];

                    switch (type)
                    {
                        case NetworkType.Position: ReceiveUnknown(message); break;
                        case NetworkType.Animation: ReceiveUnknown(message); break;
                        case NetworkType.Direction: ReceiveUnknown(message); break;
                        case NetworkType.EnterScene: ReceiveUnknown(message); break;
                        case NetworkType.LeaveScene: ReceiveUnknown(message); break;
                        case NetworkType.Skin: ReceiveUnknown(message); break;
                        case NetworkType.Team: ReceiveUnknown(message); break;
                        case NetworkType.Connection: ReceiveUnknown(message); break;
                        case NetworkType.Intro: ReceiveIntro(message); break;
                        case NetworkType.Progress: ReceiveUnknown(message); break;
                        case NetworkType.Attack: ReceiveUnknown(message); break;
                        case NetworkType.Effect: ReceiveUnknown(message); break;
                        default:
                            MainForm.Log($"*** Data type '{type}' is not valid ***"); break;
                    }
                    startIdx += 3 + length;
                }

                if (startIdx != data.Length)
                    MainForm.Log("*** Received data was formatted incorrectly ***");
            }
        }

        private void ReceiveUnknown(byte[] message)
        {

        }

        private void ReceiveIntro(byte[] message)
        {
            MainForm.Log("Received intro: " + message.Length);
        }
    }
}
