using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Windwaker.Multiplayer.Client.Progress;

namespace Windwaker.Multiplayer.Client.Dolphin
{
    internal class DolphinManager
    {
        private const int SYNC_DELAY = 2000;
        private const ulong BASE_ADDRESS = 0x803B0000;

        [DllImport("kernel32.dll")]
        static extern bool WriteProcessMemory(nint hProcess, nint lpBaseAddress, byte[] lpBuffer, int nSize, out int lpNumberOfBytesWritten);
        [DllImport("kernel32.dll")]
        static extern bool ReadProcessMemory(nint hProcess, nint lpBaseAddress, byte[] lpBuffer, int nSize, out int lpNumberOfBytesWritten);

        private bool _reading;

        private byte _currentStage;

        /// <summary>
        /// Starts the async task of reading memory in a loop
        /// </summary>
        public void StartLoop()
        {
            _currentStage = 0xFF;
            _reading = true;
            Task.Run(ReadLoop);
        }

        /// <summary>
        /// Stops the async task of reading memory in a loop
        /// </summary>
        public void StopLoop()
        {
            Core.UIManager.UpdateStatusBox(ConnectionType.Disconnected);
            _currentStage = 0xFF;
            _reading = false;
        }

        /// <summary>
        /// Every certain number of seconds, the dolphin memory will be read to determine if anything has changed
        /// </summary>
        private async Task ReadLoop()
        {
            while (_reading)
            {
                int timeStart = Environment.TickCount;

                // Check if the player is in a stage and which one
                CheckNewStage();

                // If in a save file, check for new progress
                if (IsSaveFileLoaded())
                {
                    CheckNewProgress();
                }

                int timeEnd = Environment.TickCount;
                //Core.UIManager.Log($"Time taken to read from memory: {timeEnd - timeStart} ms");

                await Task.Delay(SYNC_DELAY);
            }
        }

        /// <summary>
        /// Checks whether Dolphin is running by hooking into its process
        /// </summary>
        private bool IsDolphinRunning(out nint process)
        {
            Process[] processes = Process.GetProcessesByName("dolphin");
            if (processes.Length > 0)
            {
                process = processes[0].Handle;
                return true;
            }
            else
            {
                process = nint.Zero;
                return false;
            }
        }

        /// <summary>
        /// Checks whether a save file is loaded by reading the player name
        /// </summary>
        private bool IsSaveFileLoaded()
        {
            if (!TryRead(0x4D64, 4, out byte[] bytes))
            {
                Core.UIManager.UpdateStatusBox(ConnectionType.Disconnected);
                return false;
            }

            bool isEmpty = bytes[0] == 0 && bytes[1] == 0 && bytes[2] == 0 && bytes[3] == 0;
            bool isLink = bytes[0] == 76 && bytes[1] == 105 && bytes[2] == 110 && bytes[3] == 107;

            bool saveLoaded = !isEmpty && !isLink;
            Core.UIManager.UpdateStatusBox(saveLoaded ? ConnectionType.ConnectedInGame : ConnectionType.ConnectedNotInGame);
            return saveLoaded;
        }

        /// <summary>
        /// Attempts to write a number of bytes to an address in memory
        /// </summary>
        public bool TryWrite(uint address, byte[] bytes)
        {
            if (IsDolphinRunning(out nint process))
            {
                WriteProcessMemory(process, (nint)(BASE_ADDRESS + address), bytes, bytes.Length, out int _);
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
            if (IsDolphinRunning(out nint process))
            {
                bytes = new byte[size];
                ReadProcessMemory(process, (nint)(BASE_ADDRESS + address), bytes, size, out int _);
                return true;
            }
            else
            {
                bytes = null;
                return false;
            }
        }

        private void CheckNewStage()
        {
            byte dolphinStage = 0xFF;
            if (IsSaveFileLoaded() && TryRead(0x53A4, 1, out byte[] bytes))
                dolphinStage = bytes[0];

            if (dolphinStage != _currentStage)
            {
                Core.UIManager.Log("Changed scene: " + dolphinStage);
                _currentStage = dolphinStage;
                Core.NetworkManager.SendScene(dolphinStage);
            }
        }

        private void CheckNewProgress()
        {
            ProgressManager progress = Core.ProgressManager;
            byte[] bytes;

            if (TryRead(0x4C09, 1, out bytes))
            {
                progress.CheckForProgress("maxhealth", bytes[0]);
            }

            if (TryRead(0x4C16, 6, out bytes))
            {
                progress.CheckForProgress("sword", bytes[0]);
                progress.CheckForProgress("shield", bytes[1]);
                progress.CheckForProgress("powerbracelets", bytes[2]);
                // Unused x 1
                progress.CheckForProgress("wallet", bytes[4]);
                progress.CheckForProgress("maxmagic", bytes[5]);
            }

            if (TryRead(0x4C44, 21, out bytes))
            {
                progress.CheckForProgress("telescope", bytes[0]);
                progress.CheckForProgress("sail", bytes[1]);
                progress.CheckForProgress("windwaker", bytes[2]);
                progress.CheckForProgress("grapplinghook", bytes[3]);
                progress.CheckForProgress("spoilsbag", bytes[4]);
                progress.CheckForProgress("boomerang", bytes[5]);
                progress.CheckForProgress("dekuleaf", bytes[6]);

                progress.CheckForProgress("tingletuner", bytes[7]);
                progress.CheckForProgress("pictobox", bytes[8]);
                progress.CheckForProgress("ironboots", bytes[9]);
                progress.CheckForProgress("magicarmor", bytes[10]);
                progress.CheckForProgress("baitbag", bytes[11]);
                progress.CheckForProgress("bow", bytes[12]);
                progress.CheckForProgress("bombs", bytes[13]);

                progress.CheckForProgress("bottle1", bytes[14]);
                progress.CheckForProgress("bottle2", bytes[15]);
                progress.CheckForProgress("bottle3", bytes[16]);
                progress.CheckForProgress("bottle4", bytes[17]);
                progress.CheckForProgress("deliverybag", bytes[18]);
                progress.CheckForProgress("hookshot", bytes[19]);
                progress.CheckForProgress("skullhammer", bytes[20]);
            }

            if (TryRead(0x4C77, 2, out bytes))
            {
                progress.CheckForProgress("maxarrows", bytes[0]);
                progress.CheckForProgress("maxbombs", bytes[1]);
            }

            if (TryRead(0x4CBF, 9, out bytes))
            {
                progress.CheckForProgress("piratescharm", bytes[0]);
                progress.CheckForProgress("heroscharm", bytes[1]);
                // Unused x 4
                progress.CheckForProgress("songs", bytes[6]);
                progress.CheckForProgress("shards", bytes[7]);
                progress.CheckForProgress("pearls", bytes[8]);
            }

            //if (TryRead(0x4CDC, 48 + 49, out bytes))
            //{
            //    for (byte i = 0; i < 8; i++)
            //    {
            //        progress.CheckForCharts("owned", i, bytes[0 + i]);
            //    }
            //    for (byte i = 0; i < 8; i++)
            //    {
            //        progress.CheckForCharts("opened", i, bytes[16 + i]);
            //    }
            //    for (byte i = 0; i < 8; i++)
            //    {
            //        progress.CheckForCharts("looted", i, bytes[32 + i]);
            //    }

            //    for (byte i = 0; i < 49; i++)
            //    {
            //        progress.CheckForSectors(i, bytes[48 + i]);
            //    }
            //}

            //if (TryRead(0x4D4D, 1, out bytes))
            //{
            //    progress.CheckForCharts("deciphered", 0, bytes[0]);
            //}

            if (TryRead(0x5296, 1, out bytes))
            {
                progress.CheckForProgress("tinglestatues", bytes[0]);
            }

            //if (TryRead(0x52CB, 6, out bytes))
            //{
            //    progress.CheckForProgress("warpwt", bytes[0]);
            //    progress.CheckForProgress("warpinter1", bytes[1]);
            //    progress.CheckForProgress("warpfw", bytes[2]);
            //    progress.CheckForProgress("warpdrc", bytes[3]);
            //    progress.CheckForProgress("warpet", bytes[4]);
            //    progress.CheckForProgress("warpinter2", bytes[5]);
            //}

            //public void CheckForCharts(string type, byte index, byte value)
            //{
            //    string key = $"charts{type}{index}";
            //    if (GetItemLevel(key) < value)
            //        ObtainItem(key, value);
            //}

            //public void CheckForSectors(byte index, byte value)
            //{
            //    string key = $"sector{index}";
            //    if (GetItemLevel(key) < value)
            //        ObtainItem(key, value);
            //}

        //public byte bagContents;

        //public byte stages;

        //public byte events;

    }
}
}
