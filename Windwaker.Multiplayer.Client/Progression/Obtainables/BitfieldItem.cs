using Windwaker.Multiplayer.Client.Memory;

namespace Windwaker.Multiplayer.Client.Progression.Obtainables
{
    /// <summary>
    /// An item that has multiple obtainables, each in a separate bit
    /// </summary>
    internal class BitfieldItem : IObtainable
    {
        private readonly string[] names;
        private readonly uint bitfieldAddress;
        private readonly byte[] bitfieldValues;

        private int currentBitfield = 0;

        public BitfieldItem(string[] names, uint bitfieldAddress, byte[] bitfieldValues)
        {
            this.names = names;
            this.bitfieldAddress = bitfieldAddress;
            this.bitfieldValues = bitfieldValues;
        }

        public bool TryRead(IMemoryReader memoryReader, out int value)
        {
            byte memoryBitfield = memoryReader.Read(bitfieldAddress, 1)[0];
            bool shouldUpdate = currentBitfield != memoryBitfield;
            // Calculate new bitfield instead masked with tracked bits

            currentBitfield = value = memoryBitfield;
            return shouldUpdate;
        }

        public void TryWrite(IMemoryReader memoryReader, int value)
        {
            currentBitfield = value;
            memoryReader.Write(bitfieldAddress, new byte[] { (byte)value });
        }

        public void Reset() => currentBitfield = 0;

        public string? GetNotificationPart(int value)
        {
            for (int i = 0; i < bitfieldValues.Length; i++)
            {
                if (bitfieldValues[i] == value)
                    return $" {names[i]}";
            }
            return null;
        }
    }
}
