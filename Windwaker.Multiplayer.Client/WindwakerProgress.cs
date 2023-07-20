
namespace Windwaker.Multiplayer.Client
{
    public class WindwakerProgress
    {
        public void ResetProgress()
        {

        }

        public void AddItem(string player, string item, byte value)
        {
            ClientForm.Log($"Receiving item: {item} from {player}");
        }

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
                ClientForm.Client.SendProgress(ProgressType.Item, "telescope", 1);
            }
        }
        public void CheckForSail(byte value)
        {
            if (value == 0x78 && !sail)
            {
                sail = true;
                ClientForm.Client.SendProgress(ProgressType.Item, "sail", 1);
            }
        }
        public void CheckForWindwaker(byte value)
        {
            if (value == 0x22 && !windwaker)
            {
                windwaker = true;
                ClientForm.Client.SendProgress(ProgressType.Item, "windwaker", 1);
            }
        }
        public void CheckForGrapplingHook(byte value)
        {
            if (value == 0x25 && !grapplinghook)
            {
                grapplinghook = true;
                ClientForm.Client.SendProgress(ProgressType.Item, "grapplinghook", 1);
            }
        }
        public void CheckForSpoilsBag(byte value)
        {
            if (value == 0x24 && !spoilsbag)
            {
                spoilsbag = true;
                ClientForm.Client.SendProgress(ProgressType.Item, "spoilsbag", 1);
            }
        }
        public void CheckForBoomerang(byte value)
        {
            if (value == 0x2D && !boomerang)
            {
                boomerang = true;
                ClientForm.Client.SendProgress(ProgressType.Item, "boomerang", 1);
            }
        }
        public void CheckForDekuLeaf(byte value)
        {
            if (value == 0x34 && !dekuleaf)
            {
                dekuleaf = true;
                ClientForm.Client.SendProgress(ProgressType.Item, "dekuleaf", 1);
            }
        }
        public void CheckForTingleTuner(byte value)
        {
            if (value == 0x21 && !tingletuner)
            {
                tingletuner = true;
                ClientForm.Client.SendProgress(ProgressType.Item, "tingletuner", 1);
            }
        }
        public void CheckForPictoBox(byte value)
        {
            if (value == 0x26 && pictobox < PictoBoxType.Deluxe)
            {
                pictobox = PictoBoxType.Deluxe;
                ClientForm.Client.SendProgress(ProgressType.Item, "pictobox", 2);
            }
            else if (value == 0x23 && pictobox < PictoBoxType.Standard)
            {
                pictobox = PictoBoxType.Standard;
                ClientForm.Client.SendProgress(ProgressType.Item, "pictobox", 1);
            }
        }
        public void CheckForIronBoots(byte value)
        {
            if (value == 0x29 && !ironboots)
            {
                ironboots = true;
                ClientForm.Client.SendProgress(ProgressType.Item, "ironboots", 1);
            }
        }
        public void CheckForMagicArmor(byte value)
        {
            if (value == 0x2A && !magicarmor)
            {
                magicarmor = true;
                ClientForm.Client.SendProgress(ProgressType.Item, "magicarmor", 1);
            }
        }
        public void CheckForBaitBag(byte value)
        {
            if (value == 0x2C && !baitbag)
            {
                baitbag = true;
                ClientForm.Client.SendProgress(ProgressType.Item, "baitbag", 1);
            }
        }
        public void CheckForBow(byte value)
        {
            if (value == 0x36 && bow < BowType.Light)
            {
                bow = BowType.Light;
                ClientForm.Client.SendProgress(ProgressType.Item, "bow", 3);
            }
            else if (value == 0x35 && bow < BowType.FireAndIce)
            {
                bow = BowType.FireAndIce;
                ClientForm.Client.SendProgress(ProgressType.Item, "bow", 2);
            }
            else if (value == 0x27 && bow < BowType.Standard)
            {
                bow = BowType.Standard;
                ClientForm.Client.SendProgress(ProgressType.Item, "bow", 1);
            }
        }
        public void CheckForBombs(byte value)
        {
            if (value == 0x31 && !bombs)
            {
                bombs = true;
                ClientForm.Client.SendProgress(ProgressType.Item, "bombs", 1);
            }
        }

        public void CheckForBottle1(byte value)
        {
            if (value >= 0x50 && value <= 0x59 && !bottle1)
            {
                bottle1 = true;
                ClientForm.Client.SendProgress(ProgressType.Item, "bottle1", 1);
            }
        }
        public void CheckForBottle2(byte value)
        {
            if (value >= 0x50 && value <= 0x59 && !bottle2)
            {
                bottle2 = true;
                ClientForm.Client.SendProgress(ProgressType.Item, "bottle2", 1);
            }
        }
        public void CheckForBottle3(byte value)
        {
            if (value >= 0x50 && value <= 0x59 && !bottle3)
            {
                bottle3 = true;
                ClientForm.Client.SendProgress(ProgressType.Item, "bottle3", 1);
            }
        }
        public void CheckForBottle4(byte value)
        {
            if (value >= 0x50 && value <= 0x59 && !bottle4)
            {
                bottle4 = true;
                ClientForm.Client.SendProgress(ProgressType.Item, "bottle4", 1);
            }
        }
        public void CheckForDeliveryBag(byte value)
        {
            if (value == 0x30 && !deliverybag)
            {
                deliverybag = true;
                ClientForm.Client.SendProgress(ProgressType.Item, "deliverybag", 1);
            }
        }
        public void CheckForHookshot(byte value)
        {
            if (value == 0x2F && !hookshot)
            {
                hookshot = true;
                ClientForm.Client.SendProgress(ProgressType.Item, "hookshot", 1);
            }
        }
        public void CheckForSkullHammer(byte value)
        {
            if (value == 0x33 && !skullhammer)
            {
                skullhammer = true;
                ClientForm.Client.SendProgress(ProgressType.Item, "skullhammer", 1);
            }
        }

        #endregion Inventory

        #region Equipment

        private SwordType sword;

        public void CheckForSword(byte value)
        {
            if (value == 0x3E && sword < SwordType.MasterFull)
            {
                sword = SwordType.MasterFull;
                ClientForm.Client.SendProgress(ProgressType.Item, "sword", 4);
            }
            else if (value == 0x3A && sword < SwordType.MasterHalf)
            {
                sword = SwordType.MasterHalf;
                ClientForm.Client.SendProgress(ProgressType.Item, "sword", 3);
            }
            else if (value == 0x39 && sword < SwordType.MasterEmpty)
            {
                sword = SwordType.MasterEmpty;
                ClientForm.Client.SendProgress(ProgressType.Item, "sword", 2);
            }
            else if (value == 0x38 && sword < SwordType.Hero)
            {
                sword = SwordType.Hero;
                ClientForm.Client.SendProgress(ProgressType.Item, "sword", 1);
            }
        }

        private ShieldType shield;

        public void CheckForShield(byte value)
        {
            if (value == 0x3C && shield < ShieldType.Mirror)
            {
                shield = ShieldType.Mirror;
                ClientForm.Client.SendProgress(ProgressType.Item, "shield", 2);
            }
            else if (value == 0x3B && shield < ShieldType.Hero)
            {
                shield = ShieldType.Hero;
                ClientForm.Client.SendProgress(ProgressType.Item, "shield", 1);
            }
        }

        private bool powerbracelets;

        public void CheckForPowerBracelets(byte value)
        {
            if (value == 0x28 && !powerbracelets)
            {
                powerbracelets = true;
                ClientForm.Client.SendProgress(ProgressType.Item, "powerbracelets", 1);
            }
        }

        private bool piratescharm;

        public void CheckForPiratesCharm(byte value)
        {
            if ((value & 0x01) > 0 && !piratescharm)
            {
                piratescharm = true;
                ClientForm.Client.SendProgress(ProgressType.Item, "piratescharm", 1);
            }
        }

        private bool heroscharm;

        public void CheckForHerosCharm(byte value)
        {
            if ((value & 0x01) > 0 && !heroscharm)
            {
                heroscharm = true;
                ClientForm.Client.SendProgress(ProgressType.Item, "heroscharm", 1);
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
