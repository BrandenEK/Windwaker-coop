
namespace Windwaker.Multiplayer.Client
{
    public class ClientSettings
    {
        public const string DEFAULT_PLAYER_NAME = "New player";
        public const string DEFAULT_GAME_NAME = "Windwaker";
        public const string DEFAULT_SERVER_IP = "127.0.0.1";
        public const int DEFAULT_SERVER_PORT = 8989;

        public readonly string playerName;
        public string ValidPlayerName => playerName ?? DEFAULT_PLAYER_NAME;

        public readonly string gameName;
        public string ValidGameName => gameName ?? DEFAULT_GAME_NAME;

        public readonly string serverIp;
        public string ValidServerIp => serverIp ?? DEFAULT_SERVER_IP;

        public readonly int serverPort;
        public int ValidServerPort => serverPort > 0 ? serverPort : DEFAULT_SERVER_PORT;

        public readonly string password;
        public string ValidPassword => password;

        public ClientSettings(string playerName, string gameName, string serverIp, int serverPort, string password)
        {
            this.playerName = playerName;
            this.gameName = gameName;
            this.serverIp = serverIp;
            this.serverPort = serverPort;
            this.password = password;
        }
    }
}
