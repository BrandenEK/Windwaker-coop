using SuperSimpleTcp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using Windwaker.Multiplayer.Server.Logging;
using Windwaker.Multiplayer.Server.Network.Packets;

namespace Windwaker.Multiplayer.Server.Network
{
    internal class NetworkServer : IServer
    {
        private SimpleTcpServer? _server;

        private readonly Dictionary<string, PlayerData> _connectedPlayers = new();
        private readonly ServerProperties _properties;
        private readonly ILogger _logger;
        private readonly IPacketSerializer _serializer;

        public event EventHandler<PacketEventArgs>? OnPacketReceived;

        public NetworkServer(ServerProperties properties, ILogger logger, IPacketSerializer serializer)
        {
            _properties = properties;
            _logger = logger;
            _serializer = serializer;

            OnPacketReceived += OnReceiveIntro;
            OnPacketReceived += OnReceiveProgress;
        }

        public bool IsListening => _server != null && _server.IsListening;

        public bool Start()
        {
            try
            {
                _connectedPlayers.Clear();
                _server = new SimpleTcpServer("127.0.0.1", _properties.Port);

                _server.Events.ClientConnected += OnClientConnected;
                _server.Events.ClientDisconnected += OnClientDisconnected;
                _server.Events.DataReceived += InternalReceive;

                _server.Start();
            }
            catch (Exception e) when (e is SocketException || e is TimeoutException)
            {
                _logger.Error($"Failed to start server on port {_properties.Port}");
                return false;
            }

            _logger.Info($"Started server on port {_properties.Port}");
            return true;
        }

        public void Stop()
        {
            _server?.Stop();
            _server?.Dispose();
            _server = null;
        }

        private void OnClientConnected(object? sender, ConnectionEventArgs e)
        {
            _logger.Debug("Received connection to server");
        }

        private void OnClientDisconnected(object? sender, ConnectionEventArgs e)
        {
            string playerName = _connectedPlayers.TryGetValue(e.IpPort, out PlayerData? player) ? player.Name : "Unknown";
            _logger.Info("Player disconnected: " + playerName);
            _connectedPlayers.Remove(e.IpPort);
            //ServerForm.UpdatePlayerGrid(_connectedPlayers.Values, _connectedPlayers.Count);
        }

        public void Send(string ip, BasePacket packet)
        {
            if (_serializer.TrySerialize(packet, out byte[] data))
            {
                _logger.Debug($"Sending packet: {packet.GetType().Name} ({data.Length})");
                InternalSend(ip, data);
                return;
            }

            _logger.Error("Failed to send invalid packet: " + packet.GetType().Name);
        }

        private void InternalSend(string ip, byte[] message)
        {
            if (message == null || message.Length == 0 || !IsListening)
                return;

            var bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes((ushort)message.Length));
            bytes.AddRange(message);

            try
            {
                _server!.Send(ip, bytes.ToArray());
            }
            catch (Exception)
            {
                _logger.Error($"*** Couldn't send data to {ip} ***");
            }
        }

        private void InternalReceive(object? sender, DataReceivedEventArgs e)
        {
            byte[] data = e.Data.ToArray();
            int startIdx = 0;

            while (startIdx < data.Length - 2)
            {
                ushort length = BitConverter.ToUInt16(data, startIdx);
                byte[] message = data[(startIdx + 2)..(startIdx + 2 + length)];

                if (_serializer.TryDeserialize(message, out BasePacket packet))
                {
                    _logger.Debug($"Receiving packet: {packet.GetType().Name} ({message.Length})");
                    OnPacketReceived?.Invoke(this, new PacketEventArgs(e.IpPort, packet));
                }
                else
                    _logger.Error("Failed to receive invalid packet: " + message[^1]);

                startIdx += 2 + length;
            }

            if (startIdx != data.Length)
                _logger.Error("*** Received data was formatted incorrectly ***");
        }

        private void OnReceiveIntro(object? sender, PacketEventArgs e)
        {
            if (e.Packet is not IntroPacket packet) return;

            // Ensure the password is correct
            if (_properties.Password != string.Empty && packet.Password != _properties.Password)
            {
                _logger.Warning("Player connection rejected: Incorrect password");
                Send(e.IpPort, new IntroPacket() { Response = 101 });
                return;
            }

            // Ensure the game is correct
            //if (ServerForm.Settings.ValidGameName != game)
            //{
            //    _logger.Warning("Player connection rejected: Incorrect game");
            //    SendIntro(playerIp, 102);
            //    return;
            //}

            // Ensure that the room doesn't already have the max number of players
            if (_connectedPlayers.Count >= _properties.MaxPlayers)
            {
                _logger.Warning("Player connection rejected: Player limit reached");
                Send(e.IpPort, new IntroPacket() { Response = 103 });
                return;
            }

            // Ensure there are no duplicate ips
            if (_connectedPlayers.ContainsKey(e.IpPort))
            {
                _logger.Warning("Player connection rejected: Duplicate ip address");
                Send(e.IpPort, new IntroPacket() { Response = 104 });
                return;
            }

            // Ensure there are no duplicate names
            if (_connectedPlayers.Values.Any(p => p.Name == packet.Name))
            {
                _logger.Warning("Player connection rejected: Duplicate name");
                Send(e.IpPort, new IntroPacket() { Response = 105 });
                return;
            }

            // Send acceptance response
            _connectedPlayers.Add(e.IpPort, new PlayerData(packet.Name));
            _logger.Info("Player connected: " + packet.Name);
            Send(e.IpPort, new IntroPacket() { Response = 200 });

            //ServerForm.UpdatePlayerGrid(_connectedPlayers.Values, _connectedPlayers.Count);
            // Also send all current game progress
        }

        // Currently unused
        private void ReceiveScene(string playerIp, ScenePacket packet)
        {
            _connectedPlayers[playerIp].UpdateScene(packet.Scene);
            //ServerForm.UpdatePlayerGrid(_connectedPlayers.Values, _connectedPlayers.Count);
            _logger.Info("Received new scene: " + _connectedPlayers[playerIp].CurrentSceneName);
        }

        // Move this to a progress checker
        private void OnReceiveProgress(object? sender, PacketEventArgs e)
        {
            if (e.Packet is not ProgressPacket packet) return;

            _logger.Info("Received new progress: " + packet.Id);
            var newPacket = new ProgressPacket()
            {
                Player = _connectedPlayers[e.IpPort].Name,
                Id = packet.Id,
                Value = packet.Value,
            };

            foreach (string ip in _connectedPlayers.Keys.Where(x => x != e.IpPort))
                Send(ip, newPacket);
        }
    }
}
