using System;

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
        public IntroPacket Deserialize(byte[] data)
        {
            throw new System.NotImplementedException();
        }

        public byte[] Serialize(IntroPacket packet)
        {
            throw new System.NotImplementedException();
        }

        public bool CanSerialize(BasePacket packet) =>
            packet.GetType() == typeof(IntroPacket);

        public bool CanDeserialize(byte type) => type == 0;

        public bool TrySerialize(BasePacket packet, out byte[] data)
        {
            if (packet is not IntroPacket iPacket)
            {
                data = Array.Empty<byte>();
                return false;
            }

            data = new byte[] { 5, 6 };
            return true;
        }

        public bool TryDeserialize(byte[] data, out BasePacket packet)
        {
            throw new System.NotImplementedException();
        }
    }
}
