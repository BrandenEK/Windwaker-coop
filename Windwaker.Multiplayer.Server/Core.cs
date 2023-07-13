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
            if (!rooms.TryGetValue(roomName, out Room existingRoom))
            {
                // Create a new room for this player
                Room newRoom = new Room("*:0", game, password);
                newRoom.QueuePlayer(playerIp, playerName);
                rooms[roomName] = newRoom;
                port = newRoom.Port;
                Console.WriteLine($"Creating new room '{roomName}' on port {port} for '{playerName}'");
                return 0;
            }

            // Validate player info before adding them to the room
            port = 0;

            // Ensure the password is correct
            if (!string.IsNullOrEmpty(existingRoom.Password) && password != existingRoom.Password)
            {
                Console.WriteLine("Player connection rejected: Incorrect password");
                return 1;
            }

            // Ensure that the room doesn't already have the max number of players
            if (existingRoom.AllPlayers.Count >= ServerSettings.maxPlayers)
            {
                Console.WriteLine("Player connection rejected: Player limit reached");
                return 2;
            }

            // Ensure there are no duplicate ips
            if (existingRoom.IsIpTaken(playerName))
            {
                Console.WriteLine("Player connection rejected: Duplicate ip address");
                return 3;
            }

            // Ensure there are no duplicate names
            if (existingRoom.IsNameTaken(playerName))
            {
                Console.WriteLine("Player connection rejected: Duplicate name");
                return 4;
            }

            existingRoom.QueuePlayer(playerIp, playerName);
            port = existingRoom.Port;
            Console.WriteLine($"Adding '{playerName}' to existing room '{roomName}' on port {port}");
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