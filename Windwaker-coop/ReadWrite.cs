using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Diagnostics;

namespace Windwaker_coop
{
    static class ReadWrite
    {
        static IntPtr gameProcess = IntPtr.Zero;

        //Returns whether dolphin is running or not and sets the processHandle accordingly
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

        //Test function - is only temporary
        public static void testProcessData()
        {
            if (!getGameProcess(1))
                return;
            Process[] processes = Process.GetProcessesByName(Program.currGame.processName);
            Process p = processes[0];

            Console.WriteLine("Name: " + p.ProcessName);
            Console.WriteLine("File name: " + p.MainModule.FileName);
            Console.WriteLine("Base Address: 0x" + p.MainModule.BaseAddress.ToInt64().ToString("X"));
            Console.WriteLine("Memory size: " + p.MainModule.ModuleMemorySize);

            //0x100A837D - oot
            //0x635A2BFF - sm64
            uint idAddress = 0x100A837D;

            idAddress = (uint)p.MainModule.BaseAddress + 0xFCA837D;

            byte[] word = Read(1, (IntPtr)idAddress, 8);
            Console.WriteLine(System.Text.Encoding.UTF8.GetString(word));
        }
    }
}
