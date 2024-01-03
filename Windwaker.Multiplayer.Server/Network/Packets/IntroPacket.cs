using System;
using System.Collections.Generic;
using System.Linq;

namespace Windwaker.Multiplayer.Server.Network.Packets
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

        // Sends response code
        public bool TrySerialize(BasePacket p, out byte[] data)
        {
            if (p is not IntroPacket packet)
            {
                data = Array.Empty<byte>();
                return false;
            }

            data = new List<byte>()
                .Append(packet.Response)
                .Append(PACKET_TYPE).ToArray();
            return true;
        }

        // Receives name and password
        public bool TryDeserialize(byte[] data, out BasePacket packet)
        {
            if (data[^1] != PACKET_TYPE)
            {
                packet = new InvalidPacket();
                return false;
            }

            packet = new IntroPacket()
            {
                Name = data.Deserialize(0, out byte nameLength),
                Password = data.Deserialize(nameLength, out byte _)
            };
            return true;
        }
    }
}
