using System.Collections.Generic;
using Windwaker.Multiplayer.Client.Progress.Helpers;

namespace Windwaker.Multiplayer.Client.Progress
{
    internal class ProgressManager
    {
        private readonly Dictionary<string, byte> items = new();
        private readonly Dictionary<string, IObtainable> progressHelpers = new();

        public ProgressManager()
        {
            InitializeHelpers();
            ResetProgress();
        }

        public void ReceiveProgress(string player, ProgressUpdate progress)
        {
            if (progress.type == ProgressType.Item)
            {
                byte current = GetItemLevel(progress.id);
                if (progress.value > current)
                {
                    items[progress.id] = progress.value;
                    //Core.UIManager.Log($"Received item: {progress.id} from {player}");
                    //Core.DolphinManager.WriteReceivedItem(progress.id, progress.value);
                    //Core.NotificationManager.DisplayProgressNotification(player, progress);
                    AddProgress(progress.id, progress.value);
                    ShowNotification(player, progress.id, progress.value);
                }
            }
        }

        public void ResetProgress()
        {
            items.Clear();
            items["maxhealth"] = 12;
        }

        public void ObtainItem(string item, byte value)
        {
            items[item] = value;
            var progress = new ProgressUpdate(ProgressType.Item, item, value);
            //Core.NotificationManager.DisplayProgressNotification(null, progress);
            ShowNotification(null, item, value);
            Core.NetworkManager.SendProgress(progress);
        }

        public void ReceiveItem(string player, string item, byte value)
        {
            
        }

        public byte GetItemLevel(string item)
        {
            return items.TryGetValue(item, out byte value) ? value : (byte)0;
        }

        private void InitializeHelpers()
        {
            progressHelpers.Add("telescope", new Telescope());
            progressHelpers.Add("sail", new Sail());

            progressHelpers.Add("maxhealth", new Health());
            progressHelpers.Add("maxmagic", new Magic());
            progressHelpers.Add("maxarrows", new Arrows());
            progressHelpers.Add("maxbombs", new Bombs());
        }

        public void CheckForProgress(string progress, byte value)
        {
            if (progressHelpers.TryGetValue(progress, out IObtainable helper))
            {
                Core.UIManager.Log(progress + ": " + value);
                helper.CheckForProgress(value);
            }
            else
            {
                Core.UIManager.LogError("Checking for unknown progress: " + progress);
            }
        }

        private void AddProgress(string progress, byte value)
        {
            if (progressHelpers.TryGetValue(progress, out IObtainable helper))
            {
                helper.AddProgress(value);
            }
            else
            {
                Core.UIManager.LogError("Adding unknown progress: " + progress);
            }
        }

        private void ShowNotification(string player, string progress, byte value)
        {
            if (progressHelpers.TryGetValue(progress, out IObtainable helper))
            {
                string playerPart = player is null ? "You have" : $"{player} has";
                string itemPart = helper.GetNotificationPart(value);

                Core.UIManager.LogProgress(playerPart + itemPart);
            }
            else
            {
                Core.UIManager.LogError("Showing notification for unknown progress: " + progress);
            }
        }

        // All of these will be gone too

        #region Inventory

        public void CheckForTelescope(byte value)
        {
            if (value == 0x20 && GetItemLevel("telescope") < 1)
                ObtainItem("telescope", 1);
        }
        public void CheckForSail(byte value)
        {
            if (value == 0x78 && GetItemLevel("sail") < 1)
                ObtainItem("sail", 1);
        }
        public void CheckForWindwaker(byte value)
        {
            if (value == 0x22 && GetItemLevel("windwaker") < 1)
                ObtainItem("windwaker", 1);
        }
        public void CheckForGrapplingHook(byte value)
        {
            if (value == 0x25 && GetItemLevel("grapplinghook") < 1)
                ObtainItem("grapplinghook", 1);
        }
        public void CheckForSpoilsBag(byte value)
        {
            if (value == 0x24 && GetItemLevel("spoilsbag") < 1)
                ObtainItem("spoilsbag", 1);
        }
        public void CheckForBoomerang(byte value)
        {
            if (value == 0x2D && GetItemLevel("boomerang") < 1)
                ObtainItem("boomerang", 1);
        }
        public void CheckForDekuLeaf(byte value)
        {
            if (value == 0x34 && GetItemLevel("dekuleaf") < 1)
                ObtainItem("dekuleaf", 1);
        }
        public void CheckForTingleTuner(byte value)
        {
            if (value == 0x21 && GetItemLevel("tingletuner") < 1)
                ObtainItem("tingletuner", 1);
        }
        public void CheckForPictoBox(byte value)
        {
            if (value == 0x26 && GetItemLevel("pictobox") < 2)
                ObtainItem("pictobox", 2);
            else if (value == 0x23 && GetItemLevel("pictobox") < 1)
                ObtainItem("pictobox", 1);
        }
        public void CheckForIronBoots(byte value)
        {
            if (value == 0x29 && GetItemLevel("ironboots") < 1)
                ObtainItem("ironboots", 1);
        }
        public void CheckForMagicArmor(byte value)
        {
            if (value == 0x2A && GetItemLevel("magicarmor") < 1)
                ObtainItem("magicarmor", 1);
        }
        public void CheckForBaitBag(byte value)
        {
            if (value == 0x2C && GetItemLevel("baitbag") < 1)
                ObtainItem("baitbag", 1);
        }
        public void CheckForBow(byte value)
        {
            if (value == 0x36 && GetItemLevel("bow") < 3)
                ObtainItem("bow", 3);
            else if (value == 0x35 && GetItemLevel("bow") < 2)
                ObtainItem("bow", 2);
            else if (value == 0x27 && GetItemLevel("bow") < 1)
                ObtainItem("bow", 1);
        }
        public void CheckForBombs(byte value)
        {
            if (value == 0x31 && GetItemLevel("bombs") < 1)
                ObtainItem("bombs", 1);
        }
        public void CheckForBottle1(byte value)
        {
            if (value >= 0x50 && value <= 0x59 && GetItemLevel("bottle1") < 1)
                ObtainItem("bottle1", 1);
        }
        public void CheckForBottle2(byte value)
        {
            if (value >= 0x50 && value <= 0x59 && GetItemLevel("bottle2") < 1)
                ObtainItem("bottle2", 1);
        }
        public void CheckForBottle3(byte value)
        {
            if (value >= 0x50 && value <= 0x59 && GetItemLevel("bottle3") < 1)
                ObtainItem("bottle3", 1);
        }
        public void CheckForBottle4(byte value)
        {
            if (value >= 0x50 && value <= 0x59 && GetItemLevel("bottle4") < 1)
                ObtainItem("bottle4", 1);
        }
        public void CheckForDeliveryBag(byte value)
        {
            if (value == 0x30 && GetItemLevel("deliverybag") < 1)
                ObtainItem("deliverybag", 1);
        }
        public void CheckForHookshot(byte value)
        {
            if (value == 0x2F && GetItemLevel("hookshot") < 1)
                ObtainItem("hookshot", 1);
        }
        public void CheckForSkullHammer(byte value)
        {
            if (value == 0x33 && GetItemLevel("skullhammer") < 1)
                ObtainItem("skullhammer", 1);
        }

        #endregion Inventory

        #region Equipment

        public void CheckForSword(byte value)
        {
            if (value == 0x3E && GetItemLevel("sword") < 4)
                ObtainItem("sword", 4);
            else if (value == 0x3A && GetItemLevel("sword") < 3)
                ObtainItem("sword", 3);
            else if (value == 0x39 && GetItemLevel("sword") < 2)
                ObtainItem("sword", 2);
            else if (value == 0x38 && GetItemLevel("sword") < 1)
                ObtainItem("sword", 1);
        }
        public void CheckForShield(byte value)
        {
            if (value == 0x3C && GetItemLevel("shield") < 2)
                ObtainItem("shield", 2);
            else if (value == 0x3B && GetItemLevel("shield") < 1)
                ObtainItem("shield", 1);
        }
        public void CheckForPowerBracelets(byte value)
        {
            if (value == 0x28 && GetItemLevel("powerbracelets") < 1)
                ObtainItem("powerbracelets", 1);
        }
        public void CheckForPiratesCharm(byte value)
        {
            if ((value & 0x01) > 0 && GetItemLevel("piratescharm") < 1)
                ObtainItem("piratescharm", 1);
        }
        public void CheckForHerosCharm(byte value)
        {
            if ((value & 0x01) > 0 && GetItemLevel("heroscharm") < 1)
                ObtainItem("heroscharm", 1);
        }
        public void CheckForWallet(byte value)
        {
            if (value == 0x02 && GetItemLevel("wallet") < 2)
                ObtainItem("wallet", 2);
            else if (value == 0x01 && GetItemLevel("wallet") < 1)
                ObtainItem("wallet", 1);
        }

        public void CheckForMaxHealth(byte value)
        {
            if (GetItemLevel("maxhealth") < value)
                ObtainItem("maxhealth", value);
        }
        public void CheckForMaxMagic(byte value)
        {
            if (GetItemLevel("maxmagic") < value)
                ObtainItem("maxmagic", value);
        }
        public void CheckForMaxArrows(byte value)
        {
            if (GetItemLevel("maxarrows") < value)
                ObtainItem("maxarrows", value);
        }
        public void CheckForMaxBombs(byte value)
        {
            if (GetItemLevel("maxbombs") < value)
                ObtainItem("maxbombs", value);
        }

        public void CheckForSongs(byte value)
        {
            if (GetItemLevel("songs") < value)
                ObtainItem("songs", value);
        }
        public void CheckForPearls(byte value)
        {
            if (GetItemLevel("pearls") < value)
                ObtainItem("pearls", value);
        }
        public void CheckForShards(byte value)
        {
            if (GetItemLevel("shards") < value)
                ObtainItem("shards", value);
        }
        public void CheckForTingleStatues(byte value)
        {
            if (GetItemLevel("tinglestatues") < value)
                ObtainItem("tinglestatues", value);
        }

        #endregion Equipment

        public void CheckForCharts(string type, byte index, byte value)
        {
            string key = $"charts{type}{index}";
            if (GetItemLevel(key) < value)
                ObtainItem(key, value);
        }

        public void CheckForSectors(byte index, byte value)
        {
            string key = $"sector{index}";
            if (GetItemLevel(key) < value)
                ObtainItem(key, value);
        }

        public byte bagContents;

        public byte warpPots;

        public byte stages;

        public byte events;
    }
}
