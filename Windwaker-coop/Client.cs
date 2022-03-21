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
        private byte[] lastReadMemory;

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
                Output.error($"{IpAddress}:{port} is not a valid ip address");
                Program.EndProgram();
            }

            client.Events.Connected += Events_Connected;
            client.Events.Disconnected += Events_Disconnected;
            client.Events.DataReceived += Events_DataReceived;

            Connect();
            sendIntroData();
        }

        public void Begin()
        {
            mr = new MemoryReader();
            //After receiving initial memory overwrite (if not first player), write before beginSyncing
            //Maybe move this stuff to still in receiveIntro function

            Program.programSyncing = true;
            beginSyncing(Program.config.syncDelay);
        }

        //Once the player is ready, starts the syncLoop
        public void beginSyncing(int syncDelay)
        {
            syncLoop(syncDelay);
        }

        private async Task syncLoop(int loopTime)
        {
            while (Program.programSyncing)
            {
                //Time logging and beginning functions
                int timeStart = Environment.TickCount;
                Program.currGame.beginningFunctions(this);

                byte[] memory = mr.readFromMemory();
                if (memory != null)
                {
                    if (lastReadMemory != null)
                    {
                        int byteListIndex = 0;
                        for (int locationListIndex = 0; locationListIndex < mr.memoryLocations.Count; locationListIndex++)
                        {
                            //Loops through each memory location and compares its value to its previous value
                            //If different it sends it to the server for processing

                            MemoryLocation memLoc = mr.memoryLocations[locationListIndex];
                            if (!compareToPreviousMemory(memory, lastReadMemory, byteListIndex, memLoc.size))
                            {
                                uint newValue = ReadWrite.bigToLittleEndian(memory, byteListIndex, memLoc.size);
                                uint oldValue = ReadWrite.bigToLittleEndian(lastReadMemory, byteListIndex, memLoc.size);
                                sendNewMemoryLocation(0, (ushort)locationListIndex, oldValue, newValue, false);
                            }
                            byteListIndex += memLoc.size;
                        }
                    }

                    lastReadMemory = memory;
                }

                //Time logging and ending functions
                Program.currGame.endingFunctions(this);
                Output.debug("Time taken to complete entire sync loop: " + (Environment.TickCount - timeStart) + " milliseconds", 1);
                await Task.Delay(loopTime);
            }

            //Compares a single memory location to its previous value while still in byte[] form
            bool compareToPreviousMemory(byte[]curr, byte[] prev, int startIdx, int length)
            {
                for (int i = startIdx; i < startIdx + length; i++)
                    if (curr[i] != prev[i])
                        return false;
                return true;
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
                Output.error($"Failed to connect to a server at {IpAddress}:{port}");
                Program.EndProgram();
            }
        }

        #region Send functions
        private void Send(byte[] data, char dataType)
        {
            if (client.IsConnected && data != null && data.Length > 0)
            {
                client.Send(calculateMessageToSend(data, dataType));
            }
        }

        public override void sendMemoryList(List<byte> data)
        {
            Send(data.ToArray(), 'm');
        }

        public override void sendNewMemoryLocation(byte writeType, ushort memLocIndex, uint oldValue, uint newValue, bool useless)
        {
            List<byte> toSend = new List<byte>(BitConverter.GetBytes(memLocIndex));
            toSend.AddRange(BitConverter.GetBytes(oldValue));
            toSend.AddRange(BitConverter.GetBytes(newValue));
            toSend.Add(writeType);

            Send(toSend.ToArray(), 'v');
        }

        public override void sendTextMessage(string message)
        {
            Send(Encoding.UTF8.GetBytes(playerName + ": " + message), 't');
        }

        public override void sendNotification(string notification, bool useless)
        {
            Send(Encoding.UTF8.GetBytes(notification), 'n');
        }

        public override void sendDelayTest()
        {
            Send(BitConverter.GetBytes(DateTime.Now.Ticks), 'd');
        }
        public override void sendIntroData()
        {
            Send(Encoding.UTF8.GetBytes(playerName), 'i');
        }
        #endregion

        #region Receive functions
        //type 'v' - locates the updated memoryLocation and writes the newValue to memory
        protected override void receiveNewMemoryLocation(byte[] data)
        {
            if (!Program.programSyncing) return;

            if (data.Length != 11)
            {
                Output.error("Received memory location was not correct length (11b)");
                return;
            }

            ushort memLocIdx = BitConverter.ToUInt16(data, 0);
            uint oldValue = BitConverter.ToUInt32(data, 2);
            uint newValue = BitConverter.ToUInt32(data, 6);
            byte writeType = data[10];

            MemoryLocation memLoc = mr.memoryLocations[memLocIdx];
            mr.saveToMemory(ReadWrite.littleToBigEndian(newValue, memLoc.size), memLoc.startAddress);
            Program.currGame.onReceiveFunctions(this, newValue, memLoc);
        }

        //type 'm' - received when first joining an existing server, save the list to memory
        protected override void receiveMemoryList(byte[] data)
        {
            mr.saveToMemory(data);
        }

        //type 'n' - displays the notification in the console
        protected override void receiveNotification(byte[] data)
        {
            Output.text(Encoding.UTF8.GetString(data), ConsoleColor.Green);
        }

        //type 't' - displays the text message in the console
        protected override void receiveTextMessage(byte[] data)
        {
            Output.text(Encoding.UTF8.GetString(data), ConsoleColor.Blue);
        }
        //type 'i' - sets the syncSettings and allows to start syncing
        protected override void receiveIntroData(byte[] data)
        {
            string jsonObject = Encoding.UTF8.GetString(data);
            Program.currGame.setSyncSettings(jsonObject);
            Begin();
        }
        #endregion

        private void Events_Disconnected(object sender, ClientDisconnectedEventArgs e)
        {
            Output.text("Disconnected from the server at " + e.IpPort);
            Program.EndProgram();
        }

        private void Events_Connected(object sender, ClientConnectedEventArgs e)
        {
            Output.text("Successfully connected to the server at " + e.IpPort);
            sendNotification(playerName + " has joined the game!", true);
        }
    }
}
