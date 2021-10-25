using System;
using System.Collections.Generic;
using System.Text;

namespace Windwaker_coop
{
    class OcarinaOfTime : Game
    {
        public OcarinaOfTime() : base(1, "Ocarina of Time", "???") { }

        //Individual functions

        public override void addMemoryLocations(List<MemoryLocation> memoryLocations, SyncSettings settings)
        {
            throw new NotImplementedException();
        }

        public override Cheat[] getCheats()
        {
            throw new NotImplementedException();
        }
    }
}
