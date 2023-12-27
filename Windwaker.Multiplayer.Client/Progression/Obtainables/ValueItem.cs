using Windwaker.Multiplayer.Client.Memory;

namespace Windwaker.Multiplayer.Client.Progression.Obtainables
{
    /// <summary>
    /// An item that simply increases in value
    /// </summary>
    public class ValueItem : IObtainable
    {
        private readonly string name;
        private readonly uint mainAddress;

        private int currentValue = 0;

        public ValueItem(string name, uint mainAddress)
        {
            this.name = name;
            this.mainAddress = mainAddress;
        }

        public bool TryRead(IMemoryReader memoryReader, out int value)
        {
            byte memoryValue = memoryReader.Read(mainAddress, 1)[0];
            bool shouldUpdate = currentValue != memoryValue;

            currentValue = value = memoryValue;
            return shouldUpdate;
        }

        public void TryWrite(IMemoryReader memoryReader, int value)
        {
            currentValue = value;
            memoryReader.Write(mainAddress, new byte[] { (byte)value });
        }

        public void Reset() => currentValue = 0;

        public string? GetNotificationPart(int value)
        {
            return value > 0 ? $" obtained {name}" : null;
        }
    }
}
