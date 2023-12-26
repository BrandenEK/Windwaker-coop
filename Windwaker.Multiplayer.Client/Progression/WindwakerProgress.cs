using System.Collections.Generic;
using Windwaker.Multiplayer.Client.Logging;
using Windwaker.Multiplayer.Client.Progression.Obtainables;

namespace Windwaker.Multiplayer.Client.Progression
{
    internal class WindwakerProgress : IProgressChecker
    {
        private readonly Dictionary<string, IObtainable> _obtainables = new();
        private readonly ILogger _logger;

        public WindwakerProgress(ILogger logger)
        {
            _logger = logger;

            RegisterObtainables();
        }

        public void CheckForProgress()
        {
            // For each obtainable, read its memory and check if new value, if so, send it
            // Also send notification
            _logger.Info("Checking for progress");
        }

        public void ReceiveProgress(string player, ProgressUpdate progress)
        {
            // Locate the exact obtainable and if new value, write it
            // Also send notification
        }

        public void ResetProgress()
        {
            foreach (var obtainable in _obtainables.Values)
            {
                obtainable.Reset();
            }
            _logger.Info("Reset all progress");
            // Maybe set health to 12
        }

        private void ShowNotification(string player, ProgressUpdate progress)
        {
            if (!_obtainables.TryGetValue(progress.Id, out IObtainable? obtainable))
            {
                _logger.Error("Showing notification for unknown progress: " + progress);
                return;
            }

            string playerPart = player is null ? "You have" : $"{player} has";
            string? itemPart = obtainable.GetNotificationPart((byte)progress.Value);

            if (itemPart is not null)
                _logger.Warning(playerPart + itemPart);
        }

        private void RegisterObtainables()
        {
            _obtainables.Add("telescope", new SingleItem("Telescope", 0x4C44, 0x20, 0x4C59));
            _obtainables.Add("sail", new SingleItem("the Sail", 0x4C45, 0x78, 0x4C5A));
            _obtainables.Add("windwaker", new SingleItem("the Windwaker", 0x4C46, 0x22, 0x4C5B));

            _obtainables.Add("sword", new MultipleItem(
                new string[] { "the Hero's Sword", "the Master Sword (Powerless)", "the Master Sword (Half power)", "the Master Sword (Full power)" },
                0x4C16, new byte[] { 0x38, 0x39, 0x3A, 0x3E }, 0x4CBC));

            _obtainables.Add("maxhealth", new ValueItem("a piece of heart", 0x4C09));

            _obtainables.Add("pearls", new BitfieldItem(
                new string[] { "obtained Din's Pearl", "obtained Farore's Pearl", "obtained Nayru's Pearl" },
                0x4CC7, new byte[] { 0x02, 0x04, 0x01 }));
        }
    }
}
