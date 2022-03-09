using System;

namespace Modbus
{
    public class ModbusException : Exception
    {
        public ExceptionCodes ExceptionCode { get; } = ExceptionCodes.Ok;

        public ModbusException(string message) : base(message) { }
        public ModbusException(string message, Exception innerException) : base(message, innerException) { }
        public ModbusException(ExceptionCodes exceptionCode) : base($"Modbus exception '{exceptionCode}' ") { ExceptionCode = exceptionCode; }
        public ModbusException(ExceptionCodes exceptionCode, string message) : base(message) { ExceptionCode = exceptionCode; }
    }
}
