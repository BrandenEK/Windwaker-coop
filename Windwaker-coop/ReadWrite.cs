using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Diagnostics;

namespace Windwaker_coop
{
    static class ReadWrite
    {
        static IntPtr gameProcess = IntPtr.Zero;
        static uint baseAddress = 0;
        static bool calculatingBaseAddress = false;

        //Returns whether the game is running or not and sets the processHandle accordingly
        public static bool getGameProcess(int playerNumber)
        {
            Process[] processes = Process.GetProcessesByName(Program.currGame.processName);
            if (processes.Length > playerNumber - 1)
            {
                gameProcess = processes[playerNumber - 1].Handle;
                if (baseAddress == 0 && !calculatingBaseAddress)
                {
                    Output.debug("Calculating base address...", 1);
                    getBaseAddress(Program.currGame.baseAddressOffsets);
                }
                return true;
            }
            else
            {
                Output.error($"{Program.currGame.processName} is not running!");
                gameProcess = IntPtr.Zero;
                return false;
            }
        }

        //Reads the games pointer path to find base address - only called once at beginning of sync
        private static void getBaseAddress(uint[] offsets)
        {
            if (offsets.Length == 1)
            {
                baseAddress = offsets[0];
                Output.debug("Base address: 0x" + baseAddress.ToString("X"), 1);
                return;
            }

            baseAddress = 0;
            uint currAddress = 0;
            calculatingBaseAddress = true;

            for (int i = 0; i < offsets.Length; i++)
            {
                byte[] temp = Read(1, currAddress + offsets[i], 4);
                if (temp == null)
                {
                    Output.error("Could not calculate base address");
                    baseAddress = 0;
                    return;
                }
                currAddress = BitConverter.ToUInt32(temp, 0);
            }

            baseAddress = currAddress;
            calculatingBaseAddress = false;
            Output.debug("Base address: 0x" + baseAddress.ToString("X"), 1);
        }

        [DllImport("kernel32.dll")]
        static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, int nSize, out int lpNumberOfBytesWritten);
        [DllImport("kernel32.dll")]
        static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, int nSize, out int lpNumberOfBytesWritten);

        public static void Write(int playerNumber, uint address, byte[] bytes)
        {
            if (!getGameProcess(playerNumber))
                return;
            int bytesWritten = 0;

            WriteProcessMemory(gameProcess, (IntPtr)(baseAddress + address), bytes, bytes.Length, out bytesWritten);
        }

        public static byte[] Read(int playerNumber, uint address, int size)
        {
            if (!getGameProcess(playerNumber))
                return null;

            int bytesWritten = 0;
            byte[] result = new byte[size];

            ReadProcessMemory(gameProcess, (IntPtr)(baseAddress + address), result, size, out bytesWritten);
            return result;
        }

        //Checks if a given bit in the number is set
        public static bool bitSet(uint number, byte bit)
        {
            return (number & (1 << bit)) != 0;
        }

        public static uint gameToAppNumber(byte[] number)
        {
            return 0;
        }

        public static byte[] appToGameNumber(uint number)
        {
            return null;
        }

        //Converts a byte[] to a number from big to little endian format
        public static uint bigToLittleEndian(byte[] arr, int startIndex, int length)
        {
            byte[] bytes = new byte[4];
            for (int i = 0; i < length; i++)
            {
                bytes[length - 1 - i] = arr[startIndex + i];
            }
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
