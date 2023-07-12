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

        private readonly Client client = new();

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
                    client.Connect(ipPort);
                }
                else
                {
                    Log("Enter a valid ip port!");
                }
            }
        }

        private bool ValidateIpPort(string ipPort)
        {
            if (string.IsNullOrEmpty(ipPort))
                return false;

            int colon = ipPort.IndexOf(':');
            return colon > 0 && colon < ipPort.Length - 1;
        }

        public static void UpdateUI()
        {
            instance.connectBtn.Text = instance.client.IsConnected ? "Disconnect" : "Connect";
        }

        public static void Log(string message)
        {
            instance.debugText.Text += message + "\r\n";
        }
    }
}