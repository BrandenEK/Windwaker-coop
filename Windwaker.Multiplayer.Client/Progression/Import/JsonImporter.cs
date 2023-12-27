using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using Windwaker.Multiplayer.Client.Logging;
using Windwaker.Multiplayer.Client.Progression.Obtainables;

namespace Windwaker.Multiplayer.Client.Progression.Import
{
    internal class JsonImporter : IDataImporter
    {
        private readonly ILogger _logger;
        private readonly string _gameName;

        public JsonImporter(ILogger logger, string gameName)
        {
            _logger = logger;
            _gameName = gameName;
        }

        public Dictionary<string, IObtainable> LoadObtainables()
        {
            var obtainables = new Dictionary<string, IObtainable>();

            string path = Path.Combine(Environment.CurrentDirectory, "data", _gameName, "obtainables.txt");
            if (!File.Exists(path))
            {
                _logger.Error($"Obtainables list for {_gameName} does not exist!");
                return obtainables;
            }

            string json = File.ReadAllText(path);
            var obtainList = JsonConvert.DeserializeObject<ObtainableList>(json)!;

            foreach (var item in obtainList.singleItems)
                obtainables.Add(item.Key, item.Value);

            foreach (var item in obtainList.multipleItems)
                obtainables.Add(item.Key, item.Value);

            foreach (var item in obtainList.bitfieldItems)
                obtainables.Add(item.Key, item.Value);

            foreach (var item in obtainList.valueItems)
                obtainables.Add(item.Key, item.Value);

            foreach (var item in obtainList.bottleItems)
                obtainables.Add(item.Key, item.Value);

            return obtainables;
        }

        class ObtainableList
        {
            public readonly Dictionary<string, SingleItem> singleItems;
            public readonly Dictionary<string, MultipleItem> multipleItems;
            public readonly Dictionary<string, BitfieldItem> bitfieldItems;
            public readonly Dictionary<string, ValueItem> valueItems;
            public readonly Dictionary<string, BottleItem> bottleItems;

            [JsonConstructor]
            public ObtainableList(
                Dictionary<string, SingleItem> singleItems,
                Dictionary<string, MultipleItem> multipleItems,
                Dictionary<string, BitfieldItem> bitfieldItems,
                Dictionary<string, ValueItem> valueItems,
                Dictionary<string, BottleItem> bottleItems)
            {
                this.singleItems = singleItems ?? new();
                this.multipleItems = multipleItems ?? new();
                this.bitfieldItems = bitfieldItems ?? new();
                this.valueItems = valueItems ?? new();
                this.bottleItems = bottleItems ?? new();
            }
        }
    }
}
