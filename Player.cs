using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Windwaker_coop
{
    class Player
    {
        public string playerName;
        public int playerNumber;
        public string serverName;
        public string serverDirectory;

        public FileSaver fs;
        public NotificationManager nm;

        private string notificationString = "";
        private int notificationAmount = 0;
        private List<byte> lastSavedData;

        private List<MemoryLocation> individualOverwrites;
        private List<uint> overwriteValues;

        public Player(string p, int n, string s, string directory)
        {
            playerName = p;
            playerNumber = n;
            serverName = s;
            serverDirectory = directory;
            fs = new FileSaver(directory);
            nm = new NotificationManager(directory, p);
            lastSavedData = new List<byte>();
            individualOverwrites = new List<MemoryLocation>();
            overwriteValues = new List<uint>();
        }

        public void beginSyncing()
        {
            //Start every x seconds saving to and reading from the file
            syncLoop(Program.syncDelay);
        }

        private async Task syncLoop(int loopTime)
        {
            while (true)
            {
                Program.setConsoleColor(5);
                if (Program.programStopped)
                    return;

                int timeStart = Environment.TickCount;
                List<byte> currentStageIdList = fs.readFromMemory(this, 0x803B53A4, 1);
                if (currentStageIdList != null)
                {
                    fs.updateCurrentStageInfo(this, currentStageIdList[0], true);
                }
                nm.ReadNotifications();

                List<byte> playerData = fs.readFromMemory(this);
                List<byte> hostData = null;
                if (playerData != null)
                    hostData = fs.readFromFile(serverDirectory + "\\host.txt");

                if (playerData != null && hostData != null)
                {
                    if (playerData.Count == hostData.Count)
                    {
                        //If one of them has any differences, then it will compare and rewrite the values
                        if (!checkIfSame(hostData, playerData))
                        {
                            List<byte> updatedData = compareHostAndPlayers(hostData, playerData);
                            fs.SaveToFile(serverDirectory + "\\host.txt", updatedData);

                            if (!checkIfSame(updatedData, lastSavedData))
                            {
                                fs.saveToMemory(this, updatedData);
                                lastSavedData = new List<byte>(updatedData);
                                if (currentStageIdList != null)
                                    fs.updateCurrentStageInfo(this, currentStageIdList[0], false);
                            }
                            else
                                Program.displayDebug("Skipping sync because this player has no differences from last time", 1);
                        }
                        else
                        {
                            Program.displayDebug("Skipping sync because this player has no differences from the host", 1);
                        }
                    }
                    else
                    {
                        Program.displayError("host.txt file is wrong size");
                    }
                }
                Program.displayDebug("Time taken to complete entire sync loop: " + (Environment.TickCount - timeStart) + " milliseconds", 1);
                Program.setConsoleColor(5);
                await Task.Delay(loopTime);
                Program.displayDebug("", 1);
            }
        }

        //Compares each line in the hostList to the playerList & updates newList to be complete
        private List<byte> compareHostAndPlayers(List<byte> host, List<byte> player)
        {
            List<byte> completeData = new List<byte>();
            notificationString = ""; notificationAmount = 0;
            int byteListIndex = 0;

            uint hostNum = 0;
            uint playerNum = 0;
            for (int locationListIndex = 0; locationListIndex < fs.memoryLocations.Count; locationListIndex++) //loops through each memoryLocation object
            {
                MemoryLocation memLoc = fs.memoryLocations[locationListIndex];
                hostNum = getNumberFromByteList(host, byteListIndex, memLoc.size);
                playerNum = getNumberFromByteList(player, byteListIndex, memLoc.size);
                uint newNum = hostNum;

                if (playerNum != hostNum)
                {
                    if (playerNum >= memLoc.lowerValue && playerNum <= memLoc.higherValue)
                    {
                        //Use each comparison type to determine the value for newNum & process/send a notification
                        switch (memLoc.compareId)
                        {
                            case 0: //Greater than previous value
                                if (playerNum > hostNum)
                                {
                                    newNum = playerNum;
                                    calculateNotification(playerNum, hostNum, memLoc);
                                }
                                break;
                            case 1: //Less than previous value
                                if (playerNum < hostNum)
                                {
                                    newNum = playerNum;
                                    calculateNotification(playerNum, hostNum, memLoc);
                                }
                                break;
                            case 2: //Greater than the previous value or changed from 255
                                if ((playerNum > hostNum && playerNum != 255) || (playerNum < 255 && hostNum == 255))
                                {
                                    newNum = playerNum;
                                    calculateNotification(playerNum, hostNum, memLoc);
                                }
                                break;
                            case 3: //Bottles 
                                if (playerNum < hostNum && hostNum == 255) //Obtained new bottle
                                {
                                    newNum = playerNum;
                                    calculateNotification(playerNum, hostNum, memLoc);
                                }
                                break;
                            case 4: //Different than previous value, but combines them (Max health, etc)

                                break;
                            case 9: //Combines the set flags in a bitfield
                                newNum = playerNum | hostNum;
                                if ((playerNum & (playerNum ^ hostNum)) > 0) //Checks if the new number has at least one bit more than the host
                                    calculateNotification(playerNum, hostNum, memLoc);

                                //If one of these events causes a time change, set the time in the other player's games
                                if (memLoc.type.Contains("time"))
                                    checkAndUpdateNewTime(hostNum, playerNum, (uint)memLoc.startAddress.ToInt64());

                                if (memLoc.type == "event" || memLoc.type == "eventtime" || memLoc.type == "dungeon") // Just for testing to see value changes
                                {
                                    Program.setConsoleColor(3);
                                    Console.WriteLine("Address 0x" + memLoc.startAddress.ToInt64().ToString("X") + " changed from " + Convert.ToString(hostNum, 2).PadLeft(32, '0') + " (host) to " + Convert.ToString(newNum, 2).PadLeft(32, '0'));
                                }
                                break;
                            default:
                                Program.displayDebug("\"" + memLoc.compareId + "\" is not a valid compareId", 2);
                                break;
                        }
                        //Reset the bytes in individual data to whatever the player had before (Certain dungeon data, bottles, hero's charm)
                        if (memLoc.individualBits != 0 && (memLoc.individualBits != 255 || memLoc.individualBits == 255 && playerNum != 255))
                        {
                            for (int i = 0; i < 32; i++) //for every bit...
                            {
                                uint mask = (uint)1 << i;
                                if ((memLoc.individualBits & mask) != 0) //if this bit should not be synced...
                                {
                                    newNum = (newNum & ~mask) | (playerNum & mask);
                                }
                            }
                            Program.displayDebug("Kept the item " + memLoc.name.Substring(0, memLoc.name.Length - 2) + " as 0d" + newNum, 2);
                        }
                    }
                    else
                    {
                        Program.displayDebug("The value at 0x" + memLoc.startAddress.ToInt64().ToString("X") + " is not inside of an acceptable range (" + playerNum + "). It was not synced to the host.", 2);
                    }
                }

                completeData.AddRange(getByteArrayFromNumber(newNum, memLoc.size));
                byteListIndex += memLoc.size;
            }

            if (notificationString.Length > 0)
            {
                nm.SendNotifications(notificationString.Substring(0, notificationString.Length - 1), notificationAmount);
            }
            return completeData;
        }

        //Determines whether or not to send a notification & calculates what it should be
        private void calculateNotification(uint playerNum, uint hostNum, MemoryLocation memLoc)
        {
            //If the memoryLocation only has one possible value
            if (memLoc.cd.values == null || memLoc.cd.text == null)
            {
                if (memLoc.name != "")
                {
                    notificationString += nm.getNotificationText(playerName, memLoc.name);
                    notificationAmount++;
                }
                return;
            }

            //If the memoryLocation has different possible levels, get exact notification
            if (memLoc.cd.bitfield)
            {
                for (int i = 0; i < memLoc.cd.values.Length; i++) //checks each bit and only sends notification if the player just set it
                {
                    if (ReadWrite.bitSet(playerNum, memLoc.cd.values[i]) && !ReadWrite.bitSet(hostNum, memLoc.cd.values[i]))
                    {
                        notificationString += nm.getNotificationText(playerName, memLoc.cd.text[i]);
                        notificationAmount++;
                    }
                }
            }
            else
            {
                for (int i = 0; i < memLoc.cd.values.Length; i++)  //compares the new value to everything in the list of possible values
                {
                    if (playerNum == memLoc.cd.values[i])
                    {
                        notificationString += nm.getNotificationText(playerName, memLoc.cd.text[i]);
                        notificationAmount++;
                        return;
                    }
                }
                Program.displayDebug("ComparisonData loop did not find any expected value", 2);
            }
        }

        //Checks if a timechanging event bit has been set and writes to that spot in memory
        private void checkAndUpdateNewTime(uint host, uint player, uint memAddress)
        {
            int bit = -1; float newTime = 0;
            switch (memAddress)
            {
                case 0x803B5234: //Open spoils bag chest
                    bit = 27; newTime = 300; break;
                case 0x803B5238: //windfall wake up
                    bit = 7; newTime = 225; break;
                case 0x803B5248: //talked to quill on greatfish
                    bit = 15; newTime = 0; break;
                default:
                    return;
                    //arrive hyrule field (180), FF2
            }
            if (bit != -1 && (host & (1 << bit)) != 0 && (player & (1 << bit)) == 0)
            {
                Program.displayDebug("Setting new time to " + (newTime / 15) + ":00", 1);
                byte[] timeBytes = BitConverter.GetBytes(newTime);
                Array.Reverse(timeBytes);
                fs.saveToMemory(this, new List<byte>(timeBytes), 0x803B4C2C, 4);
            }
        }

        private static uint getNumberFromByteList(List<byte> list, int startIndex, int length)
        {
            byte[] bytes = new byte[4];
            string debugOuput = "Converting byte[] { ";
            for (int i = 0; i < length; i++)
            {
                bytes[length - 1 - i] = list[startIndex + i];
                debugOuput += list[startIndex + i].ToString("X") + " ";
            }
            Program.displayDebug(debugOuput + "} to integer: " + BitConverter.ToUInt32(bytes), 4);
            return BitConverter.ToUInt32(bytes);
        }

        public static byte[] getByteArrayFromNumber(uint number, int length)
        {
            byte[] fourByte = BitConverter.GetBytes(number);
            byte[] result = new byte[length];

            for (int i = 0; i < length; i++)
            {
                result[length - 1 - i] = fourByte[i];
            }
            return result;
        }

        //Assumes neither are null
        private bool checkIfSame(List<byte> one, List<byte> two)
        {
            if (one.Count != two.Count)
                return false;
            for (int i = 0; i < one.Count; i++)
                if (one[i] != two[i])
                    return false;
            return true;
        }
    }
}
