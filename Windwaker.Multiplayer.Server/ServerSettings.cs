
namespace Windwaker.Multiplayer.Server
{
    public class ServerSettings
    {
        public const int DEFAULT_MAX_PLAYERS = 8;
        public const string DEFAULT_GAME_NAME = "Windwaker";
        public const int DEFAULT_SERVER_PORT = 8989;

        public readonly int maxPlayers;

        public readonly string gameName;

        public readonly int serverPort;

        public readonly string password;

        public ServerSettings(int maxPlayers, string gameName, int serverPort, string password)
        {
            this.maxPlayers = maxPlayers;
            this.gameName = gameName;
            this.serverPort = serverPort;
            this.password = password;
        }
    }
}
