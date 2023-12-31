using System;
using System.Collections.Generic;
using System.Linq;

namespace Windwaker.Multiplayer.Client.Network.Packets
{
    public class ProgressPacket : BasePacket
    {
        public string? Player { get; init; }
        public string? Id { get; init; }
        public int Value { get; init; }
    }

    internal class ProgressPacketSerializer : IPacketSerializer
    {
        private const byte PACKET_TYPE = 1;

        public bool TrySerialize(BasePacket p, out byte[] data)
        {
            if (p is not ProgressPacket packet)
            {
                data = Array.Empty<byte>();
                return false;
            }

            data = new List<byte>()
                .Concat(packet.Player.Serialize())
                .Concat(packet.Id.Serialize())
                .Concat(BitConverter.GetBytes(packet.Value))
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

            packet = new ProgressPacket()
            {
                Player = data.Deserialize(0, out byte playerLength),
                Id = data.Deserialize(playerLength, out byte idLength),
                Value = BitConverter.ToInt32(data, idLength)
            };
            return true;
        }
    }
}
