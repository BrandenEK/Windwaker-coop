using SuperSimpleTcp;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using Windwaker.Multiplayer.Client.Logging;
using Windwaker.Multiplayer.Client.Network.Packets;

namespace Windwaker.Multiplayer.Client.Network
{
    internal class NetworkClient : IClient
    {
        private SimpleTcpClient? _client;
        private bool _receivedIntro;

        private readonly ILogger _logger;
        private readonly IPacketSerializer _serializer;

        public event EventHandler? OnConnect;
        public event EventHandler? OnDisconnect;
        public event EventHandler<PacketEventArgs>? OnPacketReceived;

        public NetworkClient(ILogger logger, IPacketSerializer serializer)
        {
            _logger = logger;
            _serializer = serializer;

            OnPacketReceived += OnReceiveIntro;
        }

        public bool IsConnected => _client != null && _client.IsConnected && _receivedIntro;

        public bool Connect(string ipAddress, int port, string player, string? password)
        {
            try
            {
                _receivedIntro = false;
                _client = new SimpleTcpClient(ipAddress, port);

                _client.Events.Connected += ServerConnect;
                _client.Events.Disconnected += ServerDisconnect;
                _client.Events.DataReceived += InternalReceive;

                _client.Connect();
                Send(new IntroPacket()
                {
                    Name = player,
                    Password = password ?? "pass"
                });
                return true;
            }
            catch (Exception e) when (e is SocketException || e is TimeoutException)
            {
                _logger.Error($"Failed to connect to {ipAddress}:{port}");
                return false;
            }
        }

        public void Disconnect()
        {
            _client?.Disconnect();
            _client = null;
        }

        private void ServerConnect(object? sender, ConnectionEventArgs e)
        {
            _logger.Info($"Established connection with server");
        }

        private void ServerDisconnect(object? sender, ConnectionEventArgs e)
        {
            _logger.Info($"Lost connection with server");
            OnDisconnect?.Invoke(this, new EventArgs());
        }

        public void Send(BasePacket packet)
        {
            if (_serializer.TrySerialize(packet, out byte[] data))
            {
                InternalSend(data);
                return;
            }

            _logger.Error("Failed to send invalid packet: " + packet.GetType().Name);
        }

        private void InternalSend(byte[] message)
        {
            if (message == null || message.Length == 0 || !IsConnected)
                return;

            List<byte> bytes = new();
            bytes.AddRange(BitConverter.GetBytes((ushort)message.Length));
            bytes.AddRange(message);

            try
            {
                _client!.Send(bytes.ToArray());
            }
            catch (Exception)
            {
                _logger.Error($"*** Couldn't send data to server ***");
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
                    OnPacketReceived?.Invoke(this, new PacketEventArgs(packet));
                else
                    _logger.Error("Failed to receive invalid packet: " + message[^1]);

                startIdx += 3 + length;
            }

            if (startIdx != data.Length)
                _logger.Error("*** Received data was formatted incorrectly ***");
        }

        private void OnReceiveIntro(object? sender, PacketEventArgs e)
        {
            if (e.Packet is not IntroPacket packet) return;

            if (packet.Response == 200)
            {
                _logger.Info($"Connection to server was approved");
                _receivedIntro = true;
                OnConnect?.Invoke(this, new EventArgs());
                return;
            }

            switch (packet.Response)
            {
                case 101: _logger.Warning("Connection refused: Incorrect password"); break;
                case 102: _logger.Warning("Connection refused: Incorrect game"); break;
                case 103: _logger.Warning("Connection refused: Player limit reached"); break;
                case 104: _logger.Warning("Connection refused: Duplicate ip address"); break;
                case 105: _logger.Warning("Connection refused: Duplicate name"); break;
            }
            Disconnect();
        }
    }
}
