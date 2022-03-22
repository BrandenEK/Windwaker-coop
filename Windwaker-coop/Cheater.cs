using System;
using System.Collections.Generic;

namespace Windwaker_coop
{
    class Cheater
    {
        private Cheat[] cheats;

        public Cheater()
        {
            cheats = Program.currGame.getCheats();
        }

        public string processCommand(Client client, string[] arguments)
        {
            if (!Program.programSyncing) return "";

            int number = -1;
            bool foundItem = false;

            if (!Program.config.enableCheats)
                return "Cheats are disabled!";
            if (arguments.Length < 1 || arguments.Length > 2)
                return "Command 'give' takes either 1 or 2 arguments!";
            if (arguments.Length == 2 && !int.TryParse(arguments[1], out number))
                return "The 'number' argument was not a valid number!";

            //searchs for the specified item
            foreach (Cheat cheat in cheats)
            {
                if (cheat.itemName == arguments[0])
                {
                    byte toWrite = 0;
                    if (!cheat.requiresNumber)
                    {
                        if (arguments.Length > 1)
                            return "Item '" + cheat.itemName + "' does not require a number!";
                        toWrite = cheat.noNumberByte;
                    }
                    else
                    {
                        //Checks the possible values the number can be and sets byteToWrite equal to the corresponding value
                        if (arguments.Length < 2)
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
                    client.mr.saveToMemory(new byte[] { toWrite }, (IntPtr)cheat.address);
                }
            }
            if (foundItem)
            {
                client.sendNotification(client.playerName + " has used a cheat!", false);
                return "Cheat activated!";
            }
            else
                return "'" + arguments[0] + "' is not a valid item!";
        }
    }
}
