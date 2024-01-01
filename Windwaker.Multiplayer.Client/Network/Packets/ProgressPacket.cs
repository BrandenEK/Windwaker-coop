using System;
using System.Collections.Generic;
using System.Linq;

namespace Windwaker.Multiplayer.Client.Network.Packets
{
    public class ProgressPacket : BasePacket
    {
        public string Player { get; init; } = string.Empty;
        public string Id { get; init; } = string.Empty;
        public int Value { get; init; } = 0;
    }

    internal class ProgressPacketSerializer : IPacketSerializer
    {
        private const byte PACKET_TYPE = 1;

        // Sends id and value
        public bool TrySerialize(BasePacket p, out byte[] data)
        {
            if (p is not ProgressPacket packet)
            {
                data = Array.Empty<byte>();
                return false;
            }

            data = new List<byte>()
                .Concat(packet.Id.Serialize())
                .Concat(BitConverter.GetBytes(packet.Value))
                .Append(PACKET_TYPE).ToArray();
            return true;
        }

        // Receives player, id, and value
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
                Value = BitConverter.ToInt32(data, playerLength + idLength)
            };
            return true;
        }
    }
}
