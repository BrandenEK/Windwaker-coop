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
            MainForm.Log("Connected to windwaker server");
        }

        protected override void ServerDisconnected(string serverIp)
        {
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
