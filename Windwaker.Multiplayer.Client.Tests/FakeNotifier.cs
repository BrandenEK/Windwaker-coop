using Windwaker.Multiplayer.Client.Notifications;

namespace Windwaker.Multiplayer.Client.Tests
{
    internal class FakeNotifier : INotifier
    {
        public bool HasShown { get; private set; }

        public void Show(string notification)
        {
            HasShown = true;
        }
    }
}
