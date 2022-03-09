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
    /// <summary>
    /// Databuffer encapsulates a byte array and offers methods to Get/Put various types of data.
    /// </summary>
    /// <todo>A different Endian types</todo>
    public class DataBuffer
    {
        private byte[] buffer;
        private int offset = 0;
        private int length = 0;
        private DateTime timeStamp;

        public DataBuffer(byte[] _buffer, int _offset, int _length)
        {
            buffer = _buffer;
            offset = _offset;
            length = _length;
            timeStamp = DateTime.UtcNow;
        }
        public DataBuffer(byte[] _buffer) : this(_buffer, 0, _buffer.Length)
        {
        }
        public DataBuffer(int length) : this(new byte[length], 0, length)
        {
        }

        public byte this[int index]
        {
            get
            {
                return buffer[offset + index];
            }
            set
            {
                buffer[offset + index] = value;
            }
        }
        public int Length => length;

        public void PutByte(int index, byte data)
        {
            this[index] = data;
        }
        public byte GetByte(int index)
        {
            return this[index];
        }

        public void PutBit(int index, int bit, bool data)
        {
            if( data)
                this[index] = (byte)((int)this[index] | (0x01 << bit));
            else
                this[index] = (byte)((int)this[index] & ~(0x01 << bit));
        }
        public bool GetBit(int index, int bit)
        {
            return ((int)this[index] & (0x01 << bit)) != 0;
        }



        public void PutShort(int index, ushort data)
        {
            var tmp = BitConverter.GetBytes(data);
            this[index + 1] = tmp[0];
            this[index] = tmp[1];
        }
        public ushort GetShort(int index)
        {
            byte[] tmp = new byte[2];
            tmp[0] = this[index + 1];
            tmp[1] = this[index + 0];
            return BitConverter.ToUInt16(tmp, 0);
        }

        public void PutInt(int index, int data)
        {
            var tmp = BitConverter.GetBytes(data);
            this[index + 1] = tmp[0];
            this[index + 0] = tmp[1];
            this[index + 3] = tmp[2];
            this[index + 2] = tmp[3];
        }
        public int GetInt(int index)
        {
            byte[] tmp = new byte[4];
            tmp[0] = this[index + 1];
            tmp[1] = this[index + 0];
            tmp[2] = this[index + 3];
            tmp[3] = this[index + 2];
            return BitConverter.ToInt32(tmp, 0);
        }


        public void PutSingle(int index, float data)
        {
            var tmp = BitConverter.GetBytes(data);
            this[index + 1] = tmp[0];
            this[index + 0] = tmp[1];
            this[index + 3] = tmp[2];
            //
            // .NET gives 0x0000FFC0 for a NaN, while Bluelog device gives 0x00007FC0, 
            //
            this[index + 2] = (float.IsNaN(data) ? (byte)0x7F : tmp[3]);
        }
        public float GetSingle(int index)
        {
            byte[] tmp = new byte[4];
            tmp[0] = this[index + 1];
            tmp[1] = this[index + 0];
            tmp[2] = this[index + 3];
            tmp[3] = this[index + 2];
            return BitConverter.ToSingle(tmp, 0);
        }

        public byte[] GetBytes()
        {
            return GetBytes(0, this.length);
        }
        public byte[] GetBytes(int offset)
        {
            return GetBytes(offset, this.length - offset);
        }
        public byte[] GetBytes(int offset, int length)
        {
            byte[] tmp = new byte[length];
            Array.Copy(buffer, this.offset + offset, tmp, 0, length);
            return tmp;
        }
        public void PutBytes(int offset, byte[] bytes)
        {
            Array.Copy(bytes, 0, buffer, this.offset + offset, bytes.Length);
        }


        public void PutData(int offset, DataBuffer data)
        {
            for (int i = 0; i < data.Length; ++i)
                this[offset + i] = data[i];
        }


        public DataBuffer GetData(int offset)
        {
            return new DataBuffer(this.buffer, this.offset + offset, this.length - offset);
        }

        public string ToHexString(int o, int l)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = o; i < (o+l); ++i)
            {
                sb.Append((sb.Length == 0 ? "" : " "));
                sb.AppendFormat("{0:X2}", buffer[offset + i]);
            }
            return sb.ToString();
        }
        public string ToHexString()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < length; ++i)
            {
                sb.Append((i == 0 ? "" : " "));
                sb.AppendFormat("{0:X2}", buffer[offset + i]);
            }
            return sb.ToString();
        }
        public string ToBinairyString()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < length; ++i)
            {
                sb.Append((i == 0 ? "" : " "));
                sb.Append(Convert.ToString(buffer[offset + i], 2).PadLeft(8, '0'));
            }
            return sb.ToString();
        }



        public void WriteToStream(Stream stream)
        {
            stream.Write(buffer, offset, length);
        }

        public TimeSpan Age => DateTime.UtcNow - timeStamp;
    }
}
