using Newtonsoft.Json;
using System;
using System.IO;

namespace Windwaker.Multiplayer.Server
{
    public class Core
    {
        public static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Program started");

            Config cfg = LoadConfig();
            Console.WriteLine("Port: " + cfg.port);

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
                Config cfg = JsonConvert.DeserializeObject<Config>(File.ReadAllText(path));
                return cfg;
            }
            else
            {
                Config cfg = new Config();
                File.WriteAllText(path, JsonConvert.SerializeObject(cfg, Formatting.Indented));
                return cfg;
            }
        }
    }
}