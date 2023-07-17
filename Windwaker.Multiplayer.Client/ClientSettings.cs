
namespace Windwaker.Multiplayer.Client
{
    internal class ClientSettings
    {
        public readonly string playerName;

        public readonly string gameName;

        public readonly string serverIp;

        public readonly int serverPort;

        public readonly string password;

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
