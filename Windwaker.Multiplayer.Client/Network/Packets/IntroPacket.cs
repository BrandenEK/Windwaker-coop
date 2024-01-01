using System;
using System.Collections.Generic;
using System.Linq;

namespace Windwaker.Multiplayer.Client.Network.Packets
{
    public class IntroPacket : BasePacket
    {
        public string Name { get; init; } = string.Empty;
        public string Password { get; init; } = string.Empty;
        public byte Response { get; init; } = 0;
    }

    internal class IntroPacketSerializer : IPacketSerializer
    {
        private const byte PACKET_TYPE = 0;

        // Sends name and password
        public bool TrySerialize(BasePacket p, out byte[] data)
        {
            if (p is not IntroPacket packet)
            {
                data = Array.Empty<byte>();
                return false;
            }

            data = new List<byte>()
                .Concat(packet.Name.Serialize())
                .Concat(packet.Password.Serialize())
                .Append(PACKET_TYPE).ToArray();
            return true;
        }

        // Receives response code
        public bool TryDeserialize(byte[] data, out BasePacket packet)
        {
            if (data[^1] != PACKET_TYPE)
            {
                packet = new InvalidPacket();
                return false;
            }

            packet = new IntroPacket()
            {
                Response = data[0]
            };
            return true;
        }
    }
}
