using System;
using System.Collections.Generic;
using System.Text;

namespace Windwaker_coop
{
    static class Output
    {
        private static void setColor(ConsoleColor color)
        {
            Console.ForegroundColor = color;
        }

        public static void error(string message) //new line
        {
            setColor(ConsoleColor.Red);
            Console.WriteLine("Error: " + message);
        }

        public static void debug(string message, byte level) //no new line
        {
            if (level <= Program.config.debugLevel)
            {
                if (level > 2)
                    setColor(ConsoleColor.DarkMagenta);
                else if (level > 1)
                    setColor(ConsoleColor.Magenta);
                else
                    setColor(ConsoleColor.White);
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
