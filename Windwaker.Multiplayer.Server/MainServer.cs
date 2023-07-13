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

        private void SendIntro(string playerIp, byte response, ushort port)
        {
            byte[] portBytes = BitConverter.GetBytes(port);

            byte[] message = new byte[] { response, portBytes[0], portBytes[1] };
            Send(playerIp, message, MainType.Intro);
        }

        private void ReceiveIntro(string playerIp, byte[] message)
        {
            Console.WriteLine("Received intro");

            string roomName = "Test Room";
            string playerName = "Test Player";
            string game = "Windwaker";
            string password = null;

            byte response = Core.ValidatePlayerIntro(playerIp, roomName, playerName, game, password, out ushort port);
            SendIntro(playerIp, response, port);
        }
    }

    internal enum MainType : byte
    {
        Intro = 0,
    }
}
