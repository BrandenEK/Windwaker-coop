using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace Windwaker_coop
{
    abstract class Game
    {
        public int gameId;
        public string gameName;
        public string processName;
        public uint identityAddress;
        public string identityText;
        public SyncSettings syncSettings;

        public Game(int gameId, string gameName, string processName, uint identityAddress, string identityText)
        {
            this.gameId = gameId;
            this.gameName = gameName;
            this.processName = processName;
            this.identityAddress = identityAddress;
            this.identityText = identityText;
            syncSettings = getDefaultSyncSettings();
        }

        public virtual void beginningFunctions(Client client)
        {
            //Called at beginning of the sync loop
        }

        public virtual void endingFunctions(Client client)
        {
            //Called at end of the sync loop
        }

        public virtual void onReceiveFunctions(Client client, List<byte> data, MemoryLocation memLoc)
        {
            //Called when client receives a new memory location
        }

        public abstract void addMemoryLocations(List<MemoryLocation> memoryLocations);

        public abstract Cheat[] getCheats();

        public abstract SyncSettings getDefaultSyncSettings();

        public void setSyncSettings(string jsonObject)
        {
            syncSettings = JsonConvert.DeserializeObject<SyncSettings>(jsonObject);
        }

        public string getSyncSettings()
        {
            return JsonConvert.SerializeObject(syncSettings);
        }

        //Reads the syncSettings from json file
        public SyncSettings GetSyncSettingsFromFile()
        {
            string path = Environment.CurrentDirectory + "/syncSettings.json";
            SyncSettings s;

            if (File.Exists(path))
            {
                string syncString = File.ReadAllText(path);
                s = JsonConvert.DeserializeObject<SyncSettings>(syncString);
            }
            else
            {
                s = getDefaultSyncSettings();
                File.WriteAllText(path, JsonConvert.SerializeObject(s, Formatting.Indented));
            }
            return s;
        }
    }
}
