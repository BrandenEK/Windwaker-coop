
namespace Windwaker.Multiplayer.Client.Network.Packets
{
    public class ScenePacket : BasePacket
    {
        public string? Player { get; init; }
        public byte Scene { get; init; }
    }
}
