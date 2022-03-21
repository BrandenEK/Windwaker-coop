﻿using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace Windwaker_coop
{
    class Program
    {
        public static bool programSyncing = false;

        private static User currUser;
        private static Cheater currCheater;

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

                //Creates new client & cheater objects
                Client c = new Client(ip, playerName);
                currUser = c;
                currCheater = new Cheater(c);
            }
            else
            {
                Output.error("The given input was neither 's' nor 'c'");
                EndProgram();
            }

            //Server is already started & client is already connected - start command loop
            if (currUser != null)
            {
                commandLoop();
            }
            else
            {
                Output.error("Something has gone terribly wrong");
            }

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


                //If the command is valid, check which one and use it
                string response = "";

                if (currUser.GetType() == typeof(Client))
                {
                    //Client commands
                    Client client = (Client)currUser;

                    switch (command)
                    {
                        case "help":
                            //Displays the available client commands
                            response = "Available client commands:\npause - temporarily disables syncing to and from the host\nunpause - resumes syncing to and from the host\n" +
                                "stop - ends syncing and closes the application\nsay [message] - sends a message to everyone in the server\n" +
                                "give [item] [number] - gives player the specified item (If cheats are enabled)\nping - tests the delay between client and server\n" +
                                "help - lists available commands";
                            break;
                        case "pause":
                            //command not implemented yet
                            response = "command not implemented yet";
                            break;
                        case "unpause":
                            //command not implemented yet
                            response = "command not implemented yet";
                            break;

                        case "say":
                            //Takes in a message and sends it to everyone else in the game
                            if (words.Length > 1)
                            {
                                string text = "";
                                for (int i = 1; i < words.Length; i++)
                                    text += words[i] + " ";
                                client.sendTextMessage(text);
                                response = "Message sent";
                            }
                            else
                                response = "Command 'say' takes at least 1 argument!";
                            break;
                        case "ping":
                            //Sends a test to the server to determine the delay
                            response = "Sending delay test!";
                            client.sendDelayTest();
                            break;
                        case "give":
                            //Gives the player a specified item
                            if (currCheater != null)
                            {
                                response = currCheater.processCommand(words);
                            }
                            else
                                Output.error("Cheater object has not been initialized");
                            break;
                        case "stop": break;
                        default:
                            response = "Command '" + command + "' not valid.";
                            break;
                    }
                }
                else if (currUser.GetType() == typeof(Server))
                {
                    //server commands
                    Server server = (Server)currUser;

                    switch (command)
                    {
                        case "help":
                            //Displays the available server commands
                            response = "Available server commands:\nlist - lists all of the currently connected players\n" +
                                "reset - resets the host to default values\n" +
                                "kick [type] [Name or IpPort] - kicks the speciifed Name or IpPort from the game\nstop - ends syncing and closes the application\n" +
                                "help - lists available commands";
                            break;
                        case "list":
                            //List the ip addresses in the server
                            response = "Connected players:\n";
                            foreach (string ip in server.clientIps.Keys)
                            {
                                response += $"{server.clientIps[ip].name} ({ip})\n";
                            }
                            if (server.clientIps.Count < 1)
                                response += "none\n";
                            response = response.Substring(0, response.Length - 1);
                            break;
                        case "reset":
                            //resets the server to default values
                            server.setServerToDefault();
                            response = "Server data has been reset to default!";
                            server.sendNotification("Server data has been reset to default!", true);
                            break;
                        case "kick":
                            //kicks the inputted player's ipPort or name from the game
                            if (words.Length == 3)
                            {
                                if (words[1] == "name" || words[1] == "n")
                                {
                                    //Change to simple lookup once names are individualized
                                    int numFound = 0;
                                    foreach (string ip in server.clientIps.Keys)
                                    {
                                        if (server.clientIps[ip].name == words[2])
                                        {
                                            server.kickPlayer(ip);
                                            response += "Player '" + words[2] + "' has been kicked from the game!";
                                            numFound++;
                                        }
                                    }
                                    if (numFound == 0)
                                        response = "Player '" + words[2] + "' does not exist in the game!";
                                }
                                else if (words[1] == "ip" || words[1] == "i")
                                {
                                    if (server.clientIps.ContainsKey(words[2]))
                                    {
                                        server.kickPlayer(words[2]);
                                        response = "IpPort '" + words[2] + "' has been kicked from the game!";
                                    }
                                    else
                                        response = "IpPort '" + words[2] + "' does not exist in the game!";
                                }
                                else
                                {
                                    response = "Invalid type.  Must be either 'name' or 'ip'";
                                }
                            }
                            else
                                response = "Command 'kick' takes 2 arguments!";
                            break;
                        case "ban":
                            //command not implemented yet
                            response = "command not implemented yet";
                            break;
                        case "stop": break;
                        default:
                            response = "Command '" + command + "' not valid.";
                            break;
                    }
                }
                else
                {
                    Output.error("User is neither a server nor a client.  WTH");
                    EndProgram();
                }

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
