using System;
using System.Collections.Generic;
using System.Text;

namespace Windwaker_coop
{
    class OracleOfAges : IGame
    {
        public int gameId { get { return 4; } }
        public string gameName { get { return "Oracle of Ages"; } }
        public string processName { get { return "mGBA"; } }
        public uint[] baseAddressOffsets { get { return new uint[] { 0x0217E208, 0x38, 0x10, 0x08, 0x90 }; } }
        public uint identityAddress { get { return 0; } }
        public string identityText { get { return "idk"; } }
        public bool bigEndian { get { return false; } }

        public void beginningFunctions(Client client)
        {
            
        }

        public void endingFunctions(Client client)
        {
            
        }

        public void onReceiveListFunctions(Client client, byte[] memory)
        {
            
        }

        public void onReceiveLocationFunctions(Client client, uint newValue, uint oldValue, MemoryLocation memLoc)
        {
            
        }

        public List<MemoryLocation> createMemoryLocations()
        {
            List<MemoryLocation> memoryLocations = new List<MemoryLocation>();
            ComparisonData empty = new ComparisonData();


            return memoryLocations;
        }

        public Cheat[] getCheats()
        {
            return null;
        }

        public SyncSettings getDefaultSyncSettings()
        {
            return new SyncSettings();
        }
    }
}
