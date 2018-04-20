using System;
using System.Net.Sockets;

namespace Chat.Core
{
    public class ConnectedClient
    {
        private Socket clientSocket;
        private SocketListener socketListener;

        public event MessageRecivedDelegate MessageRecived
        {
            add
            {
                socketListener.MessageRecived += value;
            }
            remove
            {
                socketListener.MessageRecived -= value;
            }
        }
        public event DisconnectDelegate Disconnected
        {
            add
            {
                socketListener.Disconnected += value;
            }
            remove
            {
                socketListener.Disconnected -= value;
            }
        }

        public ConnectedClient(Socket clientSocket)
        {
            this.clientSocket = clientSocket;
            socketListener = new SocketListener();
        }

        public void StartListen()
        {
            socketListener.StartReciving(clientSocket);
        }

        public void Send(byte[] buffer)
        {
            if (clientSocket == null)
            {
                throw new Exception("Can't send data. ConnectedClient is Closed!");
            }
            clientSocket.Send(buffer);
            
        }

        public void Stop()
        {
            socketListener.StopListening();
            clientSocket = null;
        }
    }
}
