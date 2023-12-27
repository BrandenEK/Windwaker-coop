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

            string path = Path.Combine(Environment.CurrentDirectory, "data", _gameName, "obtainables.json");
            if (!File.Exists(path))
            {
                _logger.Error($"Obtainables list for {_gameName} does not exist!");
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

        class ObtainableList
        {
            public Dictionary<string, SingleItem> singleItems;
            public Dictionary<string, MultipleItem> multipleItems;

            [JsonConstructor]
            public ObtainableList(Dictionary<string, SingleItem> singleItems, Dictionary<string, MultipleItem> multipleItems)
            {
                this.singleItems = singleItems ?? new();
                this.multipleItems = multipleItems ?? new();
            }

            public ObtainableList()
            {
                singleItems = new();
                multipleItems = new();
            }
        }
    }
}
