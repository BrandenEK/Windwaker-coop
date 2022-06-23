using System;
using System.Collections.Generic;

namespace Windwaker_coop
{
    interface IGame
    {
        int gameId { get; }
        string gameName { get; } 
        string processName { get; }
        uint[] baseAddressOffsets { get; }
        uint identityAddress { get; }
        string identityText { get; }
        bool bigEndian { get; }
        //SyncSettings syncSettings { get; set; }

        void beginningFunctions(Client client);

        void endingFunctions(Client client);

        void onReceiveLocationFunctions(Client client, uint newValue, uint oldValue, MemoryLocation memLoc);

        void onReceiveListFunctions(Client client, byte[] memory);

        List<MemoryLocation> createMemoryLocations();

        Cheat[] getCheats();

        SyncSettings getDefaultSyncSettings();
    }
}
