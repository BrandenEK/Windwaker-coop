
namespace Windwaker.Multiplayer.Client.Progress
{
    internal class ProgressUpdate
    {
        public readonly ProgressType type;
        public readonly string id;
        public readonly byte value;

        public ProgressUpdate(ProgressType type, string id, byte value)
        {
            this.type = type;
            this.id = id;
            this.value = value;
        }
    }
}
