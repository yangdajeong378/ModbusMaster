using ModbusMaster.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModbusMaster.Interface
{
    public interface IModel
    {
        string ConnectedIP { get; } //커넥트 뷰모델에서 생성자로 겟을 해서 디폴트값으로 바꿔놓기
        int ConnectedPort { get; }

        bool IsConnected { get; }

        event ReadEventDelegate ReadHoldingRegEventHandler;
        event ReadEventDelegate ReadCoilEventHandler;
        event ConnectDelegate ConnectEventHandler;

        bool Connect(string ip, int port);
        void Disconnect();

        void ReadCoil(int slaveId, int startAddr, int count);
        void ReadHoldingReg(int slaveId, int startAddr, int count);
    }

    public delegate void ReadEventDelegate(int startAddr, int count, List<DataItema> datas);
    public delegate void ConnectDelegate(bool isConnect);
}
