using Newtonsoft.Json;
using System;
using System.IO;

namespace Windwaker.Multiplayer.Server
{
    public class Core
    {
        public static Config ServerSettings { get; private set; }

        public static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Program started");

            ServerSettings = LoadConfig();
            Console.WriteLine("Port: " + ServerSettings.port);

            var server = new Server();

            Console.ReadKey(true);
        }

        /// <summary>
        /// Loads the server settings from the file if it exists, or creates a new file with default settings
        /// </summary>
        /// <returns>The configuration object</returns>
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