using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Windwaker.Multiplayer.Server
{
    internal class WindwakerServer : AbstractServer<WindwakerType>
    {
        public WindwakerServer()
        {
            Initialize(new Dictionary<WindwakerType, Action<string, byte[]>>()
            {
                { WindwakerType.Position, ReceivePosition },
                { WindwakerType.MemoryLocation, ReceiveMemoryLocation },
                { WindwakerType.FullMemory, ReceiveFullMemory },
            });
        }

        // Connection

        protected override void ClientConnected(string clientIp)
        {
            Console.WriteLine("Received connection to Windwaker server");
        }

        protected override void ClientDisconnected(string clientIp)
        {
            Console.WriteLine("Client disconnected from Windwaker server");
        }

        // Position

        private void SendPosition(string playerIp)
        {

        }

        private void ReceivePosition(string playerIp, byte[] message)
        {

        }

        // Memory location

        private void SendMemoryLocation(string playerIp)
        {

        }

        private void ReceiveMemoryLocation(string playerIp, byte[] message)
        {

        }

        // Full memory

        private void SendFullMemory(string playerIp)
        {

        }

        private void ReceiveFullMemory(string playerIp, byte[] message)
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
