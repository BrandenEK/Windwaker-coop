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

        private WindwakerProgress progress;

        /// <summary>
        /// Starts the async task of reading memory in a loop
        /// </summary>
        public void StartLoop()
        {
            _reading = true;
            progress = new WindwakerProgress();
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

                // Check if the player is in a stage and which one
                CheckNewStage();

                // If in a save file, check for new progress
                if (IsSaveFileLoaded())
                {
                    CheckNewProgress();
                }
                else
                {
                    ClientForm.Log("Save file is not loaded yet!");
                }

                int timeEnd = Environment.TickCount;
                ClientForm.Log($"Time taken to read from memory: {timeEnd - timeStart} ms");

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
                ClientForm.Log($"Dolphin is not running!");
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

        private void CheckNewStage()
        {
            byte currentStage = 0xFF;
            if (IsSaveFileLoaded() && TryRead(0x53A4, 1, out byte[] bytes))
                currentStage = bytes[0];

            if (currentStage != progress.stageId)
            {
                progress.stageId = currentStage;
                ClientForm.Log("Changed scene: " + currentStage);
                ClientForm.Client.SendScene(currentStage);
            }
        }

        private void CheckNewProgress()
        {
            byte[] bytes;

            if (TryRead(0x4C16, 6, out bytes))
            {
                progress.CheckForSword(bytes[0]);
                progress.CheckForShield(bytes[1]);
                progress.CheckForPowerBracelets(bytes[2]);
                // Unknown
                progress.CheckForWallet(bytes[4]);
                progress.CheckForMaxMagic(bytes[5]);
            }

            if (TryRead(0x4C44, 21, out bytes))
            {
                progress.CheckForTelescope(bytes[0]);
                progress.CheckForSail(bytes[1]);
                progress.CheckForWindwaker(bytes[2]);
                progress.CheckForGrapplingHook(bytes[3]);
                progress.CheckForSpoilsBag(bytes[4]);
                progress.CheckForBoomerang(bytes[5]);
                progress.CheckForDekuLeaf(bytes[6]);

                progress.CheckForTingleTuner(bytes[7]);
                progress.CheckForPictoBox(bytes[8]);
                progress.CheckForIronBoots(bytes[9]);
                progress.CheckForMagicArmor(bytes[10]);
                progress.CheckForBaitBag(bytes[11]);
                progress.CheckForBow(bytes[12]);
                progress.CheckForBombs(bytes[13]);

                progress.CheckForBottle1(bytes[14]);
                progress.CheckForBottle2(bytes[15]);
                progress.CheckForBottle3(bytes[16]);
                progress.CheckForBottle4(bytes[17]);
                progress.CheckForDeliveryBag(bytes[18]);
                progress.CheckForHookshot(bytes[19]);
                progress.CheckForSkullHammer(bytes[20]);
            }

            if (TryRead(0x4C77, 2, out bytes))
            {
                progress.CheckForMaxArrows(bytes[0]);
                progress.CheckForMaxBombs(bytes[1]);
            }

            if (TryRead(0x4CBF, 2, out bytes))
            {
                progress.CheckForPiratesCharm(bytes[0]);
                progress.CheckForHerosCharm(bytes[1]);
            }
        }
    }
}
