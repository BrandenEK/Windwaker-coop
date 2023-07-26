using System;
using System.Collections.Generic;
using Windwaker.Multiplayer.Client.Progress;

namespace Windwaker.Multiplayer.Client.Notifications
{
    internal class NotificationManager
    {
        public void Initialize()
        {
            Core.NetworkManager.OnReceiveProgress += OnReceiveProgress;
        }

        private void OnReceiveProgress(string player, ProgressUpdate progress)
        {
            // Display notification
        }
    }
}
