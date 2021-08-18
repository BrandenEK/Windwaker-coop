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

        private void compareHostAndPlayer(List<byte> playerData)
        {
            //reverse byte endian first!!
            //if new data add it to host list
            //send new memoryLocation to everyone & notification
            //now has to check if player has new number and if host has new number
            //maybe between low/high values

            int byteListIndex = 0;
            uint hostNumber = 0;
            uint playerNumber = 0;

            for (int locationListIndex = 0; locationListIndex < mr.memoryLocations.Count; locationListIndex++)
            {
                MemoryLocation memLoc = mr.memoryLocations[locationListIndex];
                hostNumber = getNumberFromByteList(hostdata, byteListIndex, memLoc.size);
                playerNumber = getNumberFromByteList(playerData, byteListIndex, memLoc.size);

                if (playerNumber != hostNumber)
                {
                    switch (memLoc.compareId)
                    {
                        case 0:
                            if (playerNumber > hostNumber)
                            {
                                //player --> host
                            }
                            else
                            {
                                //host --> player
                            }
                            break;
                        default:
                            Program.displayError("Invalid compareId");
                            break;
                    }
                }

                byteListIndex += memLoc.size;
            }

            sendNotification("Server is giving out more max hearts");
            sendNewMemoryLocation(2, 5);
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

        protected override void sendNewMemoryLocation(short memLocIndex, uint newValue)
        {
            List<byte> toSend = new List<byte>();
            toSend.AddRange(BitConverter.GetBytes(memLocIndex));
            toSend.AddRange(getByteArrayFromNumber(newValue, mr.memoryLocations[memLocIndex].size));
            toSend.Add(118);
            byte[] toSendArray = toSend.ToArray();

            foreach (string ip in clientIps)
            {
                //if (ip != currIp) //some data will go to curr ip, some will go to only others
                    Send(ip, toSendArray);
            }
        }
        protected override void sendNotification(string notification)
        {
            foreach (string ip in clientIps)
            {
                //send name notification to else, send you to this player
                Send(ip, Encoding.UTF8.GetBytes(notification + 'n'));
            }
        }
        #endregion

        #region Receive functions
        //type 'm' - reads player name & memory list and compares this to host data
        protected override void receiveMemoryList(List<byte> playerData)
        {
            if (playerData == null || playerData.Count < 1)
                Program.displayError("byte[] received from client is null or empty");

            string player = seperatePlayerAndData(playerData);

            if (playerData.Count == hostdata.Count)
            {
                if (!ReadWrite.checkIfSame(playerData, hostdata))
                {
                    compareHostAndPlayer(playerData);
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
