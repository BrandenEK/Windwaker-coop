using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Windwaker.Multiplayer.Client
{
    internal class WindwakerClient : AbstractClient<WindwakerType>
    {
        public WindwakerClient()
        {
            Initialize(new Dictionary<WindwakerType, Action<byte[]>>()
            {
                { WindwakerType.Position, ReceivePosition },
                { WindwakerType.MemoryLocation, ReceiveMemoryLocation },
                { WindwakerType.FullMemory, ReceiveFullMemory },
            });
        }

        // Connection

        protected override void ServerConnected(string serverIp)
        {
            MainForm.Log("Established connection to Windwaker server");
            MainForm.UpdateUI();
        }

        protected override void ServerDisconnected(string serverIp)
        {
            MainForm.Log("Disconnected from Windwaker server");
            MainForm.UpdateUI();
        }

        // Position

        private void SendPosition()
        {

        }

        private void ReceivePosition(byte[] message)
        {

        }

        // Memory location

        private void SendMemoryLocation()
        {

        }

        private void ReceiveMemoryLocation(byte[] message)
        {

        }

        // Full memory

        private void SendFullMemory()
        {

        }

        private void ReceiveFullMemory(byte[] message)
        {

        }
    }

    internal enum WindwakerType : byte
    {
        Position,
        MemoryLocation,
        FullMemory,
    }
}
