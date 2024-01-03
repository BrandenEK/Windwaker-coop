using System;
using Windwaker.Multiplayer.Server.Network.Packets;

namespace Windwaker.Multiplayer.Server.Network
{
    public class PacketEventArgs : EventArgs
    {
        public string IpPort { get; }
        public BasePacket Packet { get; }

        public PacketEventArgs(string ipPort, BasePacket packet)
        {
            IpPort = ipPort;
            Packet = packet;
        }
    }
}
