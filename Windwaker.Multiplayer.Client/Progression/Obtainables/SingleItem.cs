
namespace Windwaker.Multiplayer.Client.Progression.Obtainables
{
    /// <summary>
    /// An item that can only be either owned or not owned, and also stores a bitfield
    /// </summary>
    internal class SingleItem : IObtainable
    {
        private readonly string name;
        private readonly uint mainAddress;
        private readonly byte mainValue;
        private readonly uint bitfieldAddress;

        private bool currentOwned = false;

        public SingleItem(string name, uint mainAddress, byte mainValue, uint bitfieldAddress)
        {
            this.name = name;
            this.mainAddress = mainAddress;
            this.mainValue = mainValue;
            this.bitfieldAddress = bitfieldAddress;
        }

        public bool TryRead(byte value, out byte progress)
        {
            // If already owned or the value is still unowned, no progress
            if (currentOwned || value != mainValue)
            {
                progress = 0;
                return false;
            }

            currentOwned = true;
            progress = 1;
            return true;
        }

        public bool TryWrite(byte value, out byte progress)
        {
            if (value == 0)
            {
                progress = 0;
                return false;
            }

            currentOwned = true;
            progress = 1;
            // Write to main and to bitfield
            return true;
        }

        public void Reset() => currentOwned = false;

        public string? GetNotificationPart(byte value)
        {
            return value > 0 ? $" obtained {name}" : null;
        }
    }
}
