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
  public  class ServerConnector
    {
       public ServerTerminal Server=new ServerTerminal();



        
      

        public void Connect(string server)
        {
            try
            {


                Server.StartListen(Convert.ToInt16(server, 10));
   
            }
            catch (Exception se)
            {
                MessageBox.Show(se.Message);
            }

        }

        public void Close()
        {
       //  
        }
       

        
    }
}
