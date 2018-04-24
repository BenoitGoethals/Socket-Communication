using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Chat.Core;
using ProjectA.Protocol;
using WpfServer.utils;

namespace WpfServer.ViewModel
{
   public class MainViewModel: INotifyDataErrorInfo
    {

        private ServerConnector ServerConnector=new ServerConnector();


        
        private List<SimpleMessage> _messages=new List<SimpleMessage>();

        public List<SimpleMessage> Messages
        {
            get { return _messages; }
            set
            {
                _messages = value;
                OnPropertyChanged(nameof(Messages));
            }
        }



        private ObservableCollection<string> _logs=new ObservableCollection<string>();

        public ObservableCollection<string> Logs
        {
            get { return _logs; }
            set
            {
                _logs = value;
                OnPropertyChanged(nameof(Logs));
            }
        }



        private void CreateEvent()
        {
           

            ServerConnector.Server.MessageRecived += new MessageRecivedDelegate(m_Terminal_MessageRecived);
            ServerConnector.Server.ClientConnect += new ConnectDelegate(m_Terminal_ClientConnected);
            ServerConnector.Server.ClientDisconnect += new DisconnectDelegate(m_Terminal_ClientDisConnected);

            
        }


        private string ConvertBytesToString(byte[] bytes)
        {
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

        private void m_Terminal_ClientDisConnected(Socket socket)
        {
            Debug.WriteLine($"Client {socket.LocalEndPoint} has been diconnected!");
            _logs.Add($"Client {socket.LocalEndPoint} has been diconnected!");
        }

        private void m_Terminal_ClientConnected(Socket socket)
        {
            Debug.WriteLine($"Client {socket.LocalEndPoint} has been connected!");
            _logs.Add($"Client {socket.LocalEndPoint} has been connected!");
        }

        private void m_Terminal_MessageRecived(Socket socket, byte[] message)
        {
            Debug.WriteLine(ConvertBytesToString(message));
            _messages.Add(new SimpleMessage(ConvertBytesToString(message)));
           
        }

        private void closeEvent()
        {
            ServerConnector.Server.MessageRecived -= new MessageRecivedDelegate(m_Terminal_MessageRecived);
            ServerConnector.Server.ClientConnect -= new ConnectDelegate(m_Terminal_ClientConnected);
            ServerConnector.Server.ClientDisconnect -= new DisconnectDelegate(m_Terminal_ClientDisConnected);

            ServerConnector.Server.Close();
        }


   private Dictionary<string, List<string>> propErrors = new Dictionary<string, List<string>>();

      
        private string _port="10001";
      


        public string Port
        {
            get { return _port; }
            set
            {
                _port = value;
                OnPropertyChanged(nameof(Port));
            }
        }

        private bool _running;



        public bool Running
        {
            get { return _running; }
            set
            {
                _running = value;
                OnPropertyChanged(nameof(Running));
            }
        }

        public RelayCommand ConnectCommand { get; private set; }
        public RelayCommand DisConnectCommand { get; private set; }


        public MainViewModel()
        {



            DisConnectCommand=new RelayCommand(() =>
            {
                closeEvent();
                _logs.Add("Stopped server" + Port);
                Running = false;
            }, () => !HasErrors
            );




            //commands
            ConnectCommand = new RelayCommand(() =>
            {
                Running = true;
                Debug.WriteLine("test"+_port);
                CreateEvent();
                ServerConnector.Connect(Port);
                _logs.Add("Started server" + Port);

            }, () => !HasErrors);

            PropertyChanged += (s, e) => Validate();
        }

        private bool Controle(string reg,  string  prop)
        {
            
            Regex regex = new Regex(@reg);
            Match match = regex.Match(prop);

            return match.Success;
        }

       


        private void Validate()
        {
            Task.Run(() => DataValidation());
        }
        
            private void DataValidation()
        {


            if (propErrors.TryGetValue(nameof(Port), out var listErrors2) == false)
                listErrors2 = new List<string>();
            else
                listErrors2.Clear();

            if (string.IsNullOrEmpty(Port) || !Controle(@"(6553[0-5]|655[0-2]\d|65[0-4]\d{2}|6[0-4]\d{3}|[1-5]\d{4}|[1-9]\d{0,3})", Port))

                listErrors2.Add("Bad Port");
            propErrors[nameof(Port)] = listErrors2;

            if (listErrors2.Count > 0)
            {
                OnPropertyErrorsChanged(nameof(Port));

            }




        }

        #region INotifyDataErrorInfo

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        private void OnPropertyErrorsChanged(string p)
        {
            if (ErrorsChanged != null)
                ErrorsChanged.Invoke(this, new DataErrorsChangedEventArgs(p));
        }

        public System.Collections.IEnumerable GetErrors(string propertyName)
        {
            List<string> errors;
            if (propertyName != null)
            {
                propErrors.TryGetValue(propertyName, out errors);
                return errors;
            }

            else
                return null;
        }

        public bool HasErrors
        {
            get
            {
                try
                {
                    var propErrorsCount = propErrors.Values.FirstOrDefault(r => r.Count > 0);
                    if (propErrorsCount != null)
                        return true;
                    else
                        return false;
                }
                catch { }
                return true;
            }
        }

        #endregion

       
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
      
        
}

