
namespace Windwaker.Multiplayer.Client.Progression
{
    public interface IProgressChecker
    {
        public void CheckForProgress();

        public void ReceiveProgress(string player, ProgressUpdate progress);

        public void ResetProgress();
    }
}
