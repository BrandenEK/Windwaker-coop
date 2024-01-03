
namespace Windwaker.Multiplayer.Client.Logging
{
    internal class MultiLogger : ILogger
    {
        private readonly ILogger[] _loggers;

        public MultiLogger(params ILogger[] loggers)
        {
            _loggers = loggers;
        }

        public void Info(object message)
        {
            foreach (var logger in _loggers)
                logger.Info(message);
        }

        public void Warning(object message)
        {
            foreach (var logger in _loggers)
                logger.Warning(message);
        }

        public void Error(object message)
        {
            foreach (var logger in _loggers)
                logger.Error(message);
        }

        public void Debug(object message)
        {
            foreach (var logger in _loggers)
                logger.Debug(message);
        }
    }
}
