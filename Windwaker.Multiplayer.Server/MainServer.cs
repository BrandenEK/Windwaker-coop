using System;
using System.Collections.Generic;

namespace Windwaker.Multiplayer.Server
{
    internal class MainServer : AbstractServer<MainType>
    {
        public MainServer()
        {
            Initialize(new Dictionary<MainType, Action<string, byte[]>>()
            {
                { MainType.Intro, ReceiveIntro },
            });
        }

        // Connection

        protected override void ClientConnected(string clientIp)
        {
            Console.WriteLine("client connected");
        }

        protected override void ClientDisconnected(string clientIp)
        {

        }

        // Intro

        private void SendIntro(string playerIp)
        {
            Send(playerIp, new byte[] { 2, 5, 5, 6, 5 }, MainType.Intro);
        }

        private void ReceiveIntro(string playerIp, byte[] message)
        {
            Console.WriteLine(message.Length);
            SendIntro(playerIp);
        }
    }

    internal enum MainType : byte
    {
        Intro = 0,
    }
}
