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
            MainForm.Log($"Established connection with Main server");
            SendIntro();
        }

        protected override void ServerDisconnected(string serverIp)
        {

        }

        // Intro

        private void SendIntro()
        {
            // Replace with real intro data (Player name, game name, room id, password)
            Send(new byte[] { 1, 2, 3 }, MainType.Intro);
        }

        private void ReceiveIntro(byte[] message)
        {
            byte response = message[0];
            ushort port = BitConverter.ToUInt16(message, 1);

            if (response == 0)
            {
                MainForm.Log($"Connection to game server was approved ({port})");
                MainForm.ReceiveGamePort(port);
            }
            else
            {
                MainForm.Log("Connection to game server was refused");
            }

            Disconnect();
        }
    }

    internal enum MainType : byte
    {
        Intro = 0,
    }
}
