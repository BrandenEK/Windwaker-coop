
namespace Windwaker.Multiplayer.Client.Progression.Obtainables
{
    internal interface IObtainable
    {
        public bool TryRead(byte value, out byte progress);

        public bool TryWrite(byte value, out byte progress);

        public void Reset();

        public string? GetNotificationPart(byte value);
    }
}
