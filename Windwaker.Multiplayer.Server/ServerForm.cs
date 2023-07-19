using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Windwaker.Multiplayer.Server
{
    public partial class ServerForm : Form
    {
        private static ServerForm instance;

        public ServerForm()
        {
            InitializeComponent();
            instance ??= this;
        }

        private readonly Server _server = new();

        private readonly PlayerGrid _grid = new();

        private ServerSettings _settings;
        public static ServerSettings Settings => instance._settings;

        /// <summary>
        /// When the start button is clicked, either start/stop the server
        /// </summary>
        private void OnClickStart(object sender, EventArgs e)
        {
            if (_server.IsListening)
            {
                _server.Stop();
                startButton.Text = "Start";
            }
            else
            {
                _settings = ValidateInputFields();
                if (_server.Start("*", _settings.ValidServerPort))
                    startButton.Text = "Stop";
            }
        }

        public static void UpdatePlayerGrid(IEnumerable<PlayerData> players, int count)
        {
            instance.BeginInvoke(new MethodInvoker(() => instance._grid.UpdateGrid(players, count)));
        }

        /// <summary>
        /// Ensures that the input fields contain valid info, and if so, update the server settings
        /// </summary>
        private ServerSettings ValidateInputFields()
        {
            // Ensure that max players is a valid number
            if (!int.TryParse(maxPlayersField.Text.Trim(), out int maxPlayers) || maxPlayers < 0)
            {
                maxPlayers = 0;
            }

            // The game will always be ww for now
            string gameName = null;

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

            maxPlayersField.Text = maxPlayers > 0 ? maxPlayers.ToString() : null;
            gameNameField.Text = gameName;
            serverPortField.Text = serverPort > 0 ? serverPort.ToString() : null;
            passwordField.Text = password;

            return new ServerSettings(maxPlayers, gameName, serverPort, password);
        }

        /// <summary>
        /// When the form is opened, load the last used settings
        /// </summary>
        private void OnFormOpen(object sender, EventArgs e)
        {
            maxPlayersField.Text = Properties.Settings.Default.maxPlayers.ToString();
            gameNameField.Text = Properties.Settings.Default.gameName;
            serverPortField.Text = Properties.Settings.Default.serverPort.ToString();
            passwordField.Text = Properties.Settings.Default.password;

            _settings = ValidateInputFields();
            _grid.UpdateGrid(Array.Empty<PlayerData>(), 0);
        }

        /// <summary>
        /// When the form is closed, save the last used settings
        /// </summary>
        private void OnFormClose(object sender, FormClosingEventArgs e)
        {
            ServerSettings settings = ValidateInputFields();
            Properties.Settings.Default.maxPlayers = settings.maxPlayers;
            Properties.Settings.Default.gameName = settings.gameName;
            Properties.Settings.Default.serverPort = settings.serverPort;
            Properties.Settings.Default.password = settings.password;
            Properties.Settings.Default.Save();
        }

        /// <summary>
        /// Logs a message to the debug console
        /// </summary>
        public static void Log(string message)
        {
            instance.debugText.AppendText(message + "\r\n");
        }

        /// <summary>
        /// Adds a panel to the registered controls
        /// </summary>
        public static void AddPanel(Panel panel)
        {
            instance.Controls.Add(panel);
        }
    }
}