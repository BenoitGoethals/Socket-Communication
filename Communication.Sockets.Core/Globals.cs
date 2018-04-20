using System.Net.Sockets;

namespace Chat.Core
{
    public delegate void MessageRecivedDelegate(Socket socket, byte[] message);
    public delegate void ConnectDelegate(Socket socket);
    public delegate void DisconnectDelegate(Socket socket);
}
