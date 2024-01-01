using System;
using System.Collections.Generic;
using Windwaker.Multiplayer.Client.Logging;
using Windwaker.Multiplayer.Client.Memory;
using Windwaker.Multiplayer.Client.Network;
using Windwaker.Multiplayer.Client.Network.Packets;
using Windwaker.Multiplayer.Client.Notifications;
using Windwaker.Multiplayer.Client.Progression.Import;
using Windwaker.Multiplayer.Client.Progression.Obtainables;

namespace Windwaker.Multiplayer.Client.Progression
{
    internal class WindwakerProgress : IProgressChecker
    {
        private readonly Dictionary<string, IObtainable> _obtainables;
        private readonly ILogger _logger;
        private readonly IMemoryReader _memoryReader;
        private readonly INotifier _notifier;
        private readonly IClient _client;

        public WindwakerProgress(ILogger logger, IMemoryReader memoryReader, INotifier notifier, IClient client, IDataImporter dataImporter)
        {
            _logger = logger;
            _memoryReader = memoryReader;
            _notifier = notifier;
            _client = client;

            _obtainables = dataImporter.LoadObtainables();
            _client.OnPacketReceived += OnReceiveProgress;
        }

        public void CheckForProgress()
        {
            int startTime = Environment.TickCount;

            foreach (var obtainable in _obtainables)
            {
                obtainable.Value.CheckProgress(_notifier, _memoryReader, _client);
            }

            int endTime = Environment.TickCount;
            _logger.Info($"Progress check time: {endTime - startTime}");
        }

        public void ReceiveProgress(ProgressPacket packet)
        {
            if (!_obtainables.TryGetValue(packet.Id, out IObtainable? obtainable))
            {
                _logger.Error("Received unknown progress: " + packet.Id);
                return;
            }

            _logger.Info("Received progress: " + packet.Id);
            obtainable.ReceiveProgress(_notifier, _memoryReader, packet);
        }

        public void ResetProgress()
        {
            foreach (var obtainable in _obtainables.Values)
            {
                obtainable.ResetProgress();
            }
            _logger.Info("Reset all progress");
            // Maybe set health to 12
        }

        private void OnReceiveProgress(object? _, PacketEventArgs e)
        {
            if (e.Packet is not ProgressPacket packet) return;

            ReceiveProgress(packet);
        }
    }
}
