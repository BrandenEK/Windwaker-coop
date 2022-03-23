using System;
using System.Collections.Generic;
using System.Text;
using SimpleTcp;

namespace Windwaker_coop
{
    class Server : User
    {
        private SimpleTcpServer server;
        public Dictionary<string, string> clientIps;
        private byte[] hostdata;
        private string currIp = "";

        Dictionary<int, string> notificationValues = new Dictionary<int, string>()
        {
            { 0, "obtained the " },
            { 1, "obtained a " },
            { 2, "obtained " },
            { 3, "learned " },
            { 4, "deciphered " },
            { 5, "placed " },
            { 9, "" } //can be anything
        };

        public Server(string ip) : base(ip)
        {
            try
            {
                server = new SimpleTcpServer(IpAddress, port);
            }
            catch (System.Net.Sockets.SocketException)
            {
                Output.error($"{IpAddress}:{port} is not a valid ip address");
                Program.EndProgram();
            }

            server.Events.ClientConnected += Events_ClientConnected;
            server.Events.ClientDisconnected += Events_ClientDisconnected;
            server.Events.DataReceived += Events_DataReceived;
            clientIps = new Dictionary<string, string>();

            //Set sync settings, create memory locations, and then start server
            Program.currGame.syncSettings = Program.currGame.GetSyncSettingsFromFile();
            mr = new MemoryReader();

            Start();
        }

        private void compareHostAndPlayer(uint playerValue, uint previousValue, ushort memLocIdx)
        {
            MemoryLocation memLoc = mr.memoryLocations[memLocIdx];

            //Calculate hostValue from the savedList
            int byteListIdx = 0;
            for (int i = 0; i < memLocIdx; i++)
                byteListIdx += mr.memoryLocations[i].size;
            uint hostValue = ReadWrite.bigToLittleEndian(hostdata, byteListIdx, memLoc.size);

            //Error conditions - not fatal, but unexpected
            if (playerValue == hostValue)
            {
                Output.debug("Player value received was the same as host value.  Server and client are out of sync", 1);
                return;
            }
            if (playerValue > memLoc.higherValue || playerValue < memLoc.lowerValue)
            {
                Output.debug($"The value at 0x{memLoc.startAddress.ToInt64().ToString("X")} is not inside of an acceptable range ({playerValue}). It was not synced to the host.", 2);
                return;
            }

            switch (memLoc.compareId)
            {
                case 0:
                    if (playerValue > hostValue) overwriteMemory(); return;
                case 1:
                    if (playerValue < hostValue) overwriteMemory(); return;
                case 2:
                    if (hostValue == 255 || playerValue > hostValue && playerValue != 255) overwriteMemory(); return;
                case 9:
                    if ((playerValue & (playerValue ^ hostValue)) > 0) overwriteMemory(); return;
                default:
                    Output.error("Invalid compareId"); return;
            }

            void overwriteMemory()
            {
                //Write new value to host data
                byte[] bytes = ReadWrite.littleToBigEndian(playerValue, memLoc.size);
                for (int i = 0; i < bytes.Length; i++)
                    hostdata[byteListIdx + i] = bytes[i];

                sendNewMemoryLocation(0, memLocIdx, previousValue, playerValue, true); //change writeType to be in data
                calculateNotification(playerValue, hostValue, memLoc);
            }

            //Determines whether or not to send a notification & calculates what it should be
            void calculateNotification(uint newValue, uint oldValue, MemoryLocation memLoc)
            {
                if (memLoc.cd.bitfield)
                {
                    //checks each bit and only sends notification if the player just set it
                    for (int i = 0; i < memLoc.cd.values.Length; i++)
                    {
                        if (ReadWrite.bitSet(newValue, memLoc.cd.values[i]) && !ReadWrite.bitSet(oldValue, memLoc.cd.values[i]))
                        {
                            processNotification(memLoc.cd.text[i]);
                        }
                    }
                    return;
                }

                if (memLoc.cd.values == null || memLoc.cd.text == null)
                {
                    //If the memoryLocation only has one possible value
                    if (memLoc.name != "")
                        processNotification(memLoc.name);
                }
                else
                {
                    //compares the new value to everything in the list of possible values
                    for (int i = 0; i < memLoc.cd.values.Length; i++)
                    {
                        if (newValue == memLoc.cd.values[i])
                        {
                            processNotification(memLoc.cd.text[i]);
                            return;
                        }
                    }
                }

                //Uses the itemText to actually send the notification
                void processNotification(string itemText)
                {
                    if (itemText != "")
                    {
                        string[] strings = itemText.Split('*', 2);
                        itemText = strings[0]; int formatId = -1;
                        int.TryParse(strings[1], out formatId);

                        string output = notificationValues[formatId] + itemText;
                        sendNotification("You have " + output, false);
                        sendNotification(clientIps[currIp] + " has " + output, true);
                    }
                    else
                        Output.error("Notification was unable to be calculated");
                }
            }
        }

        public void setServerToDefault()
        {
            //hostdata = mr.getDefaultValues();
        }

        public void kickPlayer(string ipPort)
        {
            server.DisconnectClient(ipPort);
        }

        //Starts the server
        public void Start()
        {
            try
            {
                server.Start();
            }
            catch (System.Net.Sockets.SocketException)
            {
                Output.error($"Failed to start the server at {IpAddress}:{port}");
                Program.EndProgram();
            }
            Output.text($"Server successfully started at {IpAddress}:{port}");
        }

        //Doesn't do anything
        public override void End()
        {
            if (server.IsListening)
                server.Stop();
        }

        //Returns the string result from processing the command
        public override string processCommand(string command, string[] args)
        {
            switch (command)
            {
                case "help":
                    //Displays the available server commands
                    return "Available server commands:\nlist - lists all of the currently connected players\n" +
                        "stats - displays the items the server currently has\nreset - resets the host to default values\n" +
                        "kick [type] [Name or IpPort] - kicks the speciifed Name or IpPort from the game\nstop - ends syncing and closes the application\n" +
                        "help - lists available commands";

                case "list":
                    //Lists the names : ip addresses in the server
                    string text = "Connected players:\n";
                    foreach (string ip in clientIps.Keys)
                    {
                        text += $"{clientIps[ip]} ({ip})\n";
                    }
                    if (clientIps.Count < 1)
                        text += "none\n";
                    return text.Substring(0, text.Length - 1);

                case "stats":
                    //command not implemented yet
                    return "command not implemented yet";

                case "reset":
                    //resets the server to default values
                    setServerToDefault();
                    sendNotification("Server data has been reset to default!", true);
                    sendNotification("Server data has been reset to default!", false);
                    return "Server data has been reset to default!";

                case "kick":
                    //kicks the inputted player's ipPort or name from the game
                    if (args.Length != 2)
                        return "Command 'kick' takes 2 arguments!";

                    if (args[0] == "name" || args[0] == "n")
                    {
                        //Change to simple lookup once names are individualized
                        int numFound = 0;
                        string texts = "";
                        foreach (string ip in clientIps.Keys)
                        {
                            if (clientIps[ip] == args[1])
                            {
                                kickPlayer(ip);
                                texts += "Player '" + args[1] + "' has been kicked from the game!\n";
                                numFound++;
                            }
                        }
                        if (numFound == 0)
                            return "Player '" + args[1] + "' does not exist in the game!";
                        return texts;
                    }
                    else if (args[0] == "ip" || args[0] == "i")
                    {
                        if (clientIps.ContainsKey(args[1]))
                        {
                            kickPlayer(args[1]);
                            return "IpPort '" + args[1] + "' has been kicked from the game!";
                        }
                        return "IpPort '" + args[1] + "' does not exist in the game!";
                    }
                    return "Invalid type.  Must be either 'name' or 'ip'";

                case "ban":
                    //command not implemented yet
                    return "command not implemented yet";

                case "stop":
                    //Ends the program
                    return "";

                default:
                    return "Command '" + command + "' not valid.";
            }
        }

        //Sets the current Ip address to the one that sent the data then call regular data received function
        protected override void Events_DataReceived(object sender, DataReceivedEventArgs e)
        {
            currIp = e.IpPort;
            base.Events_DataReceived(sender, e);
        }

        #region Send functions
        private void Send(string ip, byte[] data, char dataType)
        {
            if (server.IsListening && data != null && data.Length > 0)
            {
                server.Send(ip, calculateMessageToSend(data, dataType));
            }
        }

        public override void sendMemoryList(byte[] memory)
        {
            Send(currIp, memory, 'm');
        }

        public override void sendNewMemoryLocation(byte writeType, ushort memLocIndex, uint oldValue, uint newValue, bool sendToAllButThis)
        {
            List<byte> toSend = new List<byte>(BitConverter.GetBytes(memLocIndex));
            toSend.AddRange(BitConverter.GetBytes(oldValue));
            toSend.AddRange(BitConverter.GetBytes(newValue));
            toSend.Add(writeType);

            if (sendToAllButThis)
            {
                foreach (string ip in clientIps.Keys)
                    if (ip != currIp)
                        Send(ip, toSend.ToArray(), 'v');
            }
            else
            {
                Send(currIp, toSend.ToArray(), 'v');
            }
        }

        public override void sendNotification(string notification, bool sendToAllButThis)
        {
            byte[] b = Encoding.UTF8.GetBytes(notification);
            if (sendToAllButThis)
            {
                foreach (string ip in clientIps.Keys)
                    if (ip != currIp)
                        Send(ip, b, 'n');
            }
            else
            {
                Send(currIp, b, 'n');
            }
        }

        public override void sendTextMessage(string message)
        {
            byte[] b = Encoding.UTF8.GetBytes(message);
            foreach (string ip in clientIps.Keys)
                if (ip != currIp)
                    Send(ip, b, 't');
        }

        public override void sendIntroData()
        {
            Send(currIp, Encoding.UTF8.GetBytes(Program.currGame.getSyncSettings()), 'i');
        }
        #endregion

        #region Receive functions
        protected override void receiveMemoryList(byte[] playerData)
        {
            if (hostdata == null)
            {
                hostdata = playerData;
                sendMemoryList(new byte[] { 255 });
            }
            else
            {
                sendMemoryList(hostdata);
            }
        }

        protected override void receiveNewMemoryLocation(byte[] data)
        {
            if (data.Length != 11)
            {
                Output.error("Received memory location was not correct length (11b)");
                return;
            }

            ushort memLocIdx = BitConverter.ToUInt16(data, 0);
            uint oldValue = BitConverter.ToUInt32(data, 2);
            uint newValue = BitConverter.ToUInt32(data, 6);

            compareHostAndPlayer(newValue, oldValue, memLocIdx);
        }

        protected override void receiveTextMessage(byte[] data)
        {
            sendTextMessage(Encoding.UTF8.GetString(data));
        }

        protected override void receiveNotification(byte[] data)
        {
            string message = Encoding.UTF8.GetString(data);
            sendNotification(message, true);
        }

        protected override void receiveDelayTest(byte[] data)
        {
            long sendTime = BitConverter.ToInt64(data);
            long timeDelta = DateTime.Now.Ticks - sendTime;
            Output.text("Byte[] received from " + currIp + " came with a delay of " + (timeDelta / 10000) + " milliseconds", ConsoleColor.Yellow);
        }
        protected override void receiveIntroData(byte[] data)
        {
            string name = Encoding.UTF8.GetString(data);
            clientIps[currIp] = name;
            sendIntroData();
        }
        #endregion

        private void Events_ClientDisconnected(object sender, ClientDisconnectedEventArgs e)
        {
            Output.text("Client disconnected at " + e.IpPort);
            currIp = e.IpPort;
            sendNotification(clientIps[currIp] + " has left the game!", true);
            clientIps.Remove(e.IpPort);
        }

        private void Events_ClientConnected(object sender, ClientConnectedEventArgs e)
        {
            Output.text("Client connected at " + e.IpPort);
            clientIps.Add(e.IpPort, "unknown");
        }
    }
}
