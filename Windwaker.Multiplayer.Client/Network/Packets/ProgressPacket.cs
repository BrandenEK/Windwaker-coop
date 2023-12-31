
namespace Windwaker.Multiplayer.Client.Network.Packets
{
    public class ProgressPacket : BasePacket
    {
        public string? Player { get; init; }
        public string? Id { get; init; }
        public int Value { get; init; }
    }
}
