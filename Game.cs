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
        public Dictionary<string, bool> syncSettings;

        public Game(int gameId, string gameName, string processName)
        {
            this.gameId = gameId;
            this.gameName = gameName;
            this.processName = processName;
            setDefaultSyncSettings();
        }

        public virtual void beginningFunctions()
        {
            //Called at beginning of the sync loop
        }

        public virtual void endingFunctions()
        {
            //Called at end of the sync loop
        }

        public virtual void onReceiveFunctions()
        {
            //Called when client receives a new memory location
        }

        public abstract void addMemoryLocations(List<MemoryLocation> memoryLocations);

        public abstract Cheat[] getCheats();

        public abstract void setDefaultSyncSettings();
    }
}
