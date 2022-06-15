using System;
using System.Collections.Generic;
using System.Text;

namespace Windwaker_coop
{
    class Zelda2 : IGame
    {
        public int gameId { get { return 5; } }
        public string gameName { get { return "Zelda II"; } }
        public string processName { get { return "fceu"; } }
        public uint[] baseAddressOffsets { get { return new uint[] { 0x59FE00 }; } }
        public uint identityAddress { get { return 0; } }
        public string identityText { get { return "idk"; } }
        public bool bigEndian { get { return true; } }

        private ushort[][] expTable = new ushort[][]
        {
            new ushort[] { 200, 500, 1000, 2000, 3000, 5000, 8000, 9000 },
            new ushort[] { 100, 300, 700, 1200, 2200, 3500, 6000, 9000 },
            new ushort[] { 50, 150, 400, 800, 1500, 2500, 4000, 9000 }
        };

        public void beginningFunctions(Client client)
        {
        }

        public void endingFunctions(Client client)
        {
        }        

        public void onReceiveListFunctions(Client client, byte[] memory)
        {
            //Update exp needed when joining initially
        }

        public void onReceiveLocationFunctions(Client client, uint newValue, uint oldValue, MemoryLocation memLoc)
        {
            //If they received an increased skill, then maybe increase the exp needed
            if (memLoc.type == "skill")
            {
                //Get necessary values
                ushort expNeeded = (ushort)ReadWrite.bigToLittleEndian(client.mr.readFromMemory(0x180, 2), 0, 2);
                byte[] currSkills = client.mr.readFromMemory(0x187, 3);
                uint skillReceived = memLoc.startAddress - 0x187;

                //If the skill recieved was of the lowest cost, then increase the exp needed
                if (expNeeded == expTable[skillReceived][currSkills[skillReceived] - 2])
                {
                    ushort minExp = Math.Min(expTable[0][currSkills[0] - 1], Math.Min(expTable[1][currSkills[1] - 1], expTable[2][currSkills[2] - 1]));
                    client.mr.saveToMemory(ReadWrite.littleToBigEndian(minExp, 2), 0x180);
                    Output.debug("Updated EXP needed for level up to " + minExp, 1);
                }
            }
        }

        public List<MemoryLocation> createMemoryLocations()
        {
            List<MemoryLocation> memoryLocations = new List<MemoryLocation>();
            ComparisonData empty = new ComparisonData();
            SyncSettings s = Program.syncSettings;

            memoryLocations.Add(new MemoryLocation(0x110, 1, "more lives*2", "life", 0, 0, 255, 0, 0, empty));

            if (s.getSetting("Tilemap"))
            {
                for (uint i = 0x10; i < 0xF0; i += 4)
                    memoryLocations.Add(new MemoryLocation(i, 4, "", "tilemap", 1, 1, uint.MaxValue, 0, 0, empty));
            }
            
            if (s.getSetting("Stats"))
            {
                memoryLocations.Add(new MemoryLocation(0x187, 1, "increased the attack skill*9", "skill", 0, 0, 8, 0, 0, empty));
                memoryLocations.Add(new MemoryLocation(0x188, 1, "increased the magic skill*9", "skill", 0, 0, 8, 0, 0, empty));
                memoryLocations.Add(new MemoryLocation(0x189, 1, "increased the life skill*9", "skill", 0, 0, 8, 0, 0, empty));
            }

            if (s.getSetting("Spells"))
            {
                memoryLocations.Add(new MemoryLocation(0x18B, 1, "Shield Magic*3", "spell", 0, 0, 1, 0, 0, empty));
                memoryLocations.Add(new MemoryLocation(0x18C, 1, "Jump Magic*3", "spell", 0, 0, 1, 0, 0, empty));
                memoryLocations.Add(new MemoryLocation(0x18D, 1, "Life Magic*3", "spell", 0, 0, 1, 0, 0, empty));
                memoryLocations.Add(new MemoryLocation(0x18E, 1, "Fairy Magic*3", "spell", 0, 0, 1, 0, 0, empty));
                memoryLocations.Add(new MemoryLocation(0x18F, 1, "Fire Magic*3", "spell", 0, 0, 1, 0, 0, empty));
                memoryLocations.Add(new MemoryLocation(0x190, 1, "Reflect Magic*3", "spell", 0, 0, 1, 0, 0, empty));
                memoryLocations.Add(new MemoryLocation(0x191, 1, "Spell Magic*3", "spell", 0, 0, 1, 0, 0, empty));
                memoryLocations.Add(new MemoryLocation(0x192, 1, "Thunder Magic*3", "spell", 0, 0, 1, 0, 0, empty));
            }

            if (s.getSetting("Stats"))
            {
                memoryLocations.Add(new MemoryLocation(0x193, 1, "magic container*1", "stat", 0, 0, 8, 0, 0, empty));
                memoryLocations.Add(new MemoryLocation(0x194, 1, "heart container*1", "stat", 0, 0, 8, 0, 0, empty));
            }

            if (s.getSetting("Equipment"))
            {
                memoryLocations.Add(new MemoryLocation(0x195, 1, "Candle*0", "equipment", 0, 0, 1, 0, 0, empty));
                memoryLocations.Add(new MemoryLocation(0x196, 1, "Glove*0", "equipment", 0, 0, 1, 0, 0, empty));
                memoryLocations.Add(new MemoryLocation(0x197, 1, "Raft*0", "equipment", 0, 0, 1, 0, 0, empty));
                memoryLocations.Add(new MemoryLocation(0x198, 1, "Boots*0", "equipment", 0, 0, 1, 0, 0, empty));
                memoryLocations.Add(new MemoryLocation(0x199, 1, "Flute*0", "equipment", 0, 0, 1, 0, 0, empty));
                memoryLocations.Add(new MemoryLocation(0x19A, 1, "Cross*0", "equipment", 0, 0, 1, 0, 0, empty));
                memoryLocations.Add(new MemoryLocation(0x19B, 1, "Hammer*0", "equipment", 0, 0, 1, 0, 0, empty));
                memoryLocations.Add(new MemoryLocation(0x19C, 1, "Magic Key*0", "equipment", 0, 0, 1, 0, 0, empty));

                memoryLocations.Add(new MemoryLocation(0x1A4, 1, "a crystal*5", "story", 1, 0, 6, 6, 0, empty));

                memoryLocations.Add(new MemoryLocation(0x1A6, 1, "", "skills", 9, 0, 255, 0, 3, new ComparisonData(new uint[] { 2, 4 }, new string[] { "the Upwards Stab*3", "the Downwards Stab*3" }, true)));
            }

            if (s.getSetting("Quests"))
            {
                memoryLocations.Add(new MemoryLocation(0x1A7, 1, "", "quest", 9, 0, 255, 0, 0, empty));
                memoryLocations.Add(new MemoryLocation(0x1A8, 1, "", "quest", 9, 0, 255, 0, 0, new ComparisonData(new uint[] { 4 }, new string[] { "trophy (quest)*0" }, true)));
                memoryLocations.Add(new MemoryLocation(0x1A9, 1, "", "quest", 9, 0, 255, 0, 0, new ComparisonData(new uint[] { 0 }, new string[] { "mirror (quest)*0" }, true)));
                memoryLocations.Add(new MemoryLocation(0x1AA, 1, "", "quest", 9, 0, 255, 0, 0, new ComparisonData(new uint[] { 3, 6 }, new string[] { "river note*0", "medicine (quest)*0" }, true)));
                memoryLocations.Add(new MemoryLocation(0x1AB, 1, "", "quest", 9, 0, 255, 0, 0, new ComparisonData(new uint[] { }, new string[] { }, true)));
                memoryLocations.Add(new MemoryLocation(0x1AC, 1, "", "quest", 9, 0, 255, 0, 0, new ComparisonData(new uint[] { }, new string[] { }, true)));
                memoryLocations.Add(new MemoryLocation(0x1AD, 1, "", "quest", 9, 0, 255, 0, 0, new ComparisonData(new uint[] { }, new string[] { }, true)));
                memoryLocations.Add(new MemoryLocation(0x1AE, 1, "", "quest", 9, 0, 255, 0, 0, new ComparisonData(new uint[] { }, new string[] { }, true)));
            }

            return memoryLocations;
        }

        public Cheat[] getCheats()
        {
            return null;
        }

        public SyncSettings getDefaultSyncSettings()
        {
            return new SyncSettings(new string[] { "Stats", "Equipment", "Spells", "Quests", "Tilemap" }, new bool[] { true, true, true, true, true });
        }
    }
}
