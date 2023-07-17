
namespace Windwaker.Multiplayer.Client
{
    internal class ClientSettings
    {
        public const string DEFAULT_PLAYER_NAME = "New player";
        public const string DEFAULT_GAME_NAME = "Windwaker";
        public const string DEFAULT_SERVER_IP = "127.0.0.1";
        public const int DEFAULT_SERVER_PORT = 8989;

        private readonly string _playerName;
        public string PlayerName => _playerName ?? DEFAULT_PLAYER_NAME;
        public string TruePlayerName => _playerName;

        private readonly string _gameName;
        public string GameName => _gameName ?? DEFAULT_GAME_NAME;
        public string TrueGameName => _gameName;

        private readonly string _serverIp;
        public string ServerIp => _serverIp ?? DEFAULT_SERVER_IP;
        public string TrueServerIp => _serverIp;

        private readonly int _serverPort;
        public int ServerPort => _serverPort > 0 ? _serverPort : DEFAULT_SERVER_PORT;
        public int TrueServerPort => _serverPort;

        private readonly string _password;
        public string Password => _password;


        public ClientSettings(string playerName, string gameName, string serverIp, int serverPort, string password)
        {
            _playerName = playerName;
            _gameName = gameName;
            _serverIp = serverIp;
            _serverPort = serverPort;
            _password = password;
        }
    }
}
