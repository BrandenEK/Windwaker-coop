using System;
using System.Collections.Generic;
using Windwaker.Multiplayer.Client.Progress;

namespace Windwaker.Multiplayer.Client.Notifications
{
    internal class NotificationManager
    {
        // All gone too
        public void DisplayProgressNotification(string player, ProgressUpdate progress)
        {
            string message;
            if (progress.id == "telescope")
                message = FormatMessage(player, "Telescope", ItemMessageFormat.ObtainedThe);
            else if (progress.id == "sail")
                message = FormatMessage(player, "Sail", ItemMessageFormat.ObtainedThe);
            else
                message = "Default obtained " + progress.id;

            Core.UIManager.LogProgress(message);
        }

        private string FormatMessage(string player, string item, ItemMessageFormat format)
        {
            string partPlayer = player == null ? "You have" : (player + " has");
            string partFormat = format == ItemMessageFormat.Obtained ? "obtained" : "obtained the";
            return $"{partPlayer} {partFormat} {item}";
        }

        internal enum ItemMessageFormat
        {
            Obtained,
            ObtainedThe,
        }
    }
}
