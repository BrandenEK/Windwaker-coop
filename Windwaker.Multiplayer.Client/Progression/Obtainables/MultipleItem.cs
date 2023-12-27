using Windwaker.Multiplayer.Client.Memory;
using Windwaker.Multiplayer.Client.Notifications;

namespace Windwaker.Multiplayer.Client.Progression.Obtainables
{
    /// <summary>
    /// An item that can have multiple levels, and also stores a bitfield
    /// </summary>
    public class MultipleItem : IObtainable
    {
        private readonly string[] names;
        private readonly uint mainAddress;
        private readonly byte[] mainValues;
        private readonly uint bitfieldAddress;

        private int currentLevel = 0;

        public MultipleItem(string[] names, uint mainAddress, byte[] mainValues, uint bitfieldAddress)
        {
            this.names = names;
            this.mainAddress = mainAddress;
            this.mainValues = mainValues;
            this.bitfieldAddress = bitfieldAddress;
        }

        public void CheckProgress(INotifier notifier, IMemoryReader memoryReader)
        {
            if (currentLevel == names.Length) return;

            int memoryLevel = GetLevelFromValue(memoryReader.Read(mainAddress, 1)[0]);
            if (memoryLevel <= currentLevel) return;

            currentLevel = memoryLevel;
            notifier.Show($"You have obtained {names[memoryLevel - 1]}");
            // Send to server
        }

        public void ReceiveProgress(INotifier notifier, IMemoryReader memoryReader, string player, ProgressUpdate progress)
        {
            if (progress.Value <= 0 || progress.Value > names.Length) return;

            byte main = GetValueFromLevel(progress.Value);
            byte bitfield = GetBitfieldFromLevel(progress.Value);

            currentLevel = progress.Value;
            notifier.Show($"{player} has obtained {names[progress.Value - 1]}");
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
