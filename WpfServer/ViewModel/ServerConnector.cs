using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Chat.Core;
using ProjectA.Protocol;

namespace WpfServer.ViewModel
{
    class ServerConnector
    {
       public ServerTerminal Server;



        private int messagesCount;
      

        public void Connect(string server)
        {
            try
            {
               
                int alPort = Convert.ToInt16(server, 10);

          //      createTerminal(alPort);
                /**
                cmdConnect.Enabled = false;
                cmdClose.Enabled = true;
    */
            }
            catch (Exception se)
            {
                MessageBox.Show(se.Message);
            }

        }

        public void Close()
        {
       //     closeTerminal();
            /**
            cmdConnect.Enabled = true;
            cmdClose.Enabled = false;
    **/
        }
        /*
        void m_Terminal_ClientDisConnected(Socket socket)
        {
            PresentMessage(listLog, string.Format("Client {0} has been disconnected!", socket.LocalEndPoint));
        }

        void m_Terminal_ClientConnected(Socket socket)
        {
            PresentMessage(listLog, string.Format("Client {0} has been connected!", socket.LocalEndPoint));
        }

        void m_Terminal_MessageRecived(Socket socket, byte[] buffer)
        {
            string message = ConvertBytesToString(buffer);

            PresentMessage(listMessages, message);

            // Send Echo
            m_ServerTerminal.DistributeMessage(buffer);
        }

        */
        
        /*
        private void createTerminal(int alPort)
        {
            m_ServerTerminal = new ServerTerminal();
            
            m_ServerTerminal.MessageRecived += new MessageRecivedDelegate(m_Terminal_MessageRecived);
            m_ServerTerminal.ClientConnect += new ConnectDelegate(m_Terminal_ClientConnected);
            m_ServerTerminal.ClientDisconnect += new DisconnectDelegate(m_Terminal_ClientDisConnected);
            
            m_ServerTerminal.StartListen(alPort);
        }

        private void closeTerminal()
        {
            m_ServerTerminal.MessageRecived -= new MessageRecivedDelegate(m_Terminal_MessageRecived);
            m_ServerTerminal.ClientConnect -= new ConnectDelegate(m_Terminal_ClientConnected);
            m_ServerTerminal.ClientDisconnect -= new DisconnectDelegate(m_Terminal_ClientDisConnected);

            m_ServerTerminal.Close();
        }
/**
        private void PresentMessage(ListBox listBox, string mes)
        {
            if (InvokeRequired)
            {
                BeginInvoke((ThreadStart) delegate { PresentMessage(listBox, mes); });
                return;
            }

            mes = string.Format("{0}: {1}", DateTime.Now.ToString("hh:mm:ss:ffff"), mes);
            listBox.Items.Add(mes);
            lblMessages.Text = messagesCount++.ToString();
            
            
        }
    */

        
    }
}
