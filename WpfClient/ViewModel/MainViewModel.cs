using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WpfClient.utils;

namespace WpfClient.ViewModel
{
    public class MainViewModel : INotifyDataErrorInfo
    {

private Dictionary<string, List<string>> propErrors = new Dictionary<string, List<string>>();

        private string _port;
        
        public string Port
        {
            get { return _port; }
            set
            {
                _port = value;
                OnPropertyChanged(nameof(Port));
            }
        }


        private string _server;

        public string Server
        {
            get { return _server; }
            set
            {
                _server = value;
                OnPropertyChanged(nameof(Server));
            }
        }



        private string _name;

        public string Name
        {
            get { return _port; }
            set
            {
                _port = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        private string _message;

        public string Message
        {
            get { return _message; }
            set
            {
                _port = value;
                OnPropertyChanged(nameof(Message));
            }
        }
        public RelayCommand ConnectCommand { get; private set; }
        public RelayCommand SendCommand { get; private set; }

        public MainViewModel()
        {



            //commands
            ConnectCommand = new RelayCommand(() =>
            {
                Debug.WriteLine("test" + _port);
             

            }, () => !HasErrors);

            //commands
            SendCommand = new RelayCommand(() =>
            {
                Debug.WriteLine("send" + _port);


            }, () => !HasErrors);

            PropertyChanged += (s, e) => Validate();
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

        private void DataValidation()
        {


            if (propErrors.TryGetValue(Server, out var listErrors) == false)
                listErrors = new List<string>();
            else
                listErrors.Clear();

            if (string.IsNullOrEmpty(Server) || !Controle(@"^(([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])\.){3}([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])$", Server))

                listErrors.Add("Bad IP");
            propErrors[nameof(Server)] = listErrors;

            if (listErrors.Count > 0)
            {
                OnPropertyErrorsChanged(nameof(Server));

            }

                if (propErrors.TryGetValue(Port, out var listErrors2) == false)
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



            if (propErrors.TryGetValue(Name, out var listErrors3) == false)
                listErrors3 = new List<string>();
            else
                listErrors3.Clear();

            if (string.IsNullOrEmpty(Name) )

                listErrors3.Add("Bad Name");
            propErrors[nameof(Name)] = listErrors3;

            if (listErrors3.Count > 0)
            {
                OnPropertyErrorsChanged(nameof(Name));

            }


            if (propErrors.TryGetValue(Name, out var listErrors4) == false)
                listErrors4 = new List<string>();
            else
               
            listErrors4.Clear();

            if (string.IsNullOrEmpty(Name))

                listErrors4.Add("Bad Message");
            propErrors[nameof(Name)] = listErrors4;

            if (listErrors4.Count > 0)
            {
                OnPropertyErrorsChanged(nameof(Name));

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

        # endregion

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
                Validate();
            }
        }

        #endregion
    }
}
