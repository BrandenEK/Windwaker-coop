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

        private readonly MainClient client = new();

        private string _lastValidIp;

        /// <summary>
        /// When the connect button is clicked, either connect/disconnect from the server
        /// </summary>
        private void OnClickConnect(object sender, EventArgs e)
        {
            if (client.IsConnected)
            {
                client.Disconnect();
            }
            else
            {
                string ipPort = serverText.Text.Trim();
                if (ValidateIpPort(ipPort))
                {
                    _lastValidIp = ipPort;
                    client.Connect(ipPort);
                }
                else
                {
                    Log("Enter a valid ip port!");
                }
            }
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
            instance.connectBtn.Text = instance.client.IsConnected ? "Disconnect" : "Connect";
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