
namespace Windwaker.Multiplayer.Server
{
    internal class ServerProperties
    {
        public int Port { get; }
        public string Password { get; }
        public int MaxPlayers { get; }

        public ServerProperties(int port, string password, int maxPlayers)
        {
            Port = port;
            Password = password;
            MaxPlayers = maxPlayers;
        }
    }
}
