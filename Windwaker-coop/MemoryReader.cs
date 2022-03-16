using System;
using System.Collections.Generic;
using System.Text;

namespace Windwaker_coop
{
    class MemoryReader
    {
        public List<MemoryLocation> memoryLocations;

        public MemoryReader()
        {
            memoryLocations = new List<MemoryLocation>();
            Program.currGame.addMemoryLocations(memoryLocations);
        }

        public List<byte> getDefaultValues(User user)
        {
            List<byte> defaults = new List<byte>();
            foreach (MemoryLocation memLoc in memoryLocations)
            {
                defaults.AddRange(user.getByteArrayFromNumber(memLoc.defaultValue, memLoc.size));
            }
            return defaults;
        }

        public List<byte> readFromMemory()
        {
           // if (!checkMemoryInitialized(1))
            //    return null;

            List<byte> memoryList = new List<byte>();
            IntPtr sequenceStart = memoryLocations[0].startAddress;
            int sequenceLength = 0;

            for (int i = 0; i < memoryLocations.Count; i++)
            {
                MemoryLocation loc = memoryLocations[i];
                sequenceLength += loc.size;

                if (!(i < memoryLocations.Count - 1 && memoryLocations[i + 1].startAddress == loc.startAddress + loc.size))
                {
                    //reads the entire sequence then resets the sequence
                    Program.displayDebug("Reading contiguous region of " + sequenceLength + " bytes", 3);
                    byte[] value = ReadWrite.Read(1, sequenceStart, sequenceLength);
                    if (value == null)
                    {
                        Program.displayError("Aborting \"ReadFromMemory()\" due to null byte[]");
                        return null;
                    }
                    memoryList.AddRange(value);
                    sequenceLength = 0;
                    if (i < memoryLocations.Count - 1)
                        sequenceStart = memoryLocations[i + 1].startAddress;
                }
            }
            return memoryList;
        }

        public List<byte> readFromMemory(IntPtr customStartAddress, int customSize)
        {
            //if (!checkMemoryInitialized(1))
               // return null;

            byte[] value = ReadWrite.Read(1, customStartAddress, customSize);
            if (value == null)
            {
                Program.displayError("Aborting \"ReadFromMemory\" due to null byte[]");
                return null;
            }
            return new List<byte>(value);
        }

        public void saveToMemory(List<byte> saveData)
        {
            //Writes each value in saveData to the player's game's memory
            int byteListIndex = 0;
            IntPtr sequenceStart = memoryLocations[0].startAddress;
            int sequenceStartIndex = 0;

            for (int i = 0; i < memoryLocations.Count; i++)
            {
                MemoryLocation loc = memoryLocations[i];
                byteListIndex += loc.size;

                if (!(i < memoryLocations.Count - 1 && memoryLocations[i + 1].startAddress == loc.startAddress + loc.size))
                {
                    Program.displayDebug("Writing contiguous region of " + (byteListIndex - sequenceStartIndex) + " bytes", 3);
                    ReadWrite.Write(1, sequenceStart, saveData.GetRange(sequenceStartIndex, byteListIndex - sequenceStartIndex).ToArray());
                    if (i < memoryLocations.Count - 1)
                    {
                        sequenceStartIndex = byteListIndex;
                        sequenceStart = memoryLocations[i + 1].startAddress;
                    }
                }
            }
        }

        public void saveToMemory(List<byte> saveData, IntPtr customStartAddress)
        {
            //Writes each value in saveData to the player's game's memory
           // if (!checkMemoryInitialized(1))
              //  return;
            ReadWrite.Write(1, customStartAddress, saveData.ToArray());
        }

        private bool checkMemoryInitialized(int playerNumber)
        {
            byte[] wwBase = ReadWrite.Read(playerNumber, (IntPtr)0x7FFF0000, 6);
            if (wwBase == null)
                return false;

            string word = Encoding.UTF8.GetString(wwBase);
            if (word != "" && word != "GZLE01")
            {
                Program.displayError($"{Program.currGame.gameName} memory not initialized!");
                return false;
            }
            return true;
        }
    }
}
