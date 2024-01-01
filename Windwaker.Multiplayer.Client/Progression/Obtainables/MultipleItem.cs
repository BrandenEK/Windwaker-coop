using Windwaker.Multiplayer.Client.Memory;
using Windwaker.Multiplayer.Client.Network;
using Windwaker.Multiplayer.Client.Network.Packets;
using Windwaker.Multiplayer.Client.Notifications;

namespace Windwaker.Multiplayer.Client.Progression.Obtainables
{
    /// <summary>
    /// An item that can have multiple levels, and also stores a bitfield
    /// </summary>
    public class MultipleItem : IObtainable
    {
        private readonly string id;
        private readonly string[] names;
        private readonly uint mainAddress;
        private readonly byte[] mainValues;
        private readonly uint bitfieldAddress;

        private int currentLevel = 0;

        public MultipleItem(string id, string[] names, uint mainAddress, byte[] mainValues, uint bitfieldAddress)
        {
            this.id = id;
            this.names = names;
            this.mainAddress = mainAddress;
            this.mainValues = mainValues;
            this.bitfieldAddress = bitfieldAddress;
        }

        public void CheckProgress(INotifier notifier, IMemoryReader memoryReader, IClient client)
        {
            if (currentLevel == names.Length) return;

            int memoryLevel = GetLevelFromValue(memoryReader.Read(mainAddress, 1)[0]);
            if (memoryLevel <= currentLevel) return;

            currentLevel = memoryLevel;
            notifier.Show($"You have obtained {names[memoryLevel - 1]}");
            client.Send(new ProgressPacket()
            {
                Id = "",
                Value = currentLevel
            });
        }

        public void ReceiveProgress(INotifier notifier, IMemoryReader memoryReader, ProgressPacket packet)
        {
            if (packet.Value <= 0 || packet.Value > names.Length) return;

            byte main = GetValueFromLevel(packet.Value);
            byte bitfield = GetBitfieldFromLevel(packet.Value);

            currentLevel = packet.Value;
            notifier.Show($"{packet.Player} has obtained {names[packet.Value - 1]}");
            memoryReader.Write(mainAddress, new byte[] { main });
            memoryReader.Write(bitfieldAddress, new byte[] { bitfield });
        }

        public void ResetProgress() => currentLevel = 0;

        private int GetLevelFromValue(byte value)
        {
            for (int i = 0; i < mainValues.Length; i++)
            {
                if (mainValues[i] == value)
                    return i + 1;
            }
            return 0;
        }

        private byte GetValueFromLevel(int level)
        {
            if (level == 0 || level > mainValues.Length)
                return 255;

            return mainValues[level - 1];
        }

        private byte GetBitfieldFromLevel(int level)
        {
            byte bitfield = 0;
            for (int i = 0; i < level; i++)
            {
                bitfield |= (byte)(1 << i);
            }
            return bitfield;
        }
    }
}
