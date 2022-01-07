using System;
using System.Collections.Generic;
using System.Text;
using SimpleTcp;

namespace Windwaker_coop
{
    class Server : User
    {
        private SimpleTcpServer server;
        private List<byte> hostdata;
        private List<string> clientIps;

        public Server(string ip, int port)
        {
            IpAddress = ip;
            this.port = port;
            try
            {
                server = new SimpleTcpServer(ip, port);
            }
            catch (System.Net.Sockets.SocketException)
            {
                Program.displayError(IpAddress + " is not a valid ip address");
                Program.EndProgram();
            }

            server.Events.ClientConnected += Events_ClientConnected;
            server.Events.ClientDisconnected += Events_ClientDisconnected;
            server.Events.DataReceived += Events_DataReceived;
            clientIps = new List<string>();
            mr = new MemoryReader();
            setServerToDefault();
        }

        public override void Begin()
        {
            Start();
        }

        private void compareHostAndPlayer(List<byte> playerData, string playerName)
        {
            //now has to check if player has new number and if host has new number
            //maybe between low/high values

            int byteListIndex = 0;
            for (int locationListIndex = 0; locationListIndex < mr.memoryLocations.Count; locationListIndex++)
            {
                MemoryLocation memLoc = mr.memoryLocations[locationListIndex];
                uint hostNumber = ReadWrite.bigToLittleEndian(hostdata, byteListIndex, memLoc.size);
                uint playerNumber = ReadWrite.bigToLittleEndian(playerData, byteListIndex, memLoc.size);
                uint newNumber = 0;

                if (playerNumber != hostNumber)
                {
                    if (playerNumber >= memLoc.lowerValue && playerNumber <= memLoc.higherValue)
                    {
                        bool gotNewItem = false;
                        switch (memLoc.compareId)
                        {
                            //Checks if the player has gotten a new item & sets the newNumber & calculates notification
                            case 0:
                                if (playerNumber > hostNumber)
                                {
                                    //player --> host & other players
                                    newNumber = playerNumber;
                                    gotNewItem = true;
                                }
                                else
                                {
                                    //host --> player
                                    //sendNewMemoryLocation((short)locationListIndex, hostNumber, false);
                                    //sendNotification("You do not have the " + memLoc.name, false);
                                }
                                break;
                            case 1:
                                if (playerNumber < hostNumber)
                                {
                                    newNumber = playerNumber;
                                    gotNewItem = true;
                                }
                                break;
                            case 2:
                                if (hostNumber == 255 || playerNumber > hostNumber && playerNumber != 255)
                                {
                                    newNumber = playerNumber;
                                    gotNewItem = true;
                                }
                                break;
                            case 3:
                                if (hostNumber == 255)
                                {
                                    newNumber = playerNumber;
                                    gotNewItem = true;
                                }
                                break;
                            case 8:
                                if (playerNumber != hostNumber)
                                {
                                    newNumber = playerNumber;
                                    gotNewItem = true;
                                }
                                break;
                            case 9:
                                if ((playerNumber & (playerNumber ^ hostNumber)) > 0)
                                {
                                    newNumber = playerNumber | hostNumber;
                                    gotNewItem = true;
                                }
                                break;
                            default:
                                Program.displayError("Invalid compareId");
                                break;
                        }
                        //If player has gotten a new item, send out new memoryLocation to everyone else along with notification
                        if (gotNewItem)
                        {
                            //Reset individual bits and don't send new memory location if only idv bits changed
                            /*uint idvNumber = newNumber;
                            if (memLoc.individualBits != 0 && (memLoc.individualBits != 255 || memLoc.individualBits == 255 && playerNumber != 255)) //used for windwaker bottles
                            {
                                for (int i = 0; i < 32; i++) //for every bit...
                                {
                                    uint mask = (uint)1 << i;
                                    if ((memLoc.individualBits & mask) != 0) //if this bit should not be synced...
                                    {
                                        idvNumber = (idvNumber & ~mask) | (playerNumber & mask);
                                    }
                                }
                                Program.displayDebug("Kept the item " + memLoc.name.Substring(0, memLoc.name.Length - 2) + " as 0d" + idvNumber + " instead of 0d" + newNumber, 2);
                            }*/
                            if (memLoc.type == "test")  // Just for testing to see value changes
                            {
                                Program.setConsoleColor(3);
                                Console.WriteLine("Address 0x" + memLoc.startAddress.ToInt64().ToString("X") + " changed from " + Convert.ToString(hostNumber, 2).PadLeft(32, '0') + " (host) to " + Convert.ToString(newNumber, 2).PadLeft(32, '0'));
                            }

                            //Convert new number to byte[] and store/send it
                            byte[] newValue = ReadWrite.littleToBigEndian(newNumber, memLoc.size);
                            for (int i = 0; i < memLoc.size; i++)
                            {
                                hostdata[byteListIndex + i] = newValue[i];
                            }
                            sendNewMemoryLocation((short)locationListIndex, newValue, true);
                            calculateNotification(playerName, newNumber, hostNumber, memLoc);
                        }
                    }
                    else
                    {
                        Program.displayDebug("The value at 0x" + memLoc.startAddress.ToInt64().ToString("X") + " is not inside of an acceptable range (" + playerNumber + "). It was not synced to the host.", 2);
                    }
                }

                byteListIndex += memLoc.size;
            }
        }

        //Determines whether or not to send a notification & calculates what it should be
        private void calculateNotification(string playerName, uint newNumber, uint hostNumber, MemoryLocation memLoc)
        {
            string itemText = "";

            if (memLoc.cd.values == null || memLoc.cd.text == null)
            {
                //If the memoryLocation only has one possible value
                if (memLoc.name != "")
                    itemText = memLoc.name;
                else
                    return;
            }
            else
            {
                //If the memoryLocation has different possible levels, get exact notification
                if (memLoc.cd.bitfield)
                {
                    for (int i = 0; i < memLoc.cd.values.Length; i++) //checks each bit and only sends notification if the player just set it
                    {
                        if (ReadWrite.bitSet(newNumber, memLoc.cd.values[i]) && !ReadWrite.bitSet(hostNumber, memLoc.cd.values[i]))
                        {
                            itemText = memLoc.cd.text[i];
                            //Only one bitfield notification can be sent at once - this could be a problem in the future
                            break;
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < memLoc.cd.values.Length; i++)  //compares the new value to everything in the list of possible values
                    {
                        if (newNumber == memLoc.cd.values[i])
                        {
                            itemText = memLoc.cd.text[i];
                            break;
                        }
                    }
                }
            }
            //use the itemText to actually send the notification
            if (itemText != "")
            {
                sendNotification(getNotificationText(playerName, itemText, true), false);
                sendNotification(getNotificationText(playerName, itemText, false), true);
            }
            else
                Program.displayError("Notification was unable to be calculated");
        }

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
                Program.displayError("Failed to start the server at " + IpAddress);
                Program.EndProgram();
            }
            Program.setConsoleColor(1);
            Console.WriteLine("Server started at " + IpAddress);
        }

        #region Send functions
        private void Send(string ip, byte[] data)
        {
            if (server.IsListening)
            {
                server.Send(ip, data);
                Program.displayDebug("Sending " + data.Length + " bytes", 2);
            }
        }

        public override void sendNewMemoryLocation(short memLocIndex, byte[] newValue, bool sendToAllButThis)
        {
            List<byte> toSend = new List<byte>();
            toSend.AddRange(BitConverter.GetBytes(memLocIndex));
            toSend.AddRange(newValue);
            toSend.AddRange(new byte[] { 126, 126, 118 });
            byte[] toSendArray = toSend.ToArray();

            if (sendToAllButThis)
            {
                foreach (string ip in clientIps)
                    if (ip != currIp)
                        Send(ip, toSendArray);
            }
            else
            {
                Send(currIp, toSendArray);
            }
        }

        public override void sendNotification(string notification, bool sendToAllButThis)
        {
            if (sendToAllButThis)
            {
                foreach (string ip in clientIps)
                    if (ip != currIp)
                        Send(ip, Encoding.UTF8.GetBytes(notification + "~~n"));
            }
            else
            {
                Send(currIp, Encoding.UTF8.GetBytes(notification + "~~n"));
            }
        }

        public override void sendTextMessage(string message)
        {
            foreach (string ip in clientIps)
                if (ip != currIp)
                    Send(ip, Encoding.UTF8.GetBytes(message + "~~t"));
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
                if (!ReadWrite.checkIfSame(playerData, hostdata))
                {
                    compareHostAndPlayer(playerData, playerName);
                    //save to host list, save to player memory, update Stage info, send notifications
                }
                else
                    Program.displayDebug("No difference between host and this player", 1);
            }
            else
                Program.displayError("Host data & player data are different sizes");
        }

        //type 't' - reads player name & message and sends it to everybody else
        protected override void receiveTextMessage(List<byte> data)
        {
            sendTextMessage(Encoding.UTF8.GetString(data.ToArray()));
        }

        //type 'n' - read notification an send it to everyone else
        protected override void receiveNotification(List<byte> data)
        {
            string message = Encoding.UTF8.GetString(data.ToArray());
            sendNotification(message, true);
        }

        //type 'd' - reads the string and converts it to a long & displays it
        protected override void receiveDelayTest(List<byte> data)
        {
            string str = Encoding.UTF8.GetString(data.ToArray());
            long timeDelta = DateTime.Now.Ticks - long.Parse(str);
            Program.setConsoleColor(4);
            Console.WriteLine("Byte[] received from " + currIp + " came with a delay of " + (timeDelta / 10000) + " milliseconds");
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
            clientIps.Add(e.IpPort);
        }
    }
}
