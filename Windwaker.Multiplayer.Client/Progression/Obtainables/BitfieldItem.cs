using Windwaker.Multiplayer.Client.Memory;
using Windwaker.Multiplayer.Client.Notifications;

namespace Windwaker.Multiplayer.Client.Progression.Obtainables
{
    /// <summary>
    /// An item that has multiple obtainables, each in a separate bit
    /// </summary>
    public class BitfieldItem : IObtainable
    {
        private readonly string[] names;
        private readonly uint bitfieldAddress;
        private readonly byte[] bitfieldValues;

        private int currentBitfield = 0;

        public BitfieldItem(string[] names, uint bitfieldAddress, byte[] bitfieldValues)
        {
            this.names = names;
            this.bitfieldAddress = bitfieldAddress;
            this.bitfieldValues = bitfieldValues;
        }

        public void CheckProgress(INotifier notifier, IMemoryReader memoryReader)
        {
            byte memoryBitfield = memoryReader.Read(bitfieldAddress, 1)[0];
            byte progressMask = 0;

            for (int i = 0; i < bitfieldValues.Length; i++)
            {
                byte mask = bitfieldValues[i];
                if ((currentBitfield & mask) == 0 && (memoryBitfield & mask) != 0)
                {
                    progressMask |= mask;
                    notifier.Show($"You have {names[i]}");
                }
            }

            if (progressMask == 0) return;

            currentBitfield |= progressMask;
            // Send to server
        }

        public void ReceiveProgress(INotifier notifier, IMemoryReader memoryReader, string player, ProgressUpdate progress)
        {
            if ((currentBitfield & progress.Value) == progress.Value) return;

            for (int i = 0; i < bitfieldValues.Length; i++)
            {
                byte mask = bitfieldValues[i];
                if ((currentBitfield & mask) == 0 && (progress.Value & mask) != 0)
                {
                    notifier.Show($"{player} has {names[i]}");
                }
            }

            currentBitfield |= progress.Value;
            memoryReader.Write(bitfieldAddress, new byte[] { (byte)currentBitfield });
        }

        public void ResetProgress() => currentBitfield = 0;
    }
}
