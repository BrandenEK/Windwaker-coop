using System;
using System.Collections.Generic;
using System.Text;

namespace Windwaker_coop
{
    class Zelda1 : Game
    {
        public Zelda1() : base(2, "Zelda 1", "fceu") { }

        //Individual functions

        public override void addMemoryLocations(List<MemoryLocation> memoryLocations, SyncSettings settings)
        {
            ComparisonData empty = new ComparisonData();

            memoryLocations.Add(new MemoryLocation(0x59FE67, 1, "sword*0", "item", 0, 0, 3, 0, 0, new ComparisonData(new uint[] { 1, 2, 3 }, new string[] { "Wooden Sword*0", "White Sword*0", "Magical Sword*0" }, false)));
            memoryLocations.Add(new MemoryLocation(0x59FE84, 1, "Boomerang*0", "item", 0, 0, 1, 0, 0, empty));

            //memoryLocations.Add(new MemoryLocation());
        }

        public override Cheat[] getCheats()
        {
            return new Cheat[] { };
        }
    }
}
