using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Windwaker.Multiplayer.Client
{
    internal class WindwakerProgress
    {
        public byte stageId = 0xFF;

        public byte swordId = 0xFF;
        public byte swordBitfield = 0;

        public byte shieldId = 0xFF;
        public byte shieldBitfield = 0;

        public byte pearls = 0;
        public byte triforceShards = 0;
    }
}
