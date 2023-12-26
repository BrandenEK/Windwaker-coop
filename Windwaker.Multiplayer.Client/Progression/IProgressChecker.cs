
namespace Windwaker.Multiplayer.Client.Progression
{
    internal interface IProgressChecker
    {
        public void CheckForProgress();

        public void ReceiveProgress(string player, ProgressUpdate progress);

        public void ResetProgress();
    }
}
