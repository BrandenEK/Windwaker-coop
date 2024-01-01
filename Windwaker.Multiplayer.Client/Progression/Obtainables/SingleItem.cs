using Windwaker.Multiplayer.Client.Memory;
using Windwaker.Multiplayer.Client.Network;
using Windwaker.Multiplayer.Client.Network.Packets;
using Windwaker.Multiplayer.Client.Notifications;

namespace Windwaker.Multiplayer.Client.Progression.Obtainables
{
    /// <summary>
    /// An item that can only be either owned or not owned, and also stores a bitfield
    /// </summary>
    public class SingleItem : IObtainable
    {
        private readonly string id;
        private readonly string name;
        private readonly uint mainAddress;
        private readonly byte mainValue;
        private readonly uint bitfieldAddress;

        private bool currentOwned = false;

        public SingleItem(string id, string name, uint mainAddress, byte mainValue, uint bitfieldAddress)
        {
            this.id = id;
            this.name = name;
            this.mainAddress = mainAddress;
            this.mainValue = mainValue;
            this.bitfieldAddress = bitfieldAddress;
        }

        public void CheckProgress(INotifier notifier, IMemoryReader memoryReader, IClient client)
        {
            if (currentOwned || memoryReader.Read(mainAddress, 1)[0] != mainValue) return;

            currentOwned = true;
            notifier.Show($"You have obtained {name}");
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
            notifier.Show($"{packet.Player} has obtained {name}");
            memoryReader.Write(mainAddress, new byte[] { mainValue });
            memoryReader.Write(bitfieldAddress, new byte[] { 0xFF });
        }

        public void ResetProgress() => currentOwned = false;
    }
}
