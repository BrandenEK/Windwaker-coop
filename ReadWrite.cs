using System;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace WW_Coop
{
    static class ReadWrite
    {
        static IntPtr dolphinProcess = IntPtr.Zero;

        //Returns whether dolphin is running or not and sets the processHandle accordingly
        public static bool getDolphinProcess(int playerNumber)
        {
            Process[] dolphins = Process.GetProcessesByName("dolphin");
            if (dolphins.Length > playerNumber - 1)
            {
                dolphinProcess = dolphins[playerNumber - 1].Handle;
                return true;
            }
            else
            {
                Program.displayError("Dolphin is not running!");
                dolphinProcess = IntPtr.Zero;
                return false;
            }
        }

        [DllImport("kernel32.dll")]
        static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, int nSize, out int lpNumberOfBytesWritten);
        [DllImport("kernel32.dll")]
        static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, int nSize, out int lpNumberOfBytesWritten);

        public static void Write(int playerNumber, IntPtr address, byte[] bytes)
        {
            if (!getDolphinProcess(playerNumber))
                return;
            int bytesWritten = 0;

            WriteProcessMemory(dolphinProcess, address, bytes, bytes.Length, out bytesWritten);
        }

        public static byte[] Read(int playerNumber, IntPtr address, int size)
        {
            if (!getDolphinProcess(playerNumber))
                return null;
            int bytesWritten = 0;
            byte[] result = new byte[size];

            ReadProcessMemory(dolphinProcess, address, result, size, out bytesWritten);
            return result;
        }
    }
}
