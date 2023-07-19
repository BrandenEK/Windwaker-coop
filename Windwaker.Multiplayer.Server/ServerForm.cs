using System;
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

            _settings = new ServerSettings(8, "Windwaker", 8989, null);

            _server.Start("*:" + _settings.serverPort);

            while (true)
            {
                // Infinite loop to keep the server alive
                Console.ReadKey(true);
            }
        }

        private ServerSettings _settings;
        public static ServerSettings Settings => instance._settings;

        private readonly Server _server = new();
    }
}