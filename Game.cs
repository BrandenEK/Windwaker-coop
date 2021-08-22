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

        public Game(int gameId, string gameName, string processName)
        {
            this.gameId = gameId;
            this.gameName = gameName;
            this.processName = processName;
        }

        public virtual void beginningFunctions()
        {
            //Beginning of the sync loop
        }

        public virtual void endingFunctions()
        {
            //End of the sync loop
        }

        public virtual void onReceiveFunctions()
        {
            //When client receives a new memory location
        }

        public abstract void addMemoryLocations(List<MemoryLocation> memoryLocations, SyncSettings settings);

        public abstract Cheat[] getCheats();
    }
}
