using System;
using Windwaker.Multiplayer.Client.Network.Packets;

namespace Windwaker.Multiplayer.Client.Network
{
    public interface IClient
    {
        public bool IsConnected { get; }

        public bool Connect(string ipAddress, int port, string player, string? password);

        public void Disconnect();

        public void Send(BasePacket packet);

        public event EventHandler OnConnect;
        public event EventHandler OnDisconnect;
        public event EventHandler<PacketEventArgs> OnPacketReceived;
    }
}
