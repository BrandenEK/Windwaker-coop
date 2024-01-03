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

            string path = Path.Combine(Environment.CurrentDirectory, "data", $"{_gameName}.txt");
            if (!File.Exists(path))
            {
                _logger.Error($"Obtainables list for {_gameName} does not exist!");
                return obtainables;
            }

            string json = File.ReadAllText(path);
            var obtains = JsonConvert.DeserializeObject<ObtainableData[]>(json)!;

            foreach (var data in obtains)
            {
                obtainables.Add(data.id, data.CreateObtainable());
            }

            return obtainables;
        }

        class ObtainableData
        {
            public readonly string id;
            public readonly string[] name;

            public readonly uint[] mainAddress;
            public readonly byte[] mainValue;

            public readonly uint[] bitfieldAddress;
            public readonly byte[] bitfieldValue;

            public readonly ObtainableType type;

            public ObtainableData(string id, string[] name, byte[] mainValue, uint[] mainAddress, byte[] bitfieldValue, uint[] bitfieldAddress, ObtainableType type)
            {
                this.id = id;
                this.name = name;
                this.mainValue = mainValue;
                this.mainAddress = mainAddress;
                this.bitfieldValue = bitfieldValue;
                this.bitfieldAddress = bitfieldAddress;
                this.type = type;
            }

            public IObtainable CreateObtainable()
            {
                return type switch
                {
                    ObtainableType.Single => new SingleItem(id, name[0], mainAddress[0], mainValue[0], bitfieldAddress[0]),
                    ObtainableType.Multiple => new MultipleItem(id, name, mainAddress[0], mainValue, bitfieldAddress[0]),
                    ObtainableType.Bitfield => new BitfieldItem(id, name, bitfieldAddress[0], bitfieldValue),
                    ObtainableType.Value => new ValueItem(id, name[0], mainAddress[0]),
                    ObtainableType.Bottle => new BottleItem(id, mainAddress[0], bitfieldAddress[0]),
                    _ => throw new Exception("Invalid obtainable type")
                };
            }
        }

        enum ObtainableType
        {
            Single = 0,
            Multiple = 1,
            Bitfield = 2,
            Value = 3,
            Bottle = 4,
        }
    }
}
