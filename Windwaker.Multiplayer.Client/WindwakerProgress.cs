using System.Collections.Generic;

namespace Windwaker.Multiplayer.Client
{
    public class WindwakerProgress
    {
        private readonly Dictionary<string, byte> items = new();

        public byte stageId = 0xFF;

        public WindwakerProgress() => ResetProgress();

        public void ResetProgress()
        {
            items.Clear();
            items["maxhealth"] = 12;
        }

        public void ObtainItem(string item, byte value)
        {
            items[item] = value;
            ClientForm.Log($"Obtained item: {item}");
            ClientForm.Client.SendProgress(ProgressType.Item, item, value);
        }

        public void ReceiveItem(string player, string item, byte value)
        {
            byte current = CheckItem(item);
            if (value > current)
            {
                ClientForm.Log($"Received item: {item} from {player}");
                items[item] = value;
            }
        }

        private byte CheckItem(string item)
        {
            return items.TryGetValue(item, out byte value) ? value : (byte)0;
        }

        #region Inventory

        public void CheckForTelescope(byte value)
        {
            if (value == 0x20 && CheckItem("telescope") < 1)
                ObtainItem("telescope", 1);
        }
        public void CheckForSail(byte value)
        {
            if (value == 0x78 && CheckItem("sail") < 1)
                ObtainItem("sail", 1);
        }
        public void CheckForWindwaker(byte value)
        {
            if (value == 0x22 && CheckItem("windwaker") < 1)
                ObtainItem("windwaker", 1);
        }
        public void CheckForGrapplingHook(byte value)
        {
            if (value == 0x25 && CheckItem("grapplinghook") < 1)
                ObtainItem("grapplinghook", 1);
        }
        public void CheckForSpoilsBag(byte value)
        {
            if (value == 0x24 && CheckItem("spoilsbag") < 1)
                ObtainItem("spoilsbag", 1);
        }
        public void CheckForBoomerang(byte value)
        {
            if (value == 0x2D && CheckItem("boomerang") < 1)
                ObtainItem("boomerang", 1);
        }
        public void CheckForDekuLeaf(byte value)
        {
            if (value == 0x34 && CheckItem("dekuleaf") < 1)
                ObtainItem("dekuleaf", 1);
        }
        public void CheckForTingleTuner(byte value)
        {
            if (value == 0x21 && CheckItem("tingletuner") < 1)
                ObtainItem("tingletuner", 1);
        }
        public void CheckForPictoBox(byte value)
        {
            if (value == 0x26 && CheckItem("pictobox") < 2)
                ObtainItem("pictobox", 2);
            else if (value == 0x23 && CheckItem("pictobox") < 1)
                ObtainItem("pictobox", 1);
        }
        public void CheckForIronBoots(byte value)
        {
            if (value == 0x29 && CheckItem("ironboots") < 1)
                ObtainItem("ironboots", 1);
        }
        public void CheckForMagicArmor(byte value)
        {
            if (value == 0x2A && CheckItem("magicarmor") < 1)
                ObtainItem("magicarmor", 1);
        }
        public void CheckForBaitBag(byte value)
        {
            if (value == 0x2C && CheckItem("baitbag") < 1)
                ObtainItem("baitbag", 1);
        }
        public void CheckForBow(byte value)
        {
            if (value == 0x36 && CheckItem("bow") < 3)
                ObtainItem("bow", 3);
            else if (value == 0x35 && CheckItem("bow") < 2)
                ObtainItem("bow", 2);
            else if (value == 0x27 && CheckItem("bow") < 1)
                ObtainItem("bow", 1);
        }
        public void CheckForBombs(byte value)
        {
            if (value == 0x31 && CheckItem("bombs") < 1)
                ObtainItem("bombs", 1);
        }
        public void CheckForBottle1(byte value)
        {
            if (value >= 0x50 && value <= 0x59 && CheckItem("bottle1") < 1)
                ObtainItem("bottle1", 1);
        }
        public void CheckForBottle2(byte value)
        {
            if (value >= 0x50 && value <= 0x59 && CheckItem("bottle2") < 1)
                ObtainItem("bottle2", 1);
        }
        public void CheckForBottle3(byte value)
        {
            if (value >= 0x50 && value <= 0x59 && CheckItem("bottle3") < 1)
                ObtainItem("bottle3", 1);
        }
        public void CheckForBottle4(byte value)
        {
            if (value >= 0x50 && value <= 0x59 && CheckItem("bottle4") < 1)
                ObtainItem("bottle4", 1);
        }
        public void CheckForDeliveryBag(byte value)
        {
            if (value == 0x30 && CheckItem("deliverybag") < 1)
                ObtainItem("deliverybag", 1);
        }
        public void CheckForHookshot(byte value)
        {
            if (value == 0x2F && CheckItem("hookshot") < 1)
                ObtainItem("hookshot", 1);
        }
        public void CheckForSkullHammer(byte value)
        {
            if (value == 0x33 && CheckItem("skullhammer") < 1)
                ObtainItem("skullhammer", 1);
        }

        #endregion Inventory

        #region Equipment

        public void CheckForSword(byte value)
        {
            if (value == 0x3E && CheckItem("sword") < 4)
                ObtainItem("sword", 4);
            else if (value == 0x3A && CheckItem("sword") < 3)
                ObtainItem("sword", 3);
            else if (value == 0x39 && CheckItem("sword") < 2)
                ObtainItem("sword", 2);
            else if (value == 0x38 && CheckItem("sword") < 1)
                ObtainItem("sword", 1);
        }
        public void CheckForShield(byte value)
        {
            if (value == 0x3C && CheckItem("shield") < 2)
                ObtainItem("shield", 2);
            else if (value == 0x3B && CheckItem("shield") < 1)
                ObtainItem("shield", 1);
        }
        public void CheckForPowerBracelets(byte value)
        {
            if (value == 0x28 && CheckItem("powerbracelets") < 1)
                ObtainItem("powerbracelets", 1);
        }
        public void CheckForPiratesCharm(byte value)
        {
            if ((value & 0x01) > 0 && CheckItem("piratescharm") < 1)
                ObtainItem("piratescharm", 1);
        }
        public void CheckForHerosCharm(byte value)
        {
            if ((value & 0x01) > 0 && CheckItem("heroscharm") < 1)
                ObtainItem("heroscharm", 1);
        }
        public void CheckForWallet(byte value)
        {
            if (value == 0x02 && CheckItem("wallet") < 2)
                ObtainItem("wallet", 2);
            else if (value == 0x01 && CheckItem("wallet") < 1)
                ObtainItem("wallet", 1);
        }

        public void CheckForMaxHealth(byte value)
        {
            if (CheckItem("maxhealth") < value)
                ObtainItem("maxhealth", value);
        }
        public void CheckForMaxMagic(byte value)
        {
            if (CheckItem("maxmagic") < value)
                ObtainItem("maxmagic", value);
        }
        public void CheckForMaxArrows(byte value)
        {
            if (CheckItem("maxarrows") < value)
                ObtainItem("maxarrows", value);
        }
        public void CheckForMaxBombs(byte value)
        {
            if (CheckItem("maxbombs") < value)
                ObtainItem("maxbombs", value);
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

    public enum WalletType { Small = 0, Medium = 1, Large = 2 }

    public enum BottleType { Empty, RedPotion, GreenPotion, BluePotion, ElixirSoupHalf, ElixirSoupFull, Water, Fairy, ForestFirefly, ForestWater }
}
