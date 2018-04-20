using System;
using System.Diagnostics;
using System.Net.Sockets;

namespace Chat.Core
{
    public class SocketListener
    {
        public class CSocketPacket
        {
            public Socket thisSocket;
            public byte[] dataBuffer;

            public CSocketPacket(int buffeLength)
            {
                dataBuffer = new byte[buffeLength];
            }
        }

        private const int BufferLength = 1000;
        AsyncCallback pfnWorkerCallBack;
        Socket m_socWorker;

        public event MessageRecivedDelegate MessageRecived;
        public event DisconnectDelegate Disconnected;

        public void StartReciving(Socket socket)
        {
            m_socWorker = socket;
            WaitForData(socket);
        }

        public void StopListening()
        {
            // Incase connection has been established with remote client - 
            // Raise the OnDisconnection event.
            if (m_socWorker != null)
            {
                // m_socWorker.Shutdown(SocketShutdown.Both);                        
                m_socWorker.Close();
                m_socWorker = null;
            }
        }

        private void WaitForData(Socket soc)
        {
            try
            {
                if (pfnWorkerCallBack == null)
                {
                    pfnWorkerCallBack = new AsyncCallback(OnDataReceived);
                }
                CSocketPacket theSocPkt = new CSocketPacket(BufferLength);
                theSocPkt.thisSocket = soc;
                // now start to listen for any data...
                soc.BeginReceive(
                    theSocPkt.dataBuffer,
                    0,
                    theSocPkt.dataBuffer.Length,
                    SocketFlags.None,
                    pfnWorkerCallBack,
                    theSocPkt);
            }
            catch (SocketException sex)
            {
                Debug.Fail(sex.ToString(), "WaitForData: Socket failed");
            }

        }

        private void OnDataReceived(IAsyncResult asyn)
        {
            CSocketPacket theSockId = (CSocketPacket)asyn.AsyncState;
            Socket socket = theSockId.thisSocket;

            if (!socket.Connected)
            {
                return;
            }

            try
            {
                int iRx;
                try
                {
                    iRx = socket.EndReceive(asyn);
                }
                catch (SocketException)
                {
                    Debug.Write("Apperently client has been closed and connot answer.");

                    OnConnectionDroped(socket);
                    return;
                }

                if (iRx == 0)
                {
                    Debug.Write("Apperently client socket has been closed.");
                    // If client socket has been closed (but client still answers)- 
                    // EndReceive will return 0.
                    OnConnectionDroped(socket);
                    return;
                }

                byte[] bytes = theSockId.dataBuffer;

                RaiseMessageRecived(bytes);

                WaitForData(m_socWorker);
            }
            catch (Exception ex)
            {
                Debug.Fail(ex.ToString(), "OnClientConnection: Socket failed");
            }
        }


        private void RaiseMessageRecived(byte[] bytes)
        {
            if (MessageRecived != null)
            {
                MessageRecived(m_socWorker, bytes);
            }
        }

        private void OnDisconnection(Socket socket)
        {
            if (Disconnected != null)
            {
                Disconnected(socket);
            }
        }

        private void OnConnectionDroped(Socket socket)
        {
            m_socWorker = null;
            OnDisconnection(socket);
        }

    }
}
