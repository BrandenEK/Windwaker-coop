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

        public void beginningFunctions(Client client)
        {
        }

        public void endingFunctions(Client client)
        {
        }        

        public void onReceiveListFunctions(Client client, byte[] memory)
        {
        }

        public void onReceiveLocationFunctions(Client client, uint newValue, uint oldValue, MemoryLocation memLoc)
        {
            //If they received an increased skill, then maybe increase the exp needed
            if (memLoc.type == "skill")
            {
                Output.text("Recieved new skill point");
            }
        }

        public List<MemoryLocation> createMemoryLocations()
        {
            List<MemoryLocation> memoryLocations = new List<MemoryLocation>();
            ComparisonData empty = new ComparisonData();
            SyncSettings s = Program.syncSettings;

            //tilemap
            
            if (s.getSetting("Stats"))
            {
                memoryLocations.Add(new MemoryLocation(0x187, 1, "increased the attack power*9", "skill", 0, 0, 8, 0, 0, empty));
                memoryLocations.Add(new MemoryLocation(0x188, 1, "increased the magic power*9", "skill", 0, 0, 8, 0, 0, empty));
                memoryLocations.Add(new MemoryLocation(0x189, 1, "increased the life power*9", "skill", 0, 0, 8, 0, 0, empty));
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
            }

            //quests

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
