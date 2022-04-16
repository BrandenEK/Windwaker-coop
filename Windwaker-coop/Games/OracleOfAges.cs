using System;
using System.Collections.Generic;
using System.Text;

namespace Windwaker_coop
{
    class OracleOfAges : Game
    {
        public OracleOfAges() : base(4, "Oracle of Ages", "mGBA", new uint[] { 0x0217E208, 0x38, 0x10, 0x08, 0x90 }, 0, "idk", false) { }

        public override List<MemoryLocation> createMemoryLocations()
        {
            List<MemoryLocation> memoryLocations = new List<MemoryLocation>();
            ComparisonData empty = new ComparisonData();


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
