using System;
using System.Collections.Generic;
using System.Text;
using SimpleTcp;
using System.Threading.Tasks;

namespace Windwaker_coop
{
    class Client
    {
        public string IpAddress;
        public int port;
        public string playerName;

        private SimpleTcpClient client;
        private MemoryReader mr;

        public Client(string ip, int port, string playerName)
        {
            IpAddress = ip;
            this.port = port;
            this.playerName = playerName;
            client = new SimpleTcpClient(IpAddress, port);
            client.Events.Connected += Events_Connected;
            client.Events.Disconnected += Events_Disconnected;
            client.Events.DataReceived += Events_DataReceived;
            mr = new MemoryReader();
        }

        private void Events_DataReceived(object sender, DataReceivedEventArgs e)
        {
            Console.WriteLine("Server: " + Encoding.UTF8.GetString(e.Data));
            //write the notification or write the new data to memory
        }

        public void Connect()
        {
            //add exception handling
            client.Connect();
        }

        public void Send(byte[] data)
        {
            if (client.IsConnected && data != null && data.Length > 0)
            {
                client.Send(data);
            }
        }
        public void Send(string message)
        {
            if (client.IsConnected && message != null && message != "")
            {
                client.Send(playerName + "~" + message);
            }
        }

        public void beginSyncing(int syncDelay)
        {
            //Start every x seconds reading from memory and sending this to the server
            syncLoop(syncDelay);
        }

        private async Task syncLoop(int loopTime)
        {
            while (true)
            {
                Program.setConsoleColor(5);
                if (Program.programStopped)
                    return;

                int timeStart = Environment.TickCount;

                //syncLoop stuff
                List<byte> memory = mr.readFromMemory();
                if (memory != null)
                {
                    Send(memory.ToArray());
                    Console.WriteLine("Sending " + memory.Count + " bytes");
                }

                Program.displayDebug("Time taken to complete entire sync loop: " + (Environment.TickCount - timeStart) + " milliseconds", 1);
                Program.setConsoleColor(5);
                await Task.Delay(loopTime);
                Program.displayDebug("", 1);
            }
        }

        private void Events_Disconnected(object sender, ClientDisconnectedEventArgs e)
        {
            Console.WriteLine("Disconnected from the server");
        }

        private void Events_Connected(object sender, ClientConnectedEventArgs e)
        {
            Console.WriteLine("Successfully connected to the server");
        }
    }
}
