using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Windwaker.Multiplayer.Client
{
    internal class DolphinReader : IMemoryReader
    {
        public byte[] Read(uint address, int size)
        {
            if (!IsDolphinRunning(out nint process))
                throw new WindwakerException("Dolphin is not running!");

            var bytes = new byte[size];
            ReadProcessMemory(process, (nint)(BASE_ADDRESS + address), bytes, size, out int _);
            return bytes;
        }

        public bool TryRead(uint address, int size, out byte[] bytes)
        {
            try
            {
                bytes = Read(address, size);
                return true;
            }
            catch (WindwakerException)
            {
                bytes = Array.Empty<byte>();
                return false;
            }
        }

        public void Write(uint address, byte[] bytes)
        {
            if (!IsDolphinRunning(out nint process))
                throw new WindwakerException("Dolphin is not running!");

            WriteProcessMemory(process, (nint)(BASE_ADDRESS + address), bytes, bytes.Length, out int _);
        }

        public bool TryWrite(uint address, byte[] bytes)
        {
            try
            {
                Write(address, bytes);
                return true;
            }
            catch (WindwakerException)
            {
                return false;
            }
        }

        private bool IsDolphinRunning(out nint process)
        {
            Process[] processes = Process.GetProcessesByName("dolphin");

            bool isRunning = processes.Length > 0;
            process = isRunning ? processes[0].Handle : nint.MinValue;
            return isRunning;
        }

        private const ulong BASE_ADDRESS = 0x803B0000;

        [DllImport("kernel32.dll")]
        static extern bool WriteProcessMemory(nint hProcess, nint lpBaseAddress, byte[] lpBuffer, int nSize, out int lpNumberOfBytesWritten);
        [DllImport("kernel32.dll")]
        static extern bool ReadProcessMemory(nint hProcess, nint lpBaseAddress, byte[] lpBuffer, int nSize, out int lpNumberOfBytesWritten);
    }
}
