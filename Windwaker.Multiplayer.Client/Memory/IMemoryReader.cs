
namespace Windwaker.Multiplayer.Client.Memory
{
    internal interface IMemoryReader
    {
        public byte[] Read(uint address, int size);

        public bool TryRead(uint address, int size, out byte[] bytes);

        public void Write(uint address, byte[] bytes);

        public bool TryWrite(uint address, byte[] bytes);
    }
}
