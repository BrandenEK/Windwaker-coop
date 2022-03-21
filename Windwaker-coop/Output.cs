using System;

namespace Windwaker_coop
{
    static class Output
    {
        private static void setColor(ConsoleColor color)
        {
            Console.ForegroundColor = color;
        }

        public static void error(string message)
        {
            setColor(ConsoleColor.Red);
            Console.WriteLine("Error: " + message);
        }

        //Level 1 - decent stuff to know, level 2 - deep stuff
        public static void debug(string message, byte level)
        {
            if (level <= Program.config.debugLevel)
            {
                if (level == 1)
                    setColor(ConsoleColor.Magenta);
                else if (level == 2)
                    setColor(ConsoleColor.DarkMagenta);
                else
                {
                    error("Invalid debug level");
                    return;
                }
                Console.WriteLine(message);
            }
        }

        public static void text(string message, ConsoleColor color = ConsoleColor.White, bool newLine = true)
        {
            setColor(color);
            if (newLine)
                Console.WriteLine(message);
            else
                Console.Write(message);
        }

        public static void clear()
        {
            Console.Clear();
        }
    }
}
