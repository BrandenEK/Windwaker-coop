using System.Text;

namespace Windwaker.Multiplayer.Server.Network
{
    public static class NetworkExtensions
    {
        public static byte[] Serialize(this string str)
        {
            byte[] stringBytes = Encoding.UTF8.GetBytes(str);
            byte[] outputBytes = new byte[str.Length + 1];

            outputBytes[0] = (byte)str.Length;
            for (int i = 0; i < stringBytes.Length; i++)
            {
                outputBytes[i + 1] = stringBytes[i];
            }

            return outputBytes;
        }

        public static string Deserialize(this byte[] bytes, int start, out byte length)
        {
            length = (byte)(bytes[start] + 1);

            return length == 1 ? string.Empty : Encoding.UTF8.GetString(bytes, start + 1, length - 1);
        }
    }
}
