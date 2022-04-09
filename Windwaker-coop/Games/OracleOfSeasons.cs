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

            //Player Stats

            memoryLocations.Add(new MemoryLocation(0x06A9, 1, "shield", "level", 0, 1, 3, 1, 0, 
                new ComparisonData(new uint[] { 2, 3 }, new string[] { "Iron Shield*0", "Mirror Shield*0" }, false)));
            memoryLocations.Add(new MemoryLocation(0x06AB, 1, "bigger bomb bag*1", "capacity", 0, 10, 30, 10, 0, empty)); //maybe more ?
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
                "Nurturing Warmth*0", "Blowing Wind*0", "Seed of Life*0", "Changing Seasons*0" }, true)));

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
