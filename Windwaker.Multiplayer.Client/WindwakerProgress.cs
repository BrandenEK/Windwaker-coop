using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Windwaker.Multiplayer.Client
{
    internal class WindwakerProgress
    {
        public byte stageId = 0xFF;

        #region Inventory

        private bool telescope;
        private bool sail;
        private bool windwaker;
        private bool grapplinghook;
        private bool spoilsbag;
        private bool boomerang;
        private bool dekuleaf;

        private bool tingletuner;
        private PictoBoxType pictobox;
        private bool ironboots;
        private bool magicarmor;
        private bool baitbag;
        private BowType bow;
        private bool bombs;

        private bool bottle1;
        private bool bottle2;
        private bool bottle3;
        private bool bottle4;
        private bool deliverybag;
        private bool hookshot;
        private bool skullhammer;

        public void CheckForTelescope(byte value)
        {
            if (value == 0x20 && !telescope)
            {
                telescope = true;
                // Send progress
            }
        }
        public void CheckForSail(byte value)
        {
            if (value == 0x78 && !sail)
            {
                sail = true;
                // Send progress
            }
        }
        public void CheckForWindwaker(byte value)
        {
            if (value == 0x22 && !windwaker)
            {
                windwaker = true;
                // Send progress
            }
        }
        public void CheckForGrapplingHook(byte value)
        {
            if (value == 0x25 && !grapplinghook)
            {
                grapplinghook = true;
                // Send progress
            }
        }
        public void CheckForSpoilsBag(byte value)
        {
            if (value == 0x24 && !spoilsbag)
            {
                spoilsbag = true;
                // Send progress
            }
        }
        public void CheckForBoomerang(byte value)
        {
            if (value == 0x2D && !boomerang)
            {
                boomerang = true;
                // Send progress
            }
        }
        public void CheckForDekuLeaf(byte value)
        {
            if (value == 0x34 && !dekuleaf)
            {
                dekuleaf = true;
                // Send progress
            }
        }

        public void CheckForTingleTuner(byte value)
        {
            if (value == 0x21 && !tingletuner)
            {
                tingletuner = true;
                // Send progress
            }
        }
        public void CheckForPictoBox(byte value)
        {
            if (value == 0x26 && pictobox < PictoBoxType.Deluxe)
            {
                pictobox = PictoBoxType.Deluxe;
                // Send progress
            }
            else if (value == 0x23 && pictobox < PictoBoxType.Standard)
            {
                pictobox = PictoBoxType.Standard;
                // Send progress
            }
        }
        public void CheckForIronBoots(byte value)
        {
            if (value == 0x29 && !ironboots)
            {
                ironboots = true;
                // Send progress
            }
        }
        public void CheckForMagicArmor(byte value)
        {
            if (value == 0x2A && !magicarmor)
            {
                magicarmor = true;
                // Send progress
            }
        }
        public void CheckForBaitBag(byte value)
        {
            if (value == 0x2C && !baitbag)
            {
                baitbag = true;
                // Send progress
            }
        }
        public void CheckForBow(byte value)
        {
            if (value == 0x36 && bow < BowType.Light)
            {
                bow = BowType.Light;
                // Send progress
            }
            else if (value == 0x35 && bow < BowType.FireAndIce)
            {
                bow = BowType.FireAndIce;
                // Send progress
            }
            else if (value == 0x27 && bow < BowType.Standard)
            {
                bow = BowType.Standard;
                // Send progress
            }
        }
        public void CheckForBombs(byte value)
        {
            if (value == 0x31 && !bombs)
            {
                bombs = true;
                // Send progress
            }
        }

        public void CheckForBottle1(byte value)
        {
            if (value >= 0x50 && value <= 0x59 && !bottle1)
            {
                bottle1 = true;
                // Send progress
            }
        }
        public void CheckForBottle2(byte value)
        {
            if (value >= 0x50 && value <= 0x59 && !bottle2)
            {
                bottle2 = true;
                // Send progress
            }
        }
        public void CheckForBottle3(byte value)
        {
            if (value >= 0x50 && value <= 0x59 && !bottle3)
            {
                bottle3 = true;
                // Send progress
            }
        }
        public void CheckForBottle4(byte value)
        {
            if (value >= 0x50 && value <= 0x59 && !bottle4)
            {
                bottle4 = true;
                // Send progress
            }
        }
        public void CheckForDeliveryBag(byte value)
        {
            if (value == 0x30 && !deliverybag)
            {
                deliverybag = true;
                // Send progress
            }
        }
        public void CheckForHookshot(byte value)
        {
            if (value == 0x2F && !hookshot)
            {
                hookshot = true;
                // Send progress
            }
        }
        public void CheckForSkullHammer(byte value)
        {
            if (value == 0x33 && !skullhammer)
            {
                skullhammer = true;
                // Send progress
            }
        }

        #endregion Inventory

        #region Equipment

        private SwordType sword;

        public void CheckForSword(byte value)
        {
            if (value == 0x3E && sword < SwordType.MasterFull)
            {
                sword = SwordType.MasterFull; // send
            }
            else if (value == 0x3A && sword < SwordType.MasterHalf)
            {
                sword = SwordType.MasterHalf; // send
            }
            else if (value == 0x39 && sword < SwordType.MasterEmpty)
            {
                sword = SwordType.MasterEmpty; // send
            }
            else if (value == 0x38 && sword < SwordType.Hero)
            {
                sword = SwordType.Hero; // send
            }
        }

        private ShieldType shield;

        public void CheckForShield(byte value)
        {
            if (value == 0x3C && shield < ShieldType.Mirror)
            {
                shield = ShieldType.Mirror; // send
            }
            else if (value == 0x3B && shield < ShieldType.Hero)
            {
                shield = ShieldType.Hero; // send
            }
        }

        private bool powerbracelets;

        public void CheckForPowerBracelets(byte value)
        {
            if (value == 0x28 && !powerbracelets)
            {
                powerbracelets = true; // send
            }
        }

        private bool piratescharm;

        public void CheckForPiratesCharm(byte value)
        {
            if ((value & 0x01) > 0 && !piratescharm)
            {
                piratescharm = true; // send
            }
        }

        private bool heroscharm;

        public void CheckForHerosCharm(byte value)
        {
            if ((value & 0x01) > 0 && !heroscharm)
            {
                heroscharm = true; // send
            }
        }

        private WalletType wallet;

        public void CheckForWallet(byte value)
        {
            if (value == 0x02 && wallet < WalletType.Large)
            {
                wallet = WalletType.Large; // send
            }
            else if (value == 0x01 && wallet < WalletType.Medium)
            {
                wallet = WalletType.Medium; // send
            }
        }

        private byte maxmagic;

        public void CheckForMaxMagic(byte value)
        {
            if (maxmagic < value)
            {
                maxmagic = value; // send
            }
        }

        private byte maxarrows;

        public void CheckForMaxArrows(byte value)
        {
            if (maxarrows < value)
            {
                maxarrows = value; // send
            }
        }

        private byte maxbombs;

        public void CheckForMaxBombs(byte value)
        {
            if (maxbombs < value)
            {
                maxbombs = value; // send
            }
        }

        #endregion Equipment

        public byte tingleStatues;

        public byte bagContents;

        public byte songs;
        public byte pearls;
        public byte triforceShards;

        // Capcities

        public byte maxHealth;

        // Charts

        public byte chartsOwned;
        public byte chartsOpened;
        public byte chartsLooted;
        public byte chartsDeciphered;

        public byte sectors;

        public byte stages;

        public byte events;
    }

    public enum PictoBoxType { None = 0, Standard = 1, Deluxe = 2 }

    public enum BowType { None = 0, Standard = 1, FireAndIce = 2, Light = 3 }

    public enum SwordType { None = 0, Hero = 1, MasterEmpty = 2, MasterHalf = 3, MasterFull = 4 }

    public enum ShieldType { None = 0, Hero = 1, Mirror = 2 }

    public enum WalletType { Small = 0, Medium = 1, Large = 2 }

    public enum BottleType { Empty, RedPotion, GreenPotion, BluePotion, ElixirSoupHalf, ElixirSoupFull, Water, Fairy, ForestFirefly, ForestWater }
}
