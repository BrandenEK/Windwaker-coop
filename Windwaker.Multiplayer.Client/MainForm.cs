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

        private readonly MainClient mainClient = new();
        private IClient gameClient;

        private string _lastValidIp;

        private bool IsConnectedToGame => gameClient != null && gameClient.IsConnected;

        /// <summary>
        /// When the connect button is clicked, either connect/disconnect from the server
        /// </summary>
        private void OnClickConnect(object sender, EventArgs e)
        {
            if (IsConnectedToGame)
            {
                gameClient.Disconnect();
            }
            else
            {
                string ipPort = serverText.Text.Trim();
                if (ValidateIpPort(ipPort))
                {
                    _lastValidIp = ipPort;
                    mainClient.Connect(ipPort);
                }
                else
                {
                    Log("Enter a valid ip port!");
                }
            }
        }

        public static void ReceiveGamePort(ushort port)
        {
            string ip = instance._lastValidIp.Split(':')[0];

            instance.gameClient = new WindwakerClient();
            instance.gameClient.Connect(ip + ":" + port);
        }

        /// <summary>
        /// Ensures that an 'ip:port' string follows a valid syntax
        /// </summary>
        private bool ValidateIpPort(string ipPort)
        {
            if (string.IsNullOrEmpty(ipPort))
                return false;

            int colon = ipPort.IndexOf(':');
            return colon > 0 && colon < ipPort.Length - 1;
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
            _lastValidIp = Properties.Settings.Default.ServerIpPort;
            serverText.Text = _lastValidIp;
        }

        /// <summary>
        /// When the form is closed, save the last used ip to settings
        /// </summary>
        private void OnFormClose(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.ServerIpPort = _lastValidIp;
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