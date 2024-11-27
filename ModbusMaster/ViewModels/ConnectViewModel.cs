using ModbusMaster.Models;
using NModbus;
using NModbus.IO;
using System.ComponentModel;
using System.Net;
using System.Net.Sockets;
using System.Windows.Input;

namespace ModbusMaster.ViewModels
{
    public class ConnectViewModel : INotifyPropertyChanged
    {
        private string _connectionStatus = "Not Connected";
        private TcpClient _tcpClient;
        private IModbusMaster _modbusMaster;

        //public string IpAddress { get; set; }
        //public string Port { get; set; }

        public ConnectModel ConnectModel { get;}  



        public string IpAddress
        {
            get { return ConnectModel.IpAddress; }
            set { ConnectModel.IpAddress = value; }
        }

        public string Port
        {
            get { return ConnectModel.Port; }
            set { ConnectModel.Port = value; }
        }

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
            ConnectModel = new ConnectModel();  //초기화

            ConnectCommand = new RelayCommand(Connect);
        }

        public void Connect()// 연결 버튼을 클릭했을 때 실행될 메서드
        {
            try
            {
                // 1. TcpClient를 생성하여 IP와 포트로 연결
                _tcpClient = new TcpClient(IpAddress, int.Parse(Port));

                // 2. TcpClient를 TcpClientAdapter로 래핑하여 IStreamResource로 변환
                var streamResource = new TcpClientAdapter(_tcpClient);

                // 3. ModbusFactory를 통해 Modbus Master 생성
                var factory = new ModbusFactory();
                _modbusMaster = factory.CreateIpMaster(streamResource);

                // 4. 연결 성공 메시지 설정
                ConnectionStatus = "Connected to Modbus Device!";
            }
            catch (Exception ex)// 연결 실패 시 처리
            {
                // 5. 오류 메시지 설정
                ConnectionStatus = $"Connection Failed: {ex.Message}";
            }
        }

        public IModbusMaster GetModbusMaster()
        {
            return _modbusMaster;
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
