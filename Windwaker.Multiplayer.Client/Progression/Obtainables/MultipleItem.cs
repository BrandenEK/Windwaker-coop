using Windwaker.Multiplayer.Client.Memory;

namespace Windwaker.Multiplayer.Client.Progression.Obtainables
{
    /// <summary>
    /// An item that can have multiple levels, and also stores a bitfield
    /// </summary>
    internal class MultipleItem : IObtainable
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

        public bool TryRead(IMemoryReader memoryReader, out int value)
        {
            int memoryLevel = GetLevelFromValue(memoryReader.Read(mainAddress, 1)[0]);
            bool shouldUpdate = currentLevel != memoryLevel;

            currentLevel = value = memoryLevel;
            return shouldUpdate;
        }

        public void TryWrite(IMemoryReader memoryReader, int value)
        {
            byte main = GetValueFromLevel(value);
            byte bitfield = GetBitfieldFromLevel(value);

            currentLevel = value;
            memoryReader.Write(mainAddress, new byte[] { main });
            memoryReader.Write(bitfieldAddress, new byte[] { bitfield });
        }

        public void Reset() => currentLevel = 0;

        public string? GetNotificationPart(int value)
        {
            return value > 0 ? $" obtained {names[value - 1]}" : null;
        }

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
