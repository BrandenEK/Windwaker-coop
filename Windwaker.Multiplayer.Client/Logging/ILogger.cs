
namespace Windwaker.Multiplayer.Client.Logging
{
    public interface ILogger
    {
        public void Info(object message);

        public void Warning(object message);

        public void Error(object message);

        public void Debug(object message);
    }
}
