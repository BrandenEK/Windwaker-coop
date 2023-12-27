using System;
using System.Collections.Generic;
using Windwaker.Multiplayer.Client.Logging;
using Windwaker.Multiplayer.Client.Memory;
using Windwaker.Multiplayer.Client.Progression.Import;
using Windwaker.Multiplayer.Client.Progression.Obtainables;

namespace Windwaker.Multiplayer.Client.Progression
{
    internal class WindwakerProgress : IProgressChecker
    {
        private readonly Dictionary<string, IObtainable> _obtainables;
        private readonly ILogger _logger;
        private readonly IMemoryReader _memoryReader;

        public WindwakerProgress(ILogger logger, IMemoryReader memoryReader, IDataImporter dataImporter)
        {
            _logger = logger;
            _memoryReader = memoryReader;
            _obtainables = dataImporter.LoadObtainables();
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
    }
}
