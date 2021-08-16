using System;
using System.IO;
using System.Configuration;

namespace Windwaker_coop
{
    class Program
    {
        public static bool programStopped = false;
        private static int debugLevel = 0;
        public static int syncDelay = 5000;
        public static bool enableCheats = true;

        private static Client currClient;
        private static Server currServer;
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
                currServer = new Server(ip, 25565);
                startText = "Wait until everybody is ready, then press any key to start the server...";
            }
            else
            {
                Console.Title = "Windwaker Coop Client";
                currClient = new Client(ip, 25565, playerName);
                currCheater = new Cheater(null); //set to client once cheater is set up
                startText = "Wait until your game is started, then press any key to connect to the server...";
            }

            //Begin host/client and start command loop
            setConsoleColor(3);
            Console.WriteLine(startText);
            Console.ReadKey();
            Console.Clear();
            setConsoleColor(3);
            Console.WriteLine("-WindWaker Coop-\n");

            if (currClient != null)
            {
                currClient.Connect();
                currClient.beginSyncing(syncDelay);
                commandLoop(1);
            }
            else if (currServer != null)
            {
                currServer.Start();
                commandLoop(0);
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

        static void commandLoop(int user)
        {
            while (true)
                Console.ReadKey();
            /*
            string lastCommand = "";
            while (lastCommand != "stop")
            {
                setConsoleColor(5);
                lastCommand = Console.ReadLine().ToLower().Trim();
                setConsoleColor(4);

                if (lastCommand == "pause")
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
                }
                else if (lastCommand == "reset")
                {
                    //resets the values stored in the host file - do this while not currently in a game
                    Console.WriteLine("Server values have been reset to default.\n");
                    currPlayer.fs.SaveToFile(currPlayer.serverDirectory + "\\host.txt", currPlayer.fs.getDefaultValues());
                    currPlayer.nm.SendNotifications(currPlayer.playerName + " has reset the server to default values!", 1);
                }
                else if (lastCommand == "help")
                {
                    Console.WriteLine("Available commands:\npause - temporarily disables syncing to and from the host\nunpause - resumes syncing to and from the host\n" +
                        "reset - resets the host to default values\nstop - ends syncing and closes the application\ngive [item] [number] - gives player the specified item (If cheats are enabled)\nhelp - lists available commands\n");

                }
                else if (lastCommand.Length >= 4 && lastCommand.Substring(0, 4) == "give")
                {
                    //send this data to the cheats object & lets it process the command
                    setConsoleColor(4);
                    if (enableCheats)
                        Console.WriteLine(currCheater.processCommand(lastCommand));
                    else
                        Console.WriteLine("Cheats are disabled!");
                }
                else if (lastCommand != "stop")
                {
                    Console.WriteLine("Command '" + lastCommand + "' not valid.\n");
                }
            }
            Console.WriteLine();
            */
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
