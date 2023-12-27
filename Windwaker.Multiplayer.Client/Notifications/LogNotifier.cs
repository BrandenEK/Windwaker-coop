using Windwaker.Multiplayer.Client.Logging;

namespace Windwaker.Multiplayer.Client.Notifications
{
    internal class LogNotifier : INotifier
    {
        private readonly ILogger _logger;

        public LogNotifier(ILogger logger)
        {
            _logger = logger;
        }

        public void Show(string notification)
        {
            _logger.Warning(notification);
        }
    }
}
