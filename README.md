### ModbusClient

Simple implementation of a client for reading and writing data to a Modbus device.


Many thanks to:
- [EasyModbusTcp library](https://sourceforge.net/projects/easymodbustcp/)
- [Modbus page on Wikipedia](https://en.wikipedia.org/wiki/Modbus)

Source code for TcpClient:
- https://github.com/microsoft/referencesource/blob/master/System/net/System/Net/Sockets/TCPClient.cs



#### Usage

```c#
using (var client = new Modbus.ModbusTcpClient("192.168.208.40"))
{
    client.UnitIdentifier = 10;
    client.Connect();

    var data = client.ReadMultipleHoldingRegisters(0, 16);

    var value = data.GetSingle(9);

    Console.WriteLine($"Value = {value}");

}
```


