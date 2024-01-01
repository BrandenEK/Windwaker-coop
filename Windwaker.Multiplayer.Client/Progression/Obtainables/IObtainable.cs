using Windwaker.Multiplayer.Client.Memory;
using Windwaker.Multiplayer.Client.Network;
using Windwaker.Multiplayer.Client.Network.Packets;
using Windwaker.Multiplayer.Client.Notifications;

namespace Windwaker.Multiplayer.Client.Progression.Obtainables
{
    public interface IObtainable
    {
        public void CheckProgress(INotifier notifier, IMemoryReader memoryReader, IClient client);

        public void ReceiveProgress(INotifier notifier, IMemoryReader memoryReader, ProgressPacket packet);

        public void ResetProgress();
    }
}
