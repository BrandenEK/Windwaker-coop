using System;
using System.Collections.Generic;

namespace Windwaker.Multiplayer.Server
{
    internal class MainServer : AbstractServer<MainType>
    {
        public override void Start(string ipPort)
        {
            Dictionary<MainType, Action<string, byte[]>> receivers = new()
            {
                { MainType.Intro, ReceiveIntro },
            };
            Initialize(ipPort, receivers);
        }

        // Connection

        protected override void ClientConnected(string playerIp)
        {
            Console.WriteLine("client connected");
        }

        protected override void ClientDisconnected(string playerIp)
        {

        }

        // Position

        private void SendIntro(string playerIp)
        {

        }

        private void ReceiveIntro(string playerIp, byte[] message)
        {
            Console.WriteLine(message.Length);
        }
    }

    internal enum MainType
    {
        Intro = 0,
    }
}
