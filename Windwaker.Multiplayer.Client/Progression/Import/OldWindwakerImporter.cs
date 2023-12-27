using System.Collections.Generic;
using Windwaker.Multiplayer.Client.Progression.Obtainables;

namespace Windwaker.Multiplayer.Client.Progression.Import
{
    internal class OldWindwakerImporter : IDataImporter
    {
        public Dictionary<string, IObtainable> LoadObtainables()
        {
            var obtainables = new Dictionary<string, IObtainable>();

            obtainables.Add("telescope", new SingleItem("Telescope", 0x4C44, 0x20, 0x4C59));
            obtainables.Add("sail", new SingleItem("the Sail", 0x4C45, 0x78, 0x4C5A));
            obtainables.Add("windwaker", new SingleItem("the Windwaker", 0x4C46, 0x22, 0x4C5B));

            obtainables.Add("sword", new MultipleItem(
                new string[] { "the Hero's Sword", "the Master Sword (Powerless)", "the Master Sword (Half power)", "the Master Sword (Full power)" },
                0x4C16, new byte[] { 0x38, 0x39, 0x3A, 0x3E }, 0x4CBC));

            obtainables.Add("maxhealth", new ValueItem("a piece of heart", 0x4C09));

            obtainables.Add("pearls", new BitfieldItem(
                new string[] { "obtained Din's Pearl", "obtained Farore's Pearl", "obtained Nayru's Pearl" },
                0x4CC7, new byte[] { 0x02, 0x04, 0x01 }));

            return obtainables;
        }
    }
}
