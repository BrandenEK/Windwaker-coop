using System.Collections.Generic;
using Windwaker.Multiplayer.Client.Progression.Obtainables;

namespace Windwaker.Multiplayer.Client.Progression.Import
{
    public interface IDataImporter
    {
        public Dictionary<string, IObtainable> LoadObtainables();
    }
}
