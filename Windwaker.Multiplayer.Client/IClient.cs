
namespace Windwaker.Multiplayer.Client
{
    internal interface IClient
    {
        public bool Connect(string ipPort);

        public void Disconnect();

        public bool IsConnected { get; }
    }
}
