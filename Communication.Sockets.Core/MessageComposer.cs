using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Chat.Core
{
    public class MessageComposer
    {
        public static byte[] Serialize(int messageKind, MessageBase msg)
        {
            MemoryStream memoryStream = new MemoryStream();
            BinaryFormatter binaryFormatter = new BinaryFormatter();

            binaryFormatter.Serialize(memoryStream, messageKind);
            binaryFormatter.Serialize(memoryStream, msg);

            return memoryStream.ToArray();
        }

        public static void Deserialize(byte[] buffer, 
            out int messageKind, out MessageBase msg)
        {
            MemoryStream ms = new MemoryStream(buffer);
            BinaryFormatter formatter = new BinaryFormatter();
            messageKind = (int)formatter.Deserialize(ms);
            msg = (MessageBase)formatter.Deserialize(ms);
        }
    }
}
