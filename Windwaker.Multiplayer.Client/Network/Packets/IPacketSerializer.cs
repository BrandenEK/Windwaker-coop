using System;
using System.Collections.Generic;

namespace Windwaker.Multiplayer.Client.Network.Packets
{
    public abstract class BasePacket { }

    internal interface IPacketSerializer
    {
        //public byte[] Serialize(T packet);
        //public T Deserialize(byte[] data);

        //public bool CanSerialize(BasePacket packet);
        //public bool CanDeserialize(byte type);

        public bool TrySerialize(BasePacket packet, out byte[] data);
        public bool TryDeserialize(byte[] data, out BasePacket packet);
    }

    internal class InvalidPacket : BasePacket { }

    internal class GlobalSerializer : IPacketSerializer
    {
        private readonly List<IPacketSerializer> _serializers;

        public GlobalSerializer()
        {
            _serializers = new List<IPacketSerializer>()
            {
                new IntroPacketSerializer(),
                new ProgressPacketSerializer(),
            };
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
    }

    

    internal class ProgressPacketSerializer : IPacketSerializer
    {
        public ProgressPacket Deserialize(byte[] data)
        {
            throw new System.NotImplementedException();
        }

        public byte[] Serialize(ProgressPacket packet)
        {
            throw new System.NotImplementedException();
        }

        public bool CanSerialize(BasePacket packet) =>
            packet.GetType() == typeof(ProgressPacket);

        public bool CanDeserialize(byte type) => type == 1;

        public bool TrySerialize(BasePacket packet, out byte[] data)
        {
            throw new System.NotImplementedException();
        }

        public bool TryDeserialize(byte[] data, out BasePacket packet)
        {
            throw new System.NotImplementedException();
        }
    }
}
