using System;
using System.Collections.Generic;
using System.Text;

namespace Windwaker_coop
{
    abstract class Game
    {
        public int gameId;
        public string gameName;
        public string processName;
        public uint identityAddress;
        public string identityText;
        public Dictionary<string, bool> syncSettings;

        public Game(int gameId, string gameName, string processName, uint identityAddress, string identityText)
        {
            this.gameId = gameId;
            this.gameName = gameName;
            this.processName = processName;
            this.identityAddress = identityAddress;
            this.identityText = identityText;
            setDefaultSyncSettings();
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

        public abstract void setDefaultSyncSettings();
    }
}
