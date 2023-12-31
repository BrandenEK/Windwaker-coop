using Windwaker.Multiplayer.Client.Memory;
using Windwaker.Multiplayer.Client.Network;
using Windwaker.Multiplayer.Client.Notifications;

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

        public void CheckProgress(INotifier notifier, IMemoryReader memoryReader)
        {
            if (currentOwned || memoryReader.Read(mainAddress, 1)[0] != mainValue) return;

            currentOwned = true;
            notifier.Show($"You have obtained {name}");
            // Send to server
        }

        public void ReceiveProgress(INotifier notifier, IMemoryReader memoryReader, string player, ProgressUpdate progress)
        {
            if (progress.Value < 1) return;

            currentOwned = true;
            notifier.Show($"{player} has obtained {name}");
            memoryReader.Write(mainAddress, new byte[] { mainValue });
            memoryReader.Write(bitfieldAddress, new byte[] { 0xFF });
        }

        public void ResetProgress() => currentOwned = false;
    }
}
