using System;
using Windwaker.Multiplayer.Server.Network.Packets;

namespace Windwaker.Multiplayer.Server.Network
{
    public interface IServer
    {
        public bool Start();

        public void Stop();

        public void Send(string ip, BasePacket packet);

        public event EventHandler<PacketEventArgs> OnPacketReceived;
    }
}
