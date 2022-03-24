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
        private Cheater cheater;
        public MemoryReader mr;

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
            cheater = new Cheater();

            Connect();
            sendIntroData();
        }

        public void Begin()
        {
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

                byte[] memory = mr.readFromMemory(memoryLocations);
                if (memory != null && lastReadMemory != null)
                {
                    int byteListIndex = 0;
                    for (int locationListIndex = 0; locationListIndex < memoryLocations.Count; locationListIndex++)
                    {
                        //Loops through each memory location and compares its value to its previous value
                        //If different it sends it to the server for processing

                        MemoryLocation memLoc = memoryLocations[locationListIndex];
                        if (!compareToPreviousMemory(memory, lastReadMemory, byteListIndex, memLoc.size))
                        {
                            uint newValue = ReadWrite.bigToLittleEndian(memory, byteListIndex, memLoc.size);
                            uint oldValue = ReadWrite.bigToLittleEndian(lastReadMemory, byteListIndex, memLoc.size);

                            //The numbers are different, but still checks to see if any non individual bits were set
                            if (((oldValue ^ newValue) & ~memLoc.individualBits) > 0 || memLoc.individualBits == uint.MaxValue)
                                sendNewMemoryLocation(0, (ushort)locationListIndex, oldValue, newValue, false);
                        }
                        byteListIndex += memLoc.size;
                    }

                    lastReadMemory = memory;
                }

                //Time logging and ending functions
                Program.currGame.endingFunctions(this);
                Output.debug("Time taken to complete entire sync loop: " + (Environment.TickCount - timeStart) + " milliseconds", 1);
                await Task.Delay(loopTime);
            }
        }

        //Assumes they are non null and of the same length
        public bool compareToPreviousMemory(byte[] curr, byte[] prev, int startIdx, int length)
        {
            for (int i = startIdx; i < startIdx + length; i++)
                if (curr[i] != prev[i])
                    return false;
            return true;
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
            catch (TimeoutException)
            {
                Output.error($"Client timed out attempting to connect to the server at {IpAddress}:{port}");
                Program.EndProgram();
            }
        }

        //Disconnects from the server
        public override void End()
        {
            if (client.IsConnected)
                client.Disconnect();
        }

        //Returns the string result from processing the command
        public override string processCommand(string command, string[] args)
        {
            switch (command)
            {
                case "help":
                    //Displays the available client commands
                    return "Available client commands:\npause - temporarily disables syncing to and from the host\nunpause - resumes syncing to and from the host\n" +
                        "stop - ends syncing and closes the application\nsay [message] - sends a message to everyone in the server\n" +
                        "give [item] [number] - gives player the specified item (If cheats are enabled)\nping - tests the delay between client and server\n" +
                        "help - lists available commands";

                case "pause":
                    //command not implemented yet
                    return "command not implemented yet";

                case "unpause":
                    //command not implemented yet
                    return "command not implemented yet";

                case "say":
                    //Takes in a message and sends it to everyone else in the game
                    if (args.Length > 0)
                    {
                        string text = "";
                        foreach (string word in args)
                            text += word + " ";
                        sendTextMessage(text);
                        return "Message sent";
                    }
                    return "Command 'say' takes at least 1 argument!";

                case "ping":
                    //Sends a test to the server to determine the delay
                    sendDelayTest();
                    return "Sending delay test!";

                case "give":
                    //Gives the player a specified item
                    if (cheater != null)
                    {
                        return cheater.processCommand(this, args);
                    }
                    Output.error("Cheater object has not been initialized");
                    return "";

                case "stop":
                    //Ends the program
                    return "";

                default:
                    return "Command '" + command + "' not valid.";
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

        public override void sendMemoryList(byte[] memory)
        {
            Send(memory, 'm');
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
            MemoryLocation memLoc = memoryLocations[memLocIdx];

            //Calculate the new value if some bits are individual
            if (memLoc.individualBits > 0 && memLoc.individualBits != uint.MaxValue)
            {
                newValue = (oldValue & memLoc.individualBits) + (newValue & ~memLoc.individualBits);
            }
            byte[] bytes = ReadWrite.littleToBigEndian(newValue, memLoc.size);

            //Save new value to lastReadMemory
            int byteListIdx = getByteIndexOfMemLocs(memLocIdx);
            for (int i = 0; i < bytes.Length; i++)
                lastReadMemory[byteListIdx + i] = bytes[i];

            //Save new value to game memory
            mr.saveToMemory(bytes, memLoc.startAddress);
            Program.currGame.onReceiveFunctions(this, newValue, memLoc);
        }

        protected override void receiveMemoryList(byte[] data)
        {
            //{ 255 } means this is a brand new server - no memory overwite
            if (!(data.Length == 1 && data[0] == 255))
            {
                //Set any individual bits to what they were in the initial memory and overwrite memory
                int byteListIndex = 0;
                for (int locationListIndex = 0; locationListIndex < memoryLocations.Count; locationListIndex++)
                {
                    MemoryLocation memLoc = memoryLocations[locationListIndex];
                    if (memLoc.individualBits > 0)
                    {
                        uint player = ReadWrite.bigToLittleEndian(lastReadMemory, byteListIndex, memLoc.size);
                        uint overwrite = ReadWrite.bigToLittleEndian(data, byteListIndex, memLoc.size);

                        //If item is fully individual after first receive, don't reset to idv if player hasn't received yet
                        if (memLoc.individualBits != uint.MaxValue || (player != 255 && overwrite != 255))
                        {
                            uint newValue = (player & memLoc.individualBits) + (overwrite & ~memLoc.individualBits);
                            byte[] bytes = ReadWrite.littleToBigEndian(newValue, memLoc.size);
                            for (int i = 0; i < bytes.Length; i++)
                                data[byteListIndex + i] = bytes[i];
                        }
                    }
                    byteListIndex += memLoc.size;
                }

                lastReadMemory = data;
                mr.saveToMemory(data, memoryLocations);
            }
            Program.currGame.onReceiveFunctions(this, 0, null);
            Begin();
        }

        protected override void receiveNotification(byte[] data)
        {
            Output.text(Encoding.UTF8.GetString(data), ConsoleColor.Green);
        }

        protected override void receiveTextMessage(byte[] data)
        {
            Output.text(Encoding.UTF8.GetString(data), ConsoleColor.Blue);
        }
        protected override void receiveIntroData(byte[] data)
        {
            string jsonObject = Encoding.UTF8.GetString(data);
            Program.currGame.setSyncSettings(jsonObject);

            memoryLocations = Program.currGame.createMemoryLocations();
            mr = new MemoryReader();
            byte[] initialMemory = mr.readFromMemory(memoryLocations);
            if (initialMemory == null)
            {
                Program.EndProgram();
            }
            else
            {
                lastReadMemory = initialMemory;
                sendMemoryList(initialMemory);
            }
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
        }
    }
}
