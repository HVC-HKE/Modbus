using System;
using System.Net.Sockets;
using System.Net;
using System.IO.Ports;
using System.Reflection;
using System.Text;
using System.Collections.Generic;
using System.IO;

namespace Modbus
{



    public class ModbusTcpPacket : DataBuffer, IModbusPacket
    {

        private DataBuffer data;
        public ModbusTcpPacket(int numDataBytes) : base(numDataBytes + 8)
        {
            data = GetData(8);
            ProtocolIndentifier = 0;
            LengthField = (ushort)(numDataBytes + 2);
        }
        public ModbusTcpPacket(byte[] buffer) : base(buffer)
        {
            data = GetData(8);
        }
        public ModbusTcpPacket(ModbusTcpPacket that, int numDataBytes) : this(numDataBytes)
        {
            this.TransactionIdentifier = that.TransactionIdentifier;
            this.ProtocolIndentifier = that.ProtocolIndentifier;
            this.UnitIdentifier = that.UnitIdentifier;
            this.FunctionCode = that.FunctionCode;

            this.Payload.PutByte(0, (byte)(numDataBytes-1));
        }
        public ModbusTcpPacket(ModbusTcpPacket that, byte[] bytes) : this(bytes.Length)
        {
            this.TransactionIdentifier = that.TransactionIdentifier;
            this.ProtocolIndentifier = that.ProtocolIndentifier;
            this.UnitIdentifier = that.UnitIdentifier;
            this.FunctionCode = that.FunctionCode;

            this.Payload.PutBytes(0, bytes);
        }



        public ushort TransactionIdentifier { get { return GetShort(0); } set { PutShort(0, value); } }
        public ushort ProtocolIndentifier { get { return GetShort(2); } set { PutShort(2, value); } }
        public int LengthField { get { return (int)GetShort(4); } set { PutShort(4, (ushort)value); } }
        public byte UnitIdentifier { get { return GetByte(6); } set { PutByte(6, value); } }
        public FunctionCodes FunctionCode
        {
            get
            {
                return (FunctionCodes)(GetByte(7) & 0x7F);
            }
            set
            {
                if (GetBit(7,7))
                {
                    PutByte(7, (byte)(((byte)value) | 0x80));
                }
                else
                {
                    PutByte(7, (byte)value);
                }
            }
        }

        public ExceptionCodes ExceptionCode
        {
            get
            {

                    if (GetBit(7,7))
                    return (ExceptionCodes)GetByte(8);
                else
                    return ExceptionCodes.Ok;
            }
            set
            {
                if( value != ExceptionCodes.Ok)
                {
                    PutBit(7, 7, true);
                    PutByte(8, (byte)value);
                    LengthField = 3;
                }
            }
        }

        public DataBuffer Payload => data;


        public override string ToString()
        {
            return $"Transaction identifier {TransactionIdentifier,-5} [{ToHexString(0, 2)}], " +
                   $"Protocol indentifier {ProtocolIndentifier,-2} [{ToHexString(2, 2)}], " +
                   $"Length field {LengthField,-2} [{ToHexString(4, 2)}], " +
                   $"Unit identifier  {UnitIdentifier,-2} [{ToHexString(6, 1)}], " +
                   $"Function code '{FunctionCode,-20}' [{ToHexString(7, 1)}], " +
                   $"Exception code {ExceptionCode}," +
                   $"Payload ({LengthField - 2})=[{Payload.ToHexString()}], " +
                   $"Packet [{this.ToHexString()}]";

        }

        public string ToString( string format )
        {
            if (string.Equals(format, "x", StringComparison.OrdinalIgnoreCase))
                return this.ToHexString();
            return ToString();
        }


        public static ModbusTcpPacket ReadFromStream(Stream stream)
        {
            byte[] tmp = new byte[256];
            int len = stream.Read(tmp, 0, tmp.Length);
            byte[] rsl = new byte[len];
            Array.Copy(tmp, 0, rsl, 0, len);
            return new ModbusTcpPacket(rsl);
        }
    }
}