
namespace Windwaker.Multiplayer.Server
{
    public class ServerSettings
    {
        public const int DEFAULT_MAX_PLAYERS = 8;
        public const string DEFAULT_GAME_NAME = "Windwaker";
        public const int DEFAULT_SERVER_PORT = 8989;

        public readonly int maxPlayers;
        public int ValidMaxPlayers => maxPlayers > 0 ? maxPlayers : DEFAULT_MAX_PLAYERS;

        public readonly string gameName;
        public string ValidGameName => gameName ?? DEFAULT_GAME_NAME;

        public readonly int serverPort;
        public int ValidServerPort => serverPort > 0 ? serverPort : DEFAULT_SERVER_PORT;

        public readonly string password;
        public string ValidPassword => password ?? string.Empty;

        public ServerSettings(int maxPlayers, string gameName, int serverPort, string password)
        {
            this.maxPlayers = maxPlayers;
            this.gameName = gameName;
            this.serverPort = serverPort;
            this.password = password;
        }
    }
}
