﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Windwaker.Multiplayer.Client.Logging
{
    internal class FormLogger : ILogger
    {
        private readonly List<Message> _messages = new();
        private readonly RichTextBox _textBox;
        private readonly bool _displayDebug;

        public FormLogger(RichTextBox textbox, bool displayDebug)
        {
            _textBox = textbox;
            _displayDebug = displayDebug;
        }

        public void Info(object message)
        {
            DisplayMessage(new Message(message.ToString(), LogLevel.Info));
        }

        public void Warning(object message)
        {
            DisplayMessage(new Message(message.ToString(), LogLevel.Warning));
        }

        public void Error(object message)
        {
            DisplayMessage(new Message(message.ToString(), LogLevel.Error));
        }

        public void Debug(object message)
        {
            if (_displayDebug)
                DisplayMessage(new Message(message.ToString(), LogLevel.Debug));
        }

        private void DisplayMessage(Message message)
        {
            _textBox.SelectionStart = _textBox.TextLength;
            _textBox.SelectionLength = 0;
            _textBox.SelectionColor = GetMessageColor(message);
            _textBox.AppendText($"[{message.Time:HH:mm:ss}] {message.Content}{Environment.NewLine}");
            _textBox.ScrollToCaret();
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
                _textBox.ScrollToCaret();
            }
        }

        private Color GetMessageColor(Message message)
        {
            return message.Level switch
            {
                LogLevel.Info => Color.White,
                LogLevel.Warning => Color.Yellow,
                LogLevel.Error => Color.DarkRed,
                LogLevel.Debug => Color.DarkBlue,
                _ => Color.Magenta
            };
        }
    }
}
