
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;

namespace Chat.Core
{
    public class ServerTerminal
    {
        
        public event ConnectDelegate ClientConnect;
        public event DisconnectDelegate ClientDisconnect;
        public event MessageRecivedDelegate MessageRecived ;

        private Socket m_socket;
        private bool m_Closed;

        private Dictionary<long, ConnectedClient> m_clients = 
            new Dictionary<long, ConnectedClient>();

    
        public void StartListen(int port)
        {
            IPEndPoint ipLocal = new IPEndPoint(IPAddress.Any, port);

            m_socket = new Socket(AddressFamily.InterNetwork, 
                SocketType.Stream, ProtocolType.Tcp);
            
            //bind to local IP Address...
            //if ip address is allready being used write to log
            try
            {
                m_socket.Bind(ipLocal);
            }
            catch(Exception ex)
            {
                Debug.Fail(ex.ToString(),
                    string.Format("Can't connect to port {0}!", port));
                
                return;
            }
            //start listening...
            m_socket.Listen(4);
            // create the call back for any client connections...
            m_socket.BeginAccept(new AsyncCallback(OnClientConnection), null);
            
        }

        private void OnClientConnection(IAsyncResult asyn)
        {
            if (m_Closed)
            {
                return;
            }

            try
            {
                Socket clientSocket = m_socket.EndAccept(asyn);

                RaiseClientConnected(clientSocket);
                
                ConnectedClient connectedClient = new ConnectedClient(clientSocket);

                connectedClient.MessageRecived += OnMessageRecived;
                connectedClient.Disconnected += OnClientDisconnection;

                connectedClient.StartListen();

                long key = clientSocket.Handle.ToInt64();
                if (m_clients.ContainsKey(key))
                {
                    Debug.Fail(string.Format(
                        "Client with handle key '{0}' already exist!", key));
                }

                m_clients[key] = connectedClient;

                // create the call back for any client connections...
                m_socket.BeginAccept(new AsyncCallback(OnClientConnection), null);

            }
            catch (ObjectDisposedException odex)
            {
                Debug.Fail(odex.ToString(),
                    "OnClientConnection: Socket has been closed");
            }
            catch (Exception sex)
            {
                Debug.Fail(sex.ToString(), 
                    "OnClientConnection: Socket failed");
            }

        }

        private void OnClientDisconnection(Socket socket)
        {
            RaiseClientDisconnected(socket);

            long key = socket.Handle.ToInt64();
            if (m_clients.ContainsKey(key))
            {
                m_clients.Remove(key);
            }
            else
            {
                Debug.Fail(string.Format(
                    "Unknown client '{0}' has been disconncted!", key));
            }
        }

        public void DistributeMessage(byte[] buffer)
        {
            try
            {
                foreach (ConnectedClient connectedClient in m_clients.Values)
                {
                    connectedClient.Send(buffer);
                }
            }
            catch (SocketException se)
            {
                Debug.Fail(se.ToString(), string.Format(
                    "Buffer could not be sent"));
            }
        }

        public void Close()
        {
            try
            {
                if (m_socket != null)
                {
                    m_Closed = true;

                    // Close the clients
                    foreach (ConnectedClient connectedClient in m_clients.Values)
                    {
                        connectedClient.Stop();
                    }

                    m_socket.Close();

                    m_socket = null;
                }
            }
            catch (ObjectDisposedException odex)
            {
                Debug.Fail(odex.ToString(), "Stop failed");
            }
        }

        private void OnMessageRecived(Socket socket, byte[] message)
        {
            if (MessageRecived != null)
            {
                MessageRecived(socket, message);
            }
        }

        private void RaiseClientConnected(Socket socket)
        {
            if (ClientConnect != null)
            {
                ClientConnect(socket);
            }
        }

        private void RaiseClientDisconnected(Socket socket)
        {
            if (ClientDisconnect != null)
            {
                ClientDisconnect(socket);
            }
        }
    }
}
