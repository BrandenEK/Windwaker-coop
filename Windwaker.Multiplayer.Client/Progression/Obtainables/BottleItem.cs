using Windwaker.Multiplayer.Client.Memory;
using Windwaker.Multiplayer.Client.Network;
using Windwaker.Multiplayer.Client.Network.Packets;
using Windwaker.Multiplayer.Client.Notifications;

namespace Windwaker.Multiplayer.Client.Progression.Obtainables
{
    /// <summary>
    /// An item (bottle) that can be obtained as any value, but is only sent as empty
    /// </summary>
    public class BottleItem : IObtainable
    {
        private readonly string id;
        private readonly uint mainAddress;
        private readonly uint bitfieldAddress;

        private bool currentOwned = false;

        public BottleItem(string id, uint mainAddress, uint bitfieldAddress)
        {
            this.id = id;
            this.mainAddress = mainAddress;
            this.bitfieldAddress = bitfieldAddress;
        }

        public void CheckProgress(INotifier notifier, IMemoryReader memoryReader, IClient client)
        {
            if (currentOwned) return;

            byte memoryLevel = memoryReader.Read(mainAddress, 1)[0];
            if (memoryLevel < 0x50 || memoryLevel > 0x59) return;

            currentOwned = true;
            notifier.Show($"You have obtained a new bottle");
            client.Send(new ProgressPacket()
            {
                Id = id,
                Value = 1
            });
        }

        public void ReceiveProgress(INotifier notifier, IMemoryReader memoryReader, ProgressPacket packet)
        {
            if (packet.Value < 1) return;

            currentOwned = true;
            notifier.Show($"{packet.Player} has obtained a new bottle");
            memoryReader.Write(mainAddress, new byte[] { 0x50 });
            memoryReader.Write(bitfieldAddress, new byte[] { 0xFF });
        }

        public void ResetProgress() => currentOwned = false;
    }
}
