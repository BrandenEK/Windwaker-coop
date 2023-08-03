using System;
using System.Collections.Generic;
using System.Reflection;
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
            ShowNotification(null, item, value);
            Core.NetworkManager.SendProgress(progress);
        }

        public byte GetItemLevel(string item)
        {
            return items.TryGetValue(item, out byte value) ? value : (byte)0;
        }

        private void InitializeHelpers()
        {
            foreach (Type t in Assembly.GetExecutingAssembly().GetTypes())
            {
                if (t.Namespace is not null && t.Namespace.EndsWith("Progress.Helpers"))
                {
                    IObtainable obtainable = Activator.CreateInstance(t) as IObtainable;
                    progressHelpers.Add(t.Name.ToLower(), obtainable);
                }
            }
        }

        public void CheckForProgress(string progress, byte value)
        {
            if (progressHelpers.TryGetValue(progress, out IObtainable helper))
            {
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

                if (itemPart is not null)
                    Core.UIManager.LogProgress(playerPart + itemPart);
            }
            else
            {
                Core.UIManager.LogError("Showing notification for unknown progress: " + progress);
            }
        }

        // All of these will be gone too

        #region Equipment


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
