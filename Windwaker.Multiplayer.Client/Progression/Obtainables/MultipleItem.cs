
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

        public bool TryRead(byte value, out byte progress)
        {
            for (int i = mainValues.Length - 1; i >= 0; i--)
            {
                byte level = (byte)(i + 1);
                if (value == mainValues[i] && currentLevel < level)
                {
                    currentLevel = progress = level;
                    return true;
                }
            }

            progress = 0;
            return false;
        }

        public bool TryWrite(byte value, out byte progress)
        {
            if (value <= currentLevel)
            {
                progress = 0;
                return false;
            }

            byte bitfield = 0x00;
            for (int i = 0; i < value; i++)
            {
                bitfield |= (byte)(1 << i);
            }

            currentLevel = value;
            progress = value;
            // Write to main and to bitfield
            return true;
        }

        public void Reset() => currentLevel = 0;

        public string? GetNotificationPart(byte value)
        {
            return value > 0 ? $" obtained {names[value - 1]}" : null;
        }
    }
}
