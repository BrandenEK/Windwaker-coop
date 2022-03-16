using System;
using System.Collections.Generic;
using System.Text;

namespace Windwaker_coop
{
    class OcarinaOfTime : Game
    {
        public OcarinaOfTime() : base(1, "Ocarina of Time", "project64", 0x7B263D84, "THE LEGEND OF ZELDA") { }

        //Individual functions

        public override void addMemoryLocations(List<MemoryLocation> memoryLocations)
        {
            ComparisonData empty = new ComparisonData();

            //memoryLocations.Add(new MemoryLocation(0xDFF5A5FC, 2, "more health*2", "stat", 0, 0, 320, 0, 0, empty));

            memoryLocations.Add(new MemoryLocation(0xDFF5A644, 1, "Fairy Bow*0", "item", 2, 3, 255, 255, 0, empty));
            memoryLocations.Add(new MemoryLocation(0xDFF5A645, 1, "Bombs*2", "item", 2, 2, 255, 255, 0, empty));
            memoryLocations.Add(new MemoryLocation(0xDFF5A646, 1, "Deku Nuts*2", "item", 2, 1, 255, 255, 0, empty));
            memoryLocations.Add(new MemoryLocation(0xDFF5A647, 1, "Deku Sticks*2", "item", 2, 0, 255, 255, 0, empty));
            memoryLocations.Add(new MemoryLocation(0xDFF5A648, 1, "ocarina*0", "item", 2, 7, 255, 255, 0, 
                new ComparisonData(new uint[] { 7, 8 }, new string[] { "Fairy Ocarina*0", "Ocarina of Time*0" }, false)));
            memoryLocations.Add(new MemoryLocation(0xDFF5A649, 1, "Fairy Slingshot*0", "item", 2, 6, 255, 255, 0, empty));

            memoryLocations.Add(new MemoryLocation(0xDFF5A64A, 1, "Din's Fire*2", "item", 2, 5, 255, 255, 0, empty));
            memoryLocations.Add(new MemoryLocation(0xDFF5A64B, 1, "Fire Arrows*0", "item", 2, 4, 255, 255, 0, empty));
            memoryLocations.Add(new MemoryLocation(0xDFF5A64C, 1, "Farore's Wind*2", "item", 2, 13, 255, 255, 0, empty));
            memoryLocations.Add(new MemoryLocation(0xDFF5A64D, 1, "Ice Arrows*0", "item", 2, 12, 255, 255, 0, empty));
            memoryLocations.Add(new MemoryLocation(0xDFF5A64E, 1, "hookshot*0", "item", 2, 10, 255, 255, 0, 
                new ComparisonData(new uint[] { 10, 11 }, new string[] { "Hookshot*0", "Longshot*0" }, false)));
            memoryLocations.Add(new MemoryLocation(0xDFF5A64F, 1, "Bombchus*2", "item", 2, 9, 255, 255, 0, empty));

            memoryLocations.Add(new MemoryLocation(0xDFF5A650, 1, "Megaton Hammer*0", "item", 2, 17, 255, 255, 0, empty));
            memoryLocations.Add(new MemoryLocation(0xDFF5A651, 1, "Magic Beans*2", "item", 2, 16, 255, 255, 0, empty));
            memoryLocations.Add(new MemoryLocation(0xDFF5A652, 1, "Lens of Truth*0", "item", 2, 15, 255, 255, 0, empty));
            memoryLocations.Add(new MemoryLocation(0xDFF5A653, 1, "Boomerang*0", "item", 2, 14, 255, 255, 0, empty));
            memoryLocations.Add(new MemoryLocation(0xDFF5A654, 1, "Bottle #2*2", "item", 2, 20, 255, 255, 0, empty));
            memoryLocations.Add(new MemoryLocation(0xDFF5A655, 1, "Bottle #1*2", "item", 2, 20, 255, 255, 0, empty));

            memoryLocations.Add(new MemoryLocation(0xDFF5A656, 1, "Nayru's Love*2", "item", 2, 19, 255, 255, 0, empty));
            memoryLocations.Add(new MemoryLocation(0xDFF5A657, 1, "Light Arrows*2", "item", 2, 18, 255, 255, 0, empty));
            memoryLocations.Add(new MemoryLocation(0xDFF5A658, 1, "child quest*0", "item", 2, 33, 255, 255, 0, empty));
            memoryLocations.Add(new MemoryLocation(0xDFF5A659, 1, "adult quest*0", "item", 2, 45, 255, 255, 0, empty));
            memoryLocations.Add(new MemoryLocation(0xDFF5A65A, 1, "Bottle #4*2", "item", 2, 20, 255, 255, 0, empty));
            memoryLocations.Add(new MemoryLocation(0xDFF5A65B, 1, "Bottle #3*2", "item", 2, 20, 255, 255, 0, empty));

            memoryLocations.Add(new MemoryLocation(0x53E3A66E, 1, "sword/shield*0", "item", 9, 0, 127, 0, 0, 
                new ComparisonData(new uint[] { 0, 1, 2, 3, 4, 5, 6 }, new string[] { "Kokiri Sword*0", "Master Sword*0", "Giant's Knife?*0", "broken the Giant's Knife*9",
                "Deku Shield*0", "Hylian Shield*0", "Mirror Shield*0" }, true)));
            memoryLocations.Add(new MemoryLocation(0x53E3A66F, 1, "tunic/boots*0", "item", 9, 0, 119, 0, 0,
                new ComparisonData(new uint[] { 0, 1, 2, 4, 5, 6 }, new string[] { "Kokiri Tunic*0", "Goron Tunic*0", "Zora Tunic*0", "Kokiri Boots*0", "Iron Boots*0", "Hover Boots*0" }, true)));

            memoryLocations.Add(new MemoryLocation(0xDFF5A670, 1, "changed capacity bitfield 1*9", "item", 0, 0, 255, 0, 0, empty));
            memoryLocations.Add(new MemoryLocation(0xDFF5A671, 1, "changed capacity bitfield 2*9", "item", 0, 0, 255, 0, 0, empty));
            memoryLocations.Add(new MemoryLocation(0xDFF5A672, 1, "changed capacity bitfield 3*9", "item", 0, 0, 255, 0, 0, empty));

            memoryLocations.Add(new MemoryLocation(0xDFF5A674, 1, "medallion/song*0", "quest", 9, 0, 255, 0, 0, new ComparisonData(new uint[] { 0, 1, 2, 3, 4, 5, 6, 7 },
                new string[] { "Forest Medallion*0", "Fire Medallion*0", "Water Medallion*0", "Spirit Medallion*0", "Shadow Medallion*0", "Light Medallion*0", "the Minuet of Forest*3", "the Bolero of Fire*3" }, true)));
            memoryLocations.Add(new MemoryLocation(0xDFF5A675, 1, "song*0", "quest", 9, 0, 255, 0, 0, new ComparisonData(new uint[] { 0, 1, 2, 3, 4, 5, 6, 7 },
                new string[] { "the Serenade of Water*3", "the Requiem of Spirit*3", "the Nocturne of Shadow*3", "the Prelude of Light*2", "Zelda's Lullaby*3", "Epona's Song*3", "Saria's Song*3", "the Sun's Song*3" }, true)));
            memoryLocations.Add(new MemoryLocation(0xDFF5A676, 1, "song/quest*0", "quest", 9, 0, 255, 0, 0, new ComparisonData(new uint[] { 0, 1, 2, 3, 4, 5, 6 },
                new string[] { "the Song of Time*3", "the Song of Storms*3", "Kokiri's Emerald*0", "Goron's Ruby*0", "Zora's Sapphire*0", "Stone of Agony*0", "Gerudo's Card*0" }, true)));

            //piece of heart
            //gold skulltulas

            //600 unknowns
            memoryLocations.Add(new MemoryLocation(0xDFF5A5FE, 2, "A5FE*0", "test", 8, 0, 255, 255, 0, empty));
            memoryLocations.Add(new MemoryLocation(0xDFF5A601, 1, "A601*0", "test", 8, 0, 255, 255, 0, empty));
            memoryLocations.Add(new MemoryLocation(0xDFF5A604, 1, "A604*0", "test", 8, 0, 255, 255, 0, empty));
            memoryLocations.Add(new MemoryLocation(0xDFF5A605, 1, "A605*0", "test", 8, 0, 255, 255, 0, empty));
            memoryLocations.Add(new MemoryLocation(0xDFF5A608, 1, "A608*0", "test", 8, 0, 255, 255, 0, empty));
            memoryLocations.Add(new MemoryLocation(0xDFF5A609, 1, "A609*0", "test", 8, 0, 255, 255, 0, empty));

            //630 unknowns
            memoryLocations.Add(new MemoryLocation(0xDFF5A63C, 1, "A63C*0", "test", 8, 0, 255, 255, 0, empty));
            memoryLocations.Add(new MemoryLocation(0xDFF5A640, 1, "A640*0", "test", 8, 0, 255, 255, 0, empty));
            memoryLocations.Add(new MemoryLocation(0xDFF5A641, 1, "A641*0", "test", 8, 0, 255, 255, 0, empty));

            //660-70 unknowns
            memoryLocations.Add(new MemoryLocation(0xDFF5A660, 1, "A660*0", "test", 8, 0, 255, 255, 0, empty));
            memoryLocations.Add(new MemoryLocation(0xDFF5A668, 1, "A668*0", "test", 8, 0, 255, 255, 0, empty));
            memoryLocations.Add(new MemoryLocation(0xDFF5A673, 1, "A673*0", "test", 8, 0, 255, 255, 0, empty));

            //repeated unknowns
            for (int i = 0; i < 45; i++)
                memoryLocations.Add(new MemoryLocation(0xDFF5A60C + (uint)i, 1, "0xA60C + 0x" + i.ToString("X") + "*0", "test", 8, 0, 255, 255, 0, empty));
            for (int i = 0; i < 40; i++)
                memoryLocations.Add(new MemoryLocation(0xDFF5A678 + (uint)i, 1, "0xA678 + 0x" + i.ToString("X") + "*0", "test", 8, 0, 255, 255, 0, empty));

        }

        public override Cheat[] getCheats()
        {
            return new Cheat[]
            {

            };
        }

        public override void setDefaultSyncSettings()
        {
            syncSettings = new SyncSettings();
        }
    }
}
