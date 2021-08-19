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
            addMemoryLocations();
        }

        public List<byte> getDefaultValues()
        {
            List<byte> defaults = new List<byte>();
            foreach (MemoryLocation memLoc in memoryLocations)
            {
                defaults.AddRange(Player.getByteArrayFromNumber(memLoc.defaultValue, memLoc.size));
            }
            return defaults;
        }

        public List<byte> readFromMemory()
        {
            if (!checkMemoryInitialized(1))
                return null;

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
            if (!checkMemoryInitialized(1))
                return null;

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
            if (!checkMemoryInitialized(1))
                return;
            ReadWrite.Write(1, customStartAddress, saveData.ToArray());
        }

        private bool checkMemoryInitialized(int playerNumber)
        {
            byte[] wwBase = ReadWrite.Read(playerNumber, (IntPtr)0x7FFF0000, 6);
            string word = "";
            if (wwBase != null)
                word = Encoding.UTF8.GetString(wwBase);
            if (word != "" && word != "GZLE01")
            {
                Program.displayError("Wind Waker memory not initialized!");
                return false;
            }
            return true;
        }

        private void addMemoryLocations()
        {
            SyncSettings settings = SyncSettings.getDefaultSettings();
            //Min & Max values are in decimal format
            ComparisonData empty = new ComparisonData();

            //Stat upgrades
            if (settings.maxHealth)
                memoryLocations.Add(new MemoryLocation(0x803B4C08, 2, "more health*2", "stat", 0, 12, 80, 12, 0, empty));

            if (settings.stats)
            {
                //memoryLocations.Add(new MemoryLocation(0x803B4DA8, 1, "began the Hero's Quest*9", "stat", 0, 0, 255, 0, 0, empty));
            }

            //Inventory items
            if (settings.inventoryItems)
            {
                memoryLocations.Add(new MemoryLocation(0x803B4C44, 1, "Telescope*0", "item", 1, 32, 255, 255, 0, empty));
                memoryLocations.Add(new MemoryLocation(0x803B4C45, 1, "Sail*0", "item", 1, 120, 255, 255, 0, empty));
                memoryLocations.Add(new MemoryLocation(0x803B4C46, 1, "Wind Waker*0", "item", 1, 34, 255, 255, 0, empty));
                memoryLocations.Add(new MemoryLocation(0x803B4C47, 1, "Grappling Hook*0", "item", 1, 37, 255, 255, 0, empty));
                memoryLocations.Add(new MemoryLocation(0x803B4C48, 1, "Spoils Bag*0", "item", 1, 36, 255, 255, 0, empty));
                memoryLocations.Add(new MemoryLocation(0x803B4C49, 1, "Boomerang*0", "item", 1, 45, 255, 255, 0, empty));
                memoryLocations.Add(new MemoryLocation(0x803B4C4A, 1, "Deku Leaf*0", "item", 1, 52, 255, 255, 0, empty));

                memoryLocations.Add(new MemoryLocation(0x803B4C4B, 1, "Tingle Tuner*0", "item", 1, 33, 255, 255, 0, empty));
                memoryLocations.Add(new MemoryLocation(0x803B4C4C, 1, "Picto Box*0", "item", 2, 35, 255, 255, 0,
                    new ComparisonData(new uint[] { 35, 38 }, new string[] { "Picto Box*0", "Deluxe Picto Box*0" }, false)));
                memoryLocations.Add(new MemoryLocation(0x803B4C4D, 1, "Iron Boots*0", "item", 1, 41, 255, 255, 0, empty));
                memoryLocations.Add(new MemoryLocation(0x803B4C4E, 1, "Magic Armor*0", "item", 1, 42, 255, 255, 0, empty));
                memoryLocations.Add(new MemoryLocation(0x803B4C4F, 1, "Bait Bag*0", "item", 1, 44, 255, 255, 0, empty));
                memoryLocations.Add(new MemoryLocation(0x803B4C50, 1, "Bow*1", "item", 2, 39, 255, 255, 0,
                    new ComparisonData(new uint[] { 39, 53, 54 }, new string[] { "Hero's Bow*0", "Fire & Ice Arrows*0", "Light Arrows*0" }, false))); //Doesn't represent items accurately
                memoryLocations.Add(new MemoryLocation(0x803B4C51, 1, "Bombs*2", "item", 1, 49, 255, 255, 0, empty));

                if (settings.bottles)
                {
                    memoryLocations.Add(new MemoryLocation(0x803B4C52, 1, "Bottle #1*2", "item", 3, 80, 255, 255, 255, empty));
                    memoryLocations.Add(new MemoryLocation(0x803B4C53, 1, "Bottle #2*2", "item", 3, 80, 255, 255, 255, empty));
                    memoryLocations.Add(new MemoryLocation(0x803B4C54, 1, "Bottle #3*2", "item", 3, 80, 255, 255, 255, empty));
                    memoryLocations.Add(new MemoryLocation(0x803B4C55, 1, "Bottle #4*2", "item", 3, 80, 255, 255, 255, empty));
                }

                memoryLocations.Add(new MemoryLocation(0x803B4C56, 1, "Delivery Bag*0", "item", 1, 48, 255, 255, 0, empty));
                memoryLocations.Add(new MemoryLocation(0x803B4C57, 1, "Hookshot*0", "item", 1, 47, 255, 255, 0, empty));
                memoryLocations.Add(new MemoryLocation(0x803B4C58, 1, "Skull Hammer*0", "item", 1, 51, 255, 255, 0, empty));

                uint baseItem = 0x803B4C59; //item ownership bitfields
                for (uint i = 0; i < 21; i++)
                {
                    if (i == 0)
                        memoryLocations.Add(new MemoryLocation(baseItem + i, 1, "done something with the weird telescope value*9", "flag", 9, 0, 255, 0, 0, empty)); //telescope is weird
                    else if (i == 8)
                        memoryLocations.Add(new MemoryLocation(baseItem + i, 1, "", "flag", 9, 0, 3, 0, 0, empty)); //pictobox is special
                    else if (i == 12)
                        memoryLocations.Add(new MemoryLocation(baseItem + i, 1, "", "flag", 9, 0, 7, 0, 0, empty)); //arrows are special
                    else
                        memoryLocations.Add(new MemoryLocation(baseItem + i, 1, "", "flag", 0, 0, 1, 0, 0, empty));
                }
            }

            //Equipment items
            if (settings.equipmentItems)
            {
                memoryLocations.Add(new MemoryLocation(0x803B4C16, 1, "sword*1", "item", 2, 56, 255, 255, 0,
                    new ComparisonData(new uint[] { 56, 57, 58, 62 }, new string[] { "Hero's Sword*0", "Master Sword (Unpowered)*0", "Master Sword (Half power)*0", "Master Sword (Fully powered)*0" }, false))); //sword level
                memoryLocations.Add(new MemoryLocation(0x803B4C17, 1, "shield*1", "item", 2, 59, 255, 255, 0,
                    new ComparisonData(new uint[] { 59, 60 }, new string[] { "Hero's Shield*0", "Mirror Shield*0" }, false))); //shield level
                memoryLocations.Add(new MemoryLocation(0x803B4C18, 1, "Power Bracelets*0", "item", 1, 40, 255, 255, 0, empty)); //power bracelets
                memoryLocations.Add(new MemoryLocation(0x803B4C19, 1, "Mystery item - I have no idea what this is for (4C19)*0", "item", 0, 0, 255, 0, 0, empty));
            }

            //Capacity upgrades
            if (settings.capacities)
            {
                memoryLocations.Add(new MemoryLocation(0x803B4C1A, 1, "wallet*1", "capacity", 0, 0, 2, 0, 0,
                    new ComparisonData(new uint[] { 1, 2 }, new string[] { "1000 rupee wallet*0", "5000 rupee wallet*0" }, false)));
                memoryLocations.Add(new MemoryLocation(0x803B4C1B, 1, "magic meter*1", "stat", 0, 0, 32, 0, 0,
                    new ComparisonData(new uint[] { 16, 32 }, new string[] { "magic meter*1", "an enhanced magic meter*2" }, false)));
                memoryLocations.Add(new MemoryLocation(0x803B4C77, 1, "quiver*1", "capacity", 0, 0, 99, 0, 0,
                    new ComparisonData(new uint[] { 30, 60, 99 }, new string[] { "small quiver*1", "medium quiver*1", "large quiver*1" }, false)));
                memoryLocations.Add(new MemoryLocation(0x803B4C78, 1, "bomb bag*1", "capacity", 0, 0, 99, 0, 0,
                    new ComparisonData(new uint[] { 30, 60, 99 }, new string[] { "small bomb bag*1", "medium bomb bag*1", "large bomb bag*1" }, false)));
            }

            //More equipment items
            if (settings.equipmentItems)
            {
                memoryLocations.Add(new MemoryLocation(0x803B4CBC, 1, "", "flag", 9, 0, 15, 0, 0, empty)); //sword bitfield
                memoryLocations.Add(new MemoryLocation(0x803B4CBD, 1, "", "flag", 9, 0, 3, 0, 0, empty)); //shield bitfield
                memoryLocations.Add(new MemoryLocation(0x803B4CBE, 1, "", "flag", 9, 0, 1, 0, 0, empty)); //bracelets bitfield
                memoryLocations.Add(new MemoryLocation(0x803B4CBF, 1, "pirate's charm*0", "item", 9, 0, 3, 0, 0,
                    new ComparisonData(new uint[] { 0 }, new string[] { "Pirate's Charm*0" }, true))); //pirate's charm bitfield
                memoryLocations.Add(new MemoryLocation(0x803B4CC0, 1, "hero's charm*0", "item", 9, 0, 3, 0, 2,
                    new ComparisonData(new uint[] { 0 }, new string[] { "Hero's Charm*0" }, true))); //hero's charm bitfield
            }

            //Progression items
            if (settings.storyItems)
            {
                memoryLocations.Add(new MemoryLocation(0x803B4CC5, 1, "song*1", "item", 9, 0, 63, 0, 0, new ComparisonData(new uint[] { 0, 1, 2, 3, 4, 5 },
                    new string[] { "Wind's Requiem*3", "Ballad of Gales*3", "Command Melody*3", "Earth God's Lyric*3", "Wind God's Aria*3", "Song of Passing*3" }, true))); //songs bitfield
                memoryLocations.Add(new MemoryLocation(0x803B4CC6, 1, "triforce shard*1", "story", 9, 0, 255, 0, 0, new ComparisonData(new uint[] { 0, 1, 2, 3, 4, 5, 6, 7 },
                    new string[] { "Triforce Shard #1*2", "Triforce Shard #2*2", "Triforce Shard #3*2", "Triforce Shard #4*2", "Triforce Shard #5*2", "Triforce Shard #6*2",
                    "Triforce Shard #7*2", "Triforce Shard #8*2", }, true))); //triforce shards bitfield
                memoryLocations.Add(new MemoryLocation(0x803B4CC7, 1, "new pearl*1", "story", 9, 0, 7, 0, 0,
                    new ComparisonData(new uint[] { 1, 2, 0 }, new string[] { "Din's Pearl*2", "Farore's Pearl*2", "Nayru's Pearl*2" }, true))); //pearls bitfield
            }

            //Charts
            if (settings.charts)
            {
                string[] chartsOne = new string[] { "Triforce Chart 1*2", "Triforce Chart 2*2", "Triforce Chart 3*2", "Triforce Chart 4*2", "Triforce Chart 5*2", "Triforce Chart 6*2", "Triforce Chart 7*2", "Triforce Chart 8*2",
                "Treasure Chart 11*2", "Treasure Chart 15*2", "Treasure Chart 30*2", "Treasure Chart 20*2", "Treasure Chart 5*2", "Treasure Chart 23*2", "Treasure Chart 31*2", "Treasure Chart 33*2",
                "Treasure Chart 2*2", "Treasure Chart 38*2", "Treasure Chart 39*2", "Treasure Chart 24*2", "Treasure Chart 6*2", "Treasure Chart 12*2", "Treasure Chart 35*2", "Treasure Chart 1*2",
                "Treasure Chart 29*2", "Treasure Chart 34*2", "Treasure Chart 18*2", "Treasure Chart 16*2", "Treasure Chart 28*2", "Treasure Chart 4*2", "Treasure Chart 3*2", "Treasure Chart 40*2" };
                string[] chartsTwo = new string[] { "Treasure Chart 10*2", "Treasure Chart 14*2", "Tingle's Chart*2", "Ghost Ship Chart*0", "Treasure Chart 9*2", "Treasure Chart 22*2", "Treasure Chart 36*2", "Treasure Chart 17*2",
                "Treasure Chart 25*2", "Treasure Chart 37*2", "Treasure Chart 8*2", "Treasure Chart 26*2", "Treasure Chart 41*2", "Treasure Chart 19*2", "Treasure Chart 32*2", "Treasure Chart 13*2",
                "Treasure Chart 21*2", "Treasure Chart 27*2", "Treasure Chart 7*2", "IN-credible Chart*0", "Octo Chart*0", "Great Fairy Chart*0", "Island Hearts Chart*0", "Sea Hearts Chart*0",
                "Secret Cave Chart*0", "Light Ring Chart*0", "Platform Chart*0", "Beedle's Chart*2", "Submarine Chart*0", "", "", "" };
                uint[] fourByteValues = new uint[32];
                for (uint i = 0; i < fourByteValues.Length; i++)
                    fourByteValues[i] = i;

                memoryLocations.Add(new MemoryLocation(0x803B4CDC, 4, "treasure/triforce chart*1", "chart", 9, 0, uint.MaxValue, 0, 0,
                    new ComparisonData(fourByteValues, chartsOne, true))); //owned charts bitfield
                memoryLocations.Add(new MemoryLocation(0x803B4CE0, 4, "treasure/triforce chart*1", "chart", 9, 0, uint.MaxValue, 0, 0,
                    new ComparisonData(fourByteValues, chartsTwo, true)));
                memoryLocations.Add(new MemoryLocation(0x803B4CE4, 4, "extra chart bitfield*9", "chart", 9, 0, uint.MaxValue, 0, 0, empty));
                memoryLocations.Add(new MemoryLocation(0x803B4CE8, 4, "extra chart bitfield*9", "chart", 9, 0, uint.MaxValue, 0, 0, empty));

                for (int i = 0; i < 4; i++)
                    memoryLocations.Add(new MemoryLocation(0x803B4CEC + 4 * (uint)i, 4, "", "chart", 9, 0, uint.MaxValue, 0, 0, empty)); //opened charts bitfield
                for (int i = 0; i < 4; i++)
                    memoryLocations.Add(new MemoryLocation(0x803B4CFC + 4 * (uint)i, 4, "", "location", 9, 0, uint.MaxValue, 0, 0, empty)); //obtained sunked treasure bitfield
            }

            //Sectors
            if (settings.seaMap)
            {
                for (uint i = 0; i < 49; i++)
                {
                    byte def = 0;
                    if (i == 0 || i == 10 || i == 43) def = 3;
                    string sectorName = "ABCDEFG".Substring((int)i / 7, 1) + (i % 7 + 1).ToString();

                    memoryLocations.Add(new MemoryLocation(0x803B4D0C + i, 1, "new sector*9", "sector", 9, 0, 3, def, 0,
                        new ComparisonData(new uint[] { 1, 0 }, new string[] { "visited sector " + sectorName + " for the first time*9", "mapped out sector " + sectorName + "*9" }, true)));
                }
            }
            if (settings.charts)
                memoryLocations.Add(new MemoryLocation(0x803B4D4D, 1, "deciphered a new triforce chart*9", "chart", 9, 0, 255, 0, 0, new ComparisonData(new uint[] { 0, 1, 2, 3, 4, 5, 6, 7 },
                    new string[] { "Triforce Chart 1*4", "Triforce Chart 2*4", "Triforce Chart 3*4", "Triforce Chart 4*4", "Triforce Chart 5*4", "Triforce Chart 6*4", "Triforce Chart 7*4", "Triforce Chart 8*4" }, true))); //deciphered charts bitfield

            //Stage infos
            if (settings.stageInfos)
            {
                string[] stageNames = new string[] { "the Great Sea", "the Great Sea (alt)", "the Forsaken Fortress", "Dragon Roost Cavern", "the Forbidden Woods", "the Tower of the Gods", "the Earth Temple", "the Wind Temple",
                "Ganon's Tower", "Flooded Hyrule", "a ship", "a house/misc", "a cave", "a cave/ship", "killing a blue chu chu", "a test map" };

                for (uint i = 0; i < 16; i++) //loops through each stage
                {
                    uint stageOffset = 0x803B4F88 + i * 36;
                    memoryLocations.Add(new MemoryLocation(stageOffset, 4, "opened a chest in " + stageNames[i] + "*9", "dungeon", 9, 0, uint.MaxValue, 0, 0, empty)); //chest open bitfield
                    for (uint j = 0; j < 4; j++)
                        memoryLocations.Add(new MemoryLocation(stageOffset + 4 + (4 * j), 4, "triggered an event in " + stageNames[i] + "*9", "dungeon", 9, 0, uint.MaxValue, 0, 0, empty)); //event flag bitfield

                    memoryLocations.Add(new MemoryLocation(stageOffset + 20, 4, "picked up an item in " + stageNames[i] + "*9", "dungeon", 9, 0, uint.MaxValue, 0, 0, empty)); //item pickup bitfield
                    memoryLocations.Add(new MemoryLocation(stageOffset + 24, 4, "visited a new room in " + stageNames[i] + "*9", "dungeon", 9, 0, uint.MaxValue, 0, 0, empty)); //visited room bitfields
                    memoryLocations.Add(new MemoryLocation(stageOffset + 28, 4, "visited a new room in " + stageNames[i] + "*9", "dungeon", 9, 0, uint.MaxValue, 0, 0, empty));

                    if (settings.smallKeys)
                        memoryLocations.Add(new MemoryLocation(stageOffset + 32, 1, "small key to " + stageNames[i] + "*1", "dungeon", 0, 0, 255, 0, 0, empty)); //number of small keys
                    memoryLocations.Add(new MemoryLocation(stageOffset + 33, 1, "important*9", "dungeon", 9, 0, 255, 0, 0,
                        new ComparisonData(new uint[] { 0, 1, 2, 3, 4 }, new string[] { "map to " + stageNames[i] + "*0", "compass to " + stageNames[i] + "*0", "big key to " + stageNames[i] + "*0",
                        "defeated the boss of " + stageNames[i] + "*9", "heart container in " + stageNames[i] + "*0" }, true))); //important dungeon flags bitfield
                    memoryLocations.Add(new MemoryLocation(stageOffset + 34, 2, "changed a stageInfoBuffer - you should not see this!*9", "buffer", 0, 0, ushort.MaxValue, 0, 0, empty));
                }

                //Warp pots
                memoryLocations.Add(new MemoryLocation(0x803B52CB, 1, "opened a WT warp pot*9", "dungeon", 9, 0, 7, 0, 0, empty));
                memoryLocations.Add(new MemoryLocation(0x803B52CC, 1, "opened a DRC-FW-TotG inter-dungeon warp pot*9", "dungeon", 9, 0, 7, 0, 0, empty));
                memoryLocations.Add(new MemoryLocation(0x803B52CD, 1, "opened a FW warp pot*9", "dungeon", 9, 0, 7, 0, 0, empty));
                memoryLocations.Add(new MemoryLocation(0x803B52CE, 1, "opened a DRC warp pot*9", "dungeon", 9, 0, 7, 0, 0, empty));
                memoryLocations.Add(new MemoryLocation(0x803B52CF, 1, "opened a ET warp pot*9", "dungeon", 9, 0, 7, 0, 0, empty));
                memoryLocations.Add(new MemoryLocation(0x803B52D0, 1, "opened a FF-ET-WT inter-dungeon warp pot*9", "dungeon", 9, 0, 7, 0, 0, empty));
            }

            //Event bitfields
            if (settings.events)
            {
                memoryLocations.Add(new MemoryLocation(0x803B522C, 4, "", "event", 9, 0, uint.MaxValue, 0, 0,
                    new ComparisonData(new uint[] { 24, 16 }, new string[] { "saw Tetra fall into the Forest of Fairies*9", "rescued Tetra from the Forest of Fairies*9" }, true))); //event field 20
                memoryLocations.Add(new MemoryLocation(0x803B5230, 4, "", "event", 9, 0, uint.MaxValue, 0, 0, empty)); //event field 30
                memoryLocations.Add(new MemoryLocation(0x803B5234, 4, "", "eventtime", 9, 0, uint.MaxValue, 0, 0, new ComparisonData(new uint[] { 24, 25, 27, 7 },
                    new string[] { "activated a spawn in the Forsaken Fortress (Do not leave through the door on the Pirate Ship)*9", "finished speaking to Tetra after rescuing her on Ouset*9",
                    "activated a spawn on the Pirate Ship (Docked) and set the time to 20:00*9", "freed Tingle from his cell*9" }, true)));
                memoryLocations.Add(new MemoryLocation(0x803B5238, 4, "", "eventtime", 9, 0, uint.MaxValue, 0, 0,
                    new ComparisonData(new uint[] { 9, 7, 2 }, new string[] { "retreived the Father's Letter from Medli*9", "activated a new spawn on Windfall Island and set the time to 15:00*9", "calmed down Prince Komali*9" }, true)));
                memoryLocations.Add(new MemoryLocation(0x803B523C, 4, "", "event", 9, 0, uint.MaxValue, 0, 0,
                    new ComparisonData(new uint[] { 17, 6 }, new string[] { "thrown Medli onto the Dragon Roost Cavern ledge*9", "caught all of the Killer Bees*9" }, true)));
                memoryLocations.Add(new MemoryLocation(0x803B5240, 4, "", "event", 9, 0, uint.MaxValue, 0, 0,
                    new ComparisonData(new uint[] { 31, 30, 28, 4, 2 }, new string[] { "Din's Pearl*5", "Farore's Pearl*5", "Nayru's Pearl*5",
                    "the first companion statue in the Tower of the Gods*5", "the second companion statue in the Tower of the Gods*5" }, true))); //event field 40
                memoryLocations.Add(new MemoryLocation(0x803B5244, 4, "", "event", 9, 0, uint.MaxValue, 0, 0,
                    new ComparisonData(new uint[] { 24, 0 }, new string[] { "slain all chu-chus on the Great Deku Tree*9", "the third companion statue in the Tower of the Gods*5" }, true)));
                memoryLocations.Add(new MemoryLocation(0x803B5248, 4, "", "event", 9, 0, uint.MaxValue, 0, 0,
                    new ComparisonData(new uint[] { 15 }, new string[] { "spoken to Quill on Greatfish Isle and set the time to 00:00*9" }, true)));
                memoryLocations.Add(new MemoryLocation(0x803B524C, 4, "", "event", 9, 0, uint.MaxValue, 0, 0, empty));
                memoryLocations.Add(new MemoryLocation(0x803B5250, 4, "", "event", 9, 0, uint.MaxValue, 0, 0,
                    new ComparisonData(new uint[] { 24 }, new string[] { "activated a spawn on the Pirate Ship (Sailing)*9" }, true))); //event field 50
                memoryLocations.Add(new MemoryLocation(0x803B5254, 4, "", "event", 9, 0, uint.MaxValue, 0, 0,
                    new ComparisonData(new uint[] { 15, 13 }, new string[] { "Hero's Clothes*0", "healed grandma with a fairy*9" }, true)));
                memoryLocations.Add(new MemoryLocation(0x803B5258, 4, "", "event", 9, 0, uint.MaxValue, 0, 0, empty));
                memoryLocations.Add(new MemoryLocation(0x803B525C, 4, "", "event", 9, 0, uint.MaxValue, 0, 0, empty));
                memoryLocations.Add(new MemoryLocation(0x803B5260, 4, "", "event", 9, 0, uint.MaxValue, 0, 0, empty)); //event field 60
                memoryLocations.Add(new MemoryLocation(0x803B5264, 4, "", "event", 9, 0, uint.MaxValue, 0, 0, empty));
                memoryLocations.Add(new MemoryLocation(0x803B5268, 4, "", "event", 9, 0, uint.MaxValue, 0, 0, empty));
                memoryLocations.Add(new MemoryLocation(0x803B526C, 4, "", "event", 9, 0, uint.MaxValue, 0, 0, empty));
                memoryLocations.Add(new MemoryLocation(0x803B5D60, 4, "", "event", 9, 0, uint.MaxValue, 0, 0, empty)); //event field super late
                memoryLocations.Add(new MemoryLocation(0x803B5D64, 4, "", "event", 9, 0, uint.MaxValue, 0, 0, empty));

                //event notifications to add - raise TOTG, talk to quill on DRI?, Pirate ship password, totg statues might be wrong, rang TOTG bell
            }

            //Bag items - id & number - might sync
        }
    }
}
