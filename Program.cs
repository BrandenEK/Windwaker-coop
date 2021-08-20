using System;
using System.Configuration;

namespace Windwaker_coop
{
    class Program
    {
        public static bool programStopped = false;
        private static int debugLevel = 0;
        public static int syncDelay = 5000;
        public static bool enableCheats = true;

        private static User currUser;
        private static Cheater currCheater;

        public static string tempIp = "172.16.16.60";


        static void Main(string[] args)
        {
            Console.Title = "Windwaker Coop Server/Client";
            setConsoleColor(3);
            Console.WriteLine("-WindWaker Coop-\n");
            string invalidCharacters = "<>:\"/\\|?*";
            readConfigFile();

            string ip = askQuestion("Enter server ip address: ");
            if (ip == "x")
                ip = tempIp;

            string playerName = askQuestion("Enter player name: ").Trim();
            if (playerName.Length < 1 || playerName.Contains('~'))
            {
                displayError("That player name is invalid");
                EndProgram();
            }

            string startText = "";
            if (playerName == "host")
            {
                Console.Title = "Windwaker Coop Server";
                currUser = new Server(ip, 25565);
                startText = "Wait until everybody is ready, then press any key to start the server...";
            }
            else
            {
                Console.Title = "Windwaker Coop Client";
                currUser = new Client(ip, 25565, playerName);
                currCheater = new Cheater((Client)currUser);
                startText = "Wait until your game is started, then press any key to connect to the server...";
            }

            //Begin host/client and start command loop
            setConsoleColor(3);
            Console.WriteLine(startText);
            Console.ReadKey();
            Console.Clear();
            setConsoleColor(3);
            Console.WriteLine("-WindWaker Coop-\n");

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

            /*
            //Ask for the player number & make sure it is valid
            string num = askQuestion("Enter player number (Which instance of dolphin is this?): ");
            if (!(num.Length == 1 && "1234".Contains(num)))  //Change for max number of players
            {
                displayError("That is not a valid number 1-4");
                EndProgram();
            }
            int playerNumber = int.Parse(num);
            */
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

                        if (lastCommand == "help")
                        {
                            Console.WriteLine("Available commands:\npause - temporarily disables syncing to and from the host\nunpause - resumes syncing to and from the host\n" +
                                "stop - ends syncing and closes the application\nsay [message] - sends a message to everyone in the server\n" +
                                "give [item] [number] - gives player the specified item (If cheats are enabled)\nhelp - lists available commands\n");
                        }
                        else if (lastCommand == "pause")
                        {
                            //command not implemented yet
                        }
                        else if (lastCommand == "unpause")
                        {
                            //command not implemented yet
                        }
                        else if (lastCommand == "say")
                        {
                            //takes in a message and sends it to everyone else in the game
                            if (words.Length > 1)
                            {
                                string text = "";
                                for (int i = 1; i < words.Length; i++)
                                    text += words[i] + " ";
                                client.sendTextMessage(text);
                            }
                            else
                                Console.WriteLine("Command 'say' takes at least 1 argument!\n");
                        }
                        else if (lastCommand == "give")
                        {
                            //gives the player a specified item
                            if (currCheater != null)
                            {
                                string result = currCheater.processCommand(words);
                                Console.WriteLine(result + "\n");
                            }
                            else
                                displayError("Cheater object has not been initialized");
                        }
                        else if (lastCommand != "stop")
                        {
                            Console.WriteLine("Command '" + lastCommand + "' not valid.\n");
                        }
                    }
                    else if (currUser.GetType() == typeof(Server))
                    {
                        //server commands
                        Server server = (Server)currUser;

                        if (lastCommand == "reset")
                        {
                            //resets the values stored in the host file - do this while not currently in a game
                            server.setServerToDefault();
                            Console.WriteLine("Server data has been reset to default!\n");
                            server.sendNotification("Server data has been reset to default!", true);
                        }
                        else if (lastCommand == "kick")
                        {
                            //kicks the inputted player ip address from the game
                            if (words.Length == 2)
                            {
                                server.kickPlayer(words[1]);
                                Console.WriteLine("Ip '" + words[1] + "' has been kicked from the game!\n");
                            }
                            else
                                Console.WriteLine("Command 'kick' takes 1 argument!\n");
                        }
                        else if (lastCommand == "ban")
                        {
                            //command not implemented yet
                        }
                        else if (lastCommand == "help")
                        {
                            Console.WriteLine("Available server commands:\nreset - resets the host to default values\nhelp - lists available commands\n");
                        }
                        else if (lastCommand != "stop")
                        {
                            Console.WriteLine("Command '" + lastCommand + "' not valid.\n");
                        }
                    }
                    else
                    {
                        displayError("User is neither a server nor a client.  Wth");
                        EndProgram();
                    }
                }
                else
                    Console.WriteLine("Syntax error with inputted command.\n");
                
                /*if (lastCommand == "pause")
                {
                    //stops sync loop until unpause is typed if sync is running
                    if (!programStopped)
                    {
                        programStopped = true;
                        Console.WriteLine("Sync has now been paused.\n");
                        currPlayer.nm.SendNotifications(currPlayer.playerName + " has paused their syncing!", 1);
                    }
                    else
                        Console.WriteLine("Sync is already paused!\n");
                }
                else if (lastCommand == "unpause")
                {
                    //starts the sync loop again if it is already paused
                    if (programStopped)
                    {
                        programStopped = false;
                        Console.WriteLine("Sync has now been resumed.\n");
                        currPlayer.nm.SendNotifications(currPlayer.playerName + " has resumed their syncing!", 1);
                        currPlayer.beginSyncing();
                    }
                    else
                        Console.WriteLine("Sync is already active!\n");
                }*/
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

            if (!int.TryParse(debug, out debugLevel) || !int.TryParse(syncTime, out syncDelay) || !bool.TryParse(cheats, out enableCheats))
            {
                displayError("Configuration file unable to be parsed");
                EndProgram();
            }
        }

        public static void setConsoleColor(int colorId)
        {
            if (colorId == 0)
                Console.ForegroundColor = ConsoleColor.Gray;
            else if (colorId == 1)
                Console.ForegroundColor = ConsoleColor.White;
            else if (colorId == 2)
                Console.ForegroundColor = ConsoleColor.Red;
            else if (colorId == 3)
                Console.ForegroundColor = ConsoleColor.Green;
            else if (colorId == 4)
                Console.ForegroundColor = ConsoleColor.Yellow;
            else if (colorId == 5)
                Console.ForegroundColor = ConsoleColor.Cyan;
            else if (colorId == 6)
                Console.ForegroundColor = ConsoleColor.Magenta;
            else if (colorId == 7)
                Console.ForegroundColor = ConsoleColor.DarkMagenta;
            else if (colorId == 8)
                Console.ForegroundColor = ConsoleColor.Blue;
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
