using System;
using System.Collections.Generic;
using System.Linq;

namespace Windwaker.Multiplayer.Client.Network.Packets
{
    public class IntroPacket : BasePacket
    {
        public string? Name { get; init; }
        public string? Password { get; init; }
        public byte Response { get; init; }
    }

    internal class IntroPacketSerializer : IPacketSerializer
    {
        private const byte PACKET_TYPE = 0;

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
                .Append(packet.Response)
                .Append(PACKET_TYPE).ToArray();
            return true;
        }

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
                Password = data.Deserialize(nameLength, out byte passwordLength),
                Response = data[passwordLength]
            };
            return true;
        }
    }
}
