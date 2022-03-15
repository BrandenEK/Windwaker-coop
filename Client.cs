using System;
using System.Collections.Generic;
using System.Text;
using SimpleTcp;
using System.Threading.Tasks;

namespace Windwaker_coop
{
    class Client : User
    {
        public string playerName;
        private SimpleTcpClient client;

        public Client(string ip, string playerName) : base(ip)
        {
            this.playerName = playerName;
            try
            {
                client = new SimpleTcpClient(IpAddress, port);
            }
            catch (System.Net.Sockets.SocketException)
            {
                Program.displayError($"{IpAddress}:{port} is not a valid ip address");
                Program.EndProgram();
            }

            client.Events.Connected += Events_Connected;
            client.Events.Disconnected += Events_Disconnected;
            client.Events.DataReceived += Events_DataReceived;
            mr = new MemoryReader();
        }

        public override void Begin()
        {
            Connect();
            beginSyncing(Program.config.syncDelay);
        }

        //Once the player is ready, starts the syncLoop
        public void beginSyncing(int syncDelay)
        {
            syncLoop(syncDelay);
        }

        private async Task syncLoop(int loopTime)
        {
            while (!Program.programStopped)
            {
                Program.setConsoleColor(5);
                int timeStart = Environment.TickCount;

                //syncLoop stuff
                Program.currGame.beginningFunctions(this);
                List<byte> memory = mr.readFromMemory();
                if (memory != null)
                {
                    sendMemoryList(memory);
                }
                Program.currGame.endingFunctions(this);

                Program.displayDebug("Time taken to complete entire sync loop: " + (Environment.TickCount - timeStart) + " milliseconds", 1);
                Program.setConsoleColor(5);
                await Task.Delay(loopTime);
                Program.displayDebug("", 1);
            }
        }

        //Connects to the server
        public void Connect()
        {
            try
            {
                client.Connect();
            }
            catch (System.Net.Sockets.SocketException)
            {
                Program.displayError($"Failed to connect to a server at {IpAddress}:{port}");
                Program.EndProgram();
            }
        }

        #region Send functions
        private void Send(List<byte> data, char dataType)
        {
            if (client.IsConnected && data != null && data.Count > 0)
            {
                data.AddRange(new byte[] { 126, 126, Convert.ToByte(dataType) });
                client.Send(data.ToArray());
                Program.displayDebug("Sending " + data.Count + " bytes", 2);
            }
        }

        public override void sendMemoryList(List<byte> data)
        {
            List<byte> toSend = new List<byte>(Encoding.UTF8.GetBytes(playerName + "~"));
            toSend.AddRange(data);
            Send(toSend, 'm');
        }

        public override void sendTextMessage(string message)
        {
            List<byte> toSend = new List<byte>(Encoding.UTF8.GetBytes(playerName + ": " + message));
            Send(toSend, 't');
        }

        public override void sendNotification(string notification, bool useless)
        {
            List<byte> toSend = new List<byte>(Encoding.UTF8.GetBytes(notification));
            Send(toSend, 'n');
        }

        public override void sendDelayTest()
        {
            List<byte> toSend = new List<byte>(BitConverter.GetBytes(DateTime.Now.Ticks));
            Send(toSend, 'd');
        }
        #endregion

        #region Receive functions
        //type 'v' - locates the updated memoryLocation and writes the newValue to memory
        protected override void receiveNewMemoryLocation(List<byte> data)
        {
            if (data.Count < 3 || data.Count > 6)
            {
                Program.displayError("New memoryLocation received from server has an invalid size");
                return;
            }

            short memLocIdx = BitConverter.ToInt16(data.GetRange(0, 2).ToArray());
            List<byte> newValue = data.GetRange(2, data.Count - 2);
            mr.saveToMemory(newValue, mr.memoryLocations[memLocIdx].startAddress);
            Program.currGame.onReceiveFunctions(this, newValue, mr.memoryLocations[memLocIdx]);
        }

        //type 'm' - received when first joining an existing server, save the list to memory
        protected override void receiveMemoryList(List<byte> data)
        {
            mr.saveToMemory(data);
        }

        //type 'n' - displays the notification in the console
        protected override void receiveNotification(List<byte> data)
        {
            Program.setConsoleColor(3);
            Console.WriteLine(Encoding.UTF8.GetString(data.ToArray()));
        }

        //type 't' - displays the text message in the console
        protected override void receiveTextMessage(List<byte> data)
        {
            Program.setConsoleColor(8);
            Console.WriteLine(Encoding.UTF8.GetString(data.ToArray()) + "\n");
        }
        #endregion

        private void Events_Disconnected(object sender, ClientDisconnectedEventArgs e)
        {
            Program.setConsoleColor(1);
            Console.WriteLine("Disconnected from the server at " + e.IpPort + "\n");
            sendNotification(playerName + " has left the game!", true);
            Program.EndProgram();
        }

        private void Events_Connected(object sender, ClientConnectedEventArgs e)
        {
            Program.setConsoleColor(1);
            Console.WriteLine("Successfully connected to the server at " + e.IpPort);
            sendNotification(playerName + " has joined the game!", true);
        }
    }
}
