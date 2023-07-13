using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Windwaker.Multiplayer.Client
{
    internal class MainClient : AbstractClient<MainType>
    {
        public MainClient()
        {
            Initialize(new Dictionary<MainType, Action<byte[]>>()
            {
                { MainType.Intro, ReceiveIntro },
            });
        }

        // Connection

        protected override void ServerConnected(string serverIp)
        {
            MainForm.Log($"Connected to {serverIp}");
            //MainForm.UpdateUI();
            SendIntro();
        }

        protected override void ServerDisconnected(string serverIp)
        {
            MainForm.Log($"Disconnected from {serverIp}");
            //MainForm.UpdateUI();
        }

        // Intro

        private void SendIntro()
        {
            Send(new byte[] { 1, 2, 3 }, MainType.Intro);
        }

        private void ReceiveIntro(byte[] message)
        {
            MainForm.Log("Intro: " + message.Length);

            byte response = message[0];
            ushort port = BitConverter.ToUInt16(message, 1);

            if (response == 0)
            {
                MainForm.ReceiveGamePort(port);
            }
            else
            {
                MainForm.Log("Connection refused");
            }

            Disconnect();
        }
    }

    internal enum MainType : byte
    {
        Intro = 0,
    }
}
