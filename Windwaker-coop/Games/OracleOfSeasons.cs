﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Windwaker_coop
{
    class OracleOfSeasons : Game
    {
        public OracleOfSeasons() : base(3, "Oracle of Seasons", "mGBA", 0, "idk", false) { }

        public override List<MemoryLocation> createMemoryLocations()
        {
            throw new NotImplementedException();
        }

        public override Cheat[] getCheats()
        {
            throw new NotImplementedException();
        }

        public override SyncSettings getDefaultSyncSettings()
        {
            throw new NotImplementedException();
        }
    }
}
