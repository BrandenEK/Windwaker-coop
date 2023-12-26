using Windwaker.Multiplayer.Client.Memory;

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

        public bool TryRead(IMemoryReader memoryReader, out int value)
        {
            byte memoryBottle = memoryReader.Read(mainAddress, 1)[0];
            bool memoryOwned = memoryBottle >= 0x50 && memoryBottle <= 0x59;
            bool shouldUpdate = currentOwned != memoryOwned;

            value = memoryOwned ? 1 : 0;
            currentOwned = memoryOwned;
            return shouldUpdate;
        }

        public void TryWrite(IMemoryReader memoryReader, int value)
        {
            bool hasItem = value == 1;

            currentOwned = hasItem;
            memoryReader.Write(mainAddress, new byte[] { (byte)(hasItem ? 0x50 : 0xFF) });
            memoryReader.Write(bitfieldAddress, new byte[] { (byte)(hasItem ? 0xFF : 0x00) });
        }

        public void Reset() => currentOwned = false;

        public string? GetNotificationPart(int value)
        {
            return value > 0 ? $" obtained a new bottle" : null;
        }
    }
}
