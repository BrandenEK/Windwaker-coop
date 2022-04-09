using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using System.Net;

namespace Windwaker_coop
{
    class Program
    {
        public static bool programSyncing = false;

        private static User currUser;

        private static Game[] games;
        public static Game currGame;
        public static Config config;

        static void Main(string[] args)
        {
            Console.Title = "The Legend of Zelda Coop Server/Client";
            Output.text("-The Legend of Zelda Coop-\n", ConsoleColor.Green);
            config = readConfigFile();

            games = loadGames();
            if (config.gameId < 0 || config.gameId >= games.Length)
            {
                Output.error("Invalid game id");
                EndProgram();
            }
            currGame = games[config.gameId];

            //Run in memory watcher mode
            if (config.runInWatcherMode)
            {
                Console.Title = $"{currGame.gameName} Memory Watcher";
                Output.text("Beginning in memory watcher mode!");
                programSyncing = true;
                Watcher watcher = new Watcher();
                watcher.beginWatching(config.syncDelay);

                string input = "";
                while (input != "stop")
                    input = Console.ReadLine().ToLower();
                EndProgram();
            }

            //Gets the type - server or client
            string type = askQuestion("Is this instance a server or a client? (s/c): ").ToLower();

            if (type == "s" || type == "server")
            {
                Console.Title = $"{currGame.gameName} Coop Server";

                //gets the ip address
                string ip = askForIp("\nEnter ip address of this machine: ");

                //Reset console
                Output.clear();
                Output.text($"-{currGame.gameName} Coop-\n", ConsoleColor.Green);

                //Creates new server object
                currUser = new Server(ip);
            }
            else if (type == "c" || type == "client")
            {
                Console.Title = $"{currGame.gameName} Coop Client";

                //gets the ip address
                string ip = askForIp("\nEnter ip address of the server: ");

                //gets the player name
                string playerName = askQuestion("\nEnter player name: ");
                if (playerName.Length < 1 || playerName.Length > 20 || playerName.Contains('~') || playerName.Contains(' '))
                {
                    Output.error("That player name is invalid - Must be between 1 and 20 characters and can not contain spaces or '~'");
                    EndProgram();
                }

                //Reset console
                Output.clear();
                Output.text($"-{currGame.gameName} Coop-\n", ConsoleColor.Green);

                //Creates new client object
                currUser = new Client(ip, playerName);
            }
            else
            {
                Output.error("The given input was neither 's' nor 'c'");
                EndProgram();
            }

            //Server/Client has begun operation - run the command loop
            commandLoop();
            EndProgram();
        }

        //Called in main to ask both server and client for ip
        private static string askForIp(string message)
        {
            //Find ipv4 addresses of machine
            string strHostName = Dns.GetHostName();
            Output.debug("Local Machine's Host Name: " + strHostName, 2);
            IPAddress[] addr = Dns.GetHostEntry(strHostName).AddressList;
            List<string> possibleIps = new List<string>();
            foreach (IPAddress ipAd in addr)
            {
                Output.debug(ipAd.ToString(), 2);
                if (ipAd.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    possibleIps.Add(ipAd.ToString());
                }
            }

            //Display potential ips
            if (possibleIps.Count > 0)
            {
                Output.text("\nUse IP address of this machine? (Enter id to select it)");
                for (int i = 0; i < possibleIps.Count; i++)
                    Output.text($"[{i}]: {possibleIps[i]}");
            }
            //Ask for ip address
            string ip = askQuestion(message);
            if (ip == "")
            {
                Output.error("You need to enter an ip address");
                EndProgram();
            }
            //Check if it is one of the provided potential ones
            byte ipId;
            if (byte.TryParse(ip, out ipId) && ipId < possibleIps.Count)
                return possibleIps[ipId];
            return ip;
        }

        static void commandLoop()
        {
            string command = "";
            while (command != "stop")
            {
                //Read command from user input and make sure it is valid
                command = Console.ReadLine().Trim();
                string[] words = command.Split(' ');
                if (words.Length < 1)
                {
                    Output.text("Enter a command.", ConsoleColor.Yellow);
                    continue;
                }

                //Seperate it into command and arguments
                string debugOuput = $"Processing command: '{words[0]}'";
                string[] args = new string[words.Length - 1];
                for (int i = 1; i < words.Length; i++)
                {
                    args[i - 1] = words[i];
                    debugOuput += " '" + words[i] + "'";
                }
                command = words[0];
                Output.debug(debugOuput, 1);

                //If the command is valid, send to user for processing and use it
                string response = currUser.processCommand(command, args);
                if (response != "")
                    Output.text(response, ConsoleColor.Yellow);
            }
        }

        //Ends the program
        public static void EndProgram()
        {
            Output.text("\nApplcation terminated.  Press any key to exit...", ConsoleColor.Gray);
            programSyncing = false;
            Console.ReadKey();
            Environment.Exit(0);
        }

        static string askQuestion(string question)
        {
            Output.text(question, ConsoleColor.White, false);
            return Console.ReadLine().Trim();
        }

        //~Reads from config.json and returns the config object
        private static Config readConfigFile()
        {
            string path = Environment.CurrentDirectory + "/config.json";
            Config c;

            if (File.Exists(path))
            {
                string configString = File.ReadAllText(path);
                c = JsonConvert.DeserializeObject<Config>(configString);
            }
            else
            {
                c = Config.getDefaultConfig();
                File.WriteAllText(path, JsonConvert.SerializeObject(c, Formatting.Indented));
            }
            return c;
        }

        private static Game[] loadGames()
        {
            Game[] games = new Game[]
            {
                new Windwaker(),
                new OcarinaOfTime(),
                new Zelda1(),
                new OracleOfSeasons()
            };
            Output.debug("Loading " + games.Length + " games", 1);
            return games;
        }
    }
}
