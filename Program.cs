using System;
using System.IO;
using System.Collections.Generic;
using System.Configuration;

namespace Windwaker_Rammer
{
    class Program
    {
        public static bool programStopped = false;
        private static int debugLevel = 0;
        public static int syncDelay = 5000;

        private static Player currPlayer;

        static void Main(string[] args)
        {
            setConsoleColor(3);
            Console.WriteLine("-WindWaker Coop-\n");
            string invalidCharacters = "<>:\"/\\|?*";
            readConfigFile();

            //Ask for the server name & check for invalid characters
            string serverName = askQuestion("Enter server name: ");
            for (int i = 0; i < serverName.Length; i++)
            {
                if (invalidCharacters.Contains(serverName.Substring(i, 1)))
                {
                    displayError("These characters can not be used in the server name: " + invalidCharacters);
                    EndProgram();
                }
            }
            string serverDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\WW-coop\\servers\\" + serverName;

            //Create the server directory if it doesn't already exist
            setConsoleColor(4);
            if (File.Exists(serverDirectory + "\\host.txt") && File.Exists(serverDirectory + "\\messageLog.txt") && File.Exists(serverDirectory + "\\syncSettings.json"))
                Console.WriteLine("Joining a pre-existing server...");
            else
            {
                Console.WriteLine("Creating a brand new server...");
                Directory.CreateDirectory(serverDirectory);

                File.WriteAllText(serverDirectory + "\\messageLog.txt", "");
                File.WriteAllText(serverDirectory + "\\syncSettings.json", SyncSettings.getDefaultSettings());
                FileSaver fs = new FileSaver(serverDirectory);
                fs.SaveToFile(serverDirectory + "\\host.txt", fs.getDefaultValues());
            }

            //Ask for the player name & make sure it is valid
            string playerName = askQuestion("Enter player name: ").Trim();
            if (playerName.Length < 1)
            {
                displayError("That player name is invalid");
                EndProgram();
            }

            //Ask for the player number & make sure it is valid
            string num = askQuestion("Enter player number (Which instance of dolphin is this?): ");
            if (!(num.Length == 1 && "1234".Contains(num)))  //Change for max number of players
            {
                displayError("That is not a valid number 1-4");
                EndProgram();
            }
            int playerNumber = int.Parse(num);

            currPlayer = new Player(playerName, playerNumber, serverName, serverDirectory);

            //Begin looping and saving to a file
            setConsoleColor(3);
            Console.WriteLine("Wait until your game is started, then press any key to start syncing...");
            Console.ReadKey();
            Console.Clear();
            setConsoleColor(3);
            Console.WriteLine("-WindWaker Coop-\n");

            currPlayer.nm.SendNotifications(playerName + " has joined the game!", 1);
            currPlayer.beginSyncing();
            commandLoop();
            currPlayer.nm.SendNotifications(playerName + " has left the game!", 1);

            EndProgram();
        }

        static void commandLoop()
        {
            string lastCommand = "";
            while (lastCommand != "stop")
            {
                setConsoleColor(5);
                lastCommand = Console.ReadLine().ToLower();
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
                        "reset - resets the host to default values\nstop - ends syncing and closes the application\nhelp - lists available commands\n");

                }
                else if (lastCommand != "stop")
                {
                    Console.WriteLine("Command '" + lastCommand + "' not valid.\n");
                }
            }
            Console.WriteLine();
        }

        public static void EndProgram()
        {
            //End program
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
            string debug = ConfigurationManager.AppSettings[0];
            string syncTime = ConfigurationManager.AppSettings[1];
            if (!int.TryParse(debug, out debugLevel) || !int.TryParse(syncTime, out syncDelay))
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
            //Console.Beep();
        }
    }
}