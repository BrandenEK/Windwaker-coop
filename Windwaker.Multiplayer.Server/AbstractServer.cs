using SuperSimpleTcp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net.Sockets;

namespace Windwaker.Multiplayer.Server
{
    internal abstract class AbstractServer<T> where T : Enum
    {
        private SimpleTcpServer _server;

        private Dictionary<T, Action<string, byte[]>> _receivers;

        protected bool Initialize(string ipPort, Dictionary<T, Action<string, byte[]>> receivers)
        {
            _receivers = receivers;
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

        public abstract void Start(string ipPort);

        /// <summary>
        /// Starts the server at the specified ip port.  
        /// </summary>
        //public bool Start(string ipPort)
        //{
        //    try
        //    {
        //        _server = new SimpleTcpServer(ipPort);

        //        _server.Events.ClientConnected += OnClientConnected;
        //        _server.Events.ClientDisconnected += OnClientDisconnected;
        //        _server.Events.DataReceived += Receive;

        //        _server.Start();
        //    }
        //    catch (Exception e) when (e is SocketException || e is TimeoutException)
        //    {
        //        Console.WriteLine($"Failed to start server at {ipPort}");
        //        return false;
        //    }

        //    Console.WriteLine($"Started server at {ipPort}");
        //    return true;
        //}

        /// <summary>
        /// Stops the server
        /// </summary>
        //public void Stop()
        //{
        //    if (_server != null)
        //    {
        //        _server.Stop();
        //        _server = null;
        //    }
        //}

        /// <summary>
        /// Called whenever a client connects to the server
        /// ???
        /// </summary>
        private void OnClientConnected(object sender, ConnectionEventArgs e)
        {
            //Console.WriteLine("Client connected: " + e.IpPort);
            ClientConnected(e.IpPort);
        }

        protected virtual void ClientConnected(string playerIp)
        {

        }

        protected virtual void ClientDisconnected(string playerIp)
        {

        }

        /// <summary>
        /// Called whenever a client disconnects from the server
        /// ???
        /// </summary>
        private void OnClientDisconnected(object sender, ConnectionEventArgs e)
        {
            //Console.WriteLine("Client disconnected: " + e.IpPort);
            ClientDisconnected(e.IpPort);
        }

        /// <summary>
        /// Sends a message to the client at the specified ip port
        /// </summary>
        protected void Send(string ip, byte[] message, T type)
        {
            if (message != null && message.Length > 0)
            {
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
        }

        /// <summary>
        /// Converts received data into a type and message and then processes it
        /// </summary>
        private void Receive(object sender, DataReceivedEventArgs e)
        {
            byte[] data = e.Data.ToArray();
            int startIdx = 0;
            Console.WriteLine("Received data");

            while (startIdx < data.Length - 3)
            {
                Console.WriteLine("Processing message");
                ushort length = BitConverter.ToUInt16(data, startIdx);
                //NetworkType type = (NetworkType)data[startIdx + 2];
                T type = (T)Enum.Parse(typeof(T), data[startIdx + 2].ToString());
                byte[] message = data[(startIdx + 3)..(startIdx + 3 + length)];
                Console.WriteLine(type.ToString());
                _receivers[type](e.IpPort, message);
                //Receive(e.IpPort, message, );

                //switch (type)
                //{
                //    case NetworkType.Position: ReceivePosition(e.IpPort, message); break;
                //    case NetworkType.Animation: ReceiveAnimation(e.IpPort, message); break;
                //    case NetworkType.Direction: ReceiveDirection(e.IpPort, message); break;
                //    case NetworkType.EnterScene: ReceiveEnterScene(e.IpPort, message); break;
                //    case NetworkType.LeaveScene: ReceiveLeaveScene(e.IpPort, message); break;
                //    case NetworkType.Skin: ReceiveSkin(e.IpPort, message); break;
                //    case NetworkType.Team: ReceiveTeam(e.IpPort, message); break;
                //    case NetworkType.Connection: ReceiveConnection(e.IpPort, message); break;
                //    case NetworkType.Intro: ReceiveIntro(e.IpPort, message); break;
                //    case NetworkType.Progress: ReceiveProgress(e.IpPort, message); break;
                //    case NetworkType.Attack: ReceiveAttack(e.IpPort, message); break;
                //    case NetworkType.Effect: ReceiveEffect(e.IpPort, message); break;
                //    default:
                //        Console.WriteLine($"*** Data type '{type}' is not valid ***"); break;
                //}
                startIdx += 3 + length;
            }

            if (startIdx != data.Length)
                Console.WriteLine("*** Received data was formatted incorrectly ***");
        }

        //protected abstract void Receive(string playerIp, byte[] message, T type);

        //protected abstract void ReceivePosition(string playerIp, byte[] message);


        //protected abstract void ReceiveAnimation(string playerIp, byte[] message);


        //protected abstract void ReceiveDirection(string playerIp, byte[] message);


        //protected abstract void ReceiveEnterScene(string playerIp, byte[] message);


        //protected abstract void ReceiveLeaveScene(string playerIp, byte[] message);


        //protected abstract void ReceiveSkin(string playerIp, byte[] message);


        //protected abstract void ReceiveTeam(string playerIp, byte[] message);


        //protected abstract void ReceiveConnection(string playerIp, byte[] message);


        //protected abstract void ReceiveIntro(string playerIp, byte[] message);


        //protected abstract void ReceiveProgress(string playerIp, byte[] message);


        //protected abstract void ReceiveAttack(string playerIp, byte[] message);


        //protected abstract void ReceiveEffect(string playerIp, byte[] message);
    }
}
