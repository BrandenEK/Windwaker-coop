using System;
using System.Drawing;
using System.Windows.Forms;
using Windwaker.Multiplayer.Client.Dolphin;

namespace Windwaker.Multiplayer.Client
{
    internal partial class ClientForm : Form
    {
        private const string CLIENT_VERSION = "0.1.0";

        private static ClientForm instance;

        public ClientForm()
        {
            InitializeComponent();
            Text = "Windwaker Multiplayer Client " + CLIENT_VERSION;
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

        public void UpdateButtonText()
        {
            BeginInvoke(new MethodInvoker(() =>
            {
                connectBtn.Text = Core.NetworkManager.IsConnected ? "Disconnect" : "Connect";
            }));
        }

        public void UpdateStatusBox(ConnectionType connection)
        {
            BeginInvoke(new MethodInvoker(() =>
            {
                if (connection == ConnectionType.ConnectedInGame)
                    statusBox.BackColor = Color.Green;
                else if (connection == ConnectionType.ConnectedNotInGame)
                    statusBox.BackColor = Color.Yellow;
                else
                    statusBox.BackColor = Color.Red;
            }));
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
            if (serverIp.Length < 1 || serverIp.Length > 100)// || serverIp.Contains(':'))
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

            int syncSettings = Properties.Settings.Default.syncSettings;
            syncInventory.Checked = (syncSettings & 0x01) > 0;
            syncEquipment.Checked = (syncSettings & 0x02) > 0;
            syncStats.Checked = (syncSettings & 0x04) > 0;
            syncCharts.Checked = (syncSettings & 0x08) > 0;
            syncWorld.Checked = (syncSettings & 0x10) > 0;
            syncKeys.Checked = (syncSettings & 0x20) > 0;

            _settings = ValidateInputFields();
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

            int syncSettings = 0;
            syncSettings |= syncInventory.Checked ? 0x01 : 0;
            syncSettings |= syncEquipment.Checked ? 0x02 : 0;
            syncSettings |= syncStats.Checked ? 0x04 : 0;
            syncSettings |= syncCharts.Checked ? 0x08 : 0;
            syncSettings |= syncWorld.Checked ? 0x10 : 0;
            syncSettings |= syncKeys.Checked ? 0x20 : 0;
            Properties.Settings.Default.syncSettings = syncSettings;

            Properties.Settings.Default.Save();
        }

        /// <summary>
        /// Logs a message to the debug console
        /// </summary>
        public void DisplayText(object message, Color color)
        {
            debugText.SelectionStart = debugText.TextLength;
            debugText.SelectionLength = 0;

            debugText.SelectionColor = color;
            debugText.AppendText(message + "\r\n");
            debugText.ScrollToCaret();
        }

        public void Log(object message)
        {
            BeginInvoke(new MethodInvoker(() => DisplayText(message, Color.White)));
        }
        public void LogWarning(object message)
        {
            BeginInvoke(new MethodInvoker(() => DisplayText(message, Color.Yellow)));
        }
        public void LogError(object message)
        {
            BeginInvoke(new MethodInvoker(() => DisplayText(message, Color.Red)));
        }
        public void LogProgress(object message)
        {
            BeginInvoke(new MethodInvoker(() => DisplayText(message, Color.Green)));
        }
    }
}