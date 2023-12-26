using System.Threading.Tasks;
using System.Windows.Forms;

namespace Windwaker.Multiplayer.Client
{
    internal partial class MainForm : Form
    {
        private readonly IMemoryReader _memoryReader;

        public MainForm()
        {
            _memoryReader = new DolphinReader();

            InitializeComponent();
        }

        private async void TestMemory()
        {
            while (true)
            {
                // log bytes
                await Task.Delay(2000);
            }
        }
    }
}