
namespace Windwaker.Multiplayer.Client.Progression.Obtainables
{
    /// <summary>
    /// An item that simply increases in value
    /// </summary>
    internal class ValueItem : IObtainable
    {
        private readonly string name;
        private readonly uint mainAddress;

        private byte currentValue = 0;

        public ValueItem(string name, uint mainAddress)
        {
            this.name = name;
            this.mainAddress = mainAddress;
        }

        public bool TryRead(byte value, out byte progress)
        {
            if (currentValue < value)
            {
                currentValue = progress = value;
                return true;
            }

            progress = 0;
            return false;
        }

        public bool TryWrite(byte value, out byte progress)
        {
            if (value <= currentValue)
            {
                progress = 0;
                return false;
            }

            currentValue = value;
            progress = value;
            // Write to main
            return true;
        }

        public void Reset() => currentValue = 0;

        public string? GetNotificationPart(byte value)
        {
            return value > 0 ? $" obtained {name}" : null;
        }
    }
}
