using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Windwaker.Multiplayer.Client
{
    internal class MemoryReader
    {
        private bool _reading = false;

        public void StartLoop()
        {
            _reading = true;
            Task.Run(Read);
        }

        public void StopLoop()
        {
            _reading = false;
        }

        public async Task Read()
        {
            while (_reading)
            {
                MainForm.Log("Looping");

                await Task.Delay(5000);
            }
        }
    }
}
