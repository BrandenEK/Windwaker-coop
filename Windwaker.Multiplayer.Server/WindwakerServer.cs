using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Windwaker.Multiplayer.Server
{
    internal class WindwakerServer : AbstractServer<WindwakerType>
    {
        public override void Start(string ipPort)
        {
            Dictionary<WindwakerType, Action<string, byte[]>> receivers = new()
            {
                { WindwakerType.Position, ReceivePosition },
                { WindwakerType.MemoryLocation, ReceiveMemoryLocation },
                { WindwakerType.FullMemory, ReceiveFullMemory },
            };
            Initialize(ipPort, receivers);
        }

        // Connection

        protected override void ClientConnected(string playerIp)
        {
            
        }

        protected override void ClientDisconnected(string playerIp)
        {
            
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

    internal enum WindwakerType
    {
        Position,
        MemoryLocation,
        FullMemory,
    }
}
