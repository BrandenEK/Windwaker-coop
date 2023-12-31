//using SuperSimpleTcp;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Net.Sockets;
//using Windwaker.Multiplayer.Server.Logging;
//using Windwaker.Multiplayer.Core.Network.Packets;

//namespace Windwaker.Multiplayer.Server.Network
//{
//    internal class NetworkServer : IServer
//    {
//        private SimpleTcpServer? _server;

//        private readonly Dictionary<string, PlayerData> _connectedPlayers = new();
//        private readonly ServerProperties _properties;
//        private readonly ILogger _logger;

//        public NetworkServer(ServerProperties properties, ILogger logger)
//        {
//            _properties = properties;
//            _logger = logger;
//        }

//        public bool IsListening => _server != null && _server.IsListening;

//        public bool Start()
//        {
//            try
//            {
//                _connectedPlayers.Clear();
//                _server = new SimpleTcpServer("127.0.0.1", _properties.Port);

//                _server.Events.ClientConnected += OnClientConnected;
//                _server.Events.ClientDisconnected += OnClientDisconnected;
//                _server.Events.DataReceived += InternalReceive;

//                _server.Start();
//            }
//            catch (Exception e) when (e is SocketException || e is TimeoutException)
//            {
//                _logger.Error($"Failed to start server on port {_properties.Port}");
//                return false;
//            }

//            _logger.Info($"Started server on port {_properties.Port}");
//            return true;
//        }

//        public void Stop()
//        {
//            _server?.Stop();
//            _server?.Dispose();
//            _server = null;
//        }

//        public void DisconnectClient(string ipPort)
//        {
//            _server?.DisconnectClient(ipPort);
//        }

//        private void OnClientConnected(object? sender, ConnectionEventArgs e)
//        {
//            _logger.Info("Received connection to server");
//        }

//        private void OnClientDisconnected(object? sender, ConnectionEventArgs e)
//        {
//            _logger.Info("Client disconnected");
//            _connectedPlayers.Remove(e.IpPort);
//            //ServerForm.UpdatePlayerGrid(_connectedPlayers.Values, _connectedPlayers.Count);
//        }

//        public void Send(string ip, IPacket packet)
//        {
//            InternalSend(ip, packet.Serialize(), packet.PacketType);
//        }

//        private void InternalSend(string ip, byte[] message, byte type)
//        {
//            if (message == null || message.Length == 0 || !IsListening)
//                return;

//            var bytes = new List<byte>();
//            bytes.AddRange(BitConverter.GetBytes((ushort)message.Length));
//            bytes.Add(type);
//            bytes.AddRange(message);

//            try
//            {
//                _server!.Send(ip, bytes.ToArray());
//            }
//            catch (Exception)
//            {
//                _logger.Error($"*** Couldn't send data to {ip} ***");
//            }
//        }

//        private void InternalReceive(object? sender, DataReceivedEventArgs e)
//        {
//            byte[] data = e.Data.ToArray();
//            int startIdx = 0;

//            while (startIdx < data.Length - 3)
//            {
//                ushort length = BitConverter.ToUInt16(data, startIdx);
//                byte type = data[startIdx + 2];
//                byte[] message = data[(startIdx + 3)..(startIdx + 3 + length)];

//                switch (type)
//                {
//                    case 0: ReceiveIntro(e.IpPort, new IntroPacket(message)); break;
//                    case 1: ReceiveProgress(e.IpPort, new ProgressPacket(message)); break;
//                    case 2: ReceiveScene(e.IpPort, new ScenePacket(message)); break;
//                    default: _logger.Error("Invalid packet type received: " + type); break;
//                }
//                startIdx += 3 + length;
//            }

//            if (startIdx != data.Length)
//                _logger.Error("*** Received data was formatted incorrectly ***");
//        }

//        private void ReceiveIntro(string playerIp, IntroPacket packet)
//        {
//            // Ensure the password is correct
//            if (_properties.Password != string.Empty && packet.Password != _properties.Password)
//            {
//                _logger.Warning("Player connection rejected: Incorrect password");
//                Send(playerIp, new IntroPacket(packet.Name, packet.Password, 101));
//                return;
//            }

//            // Ensure the game is correct
//            //if (ServerForm.Settings.ValidGameName != game)
//            //{
//            //    _logger.Warning("Player connection rejected: Incorrect game");
//            //    SendIntro(playerIp, 102);
//            //    return;
//            //}

//            // Ensure that the room doesn't already have the max number of players
//            if (_connectedPlayers.Count >= _properties.MaxPlayers)
//            {
//                _logger.Warning("Player connection rejected: Player limit reached");
//                Send(playerIp, new IntroPacket(packet.Name, packet.Password, 103));
//                return;
//            }

//            // Ensure there are no duplicate ips
//            if (_connectedPlayers.ContainsKey(playerIp))
//            {
//                _logger.Warning("Player connection rejected: Duplicate ip address");
//                Send(playerIp, new IntroPacket(packet.Name, packet.Password, 104));
//                return;
//            }

//            // Ensure there are no duplicate names
//            if (_connectedPlayers.Values.Any(p => p.Name == packet.Name))
//            {
//                _logger.Warning("Player connection rejected: Duplicate name");
//                Send(playerIp, new IntroPacket(packet.Name, packet.Password, 105));
//                return;
//            }

//            // Send acceptance response
//            _connectedPlayers.Add(playerIp, new PlayerData(packet.Name));
//            Send(playerIp, new IntroPacket(packet.Name, packet.Password, 200));

//            //ServerForm.UpdatePlayerGrid(_connectedPlayers.Values, _connectedPlayers.Count);
//            // Also send all current game progress
//        }

//        private void ReceiveScene(string playerIp, ScenePacket packet)
//        {
//            _connectedPlayers[playerIp].UpdateScene(packet.Scene);
//            //ServerForm.UpdatePlayerGrid(_connectedPlayers.Values, _connectedPlayers.Count);
//            _logger.Info("Received new scene: " + _connectedPlayers[playerIp].CurrentSceneName);
//        }

//        private void ReceiveProgress(string playerIp, ProgressPacket packet)
//        {
//            _logger.Info("Received new progress: " + packet.Id);
//            foreach (string ip in _connectedPlayers.Keys.Where(x => x != playerIp))
//                Send(ip, packet);
//        }
//    }
//}
