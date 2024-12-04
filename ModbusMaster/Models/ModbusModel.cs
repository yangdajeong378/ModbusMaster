using ModbusMaster.Interface;
using NModbus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ModbusMaster.Models
{
    public partial class ModbusModel : IModel
    {
        private static ModbusModel instance;
        public static ModbusModel Instance => instance ?? (instance = new ModbusModel());


    }

    public partial class ModbusModel : IModel
    {
        private TcpClient _tcpClient; // TCP 연결 객체  
        private NetworkStream _networkStream;// 데이터 송수신 스트림

        private IModbusMaster modbusMaster;

        public event ReadEventDelegate ReadHoldingRegEventHandler;
        public event ReadEventDelegate ReadCoilEventHandler;
        public event ConnectDelegate ConnectEventHandler;

        public string ConnectedIP { get; private set; } //커넥트 뷰모델에서 생성자로 겟을 해서 디폴트값으로 바꿔놓기
        public int ConnectedPort { get; private set; }

        public bool IsConnected => _tcpClient != null && _tcpClient.Connected; // 현재 연결 상태


        public bool Connect(string ip, int port)
        {
            try
            {
                _tcpClient = new TcpClient();// TCP 클라이언트를 생성

                _tcpClient.Connect(ip, port);// 지정된 IP와 Port로 서버에 연결
                _networkStream = _tcpClient.GetStream(); // 연결된 TCP 소켓의 데이터 송수신을 위한 스트림을 가져옴

                IModbusFactory fac = new ModbusFactory();
                var ipModbus = fac.CreateMaster(_tcpClient);
                this.modbusMaster = ipModbus;

                //스트림이란?
                //데이터의 연속적인 흐름을 처리하는 추상적인 개념
                // TCP에서는 :  TCP 연결 위에서 데이터를 주고받는 통로 역할


                // 연결 정보 저장
                ConnectedIP = ip;
                ConnectedPort = port;

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"연결 실패 : {ex.Message}");
                return false;
            }
        }

        public void Disconnect()
        {
            if (_tcpClient != null)
            {
                _networkStream?.Close();
                _tcpClient.Close(); //TCP 연결을 통해 데이터 송수신을 담당하는 스트림을 명시적으로 닫음
                                    //명시적으로 닫음으로써 스트림 관련 자원을 조기에 해제
                _tcpClient = null;

                modbusMaster.Dispose();

                // 연결 정보 초기화
                ConnectedIP = null;
                ConnectedPort = 0;
            }
        }

        public void ReadCoil(int slaveId, int startAddr, int count)
        {
            var stAddr = startAddr;
            bool[] coils = modbusMaster.ReadCoils((byte)slaveId, (ushort)startAddr, (ushort)count);
            
            //List<DataItem<bool>> coilList = new List<DataItem<bool>>();
            //foreach( var coil in coils )
            //{
            //    coilList.Add(new DataItem<bool> { Address = startAddr++, Value = coil });
            //}
            var coilList = coils.Select(coil => new DataItema { Address = startAddr++, Value = (ushort)(coil ? 1:0) }).ToList();
            ReadCoilEventHandler?.Invoke(startAddr, count, coilList);
        }

        public void ReadHoldingReg(int slaveId, int startAddr, int count)
        {
            var stAddr = startAddr;
            ushort[] regs = modbusMaster.ReadHoldingRegisters((byte)slaveId, (ushort)startAddr, (byte)count);
            var regList = regs.Select(reg => new DataItema { Address = startAddr++, Value = reg }).ToList();

            ReadHoldingRegEventHandler?.Invoke(startAddr, count, regList);
        }
    }

    public class DataItema
    {
        public int Address { get; set; }
        public ushort Value { get; set; }
    }
}
