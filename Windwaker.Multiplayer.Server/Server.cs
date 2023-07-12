using SuperSimpleTcp;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Windwaker.Multiplayer.Server
{
    internal class Server
    {
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

            Console.WriteLine($"Started server at {ipPort}");
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
        /// Sends a message to the client at the specified ip port
        /// </summary>
        private void Send(string ip, byte[] data, NetworkType dataType)
        {
            if (data != null && data.Length > 0)
            {
                var list = new List<byte>();
                list.AddRange(BitConverter.GetBytes((ushort)data.Length));
                list.Add((byte)dataType);
                list.AddRange(data);

                try
                {
                    _server.Send(ip, list.ToArray());
                }
                catch (Exception)
                {
                    Console.WriteLine($"*** Couldn't send data to {ip} ***");
                }
            }
        }

        /// <summary>
        /// Converts received data into a type and message and then processes it
        /// </summary>
        private void OnDataReceived(object sender, DataReceivedEventArgs e)
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
                    case NetworkType.Position: ReceiveUnknown(e.IpPort, message); break;
                    case NetworkType.Animation: ReceiveUnknown(e.IpPort, message); break;
                    case NetworkType.Direction: ReceiveUnknown(e.IpPort, message); break;
                    case NetworkType.EnterScene: ReceiveUnknown(e.IpPort, message); break;
                    case NetworkType.LeaveScene: ReceiveUnknown(e.IpPort, message); break;
                    case NetworkType.Skin: ReceiveUnknown(e.IpPort, message); break;
                    case NetworkType.Team: ReceiveUnknown(e.IpPort, message); break;
                    case NetworkType.Connection: ReceiveUnknown(e.IpPort, message); break;
                    case NetworkType.Intro: ReceiveIntro(e.IpPort, message); break;
                    case NetworkType.Progress: ReceiveUnknown(e.IpPort, message); break;
                    case NetworkType.Attack: ReceiveUnknown(e.IpPort, message); break;
                    case NetworkType.Effect: ReceiveUnknown(e.IpPort, message); break;
                    default:
                        Console.WriteLine($"*** Data type '{type}' is not valid ***"); break;
                }
                startIdx += 3 + length;
            }

            if (startIdx != data.Length)
                Console.WriteLine("*** Received data was formatted incorrectly ***");
        }

        private void ReceiveUnknown(string playerIp, byte[] message)
        {

        }

        private void ReceiveIntro(string playerIp, byte[] message)
        {
            Console.WriteLine("Received intro: " + message.Length);
            Send(playerIp, new byte[] { 4, 5, 6 }, NetworkType.Intro);
        }
    }
}
