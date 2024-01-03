using System;
using System.Collections.Generic;
using System.Linq;

namespace Windwaker.Multiplayer.Server.Network.Packets
{
    public class ScenePacket : BasePacket
    {
        public string Player { get; init; } = string.Empty;
        public byte Scene { get; init; } = 0;
    }

    internal class ScenePacketSerializer : IPacketSerializer
    {
        private const byte PACKET_TYPE = 2;

        // Sends player scene id
        public bool TrySerialize(BasePacket p, out byte[] data)
        {
            if (p is not ScenePacket packet)
            {
                data = Array.Empty<byte>();
                return false;
            }

            data = new List<byte>()
                .Concat(packet.Player.Serialize())
                .Append(packet.Scene)
                .Append(PACKET_TYPE).ToArray();
            return true;
        }

        // Receives scene id
        public bool TryDeserialize(byte[] data, out BasePacket packet)
        {
            if (data[^1] != PACKET_TYPE)
            {
                packet = new InvalidPacket();
                return false;
            }

            packet = new ScenePacket()
            {
                Scene = data[0]
            };
            return true;
        }
    }
}
