﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Windwaker_coop
{
    class Windwaker : IGame
    {
        public int gameId { get { return 0; } }
        public string gameName { get { return "Windwaker"; } }
        public string processName { get { return "dolphin"; } }
        public uint[] baseAddressOffsets { get { return new uint[] { 0x803B0000 }; } }
        public uint identityAddress { get { return 0x7FFF0000; } }
        public string identityText { get { return "GZLE01"; } }
        public bool bigEndian { get { return true; } }

        private byte[] lastCurrentStageData;
        private byte lastCurrentStageId;
        private TimeFlag[] timeFlags;

        public Windwaker()
        {
            timeFlags = new TimeFlag[]
            {
                new TimeFlag(0x5234, 27, 300),  //Open Spoils Bag chest
                new TimeFlag(0x5238, 7, 225),   //Wake up on Windfall
                new TimeFlag(0x5248, 15, 0)     //Talked to Quill on Greatfish
                //Arrive at Hyrule Field (180), FF2
            };
        }

        public void beginningFunctions(Client client)
        {
            updateStageInfo(client, true, "");
        }

        public void endingFunctions(Client client)
        {
            
        }

        public void onReceiveLocationFunctions(Client client, uint newValue, uint oldValue, MemoryLocation memLoc)
        {
            updateStageInfo(client, false, memLoc.type);
            checkForNewTime(client, newValue, oldValue, memLoc);
        }

        public void onReceiveListFunctions(Client client, byte[] memory)
        {
            updateStageInfo(client, false, "stage255"); //copy static to current regardless
        }

        //Takes the current stageInfo & copies it onto the corresponding unchanging stageInfo or vice versa
        private void updateStageInfo(Client client, bool currentToStatic, string type)
        {
            //Get current stage id & calculate addresses
            byte[] stageIdList = client.mr.readFromMemory(0x53A4, 1);
            if (stageIdList == null || stageIdList.Length < 1)
                return;
            byte stageId = stageIdList[0];
            uint currentAdr = 0x5380;
            uint staticAdr = 0x4F88 + (uint)stageId * 36;

            if (currentToStatic)
            {
                byte[] currentStageData = client.mr.readFromMemory(currentAdr, 35);
                if (currentStageData == null)
                    return;

                //Only update if you haven't changed stages and the currentStageData has changed (lastCurrentStageData should never be null after receiveMemoryList)
                if (lastCurrentStageId == stageId && !client.compareToPreviousMemory(currentStageData, lastCurrentStageData, 0, 35))
                {
                    Output.debug("Copying current stage data to static stage data " + stageId, 1);
                    client.mr.saveToMemory(currentStageData, staticAdr);
                }
                lastCurrentStageData = currentStageData;
                lastCurrentStageId = stageId;
            }
            else
            {
                //Only update the current stageInfo if data received was the stageinfo that the player is currently in
                if (type.IndexOf("stage") < 0 || (type.Substring(5) != stageId.ToString() && type.Substring(5) != "255"))
                    return;

                Output.debug("Copying static stage data " + stageId + " to current stage data", 1);
                byte[] staticStageData = client.mr.readFromMemory(staticAdr, 35);
                if (staticStageData != null)
                {
                    client.mr.saveToMemory(staticStageData, currentAdr);
                    lastCurrentStageData = staticStageData;
                    lastCurrentStageId = stageId;
                }
            }
        }

        //Checks if this memloc has an associated time flag, and sets a new time if the event was just triggered
        private void checkForNewTime(Client client, uint newValue, uint oldValue, MemoryLocation memLoc)
        {
            if (memLoc.type.IndexOf("time") < 0)
                return;

            foreach (TimeFlag t in timeFlags)
            {
                if (t.address == memLoc.startAddress) //This memory location has an event that sets the time
                {
                    if (!ReadWrite.bitSet(newValue, t.bit) || ReadWrite.bitSet(oldValue, t.bit))
                        return;

                    Output.debug("Setting new time to " + t.newTime, 1);
                    byte[] timeBytes = BitConverter.GetBytes(t.newTime);
                    Array.Reverse(timeBytes);
                    client.mr.saveToMemory(timeBytes, 0x4C2C);
                    return;
                }
            }
        }

        public List<MemoryLocation> createMemoryLocations()
        {
            //Min & Max values are in decimal format
            List<MemoryLocation> memoryLocations = new List<MemoryLocation>();
            SyncSettings syncSettings = Program.syncSettings;
            ComparisonData empty = new ComparisonData();

            //Stat upgrades
            if (syncSettings.getSetting("Player Stats"))
            {
                memoryLocations.Add(new MemoryLocation(0x4C08, 2, "more health*2", "stat", 0, 12, 80, 12, 0, empty));
                //memoryLocations.Add(new MemoryLocation(0x4DA8, 1, "began the Hero's Quest*9", "stat", 0, 0, 255, 0, 0, empty));
            }

            //Inventory items
            if (syncSettings.getSetting("Inventory Items"))
            {
                memoryLocations.Add(new MemoryLocation(0x4C44, 1, "Telescope*0", "item", 1, 32, 255, 255, 0, empty));
                memoryLocations.Add(new MemoryLocation(0x4C45, 1, "Sail*0", "item", 1, 120, 255, 255, 0, empty));
                memoryLocations.Add(new MemoryLocation(0x4C46, 1, "Wind Waker*0", "item", 1, 34, 255, 255, 0, empty));
                memoryLocations.Add(new MemoryLocation(0x4C47, 1, "Grappling Hook*0", "item", 1, 37, 255, 255, 0, empty));
                memoryLocations.Add(new MemoryLocation(0x4C48, 1, "Spoils Bag*0", "item", 1, 36, 255, 255, 0, empty));
                memoryLocations.Add(new MemoryLocation(0x4C49, 1, "Boomerang*0", "item", 1, 45, 255, 255, 0, empty));
                memoryLocations.Add(new MemoryLocation(0x4C4A, 1, "Deku Leaf*0", "item", 1, 52, 255, 255, 0, empty));

                memoryLocations.Add(new MemoryLocation(0x4C4B, 1, "Tingle Tuner*0", "item", 1, 33, 255, 255, 0, empty));
                memoryLocations.Add(new MemoryLocation(0x4C4C, 1, "Picto Box*0", "item", 2, 35, 255, 255, 0,
                    new ComparisonData(new uint[] { 35, 38 }, new string[] { "Picto Box*0", "Deluxe Picto Box*0" }, false)));
                memoryLocations.Add(new MemoryLocation(0x4C4D, 1, "Iron Boots*0", "item", 1, 41, 255, 255, 0, empty));
                memoryLocations.Add(new MemoryLocation(0x4C4E, 1, "Magic Armor*0", "item", 1, 42, 255, 255, 0, empty));
                memoryLocations.Add(new MemoryLocation(0x4C4F, 1, "Bait Bag*0", "item", 1, 44, 255, 255, 0, empty));
                memoryLocations.Add(new MemoryLocation(0x4C50, 1, "Bow*1", "item", 2, 39, 255, 255, 0,
                    new ComparisonData(new uint[] { 39, 53, 54 }, new string[] { "Hero's Bow*0", "Fire & Ice Arrows*0", "Light Arrows*0" }, false))); //Doesn't represent items accurately
                memoryLocations.Add(new MemoryLocation(0x4C51, 1, "Bombs*2", "item", 1, 49, 255, 255, 0, empty));

                memoryLocations.Add(new MemoryLocation(0x4C52, 1, "Bottle #1*2", "item", 3, 80, 255, 255, uint.MaxValue, empty));
                memoryLocations.Add(new MemoryLocation(0x4C53, 1, "Bottle #2*2", "item", 3, 80, 255, 255, uint.MaxValue, empty));
                memoryLocations.Add(new MemoryLocation(0x4C54, 1, "Bottle #3*2", "item", 3, 80, 255, 255, uint.MaxValue, empty));
                memoryLocations.Add(new MemoryLocation(0x4C55, 1, "Bottle #4*2", "item", 3, 80, 255, 255, uint.MaxValue, empty));
                memoryLocations.Add(new MemoryLocation(0x4C56, 1, "Delivery Bag*0", "item", 1, 48, 255, 255, 0, empty));
                memoryLocations.Add(new MemoryLocation(0x4C57, 1, "Hookshot*0", "item", 1, 47, 255, 255, 0, empty));
                memoryLocations.Add(new MemoryLocation(0x4C58, 1, "Skull Hammer*0", "item", 1, 51, 255, 255, 0, empty));

                uint baseItem = 0x4C59; //item ownership bitfields
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
            if (syncSettings.getSetting("Equipment Items"))
            {
                memoryLocations.Add(new MemoryLocation(0x4C16, 1, "sword*1", "item", 2, 56, 255, 255, 0,
                    new ComparisonData(new uint[] { 56, 57, 58, 62 }, new string[] { "Hero's Sword*0", "Master Sword (Unpowered)*0", "Master Sword (Half power)*0", "Master Sword (Fully powered)*0" }, false))); //sword level
                memoryLocations.Add(new MemoryLocation(0x4C17, 1, "shield*1", "item", 2, 59, 255, 255, 0,
                    new ComparisonData(new uint[] { 59, 60 }, new string[] { "Hero's Shield*0", "Mirror Shield*0" }, false))); //shield level
                memoryLocations.Add(new MemoryLocation(0x4C18, 1, "Power Bracelets*0", "item", 1, 40, 255, 255, 0, empty)); //power bracelets
                memoryLocations.Add(new MemoryLocation(0x4C19, 1, "Mystery item - I have no idea what this is for (4C19)*0", "item", 0, 0, 255, 0, 0, empty));
            }

            //Capacity upgrades
            if (syncSettings.getSetting("Capacity Upgrades"))
            {
                memoryLocations.Add(new MemoryLocation(0x4C1A, 1, "wallet*1", "capacity", 0, 0, 2, 0, 0,
                    new ComparisonData(new uint[] { 1, 2 }, new string[] { "1000 rupee wallet*0", "5000 rupee wallet*0" }, false)));
                memoryLocations.Add(new MemoryLocation(0x4C1B, 1, "magic meter*1", "stat", 0, 0, 32, 0, 0,
                    new ComparisonData(new uint[] { 16, 32 }, new string[] { "magic meter*1", "an enhanced magic meter*2" }, false)));
                memoryLocations.Add(new MemoryLocation(0x4C77, 1, "quiver*1", "capacity", 0, 0, 99, 0, 0,
                    new ComparisonData(new uint[] { 30, 60, 99 }, new string[] { "small quiver*1", "medium quiver*1", "large quiver*1" }, false)));
                memoryLocations.Add(new MemoryLocation(0x4C78, 1, "bomb bag*1", "capacity", 0, 0, 99, 0, 0,
                    new ComparisonData(new uint[] { 30, 60, 99 }, new string[] { "small bomb bag*1", "medium bomb bag*1", "large bomb bag*1" }, false)));
            }

            //Bag Contents
            if (syncSettings.getSetting("Bag Contents"))
            {
                uint spoilBase = 0x4C7E;  uint snumBase = 0x4CA4;
                uint baitBase = 0x4C86;  uint bnumBase = 0x4CAC;
                uint deliveryBase = 0x4C8E;
                string[] spoils = new string[] { "Skull Necklace", "Boko Baba Seed", "Golden Feather", "Knight's Crest", "Red Chu Jelly", "Green Chu Jelly", "Blue Chu Jelly", "Joy Pendant" };

                for (uint i = 0; i < 8; i++)
                {
                    //memoryLocations.Add(new MemoryLocation(spoilBase + i, 1, "", "bag", 8, 0, 255, 255, 0, empty)); //Spoils Bag items
                    //memoryLocations.Add(new MemoryLocation(baitBase + i, 1, "", "bag", 8, 0, 255, 255, 0, new ComparisonData(new uint[] { 0x82, 0x83 }, new string[] { "bought All-Purpose Bait*9", "bought a Hyoi Pear*9" }, true))); //Bait Bag Items
                    memoryLocations.Add(new MemoryLocation(deliveryBase + i, 1, "", "bag", 8, 0, 255, 255, 0, empty)); //Delivery Bag Items
                    //memoryLocations.Add(new MemoryLocation(snumBase + i, 1, "", "bag", 8, 0, 99, 0, 0, new ComparisonData(new uint[] { 0, 1 }, new string[] { $"found a {spoils[i]}*9", $"spent some {spoils[i]}s*9" }, false))); //Spoils Bag counts
                    //memoryLocations.Add(new MemoryLocation(bnumBase + i, 1, "", "bag", 8, 0, 3, 0, 0, empty)); //Bait Bag counts
                }
            }

            //More equipment items
            if (syncSettings.getSetting("Equipment Items"))
            {
                memoryLocations.Add(new MemoryLocation(0x4CBC, 1, "", "flag", 9, 0, 15, 0, 0, empty)); //sword bitfield
                memoryLocations.Add(new MemoryLocation(0x4CBD, 1, "", "flag", 9, 0, 3, 0, 0, empty)); //shield bitfield
                memoryLocations.Add(new MemoryLocation(0x4CBE, 1, "", "flag", 9, 0, 1, 0, 0, empty)); //bracelets bitfield
                memoryLocations.Add(new MemoryLocation(0x4CBF, 1, "pirate's charm*0", "item", 9, 0, 3, 0, 0,
                    new ComparisonData(new uint[] { 0 }, new string[] { "Pirate's Charm*0" }, true))); //pirate's charm bitfield
                memoryLocations.Add(new MemoryLocation(0x4CC0, 1, "hero's charm*0", "item", 9, 0, 3, 0, 2,
                    new ComparisonData(new uint[] { 0 }, new string[] { "Hero's Charm*0" }, true))); //hero's charm bitfield

                memoryLocations.Add(new MemoryLocation(0x5296, 1, "tingle-statue*0", "item", 9, 0, 255, 0, 0,
                    new ComparisonData(new uint[] { 2, 3, 4, 5, 6 }, new string[] { "Dragon Tingle Statue*0", "Forbidden Tingle Statue*0", "Goddess Tingle Statue*0", "Earth Tingle Statue*0", "Wind Tingle Statue*0" }, true)));
            }

            //Progression items
            if (syncSettings.getSetting("Story Items"))
            {
                memoryLocations.Add(new MemoryLocation(0x4CC5, 1, "song*1", "item", 9, 0, 63, 0, 0, new ComparisonData(new uint[] { 0, 1, 2, 3, 4, 5 },
                    new string[] { "the Wind's Requiem*3", "the Ballad of Gales*3", "the Command Melody*3", "the Earth God's Lyric*3", "the Wind God's Aria*3", "the Song of Passing*3" }, true))); //songs bitfield
                memoryLocations.Add(new MemoryLocation(0x4CC6, 1, "triforce shard*1", "story", 9, 0, 255, 0, 0, new ComparisonData(new uint[] { 0, 1, 2, 3, 4, 5, 6, 7 },
                    new string[] { "Triforce Shard #1*2", "Triforce Shard #2*2", "Triforce Shard #3*2", "Triforce Shard #4*2", "Triforce Shard #5*2", "Triforce Shard #6*2",
                    "Triforce Shard #7*2", "Triforce Shard #8*2", }, true))); //triforce shards bitfield
                memoryLocations.Add(new MemoryLocation(0x4CC7, 1, "new pearl*1", "story", 9, 0, 7, 0, 0,
                    new ComparisonData(new uint[] { 1, 2, 0 }, new string[] { "Din's Pearl*2", "Farore's Pearl*2", "Nayru's Pearl*2" }, true))); //pearls bitfield
            }

            //Charts
            if (syncSettings.getSetting("Charts"))
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

                memoryLocations.Add(new MemoryLocation(0x4CDC, 4, "treasure/triforce chart*1", "chart", 9, 0, uint.MaxValue, 0, 0,
                    new ComparisonData(fourByteValues, chartsOne, true))); //owned charts bitfield
                memoryLocations.Add(new MemoryLocation(0x4CE0, 4, "treasure/triforce chart*1", "chart", 9, 0, uint.MaxValue, 0, 0,
                    new ComparisonData(fourByteValues, chartsTwo, true)));
                memoryLocations.Add(new MemoryLocation(0x4CE4, 4, "extra chart bitfield*9", "chart", 9, 0, uint.MaxValue, 0, 0, empty));
                memoryLocations.Add(new MemoryLocation(0x4CE8, 4, "extra chart bitfield*9", "chart", 9, 0, uint.MaxValue, 0, 0, empty));

                for (int i = 0; i < 4; i++)
                    memoryLocations.Add(new MemoryLocation(0x4CEC + 4 * (uint)i, 4, "", "chart", 9, 0, uint.MaxValue, 0, 0, empty)); //opened charts bitfield
                for (int i = 0; i < 4; i++)
                    memoryLocations.Add(new MemoryLocation(0x4CFC + 4 * (uint)i, 4, "", "location", 9, 0, uint.MaxValue, 0, 0, empty)); //obtained sunked treasure bitfield
            }

            //Sectors
            if (syncSettings.getSetting("Sea Map"))
            {
                for (uint i = 0; i < 49; i++)
                {
                    byte def = 0;
                    if (i == 0 || i == 10 || i == 43) def = 3;
                    string sectorName = "ABCDEFG".Substring((int)i / 7, 1) + (i % 7 + 1).ToString();

                    memoryLocations.Add(new MemoryLocation(0x4D0C + i, 1, "new sector*9", "sector", 9, 0, 3, def, 0,
                        new ComparisonData(new uint[] { 1, 0 }, new string[] { $"visited sector {sectorName} for the first time*9", $"mapped out sector {sectorName}*9" }, true)));
                }
            }

            //Charts
            if (syncSettings.getSetting("Charts"))
                memoryLocations.Add(new MemoryLocation(0x4D4D, 1, "deciphered a new triforce chart*9", "chart", 9, 0, 255, 0, 0, new ComparisonData(new uint[] { 0, 1, 2, 3, 4, 5, 6, 7 },
                    new string[] { "Triforce Chart 1*4", "Triforce Chart 2*4", "Triforce Chart 3*4", "Triforce Chart 4*4", "Triforce Chart 5*4", "Triforce Chart 6*4", "Triforce Chart 7*4", "Triforce Chart 8*4" }, true))); //deciphered charts bitfield

            //Stage infos
            if (syncSettings.getSetting("Stage Infos"))
            {
                string[] stageNames = new string[] { "the Great Sea", "the Great Sea (alt)", "the Forsaken Fortress", "Dragon Roost Cavern", "the Forbidden Woods", "the Tower of the Gods", "the Earth Temple", "the Wind Temple",
                        "Ganon's Tower", "Flooded Hyrule", "a ship", "a house/misc", "a cave", "a cave/ship", "killing a blue chu chu", "a test map" };

                for (uint i = 0; i < 16; i++) //loops through each stage
                {
                    uint stageOffset = 0x4F88 + i * 36;
                    memoryLocations.Add(new MemoryLocation(stageOffset, 4, $"opened a chest in {stageNames[i]}*9", "stage" + i, 9, 0, uint.MaxValue, 0, 0, empty)); //chest open bitfield
                    for (uint j = 0; j < 4; j++)
                        memoryLocations.Add(new MemoryLocation(stageOffset + 4 + (4 * j), 4, $"triggered an event in {stageNames[i]}*9", "stage" + i, 9, 0, uint.MaxValue, 0, 0, empty)); //event flag bitfield

                    memoryLocations.Add(new MemoryLocation(stageOffset + 20, 4, $"picked up an item in {stageNames[i]}*9", "stage" + i, 9, 0, uint.MaxValue, 0, 0, empty)); //item pickup bitfield
                    memoryLocations.Add(new MemoryLocation(stageOffset + 24, 4, $"visited a new room in {stageNames[i]}*9", "stage" + i, 9, 0, uint.MaxValue, 0, 0, empty)); //visited room bitfields
                    memoryLocations.Add(new MemoryLocation(stageOffset + 28, 4, $"visited a new room in {stageNames[i]}*9", "stage" + i, 9, 0, uint.MaxValue, 0, 0, empty));

                    if (syncSettings.getSetting("Small Keys"))
                        memoryLocations.Add(new MemoryLocation(stageOffset + 32, 1, "", "stage" + i, 8, 0, 255, 0, 0, 
                            new ComparisonData(new uint[] { 0, 1 }, new string[] { $"found a small key to {stageNames[i]}*9", $"used a small key to {stageNames[i]}*9" }, false))); //number of small keys

                    memoryLocations.Add(new MemoryLocation(stageOffset + 33, 1, "important*9", "stage" + i, 9, 0, 255, 0, 0,
                        new ComparisonData(new uint[] { 0, 1, 2, 3, 4 }, new string[] { $"map to {stageNames[i]}*0", $"compass to {stageNames[i]}*0", $"big key to {stageNames[i]}*0",
                        $"defeated the boss of {stageNames[i]}*9", $"heart container in {stageNames[i]}*0" }, true))); //important dungeon flags bitfield
                    memoryLocations.Add(new MemoryLocation(stageOffset + 34, 2, "changed a stageInfoBuffer - you should not see this!*9", "buffer", 0, 0, ushort.MaxValue, 0, 0, empty));
                }

                //Warp pots
                memoryLocations.Add(new MemoryLocation(0x52CB, 1, "opened a WT warp pot*9", "dungeon", 9, 0, 7, 0, 0, empty));
                memoryLocations.Add(new MemoryLocation(0x52CC, 1, "opened a DRC-FW-TotG inter-dungeon warp pot*9", "dungeon", 9, 0, 7, 0, 0, empty));
                memoryLocations.Add(new MemoryLocation(0x52CD, 1, "opened a FW warp pot*9", "dungeon", 9, 0, 7, 0, 0, empty));
                memoryLocations.Add(new MemoryLocation(0x52CE, 1, "opened a DRC warp pot*9", "dungeon", 9, 0, 7, 0, 0, empty));
                memoryLocations.Add(new MemoryLocation(0x52CF, 1, "opened a ET warp pot*9", "dungeon", 9, 0, 7, 0, 0, empty));
                memoryLocations.Add(new MemoryLocation(0x52D0, 1, "opened a FF-ET-WT inter-dungeon warp pot*9", "dungeon", 9, 0, 7, 0, 0, empty));
            }

            //Event bitfields
            if (syncSettings.getSetting("Events"))
            {
                memoryLocations.Add(new MemoryLocation(0x522C, 4, "", "event", 9, 0, uint.MaxValue, 0, 0,
                    new ComparisonData(new uint[] { 24, 16 }, new string[] { "witnessed Tetra fall into the Forest of Fairies*9", "rescued Tetra from the Forest of Fairies*9" }, true))); //event field 20
                memoryLocations.Add(new MemoryLocation(0x5230, 4, "", "event", 9, 0, uint.MaxValue, 0, 0, empty)); //event field 30
                memoryLocations.Add(new MemoryLocation(0x5234, 4, "", "eventtime", 9, 0, uint.MaxValue, 0, 0, new ComparisonData(new uint[] { 24, 25, 27, 7 },
                    new string[] { "activated a spawn in the Forsaken Fortress (Do not leave through the door on the Pirate Ship)*9", "finished speaking to Tetra after rescuing her on Ouset*9",
                    "activated a spawn on the Pirate Ship (Docked) and set the time to 20:00*9", "freed Tingle from his cell*9" }, true)));
                memoryLocations.Add(new MemoryLocation(0x5238, 4, "", "eventtime", 9, 0, uint.MaxValue, 0, 0,
                    new ComparisonData(new uint[] { 9, 7, 2 }, new string[] { "retreived the Father's Letter from Medli*9", "activated a new spawn on Windfall Island and set the time to 15:00*9", "calmed down Prince Komali*9" }, true)));
                memoryLocations.Add(new MemoryLocation(0x523C, 4, "", "event", 9, 0, uint.MaxValue, 0, 0,
                    new ComparisonData(new uint[] { 17, 6 }, new string[] { "thrown Medli onto the Dragon Roost Cavern ledge*9", "caught all of the Killer Bees*9" }, true)));
                memoryLocations.Add(new MemoryLocation(0x5240, 4, "", "event", 9, 0, uint.MaxValue, 0, 0,
                    new ComparisonData(new uint[] { 31, 30, 28, 4, 2 }, new string[] { "Din's Pearl*5", "Farore's Pearl*5", "Nayru's Pearl*5",
                    "the first companion statue in the Tower of the Gods*5", "the second companion statue in the Tower of the Gods*5" }, true))); //event field 40
                memoryLocations.Add(new MemoryLocation(0x5244, 4, "", "event", 9, 0, uint.MaxValue, 0, 0,
                    new ComparisonData(new uint[] { 24, 0 }, new string[] { "slain all chu-chus on the Great Deku Tree*9", "the third companion statue in the Tower of the Gods*5" }, true)));
                memoryLocations.Add(new MemoryLocation(0x5248, 4, "", "event", 9, 0, uint.MaxValue, 0, 0,
                    new ComparisonData(new uint[] { 15 }, new string[] { "spoken to Quill on Greatfish Isle and set the time to 00:00*9" }, true)));
                memoryLocations.Add(new MemoryLocation(0x524C, 4, "", "event", 9, 0, uint.MaxValue, 0, 0, empty));
                memoryLocations.Add(new MemoryLocation(0x5250, 4, "", "event", 9, 0, uint.MaxValue, 0, 0,
                    new ComparisonData(new uint[] { 24 }, new string[] { "activated a spawn on the Pirate Ship (Sailing)*9" }, true))); //event field 50
                memoryLocations.Add(new MemoryLocation(0x5254, 4, "", "event", 9, 0, uint.MaxValue, 0, 0,
                    new ComparisonData(new uint[] { 15, 13 }, new string[] { "Hero's Clothes*0", "healed grandma with a fairy*9" }, true)));
                memoryLocations.Add(new MemoryLocation(0x5258, 4, "", "event", 9, 0, uint.MaxValue, 0, 0, empty));
                memoryLocations.Add(new MemoryLocation(0x525C, 4, "", "event", 9, 0, uint.MaxValue, 0, 0, empty));
                memoryLocations.Add(new MemoryLocation(0x5260, 4, "", "event", 9, 0, uint.MaxValue, 0, 0, empty)); //event field 60
                memoryLocations.Add(new MemoryLocation(0x5264, 4, "", "event", 9, 0, uint.MaxValue, 0, 0, empty));
                memoryLocations.Add(new MemoryLocation(0x5268, 4, "", "event", 9, 0, uint.MaxValue, 0, 0, empty));
                memoryLocations.Add(new MemoryLocation(0x526C, 4, "", "event", 9, 0, uint.MaxValue, 0, 0, empty));
                memoryLocations.Add(new MemoryLocation(0x5D60, 4, "", "event", 9, 0, uint.MaxValue, 0, 0, empty)); //event field super late
                memoryLocations.Add(new MemoryLocation(0x5D64, 4, "", "event", 9, 0, uint.MaxValue, 0, 0, empty));

                //event notifications to add - raise TOTG, talk to quill on DRI?, Pirate ship password, totg statues might be wrong, rang TOTG bell
            }

            return memoryLocations;
            //Bag items - id & number - might sync
        }

        public Cheat[] getCheats()
        {
            uint baseItem = 0x4C44, baseOwner = 0x4C59;
            return new Cheat[]
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

                new Cheat("sword", 0x4C16, 0, true, 4, new byte[] { 56, 57, 58, 62 }),
                new Cheat("sword", 0x4CBC, 0, true, 4, new byte[] { 1, 3, 7, 15 }),
                new Cheat("shield", 0x4C17, 0, true, 2, new byte[] { 59, 60 }),
                new Cheat("shield", 0x4CBD, 0, true, 2, new byte[] { 1, 3 }),
                new Cheat("power-bracelets", 0x4C18, 40, false),
                new Cheat("power-bracelets", 0x4CBE, 1, false),
                new Cheat("pirates-charm", 0x4CBF, 1, false),
                new Cheat("heros-charm", 0x4CC0, 1, false),

                new Cheat("song", 0x4CC5, 0, true, 6, new byte[] { 1, 3, 7, 15, 31, 63 }),
                new Cheat("pearl", 0x4CC7, 0, true, 3, new byte[] { 2, 6, 7 }),
                new Cheat("triforce-shard", 0x4CC6, 0, true, 8, new byte[] { 1, 3, 7, 15, 31, 63, 127, 255 }),

                new Cheat("wallet", 0x4C1A, 0, true, 3, new byte[] { 0, 1, 2 }),
                new Cheat("magic", 0x4C1B, 0, true, 2, new byte[] { 16, 32 }),
                new Cheat("quiver", 0x4C77, 0, true, 3, new byte[] { 30, 60, 99 }),
                new Cheat("bomb-bag", 0x4C78, 0, true, 3, new byte[] { 30, 60, 99 })
            };
        }

        public SyncSettings getDefaultSyncSettings()
        {
            return new SyncSettings(new string[] { "Inventory Items", "Equipment Items", "Story Items", "Player Stats", "Capacity Upgrades", "Bag Contents",
                "Charts", "Sea Map", "Stage Infos", "Small Keys", "Events" }, new bool[] { true, true, true, true, true, true, true, true, true, true, true });
        }
    }
}