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
        private List<byte> hostdata;

        private bool newServer;

        public Server(string ip) : base(ip)
        {
            newServer = true;

            try
            {
                server = new SimpleTcpServer(IpAddress, port);
            }
            catch (System.Net.Sockets.SocketException)
            {
                Program.displayError($"{IpAddress}:{port} is not a valid ip address");
                Program.EndProgram();
            }

            server.Events.ClientConnected += Events_ClientConnected;
            server.Events.ClientDisconnected += Events_ClientDisconnected;
            server.Events.DataReceived += Events_DataReceived;
            clientIps = new Dictionary<string, PlayerInfo>();
            mr = new MemoryReader();
            setServerToDefault();
        }

        private void compareHostAndPlayer(List<byte> playerData, string playerName)
        {
            int byteListIndex = 0;
            for (int locationListIndex = 0; locationListIndex < mr.memoryLocations.Count; locationListIndex++)
            {
                MemoryLocation memLoc = mr.memoryLocations[locationListIndex];
                uint hostNumber = ReadWrite.bigToLittleEndian(hostdata, byteListIndex, memLoc.size);
                uint playerNumber = ReadWrite.bigToLittleEndian(playerData, byteListIndex, memLoc.size);

                if (playerNumber != hostNumber)
                {
                    bool gotNewItem = false;

                    if (playerNumber >= memLoc.lowerValue && playerNumber <= memLoc.higherValue)
                    {
                        //Compare the two numbers and check which should be updated
                        switch (memLoc.compareId)
                        {
                            case 0: //greater than
                                if (playerNumber > hostNumber) gotNewItem = true;
                                    break;
                            case 1: //less than
                                if (playerNumber < hostNumber) gotNewItem = true;
                                break;
                            case 2: //greater than & not 255
                                if (hostNumber == 255 || playerNumber > hostNumber && playerNumber != 255) gotNewItem = true;
                                break;
                            case 3: //bottles - only send once
                                if (hostNumber == 255) gotNewItem = true;
                                break;
                            case 8: //different at all
                                if (playerNumber != hostNumber) gotNewItem = true;
                                break;
                            case 9: //bitfields - just to make sure bits aren't unset
                                if ((playerNumber & (playerNumber ^ hostNumber)) > 0) gotNewItem = true;
                                //Maybe bitwise or the playernumber
                                break;
                            default:
                                Program.displayError("Invalid compareId");
                                break;
                        }
                    }
                    else
                    {
                        Program.displayDebug("The value at 0x" + memLoc.startAddress.ToInt64().ToString("X") + " is not inside of an acceptable range (" + playerNumber + "). It was not synced to the host.", 2);
                    }

                    if (gotNewItem)
                    {
                        //Update the host value and send new notification
                        byte[] newValue = ReadWrite.littleToBigEndian(playerNumber, memLoc.size);
                        for (int i = 0; i < memLoc.size; i++)
                        {
                            hostdata[byteListIndex + i] = newValue[i];
                        }
                        sendNewMemoryLocation((short)locationListIndex, playerNumber, newValue, true);
                        calculateNotification(playerName, playerNumber, hostNumber, memLoc); 
                    }
                }

                byteListIndex += memLoc.size;
            }
        }

        //Determines whether or not to send a notification & calculates what it should be
        private void calculateNotification(string playerName, uint playerNumber, uint hostNumber, MemoryLocation memLoc)
        {
            if (memLoc.cd.bitfield)
            {
                //checks each bit and only sends notification if the player just set it
                for (int i = 0; i < memLoc.cd.values.Length; i++) 
                {
                    if (ReadWrite.bitSet(playerNumber, memLoc.cd.values[i]) && !ReadWrite.bitSet(hostNumber, memLoc.cd.values[i]))
                    {
                        processNotification(playerName, memLoc.cd.text[i]);
                    }
                }
                return;
            }

            if (memLoc.cd.values == null || memLoc.cd.text == null)
            {
                //If the memoryLocation only has one possible value
                if (memLoc.name != "")
                    processNotification(playerName, memLoc.name);
            }
            else
            {
                //compares the new value to everything in the list of possible values
                for (int i = 0; i < memLoc.cd.values.Length; i++)  
                {
                    if (playerNumber == memLoc.cd.values[i])
                    {
                        processNotification(playerName, memLoc.cd.text[i]);
                        return;
                    }
                }
            }
        }

        //Uses the itemText to actually send the notification
        private void processNotification(string playerName, string itemText)
        {
            if (itemText != "")
            {
                sendNotification(getNotificationText(playerName, itemText, true), false);
                sendNotification(getNotificationText(playerName, itemText, false), true);
            }
            else
                Program.displayError("Notification was unable to be calculated");
        }

        //Takes in the text stored in the memoryLocation and converts it to a full notification
        private string getNotificationText(string playerName, string itemText, bool yourself)
        {
            string[] strings = itemText.Split('*', 2);
            itemText = strings[0]; int formatId = -1;
            int.TryParse(strings[1], out formatId);
            string output = "";

            if (formatId == 0)
                output = "obtained the " + itemText;
            else if (formatId == 1)
                output = "obtained a " + itemText;
            else if (formatId == 2)
                output = "obtained " + itemText;
            else if (formatId == 3)
                output = "learned " + itemText;
            else if (formatId == 4)
                output = "deciphered " + itemText;
            else if (formatId == 5)
                output = "placed " + itemText;
            else if (formatId == 9)
                output = itemText;
            else
                output = "format id was wrong lol";

            if (yourself)
                return "You have " + output;
            else
                return playerName + " has " + output;
        }

        public override void Begin()
        {
            Start();
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
                Program.displayError($"Failed to start the server at {IpAddress}:{port}");
                Program.EndProgram();
            }
            Program.setConsoleColor(1);
            Console.WriteLine($"Server successfully started at {IpAddress}:{port}");
        }

        #region Send functions
        private void Send(string ip, List<byte> data, char dataType)
        {
            if (server.IsListening)
            {
                data.AddRange(new byte[] { 126, 126, Convert.ToByte(dataType) });
                server.Send(ip, data.ToArray());
                Program.displayDebug("Sending " + data.Count + " bytes", 2);
            }
        }

        public override void sendMemoryList(List<byte> memory)
        {
            Send(currIp, memory, 'm');
        }

        public override void sendNewMemoryLocation(short memLocIndex, uint previousValue, byte[] newValue, bool sendToAllButThis)
        {
            List<byte> toSend = new List<byte>();
            toSend.AddRange(BitConverter.GetBytes(memLocIndex));
            //toSend.AddRange(BitConverter.GetBytes(previousValue));
            toSend.AddRange(newValue);

            if (sendToAllButThis)
            {
                foreach (string ip in clientIps.Keys)
                    if (ip != currIp)
                        Send(ip, toSend, 'v');
            }
            else
            {
                Send(currIp, toSend, 'v');
            }
        }

        public override void sendNotification(string notification, bool sendToAllButThis)
        {
            List<byte> toSend = new List<byte>(Encoding.UTF8.GetBytes(notification));
            if (sendToAllButThis)
            {
                foreach (string ip in clientIps.Keys)
                    if (ip != currIp)
                        Send(ip, toSend, 'n');
            }
            else
            {
                Send(currIp, toSend, 'n');
            }
        }

        public override void sendTextMessage(string message)
        {
            List<byte> toSend = new List<byte>(Encoding.UTF8.GetBytes(message));
            foreach (string ip in clientIps.Keys)
                if (ip != currIp)
                    Send(ip, toSend, 't');
        }

        public override void sendIntroData()
        {
            List<byte> toSend = new List<byte>(Encoding.UTF8.GetBytes(Program.currGame.getSyncSettings()));
            Send(currIp, toSend, 'i');
        }
        #endregion

        #region Receive functions
        //type 'm' - reads player name & memory list and compares this to host data
        protected override void receiveMemoryList(List<byte> playerData)
        {
            if (playerData == null || playerData.Count < 1)
                Program.displayError("byte[] received from client is null or empty");

            string playerName = seperatePlayerAndData(playerData);

            if (playerData.Count == hostdata.Count)
            {
                if (newServer)
                {
                    //If this is the first player to join, copy their memory to the host
                    hostdata = playerData;
                    newServer = false;
                    clientIps[currIp].repeat = true;
                }
                else if (!clientIps[currIp].repeat)
                {
                    //If this player is just now syncing with the server, send them the hostData to copy
                    sendMemoryList(hostdata);
                    clientIps[currIp].repeat = true;
                }
                else if (!ReadWrite.checkIfSame(playerData, hostdata))
                {
                    //Otherwise, if they're different, do these - save to host list, save to player memory, update Stage info, send notifications
                    compareHostAndPlayer(playerData, playerName);
                }
                else
                    Program.displayDebug("No differences between host and " + playerName, 1);
            }
            else
                Program.displayError("Host data & player data are different sizes");
        }

        //type 't' - reads player name & message and sends it to everybody else
        protected override void receiveTextMessage(List<byte> data)
        {
            sendTextMessage(Encoding.UTF8.GetString(data.ToArray()));
        }

        //type 'n' - read notification and send it to everyone else
        protected override void receiveNotification(List<byte> data)
        {
            string message = Encoding.UTF8.GetString(data.ToArray());
            sendNotification(message, true);
        }

        //type 'd' - reads the string and converts it to a long & displays it
        protected override void receiveDelayTest(List<byte> data)
        {
            long sendTime = BitConverter.ToInt64(data.ToArray());
            long timeDelta = DateTime.Now.Ticks - sendTime;
            Program.setConsoleColor(4);
            Console.WriteLine("Byte[] received from " + currIp + " came with a delay of " + (timeDelta / 10000) + " milliseconds");
        }
        //type 'i' - reads the player name and sets it in the dictionary, then sends the sync settings
        protected override void receiveIntroData(List<byte> data)
        {
            string name = Encoding.UTF8.GetString(data.ToArray());
            clientIps[currIp].name = name;
            sendIntroData();
        }
        #endregion

        private void Events_ClientDisconnected(object sender, ClientDisconnectedEventArgs e)
        {
            Program.setConsoleColor(1);
            Console.WriteLine("Client disconnected at " + e.IpPort);
            clientIps.Remove(e.IpPort);
        }

        private void Events_ClientConnected(object sender, ClientConnectedEventArgs e)
        {
            Program.setConsoleColor(1);
            Console.WriteLine("Client connected at " + e.IpPort);
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
