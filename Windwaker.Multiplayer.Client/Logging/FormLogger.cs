using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Windwaker.Multiplayer.Client.Logging
{
    internal class FormLogger : ILogger
    {
        private readonly List<Message> _messages = new();
        private readonly RichTextBox _textBox;

        public FormLogger(RichTextBox textbox)
        {
            _textBox = textbox;
        }

        public void Info(object message)
        {
            _messages.Add(new Message(message.ToString(), LogLevel.Info));
            UpdateTextBox();
        }

        public void Warning(object message)
        {
            _messages.Add(new Message(message.ToString(), LogLevel.Warning));
            UpdateTextBox();
        }

        public void Error(object message)
        {
            _messages.Add(new Message(message.ToString(), LogLevel.Error));
            UpdateTextBox();
        }

        private void UpdateTextBox()
        {
            _textBox.Clear();
            
            foreach (var message in _messages)
            {
                _textBox.SelectionStart = _textBox.TextLength;
                _textBox.SelectionLength = 0;
                _textBox.SelectionColor = GetMessageColor(message);
                _textBox.AppendText($"[{message.Time:HH:mm:ss}] {message.Content}{Environment.NewLine}");
            }
        }

        private Color GetMessageColor(Message message)
        {
            return message.Level switch
            {
                LogLevel.Info => Color.White,
                LogLevel.Warning => Color.Yellow,
                LogLevel.Error => Color.DarkRed,
                _ => Color.Magenta
            };
        }
    }
}
