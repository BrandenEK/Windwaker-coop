using System;
using System.Collections.Generic;
using System.Text;
using SimpleTcp;

namespace Windwaker_coop
{
    abstract class User
    {
        public string IpAddress;
        public int port;
        public MemoryReader mr { get; protected set; }

        private Dictionary<byte, Action<byte[]>> receiveDataFunctions = new Dictionary<byte, Action<byte[]>>();

        public User(string ip)
        {
            //sets the ipAddress and port variables - separates them if combined
            int colon = ip.IndexOf(':');
            if (colon >= 0)
            {
                IpAddress = ip.Substring(0, colon);
                port = int.Parse(ip.Substring(colon + 1));
            }
            else
            {
                IpAddress = ip;
                port = Program.config.defaultPort;
            }

            receiveDataFunctions.Add(100, receiveDelayTest); //d
            receiveDataFunctions.Add(105, receiveIntroData); //i
            receiveDataFunctions.Add(109, receiveMemoryList); //m
            receiveDataFunctions.Add(110, receiveNotification); //n
            receiveDataFunctions.Add(116, receiveTextMessage); //t
            receiveDataFunctions.Add(118, receiveNewMemoryLocation); //v
        }

        //Each data message should be in the form [ length1 length2 type data data data ... ]
        protected virtual void Events_DataReceived(object sender, DataReceivedEventArgs e)
        {
            Output.debug("Bytes received: " + e.Data.Length, 1);

            int startIdx = 0;
            while (startIdx < e.Data.Length - 3)
            {
                ushort length = BitConverter.ToUInt16(e.Data, startIdx);
                byte type = e.Data[startIdx + 2];
                byte[] messageData = e.Data[(startIdx + 3)..(startIdx + 3 + length)];

                processDataReceived(type, messageData);
                startIdx += 3 + length;
            }
            if (startIdx != e.Data.Length)
                Output.error("Received data was formatted incorrectly");
        }

        private void processDataReceived(byte type, byte[] data)
        {
            if (!receiveDataFunctions.ContainsKey(type))
            {
                Output.error("Unrecognized data type (d, i, m, n, t, v)");
                return;
            }
            receiveDataFunctions[type](data);
        }

        protected byte[] calculateMessageToSend(byte[] message, char type)
        {
            List<byte> d = new List<byte>(BitConverter.GetBytes((ushort)message.Length));
            d.Add(Convert.ToByte(type));
            d.AddRange(message);

            Output.debug("Sending " + d.Count + " bytes", 1);
            return d.ToArray();
        }

        //Send new data functions
        public virtual void sendMemoryList(byte[] memory)
        {
            Output.error("sendMemoryList() not implemented here");
        }
        public virtual void sendNewMemoryLocation(byte writeType, ushort memLocIndex, uint oldValue, uint newValue, bool sendToAllButThis)
        {
            Output.error("sendNewMemoryLocation() not implemented here");
        }
        public virtual void sendTextMessage(string message)
        {
            Output.error("sendTextMessage() not implemented here");
        }
        public virtual void sendNotification(string notification, bool sendToAllButThis)
        {
            Output.error("sendNotification() not implemented here");
        }
        public virtual void sendDelayTest()
        {
            Output.error("sendDelayTest() not implemented here");
        }
        public virtual void sendIntroData() //add params
        {
            Output.error("sendIntroData() not implemented here");
        }

        //Receive new data functions
        protected virtual void receiveMemoryList(byte[] data)
        {
            Output.error("receiveMemoryList() not implemented here");
        }
        protected virtual void receiveNewMemoryLocation(byte[] data)
        {
            Output.error("receiveNewMemoryLocation() not implemented here");
        }
        protected virtual void receiveTextMessage(byte[] data)
        {
            Output.error("receiveTextMessage() not implemented here");
        }
        protected virtual void receiveNotification(byte[] data)
        {
            Output.error("receiveNotification() not implemented here");
        }
        protected virtual void receiveDelayTest(byte[] data)
        {
            Output.error("receiveDelayTest() not implemented here");
        }
        protected virtual void receiveIntroData(byte[] data)
        {
            Output.error("receiveIntroData() not implemented here");
        }

        public abstract string processCommand(string command, string[] args);
    }
}
