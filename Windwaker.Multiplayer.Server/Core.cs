using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace Windwaker.Multiplayer.Server
{
    internal static class Core
    {
        public static Config ServerSettings { get; private set; }

        private static readonly MainServer mainServer = new();

        private static readonly Dictionary<string, Room> rooms = new();

        public static void Main()
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.Title = "Windwaker Multiplayer Server";
            ServerSettings = LoadConfig();

            mainServer.Start("*:" + ServerSettings.port);

            while (true)
            {
                // Infinite loop to keep the server alive
                Console.ReadKey(true);
            }
        }

        /// <summary>
        /// Takes in the data sent from an initial client connection, and either creates a new room for the player, adds them to an existing room, or rejects them
        /// </summary>
        public static byte ValidatePlayerIntro(string playerIp, string roomName, string playerName, string game, string password, out ushort port)
        {
            if (rooms.TryGetValue(roomName, out Room existingRoom))
            {
                // Validate player info before adding them to the room
                existingRoom.AllowPlayer(playerIp);
                Console.WriteLine($"Adding '{playerName}' to existing room '{roomName}' on port 8990");
            }
            else
            {
                // Create a new room for this player
                string tempIp = "*:8990";
                Room newRoom = new Room(tempIp, game, password);
                newRoom.AllowPlayer(playerIp);
                rooms[roomName] = newRoom;
                Console.WriteLine($"Creating new room '{roomName}' on port 8990 for '{playerName}'");
            }

            // Regardless, send back a response and possibly a port
            port = 8990;
            return 0;
        }



        /// <summary>
        /// Loads the server settings from the file if it exists, or creates a new file with default settings
        /// </summary>
        private static Config LoadConfig()
        {
            string path = Environment.CurrentDirectory + "/multiplayer.cfg";
            if (File.Exists(path))
            {
                var cfg = JsonConvert.DeserializeObject<Config>(File.ReadAllText(path));
                return cfg;
            }
            else
            {
                var cfg = new Config();
                File.WriteAllText(path, JsonConvert.SerializeObject(cfg, Formatting.Indented));
                return cfg;
            }
        }
    }
}