using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Modbus
{
    public interface IModbusPacket
    {
        public ushort TransactionIdentifier { get; set; }
        public ushort ProtocolIndentifier { get; set;  }
        public int LengthField { get; set;  }
        public byte UnitIdentifier { get; set; }
        public FunctionCodes FunctionCode { get; set; }
        public ExceptionCodes ExceptionCode { get; set; }
        public DataBuffer Payload { get; }

    }
}
