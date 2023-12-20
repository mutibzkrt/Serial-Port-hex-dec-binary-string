using COMProject.Enums;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace COMProject
{
    public class SendDataSettings
    {
        public SerialPort SerialPort { get; set; }
        public string ComPortName { get; set; }
        public int BaudRate { get; set; }
        public int DataBits { get; set; }
        public StopBits StopBits { get; set; }
        public Parity ParityBits { get; set; }
        public bool DTREnable { get;set; }
        public bool RTSEnable { get;set; }
        public bool ISPortOpen { get;set; }
        public string SendDataText { get; set; }
        public SendDataTypeEnum SendDataType { get; set; }

    }
}
