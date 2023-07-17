using System;

namespace Windwaker.Multiplayer.Server
{
    internal abstract class GameServer<T> : AbstractServer<T> where T : Enum
    {
        protected readonly Room _room;

        public GameServer(Room room)
        {
            _room = room;
        }

        protected override void ClientConnected(string clientIp)
        {
            if (!_room.AddPlayer(clientIp))
                DisconnectClient(clientIp);
        }

        protected override void ClientDisconnected(string clientIp)
        {
            _room.RemovePlayer(clientIp);
        }
    }
}
