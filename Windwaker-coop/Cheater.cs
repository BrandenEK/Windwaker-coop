using System;
using System.Collections.Generic;

namespace Windwaker_coop
{
    class Cheater
    {
        private Client client;
        private Cheat[] cheats;

        public Cheater(Client client)
        {
            this.client = client;
            cheats = Program.currGame.getCheats();
        }

        public string processCommand(string[] arguments)
        {
            if (!Program.programSyncing) return "";

            int number = -1;
            bool foundItem = false;

            if (!Program.config.enableCheats)
                return "Cheats are disabled!";
            if (arguments.Length < 2 || arguments.Length > 3)
                return "Command 'give' takes either 1 or 2 arguments!";
            if (arguments.Length == 3 && !int.TryParse(arguments[2], out number))
                return "The 'number' argument was not a valid number!";

            //searchs for the specified item
            foreach (Cheat cheat in cheats)
            {
                if (cheat.itemName == arguments[1])
                {
                    byte toWrite = 0;
                    if (!cheat.requiresNumber)
                    {
                        if (arguments.Length > 2)
                            return "Item '" + cheat.itemName + "' does not require a number!";
                        toWrite = cheat.noNumberByte;
                    }
                    else
                    {
                        //Checks the possible values the number can be and sets byteToWrite equal to the corresponding value
                        if (arguments.Length < 3)
                            return "Item '" + cheat.itemName + "' requires a number!";
                        bool foundValue = false;

                        for (int i = 1; i <= cheat.maxValue; i++)
                        {
                            if (number == i)
                            {
                                toWrite = cheat.valuesToWrite[i - 1];
                                foundValue = true;
                                break;
                            }
                        }
                        if (!foundValue)
                            return "'" + number + "' is not a possible value for this item!";
                    }

                    foundItem = true;
                    writeCheatResult(cheat.address, toWrite);
                }
            }
            if (foundItem)
            {
                client.sendNotification(client.playerName + " has used a cheat!", false);
                return "Cheat activated!";
            }
            else
                return "'" + arguments[1] + "' is not a valid item!";
        }

        private void writeCheatResult(uint address, byte theByte)
        {
            List<byte> data = new List<byte>(); data.Add(theByte);
            client.mr.saveToMemory(data, (IntPtr)address);
        }
    }
}
