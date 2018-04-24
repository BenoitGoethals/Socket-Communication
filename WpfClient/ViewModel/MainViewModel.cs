using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Chat.Core;
using ProjectA.Protocol;
using WpfClient.utils;
using ValidationResult = System.ComponentModel.DataAnnotations.ValidationResult;

namespace WpfClient.ViewModel
{
    public class MainViewModel : INotifyDataErrorInfo
    {



        ClientTerminal m_ClientTerminal = new ClientTerminal();


        private List<SimpleMessage> _messages=new List<SimpleMessage>();

        public List<SimpleMessage> Messages
        {
            get { return _messages; }
            set { _messages = value; }
        }


    
        private List<string> _logs=new List<string>();

        public List<string> Logs
        {
            get { return _logs; }
            set { _logs = value; }
        }


        void m_TerminalClient_MessageRecived(Socket socket, byte[] bytes)
        {
            string message = ConvertBytesToString(bytes);
            //   PresentMessage(listMessages, message);
            Debug.WriteLine("Received : "+message);
            _messages.Add(new SimpleMessage(message));
        }






        void m_TerminalClient_Connected(Socket socket)
        {
            SimpleMessage message = new SimpleMessage("Hello There");

            int messageKing = (int) MessageKind.Simple;

            byte[] buffer = MessageComposer.Serialize(messageKing, message);

            m_ClientTerminal.SendMessage(buffer);


            //  PresentMessage(listLog, "Connection Opened!");
            _logs.Add("Connection Opened!");
        
            m_ClientTerminal.StartListen();
            Debug.WriteLine("Received : " + message);
            _logs.Add("Received : " + message);
            //      PresentMessage(listLog, "Start listening to server messages");
        }

        void m_TerminalClient_ConnectionDroped(Socket socket)
        {
          
        new DisconnectDelegate(m_TerminalClient_ConnectionDroped).Invoke(socket);
                return;
          
         
         //   PresentMessage(listLog, "Server has been disconnected!");
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

            MessageKind kind = (MessageKind) messageKind;

            switch (kind)
            {
                case MessageKind.SendingTime:
                    SendingTimeMessage sendingTimeMessage = (SendingTimeMessage) msg;
                    return "SendingTimeMessage: " + sendingTimeMessage.Message;

                case MessageKind.Simple:
                    SimpleMessage simpleMessage = (SimpleMessage) msg;
                    return "SimpleMessage: " + simpleMessage.Message;
            }

            return "UnKnown";

        }

   



















































private readonly Dictionary<string, ICollection<string>>
            _validationErrors = new Dictionary<string, ICollection<string>>();
     

        private string _port = "10001";

      
        [StringLength(6, MinimumLength = 4,
            ErrorMessage = "The Port must be between 4 and 6 characters long")]

        public string Port
        {
            get => _port;
            set
            {
                _port = value;
             
                ValidateModelProperty(value, nameof(Port));
            }
        }


        private string _server = "127.0.0.1";
      
        [RegularExpression(@"^(([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])\.){3}([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])$", ErrorMessage = "The Server must only contain letters (a-z, A-Z).")]
        public string Server
        {
            get => _server;
            set
            {
                _server = value;
                ValidateModelProperty(value, nameof(Server));
            }
        }



        private string _name = "benoit";

        [StringLength(50, MinimumLength = 10,
            ErrorMessage = "The Name must be between 4 and 10 characters long")]
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                ValidateModelProperty(value, nameof(Name));
            }
        }

        private string _message = "test";
    

        [StringLength(50, MinimumLength = 4,
            ErrorMessage = "The Message must be between 4 and 10 characters long")]    
        public string Message
        {
            get => _message;
            set
            {
                _message = value;
                ValidateModelProperty(value, nameof(Message));
            }
        }
        public RelayCommand ConnectCommand { get; private set; }
        public RelayCommand SendCommand { get; private set; }

        public MainViewModel()
        {

            m_ClientTerminal.Connected += m_TerminalClient_Connected;
            m_ClientTerminal.Disconncted += m_TerminalClient_ConnectionDroped;
            m_ClientTerminal.MessageRecived += m_TerminalClient_MessageRecived;

            //commands
            ConnectCommand = new RelayCommand(() =>
            {
                Debug.WriteLine("test" + _port+_server+_name);
                try
                {

                    m_ClientTerminal.Connect(IPAddress.Parse(Server), Convert.ToInt16(Port, 10));
                }
                catch (SocketException se)
                {
                    MessageBox.Show(se.Message);
                }

            }, () => !HasErrors);

         
            SendCommand = new RelayCommand(() =>
            {
                Debug.WriteLine("send" + _message);
                try
                {


                    // Create the concrete message
                    SendingTimeMessage message = new SendingTimeMessage(Message);

                    int messageKind = (int)MessageKind.SendingTime;

                    byte[] buffer = MessageComposer.Serialize(messageKind, message);

                    // Send the message (as bytes) to the server.
                    m_ClientTerminal.SendMessage(buffer);
                    _messages.Add(new SimpleMessage(_message));
                }
                catch (SocketException se)
                {
                    MessageBox.Show(se.Message);
                }

            }, () =>!HasErrors);

         //   PropertyChanged += (s, e) => Validate();
          
        }

        private bool Controle(string reg, string prop)
        {

            Regex regex = new Regex(@reg);
            Match match = regex.Match(prop);

            return match.Success;
        }


        private void Validate()
        {
           Task.Run(() => DataValidation());
        }

        protected void ValidateModelProperty(object value, string propertyName)
        {
            if (_validationErrors.ContainsKey(propertyName))
                _validationErrors.Remove(propertyName);

            ICollection<ValidationResult> validationResults = new List<ValidationResult>();
            ValidationContext validationContext =
                new ValidationContext(this, null, null) { MemberName = propertyName };
            if (!Validator.TryValidateProperty(value, validationContext, validationResults))
            {
                _validationErrors.Add(propertyName, new List<string>());
                foreach (ValidationResult validationResult in validationResults)
                {
                    _validationErrors[propertyName].Add(validationResult.ErrorMessage);
                }
            }
            RaiseErrorsChanged(propertyName);
        }

        /* Alternative solution using LINQ */
        protected void ValidateModelProperty_(object value, string propertyName)
        {
            if (_validationErrors.ContainsKey(propertyName))
                _validationErrors.Remove(propertyName);

            PropertyInfo propertyInfo = this.GetType().GetProperty(propertyName);
            IList<string> validationErrors =
                  (from validationAttribute in propertyInfo.GetCustomAttributes(true).OfType<ValidationAttribute>()
                   where !validationAttribute.IsValid(value)
                   select validationAttribute.FormatErrorMessage(string.Empty))
                   .ToList();

            _validationErrors.Add(propertyName, validationErrors);
            RaiseErrorsChanged(propertyName);
        }

        protected void DataValidation()
        {
            _validationErrors.Clear();
            ICollection<ValidationResult> validationResults = new List<ValidationResult>();
            ValidationContext validationContext = new ValidationContext(this, null, null);
            if (!Validator.TryValidateObject(this, validationContext, validationResults, true))
            {
                foreach (ValidationResult validationResult in validationResults)
                {
                    string property = validationResult.MemberNames.ElementAt(0);
                    if (_validationErrors.ContainsKey(property))
                    {
                        _validationErrors[property].Add(validationResult.ErrorMessage);
                    }
                    else
                    {
                        _validationErrors.Add(property, new List<string> {validationResult.ErrorMessage});
                    }
                }
            }
            RaiseErrorsChanged(nameof(Name));
            RaiseErrorsChanged(nameof(Port));
            RaiseErrorsChanged(nameof(Server));
            RaiseErrorsChanged(nameof(Message));
        }

      

            #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
               // Validate();
            }
        }

        #endregion

        #region INotifyDataErrorInfo members
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;
        private void RaiseErrorsChanged(string propertyName)
        {
            if (ErrorsChanged != null)
                ErrorsChanged(this, new DataErrorsChangedEventArgs(propertyName));
        }

        public System.Collections.IEnumerable GetErrors(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName)
                || !_validationErrors.ContainsKey(propertyName))
                return null;

            return _validationErrors[propertyName];
        }

        public bool HasErrors
        {
            get { return _validationErrors.Count > 0; }
        }
        #endregion
    }
}
