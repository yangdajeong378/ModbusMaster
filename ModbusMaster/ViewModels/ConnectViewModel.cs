using ModbusMaster.Interface;
using ModbusMaster.Models;
using System.ComponentModel;
using System.Windows.Input;

namespace ModbusMaster.ViewModels
{
    public class ConnectViewModel : INotifyPropertyChanged
    {
        IModel mainModel = ModbusModel.Instance; // MainModel.Instance;

        private string _connectionStatus;

        public string IpAddress { get; set; }

        public int Port { get; set; }



        public string ConnectionStatus
        {
            get { return _connectionStatus; }
            set
            {
                _connectionStatus = value;
                OnPropertyChanged("ConnectionStatus");// UI에 변경 알림
            }
        }

        public ICommand ConnectCommand { get; }

        public ConnectViewModel()
        {
            ConnectCommand = new RelayCommand(Connect);
        }

        public void Connect()// 연결 버튼을 클릭했을 때 실행될 메서드
        {
            mainModel.Connect(IpAddress, Port);

            ConnectionStatus = mainModel.IsConnected ? "Connect" : "Not Connect";
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
