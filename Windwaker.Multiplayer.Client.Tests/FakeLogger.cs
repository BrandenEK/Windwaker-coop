using Windwaker.Multiplayer.Client.Logging;

namespace Windwaker.Multiplayer.Client.Tests
{
    internal class FakeLogger : ILogger
    {
        public void Info(object message) { }

        public void Warning(object message) { }

        public void Error(object message) { }

        public void Debug(object message) { }
    }
}
