using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Diagnostics;

namespace Windwaker_coop
{
    static class ReadWrite
    {
        static IntPtr gameProcess = IntPtr.Zero;

        //Returns whether the game is running or not and sets the processHandle accordingly
        public static bool getGameProcess(int playerNumber)
        {
            Process[] processes = Process.GetProcessesByName(Program.currGame.processName);
            if (processes.Length > playerNumber - 1)
            {
                gameProcess = processes[playerNumber - 1].Handle;
                return true;
            }
            else
            {
                Program.displayError($"{Program.currGame.processName} is not running!");
                gameProcess = IntPtr.Zero;
                return false;
            }
        }

        [DllImport("kernel32.dll")]
        static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, int nSize, out int lpNumberOfBytesWritten);
        [DllImport("kernel32.dll")]
        static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, int nSize, out int lpNumberOfBytesWritten);

        public static void Write(int playerNumber, IntPtr address, byte[] bytes)
        {
            if (!getGameProcess(playerNumber))
                return;
            int bytesWritten = 0;

            WriteProcessMemory(gameProcess, address, bytes, bytes.Length, out bytesWritten);
        }

        public static byte[] Read(int playerNumber, IntPtr address, int size)
        {
            if (!getGameProcess(playerNumber))
                return null;
            int bytesWritten = 0;
            byte[] result = new byte[size];

            ReadProcessMemory(gameProcess, address, result, size, out bytesWritten);
            return result;
        }

        //Assumes neither are null
        public static bool checkIfSame(List<byte> one, List<byte> two)
        {
            if (one.Count != two.Count)
                return false;
            for (int i = 0; i < one.Count; i++)
                if (one[i] != two[i])
                    return false;
            return true;
        }

        public static bool bitSet(uint number, uint bit)
        {
            return (number & (1 << (int)bit)) != 0;
        }
    }
}
