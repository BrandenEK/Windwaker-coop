using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace Windwaker.Multiplayer.Server
{
    internal static class Core
    {
        public static Config ServerSettings { get; private set; }

        private static readonly Server _server = new();

        public static void Main()
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.Title = "Windwaker Multiplayer Server";
            ServerSettings = LoadConfig();

            _server.Start("*:" + ServerSettings.port);

            while (true)
            {
                // Infinite loop to keep the server alive
                Console.ReadKey(true);
            }
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