﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Windwaker_coop
{
    class Watcher
    {
        private MemoryReader mr;

        private uint[] addressStarts;
        private int[] addressLengths;
        private byte[] previousValues;
        private bool firstTime = true;

        public Watcher()
        {
            mr = new MemoryReader();
            createArrays();
        }

        private void createArrays()
        {
            //Windwaker event bitfields
            //addressStarts = new uint[] { 0x803B522C };
            //addressLengths = new int[] { 64 };

            // OOS Unknowns
            addressStarts = new uint[] { 0x5B0, 0x600, 0x610, 0x62A, 0x631, 0x63F, 0x65E, 0x692, 0x6BD, 0x6CA, 0x700 };
            addressLengths = new int[] { 16, 2, 6, 1, 9, 9, 4, 16, 1, 54, 0x400 };

            // OOA Unknown bitfields
            //uint baseAddr = 0x03800600;
            //addressStarts = new uint[] { baseAddr + 0x61, baseAddr + 0x9A, baseAddr + 0xB1, baseAddr + 0xB4, baseAddr + 0xC0, baseAddr + 0x16 };
            //addressLengths = new int[] { 0x11, 0x10, 0x01, 0x02, 0x10, 0x08 };

            int totalLength = 0;
            for (int i = 0; i < addressStarts.Length; i++)
            {
                totalLength += addressLengths[i];
                Output.text($"Watching addresses 0x{addressStarts[i].ToString("X")} - 0x{(addressStarts[i] + addressLengths[i] - 1).ToString("X")}", ConsoleColor.Yellow);
            }

            previousValues = new byte[totalLength];
        }

        public void beginWatching(int syncDelay)
        {
            watchLoop(syncDelay);
        }

        private async Task watchLoop(int loopTime)
        {
            while (Program.programSyncing)
            {
                //Gets the list of all bytes that are being watched
                List<byte> memory = new List<byte>();
                for (int i = 0; i < addressStarts.Length; i++)
                {
                    byte[] bytes = mr.readFromMemory(addressStarts[i], addressLengths[i]);
                    if (bytes == null) return;
                    memory.AddRange(bytes);
                }

                //For each memoryLocation, compare the value to previous value
                if (!firstTime)
                {
                    for (int i = 0; i < memory.Count; i++)
                    {
                        //Compare each byte to its previous value
                        if (memory[i] != previousValues[i])
                        {
                            Output.text($"Address: 0x{getAddressFromIdx(i).ToString("X")} has changed from 0x{previousValues[i].ToString("X")} to 0x{memory[i].ToString("X")}", ConsoleColor.Green);
                        }
                    }
                }

                previousValues = memory.ToArray();
                firstTime = false;
                Output.text("...", ConsoleColor.Yellow);
                await Task.Delay(loopTime);
            }
        }

        private uint getAddressFromIdx(int targetIdx)
        {
            int currIdx = 0;
            for (int i = 0; i < addressStarts.Length; i++)
            {
                if (currIdx + addressLengths[i] <= targetIdx)
                {
                    currIdx += addressLengths[i];
                }
                else
                {
                    return addressStarts[i] + (uint)targetIdx - (uint)currIdx;
                }
            }
            Output.error("Address not found in watcher list");
            return 0;
        }
    }
}
