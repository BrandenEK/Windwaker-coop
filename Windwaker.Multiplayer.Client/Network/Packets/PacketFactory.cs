using System;
using System.Collections.Generic;

namespace Windwaker.Multiplayer.Client.Network.Packets
{
    internal class PacketFactory
    {
        public static IEnumerable<byte> SerializePacket(BasePacket packet)
        {
            return packet switch
            {
                IntroPacket iPacket => SerializeIntro(iPacket),
                ProgressPacket pPacket => SerializeProgress(pPacket),
                ScenePacket sPacket => SerializeScene(sPacket),
                _ => throw new Exception("Invalid packet type: " + packet.GetType().Name),
            };
        }

        public static BasePacket DeserializePacket(byte[] data)
        {
            return data[^1] switch
            {
                0 => DeserializeIntro(data),
                1 => DeserializeProgress(data),
                2 => DeserializeScene(data),
                _ => throw new Exception("Invalid packet type: " + data[^1]),
            };
        }

        public static IEnumerable<byte> SerializeIntro(IntroPacket packet)
        {
            var bytes = new List<byte>();
            bytes.AddRange(packet.Name.Serialize());
            bytes.AddRange(packet.Password.Serialize());
            bytes.Add(packet.Response);

            bytes.Add(0);
            return bytes;
        }

        public static IntroPacket DeserializeIntro(byte[] data)
        {
            return new IntroPacket()
            {
                Name = data.Deserialize(0, out byte nameLength),
                Password = data.Deserialize(nameLength, out byte passwordLength),
                Response = data[passwordLength]
            };
        }

        public static IEnumerable<byte> SerializeProgress(ProgressPacket packet)
        {
            var bytes = new List<byte>();
            bytes.AddRange(packet.Player.Serialize());
            bytes.AddRange(packet.Id.Serialize());
            bytes.AddRange(BitConverter.GetBytes(packet.Value));

            bytes.Add(1);
            return bytes;
        }

        public static ProgressPacket DeserializeProgress(byte[] data)
        {
            return new ProgressPacket()
            {
                Player = data.Deserialize(0, out byte playerLength),
                Id = data.Deserialize(playerLength, out byte idLength),
                Value = BitConverter.ToInt32(data, idLength)
            };
        }

        public static IEnumerable<byte> SerializeScene(ScenePacket packet)
        {
            var bytes = new List<byte>();
            bytes.AddRange(packet.Player.Serialize());
            bytes.Add(packet.Scene);

            bytes.Add(2);
            return bytes;
        }

        public static ScenePacket DeserializeScene(byte[] data)
        {
            return new ScenePacket()
            {
                Player = data.Deserialize(0, out byte playerLength),
                Scene = data[playerLength]
            };
        }
    }
}
