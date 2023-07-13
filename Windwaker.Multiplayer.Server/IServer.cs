
namespace Windwaker.Multiplayer.Server
{
    internal interface IServer
    {
        public bool Start(string ipPort);

        public void Stop();

        public int Port { get; }
    }
}
