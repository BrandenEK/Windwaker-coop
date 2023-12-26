
namespace Windwaker.Multiplayer.Client.Progression.Obtainables
{
    /// <summary>
    /// An item (bottle) that can be obtained as any value, but is only sent as empty
    /// </summary>
    internal class BottleItem : IObtainable
    {
        private readonly uint mainAddress;
        private readonly uint bitfieldAddress;

        private bool currentOwned = false;

        public BottleItem(uint mainAddress, uint bitfieldAddress)
        {
            this.mainAddress = mainAddress;
            this.bitfieldAddress = bitfieldAddress;
        }

        public bool TryRead(byte value, out byte progress)
        {
            if (value >= 0x50 && value <= 0x59 && !currentOwned)
            {
                currentOwned = true;
                progress = 1;
                return true;
            }

            progress = 0;
            return false;
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
            return value > 0 ? $" obtained a new bottle" : null;
        }
    }
}
