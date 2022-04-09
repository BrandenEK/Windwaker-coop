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

            memoryLocations.Add(new MemoryLocation(0x06A9, 1, "better shield*1", "level", 0, 1, 3, 1, 0, empty));




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
