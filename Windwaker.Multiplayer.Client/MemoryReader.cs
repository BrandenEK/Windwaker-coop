using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Windwaker.Multiplayer.Client
{
    public class MemoryReader
    {
        [DllImport("kernel32.dll")]
        static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, int nSize, out int lpNumberOfBytesWritten);
        [DllImport("kernel32.dll")]
        static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, int nSize, out int lpNumberOfBytesWritten);

        private bool _reading = false;

        private WindwakerProgress _progress;

        /// <summary>
        /// Starts the async task of reading memory in a loop
        /// </summary>
        public void StartLoop()
        {
            _reading = true;
            _progress = new WindwakerProgress();
            Task.Run(ReadLoop);
        }

        /// <summary>
        /// Stops the async task of reading memory in a loop
        /// </summary>
        public void StopLoop()
        {
            _reading = false;
        }

        /// <summary>
        /// Every certain number of seconds, the dolphin memory will be read to determine if anything has changed
        /// </summary>
        public async Task ReadLoop()
        {
            while (_reading)
            {
                int timeStart = Environment.TickCount;

                if (IsSaveFileLoaded())
                {
                    ReadAllMemory();
                }
                else
                {
                    MainForm.Log("Save file is not loaded yet!");
                }

                int timeEnd = Environment.TickCount;
                MainForm.Log($"Time taken to read from memory: {timeEnd - timeStart} ms");

                await Task.Delay(2000);
            }
        }

        /// <summary>
        /// Checks whether Dolphin is running by hooking into its process
        /// </summary>
        private bool IsDolphinRunning(out IntPtr process)
        {
            Process[] processes = Process.GetProcessesByName("dolphin");
            if (processes.Length > 0)
            {
                process = processes[0].Handle;
                return true;
            }
            else
            {
                MainForm.Log($"Dolphin is not running!");
                process = IntPtr.Zero;
                return false;
            }
        }

        /// <summary>
        /// Checks whether a save file is loaded by reading the player name
        /// </summary>
        private bool IsSaveFileLoaded()
        {
            if (!TryRead(0x4D64, 4, out byte[] bytes)) return false;

            bool isEmpty = bytes[0] == 0 && bytes[1] == 0 && bytes[2] == 0 && bytes[3] == 0;
            bool isLink = bytes[0] == 76 && bytes[1] == 105 && bytes[2] == 110 && bytes[3] == 107;

            return !isEmpty && !isLink;
        }

        /// <summary>
        /// Attempts to write a number of bytes to an address in memory
        /// </summary>
        public bool TryWrite(uint address, byte[] bytes)
        {
            if (IsDolphinRunning(out IntPtr process))
            {
                WriteProcessMemory(process, (IntPtr)(0x803B0000 + address), bytes, bytes.Length, out int _);
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Attempts to read a number of bytes from an address in memory
        /// </summary>
        public bool TryRead(uint address, int size, out byte[] bytes)
        {
            if (IsDolphinRunning(out IntPtr process))
            {
                bytes = new byte[size];
                ReadProcessMemory(process, (IntPtr)(0x803B0000 + address), bytes, size, out int _);
                return true;
            }
            else
            {
                bytes = null;
                return false;
            }
        }

        private void ReadAllMemory()
        {
            byte currentStage = 0xFF;
            if (TryRead(0x53A4, 1, out byte[] bytes))
                currentStage = bytes[0];

            if (currentStage != _progress.stageId)
            {
                _progress.stageId = currentStage;
                MainForm.Log("Changed scene: " +  currentStage);
                MainForm.Client.SendScene(currentStage);
            }
        }
    }
}
