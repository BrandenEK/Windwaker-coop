using System;
using System.Collections.Generic;
using System.Text;
using SimpleTcp;

namespace Windwaker_coop
{
    class Server : User
    {
        private SimpleTcpServer server;
        public Dictionary<string, PlayerInfo> clientIps;
        private byte[] hostdata;
        private string currIp = "";

        private bool newServer;
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
            newServer = true;

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
            clientIps = new Dictionary<string, PlayerInfo>();

            //Set sync settings, create memory locations, and then start server
            Program.currGame.syncSettings = Program.currGame.GetSyncSettingsFromFile();
            mr = new MemoryReader();
            setServerToDefault();

            Start();
        }

        private void compareHostAndPlayer(uint playerValue, uint hostValue, ushort memLocIdx)
        {
            MemoryLocation memLoc = mr.memoryLocations[memLocIdx];
            //Calculate hostValue from the savedList or maybe from oldValue sent in

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
                //write new value to host data
                sendNewMemoryLocation(0, memLocIdx, hostValue, playerValue, true); //change writeType to be in data
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
                        sendNotification(clientIps[currIp].name + " has " + output, true);
                    }
                    else
                        Output.error("Notification was unable to be calculated");
                }
            }
        }

        public void setServerToDefault()
        {
            hostdata = mr.getDefaultValues();
        }

        public void kickPlayer(string ipPort)
        {
            server.DisconnectClient(ipPort);
        }

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

        //Sets the current Ip address to the one that sent the data then call regular data received function
        protected override void Events_DataReceived(object sender, DataReceivedEventArgs e)
        {
            currIp = e.IpPort;
            base.Events_DataReceived(sender, e);
        }

        #region Send functions
        private void Send(string ip, byte[] data, char dataType)
        {
            if (server.IsListening)
            {
                List<byte> d = new List<byte>(BitConverter.GetBytes((ushort)data.Length));
                d.Add(Convert.ToByte(dataType));
                d.AddRange(data);
                server.Send(ip, d.ToArray());
                Output.debug("Sending " + d.Count + " bytes", 2);
            }
        }

        public override void sendMemoryList(List<byte> memory)
        {
            Send(currIp, memory.ToArray(), 'm');
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
        //type 'm' - reads player memory list and compares this to host data
        protected override void receiveMemoryList(byte[] playerData)
        {
            //Basically just going to be this - dont call this function yet though
            if (newServer)
            {
                //If this is the first player to join, copy their memory to the host
                hostdata = playerData;
                newServer = false;
                clientIps[currIp].repeat = true;
            }
        }

        //type 'v' - reads the single memoryLocation's new value whenever the player's has changed
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

        //type 't' - reads player name & message and sends it to everybody else
        protected override void receiveTextMessage(byte[] data)
        {
            sendTextMessage(Encoding.UTF8.GetString(data));
        }

        //type 'n' - read notification and send it to everyone else
        protected override void receiveNotification(byte[] data)
        {
            string message = Encoding.UTF8.GetString(data);
            sendNotification(message, true);
        }

        //type 'd' - reads the string and converts it to a long & displays it
        protected override void receiveDelayTest(byte[] data)
        {
            long sendTime = BitConverter.ToInt64(data);
            long timeDelta = DateTime.Now.Ticks - sendTime;
            Output.text("Byte[] received from " + currIp + " came with a delay of " + (timeDelta / 10000) + " milliseconds", ConsoleColor.Yellow);
        }
        //type 'i' - reads the player name and sets it in the dictionary, then sends the sync settings
        protected override void receiveIntroData(byte[] data)
        {
            string name = Encoding.UTF8.GetString(data);
            clientIps[currIp].name = name;
            sendIntroData();
        }
        #endregion

        private void Events_ClientDisconnected(object sender, ClientDisconnectedEventArgs e)
        {
            Output.text("Client disconnected at " + e.IpPort);
            currIp = e.IpPort;
            sendNotification(clientIps[currIp].name + " has left the game!", true);
            clientIps.Remove(e.IpPort);
        }

        private void Events_ClientConnected(object sender, ClientConnectedEventArgs e)
        {
            Output.text("Client connected at " + e.IpPort);
            clientIps.Add(e.IpPort, new PlayerInfo("unknown", false));
        }
    }

    class PlayerInfo
    {
        public string name;
        public bool repeat;

        public PlayerInfo(string name, bool repeat)
        {
            this.name = name;
            this.repeat = repeat;
        }
    }
}
