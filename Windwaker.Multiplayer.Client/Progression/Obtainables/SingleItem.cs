using Windwaker.Multiplayer.Client.Memory;

namespace Windwaker.Multiplayer.Client.Progression.Obtainables
{
    /// <summary>
    /// An item that can only be either owned or not owned, and also stores a bitfield
    /// </summary>
    public class SingleItem : IObtainable
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

        public bool TryRead(IMemoryReader memoryReader, out int value)
        {
            bool memoryOwned = memoryReader.Read(mainAddress, 1)[0] == mainValue;
            bool shouldUpdate = currentOwned != memoryOwned;

            value = memoryOwned ? 1 : 0;
            currentOwned = memoryOwned;
            return shouldUpdate;
        }

        public void TryWrite(IMemoryReader memoryReader, int value)
        {
            bool hasItem = value == 1;

            currentOwned = hasItem;
            memoryReader.Write(mainAddress, new byte[] { (byte)(hasItem ? mainValue : 0xFF) });
            memoryReader.Write(bitfieldAddress, new byte[] { (byte)(hasItem ? 0xFF : 0x00) });
        }

        public void Reset() => currentOwned = false;

        public string? GetNotificationPart(int value)
        {
            return value > 0 ? $" obtained {name}" : null;
        }
    }
}
