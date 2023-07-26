using System;
using System.Windows.Forms;
using Windwaker.Multiplayer.Client.Network;
using Windwaker.Multiplayer.Client.Notifications;
using Windwaker.Multiplayer.Client.Progress;

namespace Windwaker.Multiplayer.Client
{
    internal static class Core
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        public static void Main()
        {
            ApplicationConfiguration.Initialize();
            Application.Run(UIManager = new ClientForm());
        }

        public static MemoryReader MemoryReader { get; private set; } = new MemoryReader();

        public static NetworkManager NetworkManager { get; private set; } = new NetworkManager();

        public static NotificationManager NotificationManager { get; private set; } = new NotificationManager();

        public static ProgressManager ProgressManager { get; private set; } = new ProgressManager();

        public static ClientForm UIManager { get; private set; }
    }
}