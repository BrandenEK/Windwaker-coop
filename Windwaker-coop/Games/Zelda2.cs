using System;
using System.Collections.Generic;
using System.Text;

namespace Windwaker_coop
{
    class Zelda2 : IGame
    {
        public int gameId { get { return 5; } }
        public string gameName { get { return "Zelda II"; } }
        public string processName { get { return "fceu"; } }
        public uint[] baseAddressOffsets { get { return new uint[] { 0x59FE00 }; } }
        public uint identityAddress { get { return 0; } }
        public string identityText { get { return "idk"; } }
        public bool bigEndian { get { return true; } }

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
            SyncSettings s = Program.syncSettings;


            return memoryLocations;
        }

        public Cheat[] getCheats()
        {
            return null;
        }

        public SyncSettings getDefaultSyncSettings()
        {
            return new SyncSettings(new string[] { "Stats", "Equipment", "Spells", "Quests" }, new bool[] { true, true, true, true });
        }
    }
}
