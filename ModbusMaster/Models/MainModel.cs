using ModbusMaster.Interface;
using NModbus;
using System.Collections.ObjectModel;
using System.Net.Sockets;

namespace ModbusMaster.Models
{
    public class MainModel : IModel
    {
        // 연결된 서버 정보
        public string ConnectedIP { get; private set; } //커넥트 뷰모델에서 생성자로 겟을 해서 디폴트값으로 바꿔놓기
        public int ConnectedPort { get; private set; }

        public bool IsConnected => _tcpClient != null && _tcpClient.Connected; // 현재 연결 상태

        private TcpClient _tcpClient; // TCP 연결 객체  
        private NetworkStream _networkStream;// 데이터 송수신 스트림

        public  ObservableCollection<DataItem> Data { get; private set; } // 연결된 서버의 데이터 리스트 

        //싱글턴 단일 인스턴스
        private static MainModel _instance;


        public event ReadEventDelegate ReadCoilEventHandler;
        public event ReadEventDelegate ReadHoldingRegEventHandler;
        public event ConnectDelegate ConnectEventHandler;

        public static MainModel Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new MainModel();

                return _instance;
            }
        }

        public MainModel()
        {
            Data = new ObservableCollection<DataItem>();
        }

        public bool Connect(string ip, int port)
        {
            try
            {
                _tcpClient = new TcpClient();// TCP 클라이언트를 생성
                
                _tcpClient.Connect(ip, port);// 지정된 IP와 Port로 서버에 연결
                _networkStream = _tcpClient.GetStream(); // 연결된 TCP 소켓의 데이터 송수신을 위한 스트림을 가져옴

                //스트림이란?
                //데이터의 연속적인 흐름을 처리하는 추상적인 개념
                // TCP에서는 :  TCP 연결 위에서 데이터를 주고받는 통로 역할

                 
                // 연결 정보 저장
                ConnectedIP = ip;
                ConnectedPort = port;
                ConnectEventHandler?.Invoke(IsConnected);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"연결 실패 : {ex.Message}");
                return false;
            }
        }

        public void Disconnect()// 연결 해제
        {
            if (_tcpClient != null)  
            {
                _networkStream?.Close();
                _tcpClient.Close(); //TCP 연결을 통해 데이터 송수신을 담당하는 스트림을 명시적으로 닫음
                                    //명시적으로 닫음으로써 스트림 관련 자원을 조기에 해제
                _tcpClient = null;

                // 연결 정보 초기화
                ConnectedIP = null;
                ConnectedPort = 0;
            }
        }


        public void LoadData() //데이터 로드 //slave ID, Adress, Quantity를 매개변수로 받기 추가
        {
            // 연결 상태 확인: 서버에 연결되어 있지 않으면 예외를 발생시켜 작업 중단
            if (!IsConnected)
                throw new InvalidOperationException("서버에 연결되어 있지 않습니다.");


            try
            {
                // 데이터를 임시로 저장할 버퍼를 선언 (256바이트 크기의 배열 생성)
                byte[] buffer = new byte[256];// 버퍼 크기 설정

                int totalBytesRead = 0;
                Data.Clear();
                                                                                     



                // _networkStream.DataAvailable: 스트림에 읽을 데이터가 남아 있는지 확인.
                // 데이터가 있을 동안 반복해서 데이터를 읽음.
                while (_networkStream.DataAvailable)//256바이트 읽어옴.
                {
                    //Read:읽은 데이터의 크기를 반환하는 함수
                    int bytesRead = _networkStream.Read(buffer, 0, buffer.Length); // bytesRead: 실제로 읽은 데이터의 크기(바이트)


                    for (int i = 0; i < bytesRead; i += 4)
                    {
                        if (i + 4 > bytesRead) break; //4바이트보다 적으면 break
                        int value = BitConverter.ToInt32(buffer, i);//BitConverter.ToInt32:바이트 배열로 표현된 데이터를 정수형(int)으로 변환.  
                        Data.Add(new DataItem { Address = (totalBytesRead + i) / 4, Value = value });
                    }

                    totalBytesRead += bytesRead; // totalBytesRead: 지금까지 읽은 모든 데이터의 총 바이트 수를 누적.
                }

                // 수신된 데이터를 처리하여 Data 리스트에 추가
                // - Address: 데이터를 구분하는 주소 (여기서는 고정 값 0)
                // - Value: buffer에서 읽은 데이터를 정수형(int)으로 변환
                //   BitConverter.ToInt32는 버퍼의 처음 4바이트를 int로 변환
                //Data.Add(new DataItem { Address = 0, Value = BitConverter.ToInt32(buffer, 0) });
            }

            //서버 연결이 끊어졌거나 데이터를 읽을 수 없는 경우 예외처리
            catch (Exception ex)
            {
                throw new InvalidOperationException($"데이터로드 실패 : {ex.Message}");
            }
        }

        public void ReadCoil(int slaveId, int startAddr, int count)
        {
            throw new NotImplementedException();
        }

        public void ReadHoldingReg(int slaveId, int startAddr, int count)
        {
            throw new NotImplementedException();
        }
    }



    // DataItem 클래스는 서버에서 수신한 데이터를 저장하기 위한 구조체 역할을 합니다.
    // 데이터를 화면에 표시하거나 다른 로직에서 사용할 때 활용됩니다.
    public class DataItem
    {
        // 데이터의 주소 (예: TCP 서버에서 전달된 데이터 위치)
        public int Address { get; set; }

        // 데이터의 값 (예: 특정 주소에서 읽어온 값)
        public int Value { get; set; }
    }
}

