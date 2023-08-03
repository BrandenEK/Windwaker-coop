using System;
using System.Collections.Generic;
using System.Reflection;

namespace Windwaker.Multiplayer.Client.Progress
{
    internal class ProgressManager
    {
        private readonly Dictionary<string, byte> items = new();
        private readonly Dictionary<string, byte> bitfields = new();

        private readonly Dictionary<string, BaseItem> progressHelpers = new();

        public ProgressManager()
        {
            InitializeHelpers();
            ResetProgress();
        }

        public void ReceiveProgress(string player, ProgressUpdate progress)
        {
            switch (progress.type)
            {
                case ProgressType.Item: ReceiveItem(player, progress); break;
                case ProgressType.Bitfield: ReceiveBitfield(player, progress); break;
            }
        }

        public void ResetProgress()
        {
            items.Clear();
            bitfields.Clear();
            items["maxhealth"] = 12;
        }

        // Items

        public void ObtainItem(string item, byte value)
        {
            var progress = new ProgressUpdate(ProgressType.Item, item, value);
            UpdateItemLevel(item, value);
            ShowNotification(null, item, value);
            Core.NetworkManager.SendProgress(progress);
        }

        private void ReceiveItem(string player, ProgressUpdate progress)
        {
            byte current = GetItemLevel(progress.id);
            if (progress.value > current)
            {
                UpdateItemLevel(progress.id, progress.value);
                AddProgress(progress.id, progress.value);
                ShowNotification(player, progress.id, progress.value);
            }
        }

        private void UpdateItemLevel(string item, byte value)
        {
            items[item] = value;
        }

        public byte GetItemLevel(string item)
        {
            return items.TryGetValue(item, out byte value) ? value : (byte)0;
        }

        // Bitfields

        public void FoundBitfield(string bitfield, byte value)
        {
            var progress = new ProgressUpdate(ProgressType.Bitfield, bitfield, value);
            UpdateBitfieldValue(bitfield, value);
            ShowNotification(null, bitfield, value);
            Core.NetworkManager.SendProgress(progress);
        }

        private void ReceiveBitfield(string player, ProgressUpdate progress)
        {
            byte current = GetBitfieldValue(progress.id);
            if (progress.value > 0 && (current & progress.value) == 0)
            {
                UpdateBitfieldValue(progress.id, progress.value);
                AddProgress(progress.id, progress.value);
                ShowNotification(player, progress.id, progress.value);
            }
        }

        private void UpdateBitfieldValue(string bitfield, byte value)
        {
            if (bitfields.ContainsKey(bitfield))
                bitfields[bitfield] |= value;
            else
                bitfields.Add(bitfield, value);

        }

        public byte GetBitfieldValue(string bitfield)
        {
            return bitfields.TryGetValue(bitfield, out byte value) ? value : (byte)0;
        }

        // Helpers

        public void CheckForProgress(string progress, byte value)
        {
            if (progressHelpers.TryGetValue(progress, out BaseItem helper))
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
            if (progressHelpers.TryGetValue(progress, out BaseItem helper))
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
            if (progressHelpers.TryGetValue(progress, out BaseItem helper))
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

        private void InitializeHelpers()
        {
            foreach (Type t in Assembly.GetExecutingAssembly().GetTypes())
            {
                if (t.Namespace is not null && t.Namespace.EndsWith("Progress.Helpers"))
                {
                    BaseItem helper = Activator.CreateInstance(t) as BaseItem;
                    progressHelpers.Add(t.Name.ToLower(), helper);
                }
            }
        }
    }
}
