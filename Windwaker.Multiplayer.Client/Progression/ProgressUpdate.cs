
namespace Windwaker.Multiplayer.Client.Progression
{
    internal class ProgressUpdate
    {
        public string Id { get; }
        public int Value { get; }

        public ProgressUpdate(string id, int value)
        {
            Id = id;
            Value = value;
        }
    }
}
