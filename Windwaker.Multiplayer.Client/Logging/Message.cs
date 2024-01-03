using System;

namespace Windwaker.Multiplayer.Client.Logging
{
    internal class Message
    {
        public string Content { get; }
        public DateTime Time { get; }
        public LogLevel Level { get; }

        public Message(string? content, LogLevel level)
        {
            Content = content ?? string.Empty;
            Time = DateTime.Now;
            Level = level;
        }
    }

    internal enum LogLevel
    {
        Info = 0,
        Warning = 1,
        Error = 2,
        Debug = 3,
    }
}
