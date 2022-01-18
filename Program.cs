using System;
using System.Configuration;
using System.Collections.Generic;
using System.Net;

namespace Windwaker_coop
{
    class Program
    {
        public static bool programStopped = false;
        private static int debugLevel = 0;
        public static int syncDelay = 2500;
        public static int defaultPort = 25565;
        public static bool enableCheats = true;
        private static bool runInWatcherMode = false;

        private static User currUser;
        private static Cheater currCheater;

        private static Game[] games;
        public static Game currGame;
        public static string tempIp = "172.16.16.83";

        private static Dictionary<byte, ConsoleColor> colorIDs = new Dictionary<byte, ConsoleColor>()
        {
            { 0, ConsoleColor.Gray },
            { 1, ConsoleColor.White },
            { 2, ConsoleColor.Red },
            { 3, ConsoleColor.Green },
            { 4, ConsoleColor.Yellow },
            { 5, ConsoleColor.Cyan },
            { 6, ConsoleColor.Magenta },
            { 7, ConsoleColor.DarkMagenta },
            { 8, ConsoleColor.Blue }
        };

        static void Main(string[] args)
        {
            Console.Title = "The Legend of Zelda Coop Server/Client";
            setConsoleColor(3);
            Console.WriteLine("-The Legend of Zelda Coop-\n");
            readConfigFile();
            games = loadGames();
            currGame = games[0];

            //Run in memory watcher mode
            if (runInWatcherMode)
            {
                Console.Title = $"{currGame.gameName} Memory Watcher";
                setConsoleColor(1);
                Console.WriteLine("Beginning in memory watcher mode!");
                Watcher watcher = new Watcher();
                watcher.beginWatching(syncDelay);

                string input = "";
                while (input != "stop")
                    input = Console.ReadLine().ToLower();
                EndProgram();
            }

            //Gets the type - server or client
            string type = askQuestion("Is this instance a server or a client? (s/c): ").ToLower();
            string startText = "";

            if (type == "s" || type == "server")
            {
                Console.Title = $"{currGame.gameName} Coop Server";

                /*IP Test
                string strHostName = Dns.GetHostName();
                Console.WriteLine("Local Machine's Host Name: " + strHostName);
                IPHostEntry ipEntry = Dns.GetHostEntry(strHostName);
                IPAddress[] addr = ipEntry.AddressList;
                for (int i = 0; i < addr.Length; i++)
                    Console.WriteLine("IP Address {0}: {1} ", i, addr[i].ToString());*/

                //Gets the ip address
                string ip = askQuestion("Enter ip address of this machine: ");
                if (ip == "x")
                    ip = tempIp;

                //Creates new server object
                currUser = new Server(ip);
                startText = "Wait until everybody is ready, then press any key to start the server...";
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
                if (playerName.Length < 1 || playerName.Contains('~'))
                {
                    displayError("That player name is invalid");
                    EndProgram();
                }

                //Creates new client & cheater object
                currUser = new Client(ip, playerName);
                currCheater = new Cheater((Client)currUser);
                startText = "Wait until your game is started, then press any key to connect to the server...";
            }
            else
            {
                displayError("The given input was neither 's' nor 'c'");
                EndProgram();
            }

            //Begin host/client and start command loop
            setConsoleColor(3);
            Console.WriteLine(startText);
            Console.ReadKey();
            Console.Clear();
            setConsoleColor(3);
            Console.WriteLine($"-{currGame.gameName} Coop-\n");

            if (currUser != null)
            {
                currUser.Begin();
                commandLoop();
            }
            else
            {
                displayError("Something has gone terribly wrong");
            }

            EndProgram();
        }

        static void commandLoop()
        {
            string lastCommand = "";
            while (lastCommand != "stop")
            {
                setConsoleColor(5);
                bool validCommand = true;
                lastCommand = Console.ReadLine().ToLower().Trim();
                string[] words = lastCommand.Split(' ');

                //Displaying debug output
                string debugOuput = "Processing command:";
                foreach (string word in words)
                    debugOuput += " \"" + word + "\"";
                displayDebug(debugOuput, 2);

                //Processes multiword command
                if (words.Length > 1)
                {
                    lastCommand = words[0];
                    foreach (string word in words)
                    {
                        if (word == "")
                        {
                            validCommand = false;
                            break;
                        }
                    }
                }
                setConsoleColor(4);

                //If the command is valid, check which one and use it
                if (validCommand)
                {
                    if (currUser.GetType() == typeof(Client))
                    {
                        //Client commands
                        Client client = (Client)currUser;

                        switch (lastCommand)
                        {
                            case "help":
                                //Displays the available client commands
                                Console.WriteLine("Available client commands:\npause - temporarily disables syncing to and from the host\nunpause - resumes syncing to and from the host\n" +
                                    "stop - ends syncing and closes the application\nsay [message] - sends a message to everyone in the server\n" +
                                    "give [item] [number] - gives player the specified item (If cheats are enabled)\nping - tests the delay between client and server\n" +
                                    "help - lists available commands\n");
                                break;
                            case "pause":
                                //command not implemented yet
                                Console.WriteLine("command not implemented yet\n");
                                break;
                            case "unpause":
                                //command not implemented yet
                                Console.WriteLine("command not implemented yet\n");
                                break;

                            case "say":
                                //Takes in a message and sends it to everyone else in the game
                                if (words.Length > 1)
                                {
                                    string text = "";
                                    for (int i = 1; i < words.Length; i++)
                                        text += words[i] + " ";
                                    client.sendTextMessage(text);
                                }
                                else
                                    Console.WriteLine("Command 'say' takes at least 1 argument!\n");
                                break;
                            case "ping":
                                //Sends a test to the server to determine the delay
                                Console.WriteLine("Sending delay test!\n");
                                client.sendDelayTest();
                                break;
                            case "give":
                                //Gives the player a specified item
                                if (currCheater != null)
                                {
                                    string result = currCheater.processCommand(words);
                                    setConsoleColor(4);
                                    Console.WriteLine(result + "\n");
                                }
                                else
                                    displayError("Cheater object has not been initialized");
                                break;
                            case "stop": break;
                            default:
                                Console.WriteLine("Command '" + lastCommand + "' not valid.\n");
                                break;
                        }
                    }
                    else if (currUser.GetType() == typeof(Server))
                    {
                        //server commands
                        Server server = (Server)currUser;

                        switch (lastCommand)
                        {
                            case "help":
                                //Displays the available server commands
                                Console.WriteLine("Available server commands:\nlist - lists all of the currently connected players\n" +
                                    "reset - resets the host to default values\n" +
                                    "kick [IpPort] - kicks the speciifed IpPort from the game\nstop - ends syncing and closes the application\n" +
                                    "help - lists available commands\n");
                                break;
                            case "list":
                                //List the ip addresses in the server
                                Console.WriteLine("Connected players:");
                                foreach (string player in server.clientIps.Keys)
                                {
                                    Console.WriteLine(player);
                                }
                                Console.WriteLine();
                                break;
                            case "reset":
                                //resets the server to default values
                                server.setServerToDefault();
                                Console.WriteLine("Server data has been reset to default!\n");
                                server.sendNotification("Server data has been reset to default!", true);
                                break;

                            case "kick":
                                //kicks the inputted player's ipPort from the game
                                if (words.Length == 2)
                                {
                                    server.kickPlayer(words[1]);
                                    Console.WriteLine("IpPort " + words[1] + " has been kicked from the game!\n");
                                }
                                else
                                    Console.WriteLine("Command 'kick' takes 1 argument!\n");
                                break;
                            case "ban":
                                //command not implemented yet
                                Console.WriteLine("command not implemented yet\n");
                                break;
                            case "stop": break;
                            default:
                                Console.WriteLine("Command '" + lastCommand + "' not valid.\n");
                                break;
                        }
                    }
                    else
                    {
                        displayError("User is neither a server nor a client.  WTH");
                        EndProgram();
                    }
                }
                else
                    Console.WriteLine("Syntax error with inputted command.\n");
            }
            Console.WriteLine();
        }

        //Ends the program
        public static void EndProgram()
        {
            setConsoleColor(0);
            Console.WriteLine("Applcation terminated.  Press any key to exit...");
            programStopped = true;
            Console.ReadKey();
            Environment.Exit(0);
        }

        static string askQuestion(string question)
        {
            setConsoleColor(1);
            Console.Write(question);
            setConsoleColor(5);
            string output = Console.ReadLine();
            Console.WriteLine();
            return output;
        }

        private static void readConfigFile() //change to for loop once I add a new setting
        {
            string debug = ConfigurationManager.AppSettings["debugLevel"];
            string syncTime = ConfigurationManager.AppSettings["syncDelay"];
            string cheats = ConfigurationManager.AppSettings["enableCheats"];
            string watcher = ConfigurationManager.AppSettings["runInWatcherMode"];
            string defPort = ConfigurationManager.AppSettings["defaultPort"];

            if (!int.TryParse(debug, out debugLevel) || !int.TryParse(syncTime, out syncDelay) || !int.TryParse(defPort, out defaultPort) ||
                !bool.TryParse(cheats, out enableCheats) || !bool.TryParse(watcher, out runInWatcherMode))
            {
                displayError("Configuration file unable to be parsed");
                EndProgram();
            }
        }

        private static Game[] loadGames()
        {
            Game[] games = new Game[]
            {
                new Windwaker(),
                new OcarinaOfTime(),
                new Zelda1()
            };
            displayDebug("Loading " + games.Length + " games", 2);
            return games;
        }

        public static void setConsoleColor(byte colorId)
        {
            Console.ForegroundColor = colorIDs[colorId];
        }

        public static void displayDebug(string message, int level)
        {
            if (level <= debugLevel)
            {
                if (level > 2)
                    setConsoleColor(7);
                else if (level > 1)
                    setConsoleColor(6);
                else
                    setConsoleColor(1);
                Console.WriteLine(message);
            }
        }

        public static void displayError(string message)
        {
            setConsoleColor(2);
            Console.WriteLine("Error: {0}\n", message);
        }
    }
}
