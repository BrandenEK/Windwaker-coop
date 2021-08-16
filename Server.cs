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
            sendNotification("Server is giving out 10 max hearts");
            sendNewMemoryLocation(2, 40);
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

        protected override void sendNewMemoryLocation(uint memLocIndex, uint newValue)
        {
            List<byte> toSend = new List<byte>();
            toSend.AddRange(BitConverter.GetBytes(memLocIndex));
            toSend.AddRange(BitConverter.GetBytes(newValue));
            toSend.Add(118);
            byte[] toSendArray = toSend.ToArray();

            foreach (byte b in toSend)
                Console.Write(b + " ");

            foreach (string ip in clientIps)
            {
                Send(ip, toSendArray);
            }
        }
        protected override void sendNotification(string notification)
        {
            foreach (string ip in clientIps)
            {
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
