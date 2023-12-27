using Windwaker.Multiplayer.Client.Memory;
using Windwaker.Multiplayer.Client.Notifications;

namespace Windwaker.Multiplayer.Client.Progression.Obtainables
{
    public interface IObtainable
    {
        public void CheckProgress(INotifier notifier, IMemoryReader memoryReader);

        public void ReceiveProgress(INotifier notifier, IMemoryReader memoryReader, string player, ProgressUpdate progress);

        public void ResetProgress();
    }
}
