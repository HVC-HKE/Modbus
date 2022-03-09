


namespace Modbus
{
    public enum FunctionCodes
    {
        ReadCoils = 1,
        ReadDiscreteInputs = 2,
        ReadMultipleHoldingRegisters = 3,
        ReadInputRegisters = 4,
        WriteSingleCoil = 5,
        WriteSingleHoldingRegister = 6,
        WriteMultipleCoils = 15,
        WriteMultipleHoldingRegisters = 16
    }
}
