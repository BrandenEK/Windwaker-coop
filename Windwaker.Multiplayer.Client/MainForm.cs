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
                string ipPort = serverText.Text;
                if (ipPort == string.Empty)
                {
                    Log("Enter an ip port to connect!");
                    return;
                }

                client.Connect(ipPort);
            }
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