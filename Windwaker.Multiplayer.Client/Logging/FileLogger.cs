using System;
using System.IO;

namespace Windwaker.Multiplayer.Client.Logging
{
    internal class FileLogger : ILogger
    {
        private readonly string _logPath = Path.Combine(Environment.CurrentDirectory, "Latest.log");

        public FileLogger()
        {
            if (File.Exists(_logPath))
                File.SetAttributes(_logPath, File.GetAttributes(_logPath) & ~FileAttributes.ReadOnly);

            File.WriteAllText(_logPath, string.Empty);
            File.SetAttributes(_logPath, FileAttributes.ReadOnly);
        }

        public void Info(object message)
        {
            AddMessage(new Message(message.ToString(), LogLevel.Info));
        }

        public void Warning(object message)
        {
            AddMessage(new Message(message.ToString(), LogLevel.Warning));
        }

        public void Error(object message)
        {
            AddMessage(new Message(message.ToString(), LogLevel.Error));
        }

        private void AddMessage(Message message)
        {
            string output = $"[{message.Time:HH:mm:ss} {message.Level}] {message.Content}{Environment.NewLine}";

            File.SetAttributes(_logPath, File.GetAttributes(_logPath) & ~FileAttributes.ReadOnly);
            File.AppendAllText(_logPath, output);
            File.SetAttributes(_logPath, FileAttributes.ReadOnly);
        }
    }
}
