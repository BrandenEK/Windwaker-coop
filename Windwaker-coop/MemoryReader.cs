using System;
using System.Collections.Generic;
using System.Text;

namespace Windwaker_coop
{
    class MemoryReader
    {
        public byte[] readFromMemory(List<MemoryLocation> memoryLocations)
        {
            if (!checkMemoryInitialized(1))
                return null;

            List<byte> memoryList = new List<byte>();
            uint sequenceStart = memoryLocations[0].startAddress;
            int sequenceLength = 0;

            for (int i = 0; i < memoryLocations.Count; i++)
            {
                MemoryLocation loc = memoryLocations[i];
                sequenceLength += loc.size;

                if (!(i < memoryLocations.Count - 1 && memoryLocations[i + 1].startAddress == loc.startAddress + loc.size))
                {
                    //reads the entire sequence then resets the sequence
                    Output.debug("Reading contiguous region of " + sequenceLength + " bytes", 2);
                    byte[] value = ReadWrite.Read(1, sequenceStart, sequenceLength);
                    if (value == null)
                    {
                        Output.error("Aborting \"ReadFromMemory()\" due to null byte[]");
                        return null;
                    }
                    memoryList.AddRange(value);
                    sequenceLength = 0;
                    if (i < memoryLocations.Count - 1)
                        sequenceStart = memoryLocations[i + 1].startAddress;
                }
            }
            return memoryList.ToArray();
        }

        public byte[] readFromMemory(uint customStartAddress, int customSize)
        {
            if (!checkMemoryInitialized(1))
                return null;

            byte[] value = ReadWrite.Read(1, customStartAddress, customSize);
            if (value == null)
            {
                Output.error("Aborting \"ReadFromMemory\" due to null byte[]");
                return null;
            }
            return value;
        }

        public void saveToMemory(byte[] saveData, List<MemoryLocation> memoryLocations)
        {
            //Writes each value in saveData to the player's game's memory
            if (!checkMemoryInitialized(1))
                return;

            List<byte> data = new List<byte>(saveData);
            int byteListIndex = 0;
            uint sequenceStart = memoryLocations[0].startAddress;
            int sequenceStartIndex = 0;

            for (int i = 0; i < memoryLocations.Count; i++)
            {
                MemoryLocation loc = memoryLocations[i];
                byteListIndex += loc.size;

                if (!(i < memoryLocations.Count - 1 && memoryLocations[i + 1].startAddress == loc.startAddress + loc.size))
                {
                    Output.debug("Writing contiguous region of " + (byteListIndex - sequenceStartIndex) + " bytes", 2);
                    ReadWrite.Write(1, sequenceStart, data.GetRange(sequenceStartIndex, byteListIndex - sequenceStartIndex).ToArray());
                    if (i < memoryLocations.Count - 1)
                    {
                        sequenceStartIndex = byteListIndex;
                        sequenceStart = memoryLocations[i + 1].startAddress;
                    }
                }
            }
        }

        public void saveToMemory(byte[] saveData, uint customStartAddress)
        {
            //Writes each value in saveData to the player's game's memory
            if (!checkMemoryInitialized(1))
                return;
            ReadWrite.Write(1, customStartAddress, saveData);
        }

        private bool checkMemoryInitialized(int playerNumber)
        {
            byte[] identityValue = ReadWrite.Read(playerNumber, Program.currGame.identityAddress, Program.currGame.identityText.Length);
            if (identityValue == null)
                return false;

            string word = Encoding.UTF8.GetString(identityValue);
            if (word != "" && word != Program.currGame.identityText)
            {
                Output.error($"{Program.currGame.gameName} memory not initialized!");
                return false;
            }
            return true;
        }
    }
}
