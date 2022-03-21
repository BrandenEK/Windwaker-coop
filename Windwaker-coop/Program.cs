using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace Windwaker_coop
{
    class Program
    {
        public static bool programSyncing = false;

        private static User currUser;

        private static Game[] games;
        public static Game currGame;
        public static Config config;
        public static string tempIp = "172.16.16.75";

        static void Main(string[] args)
        {
            Console.Title = "The Legend of Zelda Coop Server/Client";
            Output.text("-The Legend of Zelda Coop-\n", ConsoleColor.Green);
            config = readConfigFile();
            games = loadGames();
            currGame = games[0];

            //Run in memory watcher mode
            if (config.runInWatcherMode)
            {
                Console.Title = $"{currGame.gameName} Memory Watcher";
                Output.text("Beginning in memory watcher mode!");
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

                //Gets the ip address
                string ip = askQuestion("Enter ip address of this machine: ");
                if (ip == "x")
                    ip = tempIp;

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
                string ip = askQuestion("Enter ip address of the server: ");
                if (ip == "x")
                    ip = tempIp;

                //gets the player name
                string playerName = askQuestion("Enter player name: ").Trim();
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
            return Console.ReadLine();
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
                c = new Config(0, 2500, 25565, true, false);
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
                new Zelda1()
            };
            Output.debug("Loading " + games.Length + " games", 1);
            return games;
        }
    }
}
