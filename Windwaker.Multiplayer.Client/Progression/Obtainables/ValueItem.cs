using Windwaker.Multiplayer.Client.Memory;
using Windwaker.Multiplayer.Client.Network;
using Windwaker.Multiplayer.Client.Network.Packets;
using Windwaker.Multiplayer.Client.Notifications;

namespace Windwaker.Multiplayer.Client.Progression.Obtainables
{
    /// <summary>
    /// An item that simply increases in value
    /// </summary>
    public class ValueItem : IObtainable
    {
        private readonly string id;
        private readonly string name;
        private readonly uint mainAddress;

        private int currentValue = 0;

        public ValueItem(string id, string name, uint mainAddress)
        {
            this.id = id;
            this.name = name;
            this.mainAddress = mainAddress;
        }

        public void CheckProgress(INotifier notifier, IMemoryReader memoryReader, IClient client)
        {
            byte memoryValue = memoryReader.Read(mainAddress, 1)[0];
            if (memoryValue <= currentValue) return;

            currentValue = memoryValue;
            notifier.Show($"You have obtained {name}");
            client.Send(new ProgressPacket()
            {
                Id = id,
                Value = currentValue
            });
        }

        public void ReceiveProgress(INotifier notifier, IMemoryReader memoryReader, ProgressPacket packet)
        {
            if (packet.Value <= currentValue) return;

            currentValue = packet.Value;
            notifier.Show($"{packet.Player} has obtained {name}");
            memoryReader.Write(mainAddress, new byte[] { (byte)packet.Value });
        }

        public void ResetProgress() => currentValue = 0;
    }
}
