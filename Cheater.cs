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
            uint baseItem = 0x803B4C44, baseOwner = 0x803B4C59;
            cheats = new Cheat[]
            {
                new Cheat("telescope", baseItem, 32, false),
                new Cheat("telescope", baseOwner, 1, false),
                new Cheat("sail", baseItem + 1, 120, false),
                new Cheat("sail", baseOwner + 1, 1, false),
                new Cheat("windwaker", baseItem + 2, 34, false),
                new Cheat("windwaker", baseOwner + 2, 1, false),
                new Cheat("grappling-hook", baseItem + 3, 37, false),
                new Cheat("grappling-hook", baseOwner + 3, 1, false),
                new Cheat("spoils-bag", baseItem + 4, 36, false),
                new Cheat("spoils-bag", baseOwner + 4, 1, false),
                new Cheat("boomerang", baseItem + 5, 45, false),
                new Cheat("boomerang", baseOwner + 5, 1, false),
                new Cheat("deku-leaf", baseItem + 6, 52, false),
                new Cheat("deku-leaf", baseOwner + 6, 1, false),

                new Cheat("tingle-tuner", baseItem + 7, 33, false),
                new Cheat("tingle-tuner", baseOwner + 7, 1, false),
                new Cheat("picto-box", baseItem + 8, 0, true, 2, new byte[] { 35, 38 }),
                new Cheat("picto-box", baseOwner + 8, 0, true, 2, new byte[] { 1, 3 }),
                new Cheat("iron-boots", baseItem + 9, 41, false),
                new Cheat("iron-boots", baseOwner + 9, 1, false),
                new Cheat("magic-armor", baseItem + 10, 42, false),
                new Cheat("magic-armor", baseOwner + 10, 1, false),
                new Cheat("bait-bag", baseItem + 11, 44, false),
                new Cheat("bait-bag", baseOwner + 11, 1, false),
                new Cheat("bow", baseItem + 12, 0, true, 3, new byte[] { 39, 53, 54 }),
                new Cheat("bow", baseOwner + 12, 0, true, 3, new byte[] { 1, 3, 7 }),
                new Cheat("bombs", baseItem + 13, 49, false),
                new Cheat("bombs", baseOwner + 13, 1, false),

                new Cheat("bottle-1", baseItem + 14, 0, true, 4, new byte[] { 80, 87, 85, 88 }),
                new Cheat("bottle-2", baseItem + 15, 0, true, 4, new byte[] { 80, 87, 85, 88 }),
                new Cheat("bottle-3", baseItem + 16, 0, true, 4, new byte[] { 80, 87, 85, 88 }),
                new Cheat("bottle-4", baseItem + 17, 0, true, 4, new byte[] { 80, 87, 85, 88 }),
                new Cheat("delivery-bag", baseItem + 18, 48, false),
                new Cheat("delivery-bag", baseOwner + 18, 1, false),
                new Cheat("hookshot", baseItem + 19, 47, false),
                new Cheat("hookshot", baseOwner + 19, 1, false),
                new Cheat("skull-hammer", baseItem + 20, 51, false),
                new Cheat("skull-hammer", baseOwner + 20, 1, false),

                new Cheat("sword", 0x803B4C16, 0, true, 4, new byte[] { 56, 57, 58, 62 }),
                new Cheat("sword", 0x803B4CBC, 0, true, 4, new byte[] { 1, 3, 7, 15 }),
                new Cheat("shield", 0x803B4C17, 0, true, 2, new byte[] { 59, 60 }),
                new Cheat("shield", 0x803B4CBD, 0, true, 2, new byte[] { 1, 3 }),
                new Cheat("power-bracelets", 0x803B4C18, 40, false),
                new Cheat("power-bracelets", 0x803B4CBE, 1, false),
                new Cheat("pirates-charm", 0x803B4CBF, 1, false),
                new Cheat("heros-charm", 0x803B4CC0, 1, false),

                new Cheat("song", 0x803B4CC5, 0, true, 6, new byte[] { 1, 3, 7, 15, 31, 63 }),
                new Cheat("pearl", 0x803B4CC7, 0, true, 3, new byte[] { 1, 3, 7 }),
                new Cheat("triforce-shard", 0x803B4CC6, 0, true, 8, new byte[] { 1, 3, 7, 15, 31, 63, 127, 255 }),

                new Cheat("wallet", 0x803B4C1A, 0, true, 3, new byte[] { 0, 1, 2 }),
                new Cheat("magic", 0x803B4C1B, 0, true, 2, new byte[] { 16, 32 }),
                new Cheat("quiver", 0x803B4C77, 0, true, 3, new byte[] { 30, 60, 99 }),
                new Cheat("bomb-bag", 0x803B4C78, 0, true, 3, new byte[] { 30, 60, 99 })

            };
        }

        public string processCommand(string[] arguments)
        {
            int number = -1;
            bool foundItem = false;

            if (!Program.enableCheats)
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
                return "Cheat avtivated!";
            else
                return "'" + arguments[1] + "' is not a valid item!";
        }

        private void writeCheatResult(uint address, byte theByte)
        {
            List<byte> data = new List<byte>(); data.Add(theByte);
            client.mr.saveToMemory(data, (IntPtr)address);
            client.sendNotification(client.playerName + " has used a cheat!", false);
        }
    }
}
