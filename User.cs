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
        protected string currIp = "";

        protected virtual void Events_DataReceived(object sender, DataReceivedEventArgs e)
        {
            Program.displayDebug("Bytes received: " + e.Data.Length, 2);
            currIp = e.IpPort;

            List<byte> newData = new List<byte>(e.Data);
            for (int i = 0; i < newData.Count; i++)
            {
                if (i < newData.Count - 2 && newData[i] == 126 && newData[i + 1] == 126)
                {
                    //If index is the start of a ~~x section
                    byte type = newData[i + 2];
                    List<byte> singleData = newData.GetRange(0, i);
                    newData.RemoveRange(0, i + 3);
                    processDataReceived(type, singleData);
                    i = -1;
                }
            }
            if (newData.Count > 0)
                Program.displayError("Received data was formatted incorrectly");
        }

        private void processDataReceived(byte type, List<byte> data)
        {
            if (type == 109) //m
                receiveMemoryList(data);
            else if (type == 110) //n
                receiveNotification(data);
            else if (type == 116) //t
                receiveTextMessage(data);
            else if (type == 118) //v
                receiveNewMemoryLocation(data);
            else
                Program.displayError("Unrecognized data type (m, n, t, v)");
        }

        //Send new data functions
        public virtual void sendMemoryList(List<byte> memory)
        {
            Program.displayError("sendMemoryList() not implemented here");
        }
        public virtual void sendNewMemoryLocation(short memLocIndex, uint newValue, bool sendToAllButThis)
        {
            Program.displayError("sendNewMemoryLocation() not implemented here");
        }
        public virtual void sendTextMessage(string message)
        {
            Program.displayError("sendTextMessage() not implemented here");
        }
        public virtual void sendNotification(string notification, bool sendToAllButThis)
        {
            Program.displayError("sendNotification() not implemented here");
        }

        //Receive new data functions
        protected virtual void receiveMemoryList(List<byte> data)
        {
            Program.displayError("receiveMemoryList() not implemented here");
        }
        protected virtual void receiveNewMemoryLocation(List<byte> data)
        {
            Program.displayError("receiveNewMemoryLocation() not implemented here");
        }
        protected virtual void receiveTextMessage(List<byte> data)
        {
            Program.displayError("receiveTextMessage() not implemented here");
        }
        protected virtual void receiveNotification(List<byte> data)
        {
            Program.displayError("receiveNotification() not implemented here");
        }

        public abstract void Begin();

        //Returns the player name from the messsage & removes it from the list leaving only the data
        protected string seperatePlayerAndData(List<byte> data)
        {
            int sepChar = -1;
            for (int i = 0; i < data.Count; i++)
            {
                if (data[i] == 126) //~
                {
                    sepChar = i;
                    break;
                }
            }

            if (sepChar < 1)
            {
                Program.displayError("Received data was not formatted correctly");
                return null;
            }

            byte[] nameArray = data.GetRange(0, sepChar).ToArray();
            data.RemoveRange(0, sepChar + 1);
            return Encoding.UTF8.GetString(nameArray);
        }

        protected string getNotificationText(string playerName, string itemText, bool yourself)
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
                output = "learned the " + itemText;
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

        public uint getNumberFromByteList(List<byte> list, int startIndex, int length)
        {
            byte[] bytes = new byte[4];
            string debugOuput = "Converting byte[] { ";
            for (int i = 0; i < length; i++)
            {
                bytes[length - 1 - i] = list[startIndex + i];
                debugOuput += list[startIndex + i].ToString("X") + " ";
            }
            Program.displayDebug(debugOuput + "} to integer: " + BitConverter.ToUInt32(bytes), 4);
            return BitConverter.ToUInt32(bytes);
        }

        public byte[] getByteArrayFromNumber(uint number, int length)
        {
            byte[] fourByte = BitConverter.GetBytes(number);
            byte[] result = new byte[length];

            for (int i = 0; i < length; i++)
            {
                result[length - 1 - i] = fourByte[i];
            }
            return result;
        }
    }
}
