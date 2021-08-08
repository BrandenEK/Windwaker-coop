using System;
using System.IO;

namespace Windwaker_Rammer
{
    class NotificationManager
    {
        private string logPath;
        private int lastReadLine;

        public NotificationManager(string serverDirectory, string playerName)
        {
            logPath = serverDirectory + "\\messageLog.txt";
            if (getMessageStrings() != null)
                lastReadLine = File.ReadAllLines(logPath).Length;
        }

        public void SendNotifications(string message, int number)
        {
            File.AppendAllText(logPath, message + "\n");
            lastReadLine += number;
        }

        public void ReadNotifications()
        {
            Program.displayDebug("Current number of lines read: " + lastReadLine, 3);
            string[] allLines = getMessageStrings();
            if (allLines != null && allLines.Length >= lastReadLine)
            {
                Program.setConsoleColor(3);
                while(lastReadLine < allLines.Length)
                {
                    Console.WriteLine(allLines[lastReadLine]);
                    lastReadLine++;
                }
            }
            else
            {
                Program.displayError("Something is wrong with the notification file!");
            }
        }

        public string getNotificationText(string playerName, string item)
        {
            string[] strings = item.Split('*', 2);
            item = strings[0]; int formatId = -2;
            int.TryParse(strings[1], out formatId);
            string output = "";

            if (formatId == 0)
                output = "obtained the " + item;
            else if (formatId == 1)
                output = "obtained a " + item;
            else if (formatId == 2)
                output = "obtained " + item;
            else if (formatId == 3)
                output = "learned the " + item;
            else if (formatId == 4)
                output = "deciphered " + item;
            else if (formatId == 5)
                output = "placed " + item;
            else if (formatId == 9)
                output = item;
            else
                output = "formatId was wrong lol";

            Program.setConsoleColor(3);
            Console.WriteLine("You have " + output);
            return playerName + " has " + output + "\n";
        }

        private string[] getMessageStrings()
        {
            if (File.Exists(logPath))
                return File.ReadAllLines(logPath);
            else
            {
                Program.displayError("messageLog.txt does not exist!");
                return null;
            }
        }
    }
}
