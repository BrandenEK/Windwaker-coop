using System;
using System.Collections.Generic;

namespace Windwaker.Multiplayer.Server.Network.Packets
{
    internal class GlobalPacketSerializer : IPacketSerializer
    {
        private readonly List<IPacketSerializer> _serializers;

        public GlobalPacketSerializer()
        {
            _serializers = new List<IPacketSerializer>()
            {
                new IntroPacketSerializer(),
                new ProgressPacketSerializer(),
                new ScenePacketSerializer(),
            };
        }

        public bool TrySerialize(BasePacket packet, out byte[] data)
        {
            foreach (var serializer in _serializers)
            {
                if (serializer.TrySerialize(packet, out data))
                    return true;
            }

            data = Array.Empty<byte>();
            return false;
        }

        public bool TryDeserialize(byte[] data, out BasePacket packet)
        {
            foreach (var serializer in _serializers)
            {
                if (serializer.TryDeserialize(data, out packet))
                    return true;
            }

            packet = new InvalidPacket();
            return false;
        }
    }
}
