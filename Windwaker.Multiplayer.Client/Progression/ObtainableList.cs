using Newtonsoft.Json;
using System.Collections.Generic;
using Windwaker.Multiplayer.Client.Progression.Obtainables;

namespace Windwaker.Multiplayer.Client.Progression
{
    internal class ObtainableList
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
