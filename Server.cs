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
            server = new SimpleTcpServer(ip, port);
            server.Events.ClientConnected += Events_ClientConnected;
            server.Events.ClientDisconnected += Events_ClientDisconnected;
            server.Events.DataReceived += Events_DataReceived;
            clientIps = new List<string>();
            mr = new MemoryReader();
            hostdata = mr.getDefaultValues();
        }

        private void compareHostAndPlayer(List<byte> playerData, string playerName)
        {
            //reverse byte endian first!!
            //if new data add it to host list
            //send new memoryLocation to everyone & notification
            //now has to check if player has new number and if host has new number
            //maybe between low/high values

            int byteListIndex = 0;
            uint hostNumber = 0;
            uint playerNumber = 0;
            uint newNumber = 0;

            for (int locationListIndex = 0; locationListIndex < mr.memoryLocations.Count; locationListIndex++)
            {
                MemoryLocation memLoc = mr.memoryLocations[locationListIndex];
                hostNumber = getNumberFromByteList(hostdata, byteListIndex, memLoc.size);
                playerNumber = getNumberFromByteList(playerData, byteListIndex, memLoc.size);
                newNumber = 0;

                if (playerNumber != hostNumber)
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
                        default:
                            //Program.displayError("Invalid compareId");
                            break;
                    }
                    //If player has gotten a new item, send out new memoryLocation to everyone else along with notification
                    if (gotNewItem)
                    {
                        byte[] newValue = getByteArrayFromNumber(newNumber, memLoc.size);
                        hostdata.RemoveRange(byteListIndex, memLoc.size);
                        hostdata.InsertRange(byteListIndex, newValue);
                        sendNewMemoryLocation((short)locationListIndex, newNumber, true);
                        sendNotification(playerName + " has gotten the " + memLoc.name, true);
                        sendNotification("You have gotten the " + memLoc.name, false);
                    }
                }

                byteListIndex += memLoc.size;
            }
        }

        public void Start()
        {
            server.Start();
            Program.setConsoleColor(1);
            Console.WriteLine("Server started at " + IpAddress);
        }

        #region Send functions
        public void Send(string ip, byte[] data)
        {
            if (server.IsListening)
            {
                server.Send(ip, data);
                Program.displayDebug("Sending " + data.Length + " bytes", 2);
            }
        }

        protected override void sendNewMemoryLocation(short memLocIndex, uint newValue, bool sendToAllButThis)
        {
            List<byte> toSend = new List<byte>();
            toSend.AddRange(BitConverter.GetBytes(memLocIndex));
            toSend.AddRange(getByteArrayFromNumber(newValue, mr.memoryLocations[memLocIndex].size));
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
        protected override void sendNotification(string notification, bool sendToAllButThis)
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
