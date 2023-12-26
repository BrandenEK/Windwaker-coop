using System.Threading.Tasks;
using System.Windows.Forms;
using Windwaker.Multiplayer.Client.Logging;
using Windwaker.Multiplayer.Client.Memory;

namespace Windwaker.Multiplayer.Client
{
    internal partial class MainForm : Form
    {
        private readonly ILogger _logger;
        private readonly IMemoryReader _memoryReader;

        public MainForm()
        {
            InitializeComponent();

            _memoryReader = new DolphinReader();
            _logger = new FormLogger(logInner);

            TestMemory();
        }

        private async void TestMemory()
        {
            while (true)
            {
                bool success = _memoryReader.TryRead(0x4C44, 1, out byte[] bytes);
                _logger.Info(success ? $"Read: {bytes[0]}" : "Failed to read memory");
                await Task.Delay(2000);
            }
        }
    }
}