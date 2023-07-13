using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Windwaker.Multiplayer.Server
{
    internal class Room
    {
        private readonly string _game;
        private readonly string _password;

        private readonly AbstractServer<AbstractType> gameServer;

        public Room(string game, string password)
        {
            _game = game;
            _password = password;
            gameServer = null;
            // Start server at first available port
        }
    }
}
