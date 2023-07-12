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

        private void OnClickConnect(object sender, EventArgs e)
        {
            string ipPort = serverText.Text;
            if (ipPort == string.Empty)
            {
                Log("Enter an ip port to connect!");
                return;
            }

            var client = new Client();
            client.Connect(ipPort);
        }

        public static void Log(string message)
        {
            instance.debugText.Text += message + "\r\n";
        }
    }
}