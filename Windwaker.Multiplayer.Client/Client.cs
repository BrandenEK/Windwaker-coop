﻿using Microsoft.VisualBasic.Logging;
using SuperSimpleTcp;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Numerics;
using System.Text;

namespace Windwaker.Multiplayer.Client
{
    internal class Client
    {
        private SimpleTcpClient _client;

        public bool IsConnected => _client != null && _client.IsConnected;

        /// <summary>
        /// Attempts to connect to a server at the specified ip port
        /// </summary>
        public bool Connect(string ip, int port)
        {
            try
            {
                _client = new SimpleTcpClient(ip, port);

                _client.Events.Connected += OnServerConnected;
                _client.Events.Disconnected += OnServerDisconnected;
                _client.Events.DataReceived += Receive;

                _client.Connect();
            }
            catch (Exception e) when (e is SocketException || e is TimeoutException)
            {
                MainForm.Log($"Failed to connect to {ip}:{port}");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Disconnects from the current server
        /// </summary>
        public void Disconnect()
        {
            _client?.Disconnect();
            _client = null;
        }

        /// <summary>
        /// Called whenever the client connects to the server
        /// </summary>
        private void OnServerConnected(object sender, ConnectionEventArgs e)
        {
            MainForm.Log($"Established connection with server");
            SendIntro(MainForm.Settings.PlayerName, MainForm.Settings.GameName, MainForm.Settings.Password);
        }

        /// <summary>
        /// Called whenever the client disconnects from the server
        /// </summary>
        private void OnServerDisconnected(object sender, ConnectionEventArgs e)
        {
            MainForm.UpdateUI();
        }

        /// <summary>
        /// Sends a message to the server
        /// </summary>
        private void Send(byte[] message, NetworkType type)
        {
            if (message == null || message.Length == 0 || !IsConnected)
                return;

            List<byte> bytes = new();
            bytes.AddRange(BitConverter.GetBytes((ushort)message.Length));
            bytes.Add((byte)type);
            bytes.AddRange(message);

            try
            {
                _client.Send(bytes.ToArray());
            }
            catch (Exception)
            {
                Console.WriteLine($"*** Couldn't send data to server ***");
            }
        }

        /// <summary>
        /// Receives a message from the server and calls the specific receive method to process it
        /// </summary>
        private void Receive(object sender, DataReceivedEventArgs e)
        {
            byte[] data = e.Data.ToArray();
            int startIdx = 0;

            while (startIdx < data.Length - 3)
            {
                ushort length = BitConverter.ToUInt16(data, startIdx);
                NetworkType type = (NetworkType)data[startIdx + 2];
                byte[] message = data[(startIdx + 3)..(startIdx + 3 + length)];

                switch (type)
                {
                    case NetworkType.Intro: ReceiveIntro(message); break;
                    case NetworkType.Scene: ReceiveScene(message); break;
                    case NetworkType.Progress: ReceiveProgress(message); break;
                }
                startIdx += 3 + length;
            }

            if (startIdx != data.Length)
                MainForm.Log("*** Received data was formatted incorrectly ***");
        }

        // Intro

        public void SendIntro(string player, string game, string password)
        {
            var bytes = new List<byte>();

            bytes.AddRange(SerializeString(player));
            bytes.AddRange(SerializeString(game));
            bytes.AddRange(SerializeString(password));

            Send(bytes.ToArray(), NetworkType.Intro);
        }

        private void ReceiveIntro(byte[] message)
        {
            byte response = message[0];

            if (response == 200)
            {
                MainForm.Log($"Connection to server was approved");
                MainForm.UpdateUI();
                // Start sync
            }
            else
            {
                switch (response)
                {
                    case 101: MainForm.Log("Connection refused: Incorrect password"); break;
                    case 102: MainForm.Log("Connection refused: Incorrect game"); break;
                    case 103: MainForm.Log("Connection refused: Player limit reached"); break;
                    case 104: MainForm.Log("Connection refused: Duplicate ip address"); break;
                    case 105: MainForm.Log("Connection refused: Duplicate name"); break;
                }

                Disconnect();
            }
        }

        // Scene

        public void SendScene(string scene)
        {

        }

        private void ReceiveScene(byte[] message)
        {

        }

        // Progress

        public void SendProgress(ProgressType type, string progressId, byte progressValue)
        {

        }

        private void ReceiveProgress(byte[] message)
        {

        }

        // Helpers

        private byte[] SerializeString(string str)
        {
            byte[] stringBytes = Encoding.UTF8.GetBytes(str);
            byte[] outputBytes = new byte[str.Length + 1];

            outputBytes[0] = (byte)str.Length;
            for (int i = 0; i < stringBytes.Length; i++)
            {
                outputBytes[i + 1] = stringBytes[i];
            }

            return outputBytes;
        }

        private string DeserializeString(byte[] bytes, int start, out byte length)
        {
            length = (byte)(bytes[start] + 1);

            return length == 1 ? null : Encoding.UTF8.GetString(bytes, start + 1, length - 1);
        }
    }
}
