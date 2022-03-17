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
                Output.error($"{Program.currGame.processName} is not running!");
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

        //Checks if a given bit in the number is set
        public static bool bitSet(uint number, uint bit)
        {
            return (number & (1 << (int)bit)) != 0;
        }

        //Converts a byte list to a number from bit to little endian format
        public static uint bigToLittleEndian(List<byte> byteList, int startIndex, int length)
        {
            byte[] bytes = new byte[4];
            string debugOuput = "Converting byte[] { ";
            for (int i = 0; i < length; i++)
            {
                bytes[length - 1 - i] = byteList[startIndex + i];
                debugOuput += byteList[startIndex + i].ToString("X") + " ";
            }
            Output.debug(debugOuput + "} to integer: " + BitConverter.ToUInt32(bytes), 4);
            return BitConverter.ToUInt32(bytes);
        }

        //Converts a number to a byte[] from little to big endian format
        public static byte[] littleToBigEndian(uint number, int length)
        {
            byte[] fourByte = BitConverter.GetBytes(number);
            byte[] result = new byte[length];

            for (int i = 0; i < length; i++)
            {
                result[length - 1 - i] = fourByte[i];
            }
            return result;
        }
    }
}
