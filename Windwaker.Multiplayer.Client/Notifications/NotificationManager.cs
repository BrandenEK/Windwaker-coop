using System;
using System.Collections.Generic;
using Windwaker.Multiplayer.Client.Progress;

namespace Windwaker.Multiplayer.Client.Notifications
{
    internal class NotificationManager
    {
        public void DisplayProgressNotification(string player, ProgressUpdate progress)
        {
            Core.UIManager.LogProgress($"{player} has obtained the {progress.id}");
        }
    }
}
