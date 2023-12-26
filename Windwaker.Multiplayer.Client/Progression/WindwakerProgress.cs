using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using Windwaker.Multiplayer.Client.Logging;
using Windwaker.Multiplayer.Client.Memory;
using Windwaker.Multiplayer.Client.Progression.Obtainables;

namespace Windwaker.Multiplayer.Client.Progression
{
    internal class WindwakerProgress : IProgressChecker
    {
        private readonly Dictionary<string, IObtainable> _obtainables;
        private readonly ILogger _logger;
        private readonly IMemoryReader _memoryReader;

        public WindwakerProgress(ILogger logger, IMemoryReader memoryReader)
        {
            _logger = logger;
            _memoryReader = memoryReader;
            _obtainables = LoadObtainables("windwaker");
        }

        public void CheckForProgress()
        {
            int startTime = Environment.TickCount;

            foreach (var obtainable in _obtainables)
            {
                if (obtainable.Value.TryRead(_memoryReader, out int progress))
                {
                    var update = new ProgressUpdate(obtainable.Key, progress);
                    _logger.Warning($"Found progress: {update.Id} {update.Value}");
                    // Send progress over network
                    ShowNotification(null, update);
                }
            }

            int endTime = Environment.TickCount;
            _logger.Info($"Progress check time: {endTime - startTime}");
        }

        public void ReceiveProgress(string player, ProgressUpdate progress)
        {
            _logger.Info("Received progress: " + progress.Id);
            if (!_obtainables.TryGetValue(progress.Id, out IObtainable? obtainable))
            {
                _logger.Error("Received unknown progress: " + progress.Id);
                return;
            }

            obtainable.TryWrite(_memoryReader, progress.Value);
            ShowNotification(player, progress);
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

        private void ShowNotification(string? player, ProgressUpdate progress)
        {
            if (!_obtainables.TryGetValue(progress.Id, out IObtainable? obtainable))
            {
                _logger.Error("Showing notification for unknown progress: " + progress.Id);
                return;
            }

            string playerPart = player is null ? "You have" : $"{player} has";
            string? itemPart = obtainable.GetNotificationPart(progress.Value);

            if (itemPart is not null)
                _logger.Warning(playerPart + itemPart);
        }

        // Unused now to load from json
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

        private Dictionary<string, IObtainable> LoadObtainables(string gameName)
        {
            var obtainables = new Dictionary<string, IObtainable>();

            string path = Path.Combine(Environment.CurrentDirectory, "data", gameName, "obtainables.json");
            if (!File.Exists(path))
            {
                _logger.Error($"Obtainables list for {gameName} does not exist!");
                return obtainables;
            }

            string json = File.ReadAllText(path);
            var obtainList = JsonConvert.DeserializeObject<ObtainableList>(json) ?? new ObtainableList();

            foreach (var singleItem in obtainList.singleItems)
                obtainables.Add(singleItem.Key, singleItem.Value);

            foreach (var multipleItem in obtainList.multipleItems)
                obtainables.Add(multipleItem.Key, multipleItem.Value);

            return obtainables;
        }
    }
}
