using System;
using System.Collections.Generic;
using System.Text;

namespace Windwaker_coop
{
    class Zelda1 : IGame
    {
        public int gameId { get { return 2; } }
        public string gameName { get { return "Zelda 1"; } }
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

        }

        public List<MemoryLocation> createMemoryLocations()
        {
            List<MemoryLocation> memoryLocations = new List<MemoryLocation>();
            ComparisonData empty = new ComparisonData();

            memoryLocations.Add(new MemoryLocation(0x67, 1, "sword*0", "item", 0, 0, 3, 0, 0, new ComparisonData(new uint[] { 1, 2, 3 }, new string[] { "Wooden Sword*0", "White Sword*0", "Magical Sword*0" }, false)));

            memoryLocations.Add(new MemoryLocation(0x69, 1, "arrows*0", "item", 0, 0, 2, 0, 0, new ComparisonData(new uint[] { 1, 2 }, new string[] { "Arrows*2", "Silver Arrows*2" }, false)));
            memoryLocations.Add(new MemoryLocation(0x6A, 1, "Bow*0", "item", 0, 0, 1, 0, 0, empty));
            memoryLocations.Add(new MemoryLocation(0x6B, 1, "candle*0", "item", 0, 0, 2, 0, 0, new ComparisonData(new uint[] { 1, 2 }, new string[] { "Blue Candle*0", "Red Candle*0" }, false)));
            memoryLocations.Add(new MemoryLocation(0x6C, 1, "Whistle*0", "item", 0, 0, 1, 0, 0, empty));

            memoryLocations.Add(new MemoryLocation(0x6F, 1, "Magical Rod*0", "item", 0, 0, 1, 0, 0, empty));
            memoryLocations.Add(new MemoryLocation(0x70, 1, "Raft*0", "item", 0, 0, 1, 0, 0, empty));
            memoryLocations.Add(new MemoryLocation(0x71, 1, "Magic Book*0", "item", 0, 0, 1, 0, 0, empty));
            memoryLocations.Add(new MemoryLocation(0x72, 1, "ring*0", "item", 0, 0, 2, 0, 0, new ComparisonData(new uint[] { 1, 2 }, new string[] { "Blue Ring*0", "Red Ring*0" }, false)));
            memoryLocations.Add(new MemoryLocation(0x73, 1, "Stepladder*0", "item", 0, 0, 1, 0, 0, empty));
            memoryLocations.Add(new MemoryLocation(0x74, 1, "Magical Key*0", "item", 0, 0, 1, 0, 0, empty));
            memoryLocations.Add(new MemoryLocation(0x75, 1, "Power Bracelet*0", "item", 0, 0, 1, 0, 0, empty));
            memoryLocations.Add(new MemoryLocation(0x76, 1, "Letter*0", "item", 0, 0, 1, 0, 0, empty));

            memoryLocations.Add(new MemoryLocation(0x77, 1, "compass*0", "item", 9, 0, 255, 0, 0, 
                new ComparisonData(new uint[] { 0, 1, 2, 3, 4, 5, 6, 7 }, new string[] { "compass to Dungeon 1*0", "compass to Dungeon 2*0", "compass to Dungeon 3*0", "compass to Dungeon 4*0",
                "compass to Dungeon 5*0", "compass to Dungeon 6*0", "compass to Dungeon 7*0", "compass to Dungeon 8*0" }, true)));
            memoryLocations.Add(new MemoryLocation(0x78, 1, "map*0", "item", 9, 0, 255, 0, 0,
                new ComparisonData(new uint[] { 0, 1, 2, 3, 4, 5, 6, 7 }, new string[] { "map to Dungeon 1*0", "map to Dungeon 2*0", "map to Dungeon 3*0", "map to Dungeon 4*0",
                "map to Dungeon 5*0", "map to Dungeon 6*0", "map to Dungeon 7*0", "map to Dungeon 8*0" }, true)));
            memoryLocations.Add(new MemoryLocation(0x79, 1, "compass to Dungeon 9*0", "item", 0, 0, 1, 0, 0, empty));
            memoryLocations.Add(new MemoryLocation(0x7A, 1, "map to Dungeon 9*0", "item", 0, 0, 1, 0, 0, empty));

            //hearts
            memoryLocations.Add(new MemoryLocation(0x7F, 1, "more health*2", "health", 0, 0, 255, 0, 0x0F, empty));
            //keys

            memoryLocations.Add(new MemoryLocation(0x81, 1, "triforce*0", "item", 9, 0, 255, 0, 0,
                new ComparisonData(new uint[] { 0, 1, 2, 3, 4, 5, 6, 7 }, new string[] { "Triforce piece from Dungeon 1*0", "Triforce piece from Dungeon 2*0", "Triforce piece from Dungeon 3*0", "Triforce piece from Dungeon 4*0",
                "Triforce piece from Dungeon 5*0", "Triforce piece from Dungeon 6*0", "Triforce piece from Dungeon 7*0", "Triforce piece from Dungeon 8*0" }, true)));

            memoryLocations.Add(new MemoryLocation(0x84, 1, "Boomerang*0", "item", 0, 0, 1, 0, 0, empty));
            memoryLocations.Add(new MemoryLocation(0x85, 1, "Magical Boomerang*0", "item", 0, 0, 1, 0, 0, empty));
            memoryLocations.Add(new MemoryLocation(0x86, 1, "Magical Shield*0", "item", 0, 0, 1, 0, 0, empty));

            memoryLocations.Add(new MemoryLocation(0x8C, 1, "bomb bag*0", "item", 0, 8, 16, 8, 0, new ComparisonData(new uint[] { 12, 16 }, new string[] { "12 bomb maximum*1", "16 bomb maximum*1" }, false)));

            return memoryLocations;
        }

        public Cheat[] getCheats()
        {
            return new Cheat[] 
            { 
                new Cheat("health", 0x7F, 0, true, 16, new byte[] { 0, 17, 34, 51, 68, 85, 102, 119, 136, 153, 170, 187, 204, 221, 238, 255 }),
                new Cheat("sword", 0x67, 0, true, 3, new byte[] { 1, 2, 3 })
            };
        }

        public SyncSettings getDefaultSyncSettings()
        {
            return new SyncSettings(new string[] { "Inventory Items", "Equipment Items", "Dungeon Data" }, new bool[] { true, true, true });
        }
    }
}
