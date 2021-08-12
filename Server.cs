using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using SimpleTcp;

namespace Windwaker_coop
{
    class Server
    {
        public string IpAddress;
        public int port;
        private List<byte> hostdata;

        private SimpleTcpServer server;
        private MemoryReader mr;
        private List<string> clientIps;

        public Server(string ip, int port)
        {
            IpAddress = ip;
            this.port = port;
            server = new SimpleTcpServer(IpAddress, port);
            server.Events.ClientConnected += Events_ClientConnected;
            server.Events.ClientDisconnected += Events_ClientDisconnected;
            server.Events.DataReceived += Events_DataReceived;
            clientIps = new List<string>();
            hostdata = mr.getDefaultValues();
            mr = new MemoryReader();
        }

        private void Events_DataReceived(object sender, DataReceivedEventArgs e)
        {
            //string[] message = Encoding.UTF8.GetString(e.Data).Split('~');
            //Console.WriteLine(message[0] + ": " + message[1]);

            Console.WriteLine("Bytes received: " + e.Data.Length);
            List<byte> playerData = new List<byte>(e.Data);
            if (playerData != null)
            {
                if (playerData.Count != hostdata.Count)
                {
                    compareHostAndPlayer(playerData);
                }
                else
                    Program.displayError("Host data & player data are different sizes");
            }
            else
                Program.displayError("byte[] received from client is null");
        }

        public void Start()
        {
            server.Start();
            Console.WriteLine("Server started");
        }

        public void Send(string message)
        {
            if (server.IsListening && clientIps.Count > 0)
            {
                server.Send(clientIps[0], message);
            }
        }

        private void compareHostAndPlayer(List<byte> playerData)
        {
            //if new data add it to host list
            //send new memoryLocation to everyone & notification
        }

        private void Events_ClientDisconnected(object sender, ClientDisconnectedEventArgs e)
        {
            Console.WriteLine("Client disconnected at " + e.IpPort);
            clientIps.Remove(e.IpPort);
        }

        private void Events_ClientConnected(object sender, ClientConnectedEventArgs e)
        {
            Console.WriteLine("Client connected at " + e.IpPort);
            clientIps.Add(e.IpPort);
        }
    }
}
