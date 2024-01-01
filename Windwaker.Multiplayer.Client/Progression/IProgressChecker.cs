using Windwaker.Multiplayer.Client.Network.Packets;

namespace Windwaker.Multiplayer.Client.Progression
{
    public interface IProgressChecker
    {
        public void CheckForProgress();

        public void ReceiveProgress(ProgressPacket packet);

        public void ResetProgress();
    }
}
