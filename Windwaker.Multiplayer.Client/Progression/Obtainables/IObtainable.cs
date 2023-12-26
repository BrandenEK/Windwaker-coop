using Windwaker.Multiplayer.Client.Memory;

namespace Windwaker.Multiplayer.Client.Progression.Obtainables
{
    internal interface IObtainable
    {
        public bool TryRead(IMemoryReader memoryReader, out int value);

        public void TryWrite(IMemoryReader memoryReader, int value);

        public void Reset();

        public string? GetNotificationPart(int value);
    }
}
