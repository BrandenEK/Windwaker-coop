using Newtonsoft.Json;
using System;
using System.IO;

namespace Windwaker.Multiplayer.Server
{
    internal static class Core
    {
        public static Config ServerSettings { get; private set; }

        public static void Main()
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.Title = "Windwaker Multiplayer Server";
            ServerSettings = LoadConfig();

            var server = new Server();
            server.Start("*:" + ServerSettings.port);

            Console.ReadKey(true);
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