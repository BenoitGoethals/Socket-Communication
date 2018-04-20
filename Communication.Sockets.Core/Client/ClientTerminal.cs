using System.Net;
using System.Net.Sockets;

namespace Chat.Core
{
    public class ClientTerminal
    {
        Socket socketClient;
        private SocketListener socketListener;

        public event MessageRecivedDelegate MessageRecived;
        public event ConnectDelegate Connected;
        public event DisconnectDelegate Disconncted;

        public void Connect(IPAddress remoteIPAddress, int alPort)
        {
            //create a new client socket ...
            socketClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint remoteEndPoint = new IPEndPoint(remoteIPAddress, alPort);
            
            // Connect
            socketClient.Connect(remoteEndPoint);

            OnServerConnection();
        }
        
        public void SendMessage(string message)
        {
            if (socketClient == null)
            {
                return;
                }
            byte[] byData = System.Text.Encoding.ASCII.GetBytes(message);
            socketClient.Send(byData);
        }

        public void SendMessage(byte[] byData)
        {
            socketClient.Send(byData);

        }

        public void StartListen()
        {
            if (socketClient == null)
            {
                return;
            }

            if (socketListener != null)
            {
                return;
            }

            socketListener = new SocketListener();
            socketListener.Disconnected += OnServerConnectionDroped;
            socketListener.MessageRecived += OnMessageRecvied;
            
            socketListener.StartReciving(socketClient);
        }

        public string ReadData()
        {
            if (socketClient == null)
            {
                return string.Empty;
            }

            byte[] buffer = new byte[1024];
            int iRx = socketClient.Receive(buffer);
            char[] chars = new char[iRx];

            System.Text.Decoder d = System.Text.Encoding.UTF8.GetDecoder();
            d.GetChars(buffer, 0, iRx, chars, 0);
            System.String szData = new System.String(chars);

            return szData;
        }

        public void Close()
        {
            if (socketClient == null)
            {
                return;
            }

            if (socketListener != null)
            {
                socketListener.StopListening();
            }

            socketClient.Close();
            socketListener = null;
            socketClient = null;
        }

        private void OnServerConnection()
        {
            if (Connected != null)
            {
                Connected(socketClient);
            }
        }

        private void OnMessageRecvied(Socket socket, byte[] message)
        {
            if (MessageRecived != null)
            {
                MessageRecived(socket, message);
            }
        }

        private void OnServerConnectionDroped(Socket socket)
        {
            Close();
            RaiseServerDisconnected(socket);
        }

        private void RaiseServerDisconnected(Socket socket)
        {
            if (Disconncted != null)
            {
                Disconncted(socket);
            }
        }


    }
}
