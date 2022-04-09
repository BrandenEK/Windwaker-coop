using System;
using System.Collections.Generic;
using System.Text;

namespace Windwaker_coop
{
    class OracleOfSeasons : Game
    {
        public OracleOfSeasons() : base(3, "Oracle of Seasons", "mGBA", new uint[] { 0x0217E208, 0x38, 0x10, 0x08, 0x90 }, 0, "idk", false) { }

        public override List<MemoryLocation> createMemoryLocations()
        {
            List<MemoryLocation> memoryLocations = new List<MemoryLocation>();
            ComparisonData empty = new ComparisonData();

            //Before stuff

            memoryLocations.Add(new MemoryLocation(0x0616, 4, "new ring*1", "ring", 9, 0, uint.MaxValue, 0, 0, empty)); //rings bitfield
            memoryLocations.Add(new MemoryLocation(0x061A, 4, "new ring*1", "ring", 9, 0, uint.MaxValue, 0, 0, empty));

            // Dungeon stuff

            memoryLocations.Add(new MemoryLocation(0x0662, 4, "", "dungeon", 9, 0, uint.MaxValue, 0, 0, empty)); //Explored floors bitfield
            memoryLocations.Add(new MemoryLocation(0x0666, 4, "", "dungeon", 9, 0, uint.MaxValue, 0, 0, empty));
            memoryLocations.Add(new MemoryLocation(0x066A, 4, "", "dungeon", 9, 0, uint.MaxValue, 0, 0, empty));
            //keys
            memoryLocations.Add(new MemoryLocation(0x066A, 2, "big key to a dungeon*1", "dungeon", 9, 0, ushort.MaxValue, 0, 0, empty));
            memoryLocations.Add(new MemoryLocation(0x066C, 2, "compass to a dungeon*1", "dungeon", 9, 0, ushort.MaxValue, 0, 0, empty));
            memoryLocations.Add(new MemoryLocation(0x066E, 2, "map to a dungeon*1", "dungeon", 9, 0, ushort.MaxValue, 0, 0, empty));
            //inventory

            // Item & Equipment Bitfields

            memoryLocations.Add(new MemoryLocation(0x0692, 4, "items", "item", 9, 0, uint.MaxValue, 0, 0, 
                new ComparisonData(new uint[] { 1, 2, 3, 5, 6, 7, 8, 15, 16, 19, 21, 22, 25, 30 }, new string[] { "something on start*2", "shield*1", "bombs*2", "sword*1", "Boomerang*0", "Rod of Seasons*0",
                "Magnetic Gloves*0", "Animal Flute*0", "Roc's Feather*0", "Slingshot*0", "Shovel*0", "Power Bracelet*0", "Seed Satchel*0", "Fool's Ore*0" }, true))); //items
            memoryLocations.Add(new MemoryLocation(0x0696, 1, "seeds", "item", 9, 0, 0x1F, 0, 0, new ComparisonData(
                new uint[] { 0, 1, 2, 3, 4 }, new string[] { "Ember seeds*2", "Scent seeds*2", "Pegasus seeds*2", "Gale seeds*2", "Mystery seeds*2" }, true)));  //seeds
            memoryLocations.Add(new MemoryLocation(0x0697, 2, "first", "initials", 9, 0, ushort.MaxValue, 0, 128, new ComparisonData(
                new uint[] { 6, 14 }, new string[] { "Zora's Flippers*0", "Huge Maku Seed*0" }, true)));

            memoryLocations.Add(new MemoryLocation(0x0699, 1, "done i have no idea what*9", "unknown", 0, 0, 255, 0, 0, empty)); //???

            memoryLocations.Add(new MemoryLocation(0x069A, 4, "equipment", "equipment", 9, 0, uint.MaxValue, 0, 0, new ComparisonData(
                new uint[] { 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20 }, new string[] { "Gnarled Key*0", "Floodgate Key*0", "Dragon Key*0", "Star-shaped Ore*0", "Ribbon*0", "Spring Banana*0", 
                "Ricky's Gloves*2", "Bomb Flower*1", "Rusty Bell*0", "Treasure Map*1", "Round Jewel*0", "Pyramid Jewel*0", "Square Jewel*0", "X-shaped Jewel*0", "Red Ore*0", "Blue Ore*0", "Hard Ore*0", 
                "Member's Card*0", "Master's Plaque*0" }, true)));  //equipment

            //Player Stats

            memoryLocations.Add(new MemoryLocation(0x06A9, 1, "shield", "level", 0, 1, 3, 1, 0, 
                new ComparisonData(new uint[] { 2, 3 }, new string[] { "Iron Shield*0", "Mirror Shield*0" }, false)));
            memoryLocations.Add(new MemoryLocation(0x06AB, 1, "bigger bomb bag*1", "capacity", 0, 0x10, 0x30, 0x10, 0, empty)); //maybe more ?
            memoryLocations.Add(new MemoryLocation(0x06AC, 1, "sword", "level", 0, 0, 3, 0, 0, 
                new ComparisonData(new uint[] { 1, 2, 3 }, new string[] { "Wooden Sword*0", "Noble Sword*0", "Master Sword*0" }, false)));
            memoryLocations.Add(new MemoryLocation(0x06AE, 1, "bigger seed satchel*1", "capacity", 0, 0, 3, 0, 0, empty));
            memoryLocations.Add(new MemoryLocation(0x06B0, 1, "seasons", "level", 9, 0, 0x1F, 0, 0, 
                new ComparisonData(new uint[] { 0, 1, 2, 3 }, new string[] { "power of Spring*0", "power of Summer*0", "power of Autumn*0", "power of Winter*0" }, true)));
            memoryLocations.Add(new MemoryLocation(0x06B1, 1, "boomerang", "level", 0, 0, 2, 0, 0, 
                new ComparisonData(new uint[] { 2 }, new string[] { "Magic Boomerang*0" }, false)));
            memoryLocations.Add(new MemoryLocation(0x06B3, 1, "slingshot", "level", 0, 0, 2, 0, 0,
                new ComparisonData(new uint[] { 2 }, new string[] { "Hyper Slingshot*0" }, false)));
            memoryLocations.Add(new MemoryLocation(0x06B4, 1, "feather", "level", 0, 0, 2, 0, 0,
                new ComparisonData(new uint[] { 2 }, new string[] { "Roc's Cape*0" }, false)));

            //Next Stuff

            memoryLocations.Add(new MemoryLocation(0x06BB, 1, "essences", "story", 9, 0, 0xFF, 0, 0,
                new ComparisonData(new uint[] { 0, 1, 2, 3, 4, 5, 6, 7 }, new string[] { "Fertile Soil*0", "Gift of Time*0", "Bright Sun*0", "Soothing Rain*0",
                "Nurturing Warmth*0", "Blowing Wind*0", "Seed of Life*0", "Changing Seasons*0" }, true))); //essences

            memoryLocations.Add(new MemoryLocation(0x06C6, 1, "ring box", "level", 0, 0, 3, 0, 0, new ComparisonData(new uint[] { 1, 2, 3 }, new string[] { "Ring Box L-1*1", "Ring Box L-2*1", "Ring Box L-3*1" }, false))); //ring box
            memoryLocations.Add(new MemoryLocation(0x06C9, 1, "golden enemies", "flag", 9, 0, 15, 0, 0, new ComparisonData(new uint[] { 0, 1, 2, 3 }, 
                new string[] { "slain the Golden Octorok*9", "slain the Golden Moblin*9", "slain the Golden Darknut*9", "slain the Golden Lynel*9" }, true))); //golden enemies

            // Stage Data

            return memoryLocations;
        }

        public override Cheat[] getCheats()
        {
            return null;
        }

        public override SyncSettings getDefaultSyncSettings()
        {
            return new SyncSettings();
        }
    }
}
