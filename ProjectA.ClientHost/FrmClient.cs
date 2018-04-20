using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Forms;
using Chat.Core;
using ProjectA.Protocol;

namespace ProjectA.ClientHost
{
    public partial class FrmClient : Form
    {
        ClientTerminal m_ClientTerminal = new ClientTerminal();

        public FrmClient()
        {
            InitializeComponent();
            m_ClientTerminal.Connected += m_TerminalClient_Connected;
            m_ClientTerminal.Disconncted += m_TerminalClient_ConnectionDroped;
            m_ClientTerminal.MessageRecived += m_TerminalClient_MessageRecived;
        }

        private void cmdConnect_Click(object sender, EventArgs e)
        {
            try
            {
                string szIPSelected = txtIPAddress.Text;
                string szPort = txtPort.Text;
                int alPort = Convert.ToInt16(szPort, 10);
                IPAddress remoteIPAddress = IPAddress.Parse(szIPSelected);

                m_ClientTerminal.Connect(remoteIPAddress, alPort);
            }
            catch (SocketException se)
            {
                MessageBox.Show(se.Message);
            }

        }

        private void cmdSendMessage_Click(object sender, EventArgs e)
        {
            try
            {
                string mes = txtData.Text;

                // Create the concrete message
                SendingTimeMessage message = new SendingTimeMessage(mes);

                int messageKind = (int)MessageKind.SendingTime;
                
                byte[] buffer = MessageComposer.Serialize(messageKind, message);

                // Send the message (as bytes) to the server.
                m_ClientTerminal.SendMessage(buffer);

            }
            catch (SocketException se)
            {
                MessageBox.Show(se.Message);
            }
        }

        void m_TerminalClient_MessageRecived(Socket socket, byte[] bytes)
        {
            string message = ConvertBytesToString(bytes);
            PresentMessage(listMessages, message);
        }

        private void cmdClose_Click(object sender, EventArgs e)
        {
            m_ClientTerminal.Close();
            
            cmdConnect.Enabled = true;
            cmdClose.Enabled = false;
        }

        private void m_btnNew_Click(object sender, EventArgs e)
        {
            new FrmClient().Show();
        }

        void m_TerminalClient_Connected(Socket socket)
        {
            SimpleMessage message = new SimpleMessage("Hello There");
            
            int messageKing = (int)MessageKind.Simple;
            
            byte[] buffer = MessageComposer.Serialize(messageKing, message);

            m_ClientTerminal.SendMessage(buffer);

            cmdConnect.Enabled = false;
            cmdClose.Enabled = true;
            PresentMessage(listLog, "Connection Opened!");

            m_ClientTerminal.StartListen();
            PresentMessage(listLog, "Start listening to server messages");
        }

        void m_TerminalClient_ConnectionDroped(Socket socket)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new DisconnectDelegate(m_TerminalClient_ConnectionDroped), socket);
                return;
            }

            cmdConnect.Enabled = true;
            cmdClose.Enabled = false;

            PresentMessage(listLog, "Server has been disconnected!");
        }

        
        private string ConvertBytesToString(byte[] bytes)
        {
            //char[] chars = new char[iRx + 1];
            //System.Text.Decoder d = System.Text.Encoding.UTF8.GetDecoder();
            //d.GetChars(bytes, 0, iRx, chars, 0);
            //string szData = new string(chars);
            //return szData;

            int messageKind;
            MessageBase msg;
            MessageComposer.Deserialize(bytes, out messageKind, out msg);

            MessageKind kind = (MessageKind)messageKind;

            switch (kind)
            {
                case MessageKind.SendingTime:
                    SendingTimeMessage sendingTimeMessage = (SendingTimeMessage)msg;
                    return "SendingTimeMessage: " + sendingTimeMessage.Message;

                case MessageKind.Simple:
                    SimpleMessage simpleMessage = (SimpleMessage)msg;
                    return "SimpleMessage: " + simpleMessage.Message;
            }

            return "UnKnown";

        }

        private void PresentMessage(ListBox listBox, string mes)
        {
            if (InvokeRequired)
            {
                BeginInvoke((ThreadStart)delegate { PresentMessage(listBox, mes); });
                return;
            }

            mes = string.Format("{0}: Echo: {1}", DateTime.Now.ToString("hh:mm:ss:ffff"), mes);
            listBox.Items.Add(mes);
        }

    }
}










































