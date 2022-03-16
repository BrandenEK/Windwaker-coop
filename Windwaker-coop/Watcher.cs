using System;
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
            Program.setConsoleColor(4);

            //Windwaker event bitfields
            addressStarts = new uint[] { 0x803B522C };
            addressLengths = new int[] { 64 };

            int totalLength = 0;
            for (int i = 0; i < addressStarts.Length; i++)
            {
                totalLength += addressLengths[i];
                Console.WriteLine($"Watching addresses 0x{addressStarts[i].ToString("X")} - 0x{(addressStarts[i] + addressLengths[i] - 1).ToString("X")}");
            }

            previousValues = new byte[totalLength];
        }

        public void beginWatching(int syncDelay)
        {
            watchLoop(syncDelay);
        }

        private async Task watchLoop(int loopTime)
        {
            while (!Program.programStopped)
            {
                //Gets the list of all bytes that are being watched
                List<byte> memory = new List<byte>();
                for (int i = 0; i < addressStarts.Length; i++)
                {
                    List<byte> bytes = mr.readFromMemory((IntPtr)addressStarts[i], addressLengths[i]);
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
                            Program.setConsoleColor(3);
                            Console.WriteLine($"Address: 0x{getAddressFromIdx(i).ToString("X")} has changed from {previousValues[i]} to {memory[i]}");
                        }
                    }
                }

                previousValues = memory.ToArray();
                firstTime = false;
                Program.setConsoleColor(4);
                Console.WriteLine("...");
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
            Program.displayError("Address not found in watcher list");
            return 0;
        }
    }
}
