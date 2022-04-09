using System;
using System.Collections.Generic;
using System.Text;

namespace Windwaker_coop
{
    class OracleOfSeasons : Game
    {
        public OracleOfSeasons() : base(3, "Oracle of Seasons", "mGBA", new uint[] { 0x01D7E208, 0x38, 0x10, 0x08, 0x90 }, 0, "idk", false) { }

        public override List<MemoryLocation> createMemoryLocations()
        {
            return null;
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
