using System;
using System.Windows.Forms;

namespace Windwaker.Multiplayer.Client
{
    internal partial class ClientForm : Form
    {
        private static ClientForm instance;

        public ClientForm()
        {
            InitializeComponent();
            instance ??= this;
        }

        public static ClientSettings Settings => instance._settings;
        private ClientSettings _settings;

        private bool IsConnectedToGame => Core.NetworkManager.IsConnected;

        /// <summary>
        /// When the connect button is clicked, either connect/disconnect from the server
        /// </summary>
        private void OnClickConnect(object sender, EventArgs e)
        {
            if (IsConnectedToGame)
            {
                Core.NetworkManager.Disconnect();
            }
            else
            {
                _settings = ValidateInputFields();
                Core.NetworkManager.Connect(_settings.ValidServerIp, _settings.ValidServerPort);
            }
        }

        /// <summary>
        /// Once this player is accepted to the server, begin reading memory and syncing it
        /// </summary>
        private void OnIntroValidated()
        {
            Log($"Connection to server was approved");
            connectBtn.Text = "Disconnect";
        }

        /// <summary>
        /// Once this player is connected to the server, send introductory data
        /// </summary>
        private void OnConnect()
        {
            Log($"Established connection with server");
            Core.NetworkManager.SendIntro(Settings.ValidPlayerName, Settings.ValidGameName, Settings.ValidPassword);
        }

        /// <summary>
        /// Once this player is disconnected from the server, stop reading memory
        /// </summary>
        private void OnDisconnect()
        {
            Log($"Lost connection with server");
            connectBtn.Text = "Connect";
        }

        /// <summary>
        /// Ensures that the input fields contain valid info, and if so, update the client settings
        /// </summary>
        private ClientSettings ValidateInputFields()
        {
            // Ensure that player name has a valid length
            string playerName = playerNameField.Text.Trim();
            if (playerName.Length < 1 || playerName.Length > 16)
            {
                playerName = null;
            }

            // The game will always be ww for now
            string gameName = null;

            // Ensure that server ip has a valid length and doesn't contain a colon
            string serverIp = serverIpField.Text.Trim();
            if (serverIp.Length < 1 || serverIp.Length > 16 || serverIp.Contains(':'))
            {
                serverIp = null;
            }

            // Ensure that the port is a valid number
            if (!int.TryParse(serverPortField.Text.Trim(), out int serverPort) || serverPort < 0)
            {
                serverPort = 0;
            }

            // Ensure that password is empty or has a valid length
            string password = passwordField.Text.Trim();
            if (password.Length > 16)
            {
                password = null;
            }

            playerNameField.Text = playerName;
            gameNameField.Text = gameName;
            serverIpField.Text = serverIp;
            serverPortField.Text = serverPort > 0 ? serverPort.ToString() : null;
            passwordField.Text = password;

            return new ClientSettings(playerName, gameName, serverIp, serverPort, password);
        }

        /// <summary>
        /// When the form is opened, load the last used settings
        /// </summary>
        private void OnFormOpen(object sender, EventArgs e)
        {
            playerNameField.Text = Properties.Settings.Default.playerName;
            gameNameField.Text = Properties.Settings.Default.gameName;
            serverIpField.Text = Properties.Settings.Default.serverIp;
            serverPortField.Text = Properties.Settings.Default.serverPort.ToString();
            passwordField.Text = Properties.Settings.Default.password;

            _settings = ValidateInputFields();

            Core.NetworkManager.OnConnect += OnConnect;
            Core.NetworkManager.OnDisconnect += OnDisconnect;
            Core.NetworkManager.OnIntroValidated += OnIntroValidated;
        }

        /// <summary>
        /// When the form is closed, save the last used settings
        /// </summary>
        private void OnFormClose(object sender, FormClosingEventArgs e)
        {
            ClientSettings settings = ValidateInputFields();
            Properties.Settings.Default.playerName = settings.playerName;
            Properties.Settings.Default.gameName = settings.gameName;
            Properties.Settings.Default.serverIp = settings.serverIp;
            Properties.Settings.Default.serverPort = settings.serverPort;
            Properties.Settings.Default.password = settings.password;
            Properties.Settings.Default.Save();
        }

        /// <summary>
        /// Logs a message to the debug console
        /// </summary>
        public static void Log(string message)
        {
            instance.BeginInvoke(new MethodInvoker(() => instance.debugText.AppendText(message + "\r\n")));
        }
    }
}