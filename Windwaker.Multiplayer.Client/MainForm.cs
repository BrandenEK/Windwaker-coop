using System;
using System.Windows.Forms;

namespace Windwaker.Multiplayer.Client
{
    public partial class MainForm : Form
    {
        private static MainForm instance;

        public MainForm()
        {
            InitializeComponent();
            instance ??= this;
        }

        private readonly Client _client = new();

        private ClientSettings _settings;

        private bool IsConnectedToGame => _client.IsConnected;

        /// <summary>
        /// When the connect button is clicked, either connect/disconnect from the server
        /// </summary>
        private void OnClickConnect(object sender, EventArgs e)
        {
            if (IsConnectedToGame)
            {
                _client.Disconnect();
            }
            else
            {
                if (ValidateAndUpdateSettings())
                {
                    _client.Connect(_settings.serverIp, _settings.serverPort);
                }
                else
                {
                    Log("Enter valid data for the input fields!");
                }
            }
        }

        /// <summary>
        /// Ensures that the input fields contain valid info, and if so, update the client settings
        /// </summary>
        /// <returns></returns>
        private bool ValidateAndUpdateSettings()
        {
            // Ensure that a name is present
            string playerName = playerNameField.Text.Trim();
            if (playerName.Length == 0)
            {
                return false;
            }

            // The game will always be ww for now
            string gameName = "Windwaker";

            // Ensure that an ip is present and doesn't contain a colon
            string serverIp = serverIpField.Text.Trim();
            if (serverIp.Length == 0 || serverIp.Contains(':'))
            {
                return false;
            }

            // Ensure that the port is a valid number
            if (!int.TryParse(serverPortField.Text.Trim(), out int serverPort))
            {
                return false;
            }

            // No validation for password, it doesn't need to be present
            string password = passwordField.Text.Trim();

            _settings = new ClientSettings(playerName, gameName, serverIp, serverPort, password);
            return true;
        }

        /// <summary>
        /// Whenever the connection status is changed, update the button UI
        /// </summary>
        public static void UpdateUI()
        {
            instance.connectBtn.Text = instance.IsConnectedToGame ? "Disconnect" : "Connect";
        }

        /// <summary>
        /// When the form is opened, load the last used ip from settings
        /// </summary>
        private void OnFormOpen(object sender, EventArgs e)
        {
            string playerName = Properties.Settings.Default.playerName;
            string gameName = Properties.Settings.Default.gameName;
            string serverIp = Properties.Settings.Default.serverIp;
            int serverPort = Properties.Settings.Default.serverPort;
            string password = Properties.Settings.Default.password;

            _settings = new ClientSettings(playerName, gameName, serverIp, serverPort, password);
            playerNameField.Text = playerName;
            // Set game name
            serverIpField.Text = serverIp;
            serverPortField.Text = serverPort > 0 ? serverPort.ToString() : string.Empty;
            passwordField.Text = password;
        }

        /// <summary>
        /// When the form is closed, save the last used ip to settings
        /// </summary>
        private void OnFormClose(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.playerName = _settings.playerName;
            Properties.Settings.Default.gameName = _settings.gameName;
            Properties.Settings.Default.serverIp = _settings.serverIp;
            Properties.Settings.Default.serverPort = _settings.serverPort;
            Properties.Settings.Default.password = _settings.password;
            Properties.Settings.Default.Save();
        }

        /// <summary>
        /// Logs a message to the debug console
        /// </summary>
        public static void Log(string message)
        {
            instance.debugText.Text += message + "\r\n";
        }
    }
}