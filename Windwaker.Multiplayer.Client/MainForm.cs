using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using Windwaker.Multiplayer.Client.Logging;
using Windwaker.Multiplayer.Client.Memory;
using Windwaker.Multiplayer.Client.Network;
using Windwaker.Multiplayer.Client.Network.Packets;
using Windwaker.Multiplayer.Client.Notifications;
using Windwaker.Multiplayer.Client.Progression;
using Windwaker.Multiplayer.Client.Progression.Import;

namespace Windwaker.Multiplayer.Client
{
    internal partial class MainForm : Form
    {
        private readonly ILogger _logger;
        private readonly IMemoryReader _memoryReader;
        private readonly IProgressChecker _progressChecker;
        private readonly IClient _client;

        private bool isSyncing = false;

        public MainForm()
        {
            InitializeComponent();

            _logger = new MultiLogger(new FormLogger(logInner, true), new FileLogger());
            _memoryReader = new DolphinReader();
            _client = new NetworkClient(_logger, new GlobalPacketSerializer());
            _progressChecker = new WindwakerProgress(_logger, _memoryReader, new LogNotifier(_logger), _client, new JsonImporter(_logger, "windwaker"));

            _client.OnConnect += OnConnect;
            _client.OnDisconnect += OnDisconnect;

            SyncLoop();
        }

        private async void SyncLoop()
        {
            while (true)
            {
                if (isSyncing)
                    _progressChecker.CheckForProgress();

                await Task.Delay(2000);
            }
        }

        private void OnConnect(object? _, EventArgs __)
        {
            _logger.Info("Connected to server");
            SetSyncingStatus(true);
        }

        private void OnDisconnect(object? _, EventArgs __)
        {
            _logger.Info("Disconnected from server");
            SetSyncingStatus(false);
        }

        private void SetSyncingStatus(bool sync)
        {
            connectBtn.Text = (isSyncing = sync) ? "Disconnect" : "Connect";
        }

        private void OnConnectBtnClicked(object sender, EventArgs e)
        {
            if (isSyncing)
            {
                _client.Disconnect();
            }
            else
            {
                _client.Connect("127.0.0.1", 8989, "Test player", null);
            }
        }
    }
}