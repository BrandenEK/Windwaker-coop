using System;
using Windwaker.Multiplayer.Client.Memory;

namespace Windwaker.Multiplayer.Client.Tests
{
    internal class TestReader : IMemoryReader
    {
        public byte[] Read(uint address, int size)
        {
            return new byte[] { 0x60 };
        }

        public bool TryRead(uint address, int size, out byte[] bytes)
        {
            throw new NotImplementedException();
        }

        public void Write(uint address, byte[] bytes)
        {
            throw new NotImplementedException();
        }

        public bool TryWrite(uint address, byte[] bytes)
        {
            throw new NotImplementedException();
        }
    }
}
