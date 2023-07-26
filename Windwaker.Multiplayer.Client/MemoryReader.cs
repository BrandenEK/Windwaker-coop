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

        /// <summary>
        /// Starts the async task of reading memory in a loop
        /// </summary>
        public void StartLoop()
        {
            _reading = true;
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

            if (currentStage != ClientForm.GameProgress.stageId)
            {
                ClientForm.GameProgress.stageId = currentStage;
                ClientForm.Log("Changed scene: " + currentStage);
                ClientForm.Client.SendScene(currentStage);
            }
        }

        private void CheckNewProgress()
        {
            WindwakerProgress progress = ClientForm.GameProgress;
            byte[] bytes;

            if (TryRead(0x4C09, 1, out bytes))
            {
                progress.CheckForMaxHealth(bytes[0]);
            }

            if (TryRead(0x4C16, 6, out bytes))
            {
                progress.CheckForSword(bytes[0]);
                progress.CheckForShield(bytes[1]);
                progress.CheckForPowerBracelets(bytes[2]);
                // Unused x 1
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

            if (TryRead(0x4CBF, 9, out bytes))
            {
                progress.CheckForPiratesCharm(bytes[0]);
                progress.CheckForHerosCharm(bytes[1]);
                // Unused x 4
                progress.CheckForSongs(bytes[6]);
                progress.CheckForShards(bytes[7]);
                progress.CheckForPearls(bytes[8]);
            }

            if (TryRead(0x4CDC, 48 + 49, out bytes))
            {
                for (byte i = 0; i < 8; i++)
                {
                    progress.CheckForCharts("owned", i, bytes[0 + i]);
                }
                for (byte i = 0; i < 8; i++)
                {
                    progress.CheckForCharts("opened", i, bytes[16 + i]);
                }
                for (byte i = 0; i < 8; i++)
                {
                    progress.CheckForCharts("looted", i, bytes[32 + i]);
                }

                for (byte i = 0; i < 49; i++)
                {
                    progress.CheckForSectors(i, bytes[48 + i]);
                }
            }

            if (TryRead(0x4D4D, 1, out bytes))
            {
                progress.CheckForCharts("deciphered", 0, bytes[0]);
            }

            if (TryRead(0x5296, 1, out bytes))
            {
                progress.CheckForTingleStatues(bytes[0]);
            }
        }

        public void WriteReceivedItem(string item, byte value)
        {
            uint mainAddress, bitfAddress;
            byte mainValue, bitfValue;            

            // Inventory

            if (item == "telescope")
            {
                mainAddress = 0x4C44; mainValue = (byte)(value == 1 ? 0x20 : 0xFF);
                bitfAddress = 0x4C59; bitfValue = (byte)(value == 1 ? 0xFF : 0x00);
            }
            else if (item == "sail")
            {
                mainAddress = 0x4C45; mainValue = (byte)(value == 1 ? 0x78 : 0xFF);
                bitfAddress = 0x4C5A; bitfValue = (byte)(value == 1 ? 0xFF : 0x00);
            }
            else if (item == "windwaker")
            {
                mainAddress = 0x4C46; mainValue = (byte)(value == 1 ? 0x22 : 0xFF);
                bitfAddress = 0x4C5B; bitfValue = (byte)(value == 1 ? 0xFF : 0x00);
            }
            else if (item == "grapplinghook")
            {
                mainAddress = 0x4C47; mainValue = (byte)(value == 1 ? 0x25 : 0xFF);
                bitfAddress = 0x4C5C; bitfValue = (byte)(value == 1 ? 0x01 : 0x00);
            }
            else if (item == "spoilsbag")
            {
                mainAddress = 0x4C48; mainValue = (byte)(value == 1 ? 0x24 : 0xFF);
                bitfAddress = 0x4C5D; bitfValue = (byte)(value == 1 ? 0x01 : 0x00);
            }
            else if (item == "boomerang")
            {
                mainAddress = 0x4C49; mainValue = (byte)(value == 1 ? 0x2D : 0xFF);
                bitfAddress = 0x4C5E; bitfValue = (byte)(value == 1 ? 0x01 : 0x00);
            }
            else if (item == "dekuleaf")
            {
                mainAddress = 0x4C4A; mainValue = (byte)(value == 1 ? 0x34 : 0xFF);
                bitfAddress = 0x4C5F; bitfValue = (byte)(value == 1 ? 0x01 : 0x00);
            }
            else if (item == "tingletuner")
            {
                mainAddress = 0x4C4B; mainValue = (byte)(value == 1 ? 0x21 : 0xFF);
                bitfAddress = 0x4C60; bitfValue = (byte)(value == 1 ? 0x01 : 0x00);
            }
            else if (item == "pictobox")
            {
                mainAddress = 0x4C4C; bitfAddress = 0x4C61;
                if (value == 2)
                {
                    mainValue = 0x26; bitfValue = 0x03;
                }
                else if (value == 1)
                {
                    mainValue = 0x23; bitfValue = 0x01;
                }
                else
                {
                    mainValue = 0xFF; bitfValue = 0x00;
                }
            }
            else if (item == "ironboots")
            {
                mainAddress = 0x4C4D; mainValue = (byte)(value == 1 ? 0x29 : 0xFF);
                bitfAddress = 0x4C62; bitfValue = (byte)(value == 1 ? 0x01 : 0x00);
            }
            else if (item == "magicarmor")
            {
                mainAddress = 0x4C4E; mainValue = (byte)(value == 1 ? 0x2A : 0xFF);
                bitfAddress = 0x4C63; bitfValue = (byte)(value == 1 ? 0x01 : 0x00);
            }
            else if (item == "baitbag")
            {
                mainAddress = 0x4C4F; mainValue = (byte)(value == 1 ? 0x2C : 0xFF);
                bitfAddress = 0x4C64; bitfValue = (byte)(value == 1 ? 0x01 : 0x00);
            }
            else if (item == "bow")
            {
                mainAddress = 0x4C50; bitfAddress = 0x4C65;
                if (value == 3)
                {
                    mainValue = 0x36; bitfValue = 0x07;
                }
                else if (value == 2)
                {
                    mainValue = 0x35; bitfValue = 0x03;
                }
                else if (value == 1)
                {
                    mainValue = 0x27; bitfValue = 0x01;
                }
                else
                {
                    mainValue = 0xFF; bitfValue = 0x00;
                }
            }
            else if (item == "bombs")
            {
                mainAddress = 0x4C51; mainValue = (byte)(value == 1 ? 0x31 : 0xFF);
                bitfAddress = 0x4C66; bitfValue = (byte)(value == 1 ? 0x01 : 0x00);
            }
            else if (item == "bottle1")
            {
                mainAddress = 0x4C52; mainValue = (byte)(value == 1 ? 0x50 : 0xFF);
                bitfAddress = 0x4C67; bitfValue = (byte)(value == 1 ? 0x01 : 0x00);
            }
            else if (item == "bottle2")
            {
                mainAddress = 0x4C53; mainValue = (byte)(value == 1 ? 0x50 : 0xFF);
                bitfAddress = 0x4C68; bitfValue = (byte)(value == 1 ? 0x01 : 0x00);
            }
            else if (item == "bottle3")
            {
                mainAddress = 0x4C54; mainValue = (byte)(value == 1 ? 0x50 : 0xFF);
                bitfAddress = 0x4C69; bitfValue = (byte)(value == 1 ? 0x01 : 0x00);
            }
            else if (item == "bottle4")
            {
                mainAddress = 0x4C55; mainValue = (byte)(value == 1 ? 0x50 : 0xFF);
                bitfAddress = 0x4C6A; bitfValue = (byte)(value == 1 ? 0x01 : 0x00);
            }
            else if (item == "deliverybag")
            {
                mainAddress = 0x4C56; mainValue = (byte)(value == 1 ? 0x30 : 0xFF);
                bitfAddress = 0x4C6B; bitfValue = (byte)(value == 1 ? 0x01 : 0x00);
            }
            else if (item == "hookshot")
            {
                mainAddress = 0x4C57; mainValue = (byte)(value == 1 ? 0x2F : 0xFF);
                bitfAddress = 0x4C6C; bitfValue = (byte)(value == 1 ? 0x01 : 0x00);
            }
            else if (item == "skullhammer")
            {
                mainAddress = 0x4C58; mainValue = (byte)(value == 1 ? 0x33 : 0xFF);
                bitfAddress = 0x4C6D; bitfValue = (byte)(value == 1 ? 0x01 : 0x00);
            }

            // Equipment

            else if (item == "sword")
            {
                mainAddress = 0x4C16; bitfAddress = 0x4CBC;
                if (value == 4)
                {
                    mainValue = 0x3E; bitfValue = 0x15;
                }
                else if (value == 3)
                {
                    mainValue = 0x3A; bitfValue = 0x07;
                }
                else if (value == 2)
                {
                    mainValue = 0x39; bitfValue = 0x03;
                }
                else if (value == 1)
                {
                    mainValue = 0x38; bitfValue = 0x01;
                }
                else
                {
                    mainValue = 0xFF; bitfValue = 0x00;
                }
            }
            else if (item == "shield")
            {
                mainAddress = 0x4C17; bitfAddress = 0x4CBD;
                if (value == 2)
                {
                    mainValue = 0x3C; bitfValue = 0x03;
                }
                else if (value == 1)
                {
                    mainValue = 0x3B; bitfValue = 0x01;
                }
                else
                {
                    mainValue = 0xFF; bitfValue = 0x00;
                }
            }
            else if (item == "powerbracelets")
            {
                mainAddress = 0x4C18; mainValue = (byte)(value == 1 ? 0x28 : 0xFF);
                bitfAddress = 0x4CBE; bitfValue = (byte)(value == 1 ? 0x01 : 0x00);
            }
            else if (item == "piratescharm")
            {
                mainAddress = 0; mainValue = 0;
                bitfAddress = 0x4CBF; bitfValue = (byte)(value == 1 ? 0x01 : 0x00);
            }
            else if (item == "heroscharm")
            {
                mainAddress = 0; mainValue = 0;
                bitfAddress = 0x4CC0; bitfValue = (byte)(value == 1 ? 0x01 : 0x00);
            }
            else if (item == "wallet")
            {
                mainAddress = 0x4C1A; bitfAddress = 0; bitfValue = 0;
                if (value == 2)
                {
                    mainValue = 0x02;
                }
                else if (value == 1)
                {
                    mainValue = 0x01;
                }
                else
                {
                    mainValue = 0x00;
                }
            }
            else if (item == "maxhealth")
            {
                mainAddress = 0x4C08; mainValue = value;
                bitfAddress = 0; bitfValue = 0;
            }
            else if (item == "maxmagic")
            {
                mainAddress = 0x4C1B; mainValue = value;
                bitfAddress = 0; bitfValue = 0;
            }
            else if (item == "maxarrows")
            {
                mainAddress = 0x4C77; mainValue = value;
                bitfAddress = 0; bitfValue = 0;
            }
            else if (item == "maxbombs")
            {
                mainAddress = 0x4C78; mainValue = value;
                bitfAddress = 0; bitfValue = 0;
            }
            else if (item == "songs")
            {
                mainAddress = 0; mainValue = 0;
                bitfAddress = 0x4CC5; bitfValue = value;
            }
            else if (item == "pearls")
            {
                mainAddress = 0; mainValue = 0;
                bitfAddress = 0x4CC7; bitfValue = value;
            }
            else if (item == "shards")
            {
                mainAddress = 0; mainValue = 0;
                bitfAddress = 0x4CC6; bitfValue = value;
            }
            else if (item == "tinglestatues")
            {
                mainAddress = 0; mainValue = 0;
                bitfAddress = 0x5296; bitfValue = value;
            }

            else
            {
                ClientForm.Log("*** Received unknown item ***");
                return;
            }

            if (mainAddress > 0)
                TryWrite(mainAddress, new byte[] { mainValue });
            if (bitfAddress > 0)
                TryWrite(bitfAddress, new byte[] { bitfValue });
        }
    }
}
