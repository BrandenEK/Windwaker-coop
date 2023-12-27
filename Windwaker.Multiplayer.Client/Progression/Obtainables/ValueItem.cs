using Windwaker.Multiplayer.Client.Memory;
using Windwaker.Multiplayer.Client.Notifications;

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

        public void CheckProgress(INotifier notifier, IMemoryReader memoryReader)
        {
            byte memoryValue = memoryReader.Read(mainAddress, 1)[0];
            if (memoryValue <= currentValue) return;

            currentValue = memoryValue;
            notifier.Show($"You have obtained {name}");
            // Send to server
        }

        public void ReceiveProgress(INotifier notifier, IMemoryReader memoryReader, string player, ProgressUpdate progress)
        {
            if (progress.Value <= currentValue) return;

            currentValue = progress.Value;
            notifier.Show($"{player} has obtained {name}");
            memoryReader.Write(mainAddress, new byte[] { (byte)progress.Value });
        }

        public void ResetProgress() => currentValue = 0;
    }
}
