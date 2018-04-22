using System.ComponentModel;

namespace WpfServer.ViewModel
{
  public  class ViewModelBase : INotifyPropertyChanged
    {

        #region Notify Property Changed Members
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}