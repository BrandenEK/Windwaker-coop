﻿
namespace Windwaker.Multiplayer.Client.Network.Packets
{
    internal interface IPacketSerializer
    {
        public bool TrySerialize(BasePacket packet, out byte[] data);

        public bool TryDeserialize(byte[] data, out BasePacket packet);
    }
}
