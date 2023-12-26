
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

        private byte currentBitfield = 0;

        public BitfieldItem(string[] names, uint bitfieldAddress, byte[] bitfieldValues)
        {
            this.names = names;
            this.bitfieldAddress = bitfieldAddress;
            this.bitfieldValues = bitfieldValues;
        }

        public bool TryRead(byte value, out byte progress)
        {
            progress = 0;

            foreach (byte b in bitfieldValues)
            {
                if ((currentBitfield & b) == 0 && (value & b) != 0)
                {
                    currentBitfield |= b;
                    progress |= b;
                }
            }

            return progress != 0;
        }

        public bool TryWrite(byte value, out byte progress)
        {
            if ((currentBitfield & value) == value)
            {
                progress = 0;
                return false;
            }

            progress = (byte)((currentBitfield ^ value) & value); // Check this
            currentBitfield |= value;
            // Write to bitfield
            return true;
        }

        public void Reset() => currentBitfield = 0;

        public string? GetNotificationPart(byte value)
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
