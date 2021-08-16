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
        protected MemoryReader mr;

        protected virtual void Events_DataReceived(object sender, DataReceivedEventArgs e)
        {
            Program.displayDebug("Bytes received: " + e.Data.Length, 2);
            byte type = e.Data[e.Data.Length - 1];
            List<byte> newData = new List<byte>(e.Data);
            newData.RemoveAt(newData.Count - 1);

            if (type == 109) //m
                receiveMemoryList(newData);
            else if (type == 110) //n
                receiveNotification(newData);
            else if (type == 116) //t
                receiveTextMessage(newData);
            else if (type == 118) //v
                receiveNewMemoryLocation(newData);
            else
                Program.displayError("Unrecognized data type (m, n, t, v)");
        }

        //Send new data functions
        protected virtual void sendMemoryList(List<byte> memory)
        {
            Program.displayError("sendMemoryList() not implemented here");
        }
        protected virtual void sendNewMemoryLocation(uint memLocIndex, uint newValue)
        {
            Program.displayError("sendNewMemoryLocation() not implemented here");
        }
        protected virtual void sendTextMessage(string message)
        {
            Program.displayError("sendTextMessage() not implemented here");
        }
        protected virtual void sendNotification(string notification)
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

        protected uint getNumberFromByteList(List<byte> list, int startIndex, int length)
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

        protected byte[] getByteArrayFromNumber(uint number, int length)
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
