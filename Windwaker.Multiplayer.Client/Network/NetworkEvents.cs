using System;
using Windwaker.Multiplayer.Client.Network.Packets;

namespace Windwaker.Multiplayer.Client.Network
{
    public class PacketEventArgs : EventArgs
    {
        public BasePacket Packet { get; }

        public PacketEventArgs(BasePacket packet)
        {
            Packet = packet;
        }
    }
}
