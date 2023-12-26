
namespace Windwaker.Multiplayer.Client.Logging
{
    internal interface ILogger
    {
        public void Info(object message);

        public void Warning(object message);

        public void Error(object message);
    }
}
