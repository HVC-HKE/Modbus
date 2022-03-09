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
    /// Implements a ModbusClient over Tcp.
    /// </summary>
    public class ModbusTcpClient : IModbusClient
    {
        private TcpClient? tcpClient;

        public ModbusTcpClient(string host, int port = 502) : this()
        {
            Host = host;
            Port = port;
        }
        public ModbusTcpClient()
        {
            tcpClient = new TcpClient();
        }


        public void Dispose()
        {
            if (tcpClient != null)
            {
                tcpClient.Close();
                tcpClient = null;
            }
        }





        /// <summary>
        /// Gets or Sets the hostname or ipaddres of the Modbus-TCP server.
        /// </summary>
		public string Host { get; set; } = "127.0.0.1";

        /// <summary>
        /// Gets or Sets the Port number of the Modbus-TCP server (Default is 502).
        /// </summary>
		public int Port { get; set; } = 502;

        /// <summary>
        /// Unit identifier of Modbus-TCP server
        /// </summary>
        public int UnitIdentifier { get; set; } = 0;

        /// <summary>
        /// Timeout value in milliseconds for connect and read/write actions
        /// </summary>
        public int Timeout { get; set; } = 5000;


        private ushort TransactionCounter { get; set; } = 0;



        #region IModbusClient

        /// <summary>
        /// Establish connection
        /// </summary>
        public void Connect()
        {
            if (tcpClient == null)
            {
                tcpClient = new TcpClient();
            }
            else
                if (tcpClient.Connected)
                {
                    tcpClient.Close();
                    tcpClient = new TcpClient();
                }

            var result = tcpClient.BeginConnect(Host, Port, null, null);
            var success = result.AsyncWaitHandle.WaitOne(Timeout);
            if (!success)
            {
                throw new ModbusException($"Connection timed out after {((decimal)Timeout) / 1000} seconds");
            }
            tcpClient.EndConnect(result);

            tcpClient.GetStream().ReadTimeout = Timeout;
        }


        /// <summary>
        /// Break connection
        /// </summary>
        public void Disconnect()
        {
            if (tcpClient != null)
            {
                tcpClient.Close();
                tcpClient = null;
            }
            TransactionCounter = 0;
        }


        public bool IsConnected
        {
            get
            {
                if (tcpClient == null || tcpClient.Client == null)
                    return false;
                else
                    return tcpClient.Connected;
            }
        }



        public DataBuffer ReadMultipleHoldingRegisters(int startingAddress, int quantity)
        {
            if (!IsConnected)
                throw new ModbusException("Not connected");

            var stream = tcpClient?.GetStream(); 

            if( stream==null)
                throw new ModbusException("No stream");


            ModbusTcpPacket tx_packet = new ModbusTcpPacket(4);

            tx_packet.TransactionIdentifier = (++TransactionCounter);
            tx_packet.FunctionCode = FunctionCodes.ReadMultipleHoldingRegisters;
            tx_packet.UnitIdentifier = (byte)UnitIdentifier;

            tx_packet.Payload.PutShort(0, (ushort)startingAddress);
            tx_packet.Payload.PutShort(2, (ushort)quantity);



            tx_packet.WriteToStream(stream);

            var rx_packet = ModbusTcpPacket.ReadFromStream(stream);

            if (rx_packet.ExceptionCode != ExceptionCodes.Ok)
                throw new ModbusException(rx_packet.ExceptionCode);

            if (rx_packet.TransactionIdentifier != tx_packet.TransactionIdentifier)
            {
                throw new ModbusException(ExceptionCodes.Protocol_Error_Wrong_TransactionIdentifier, $"Received TransactionIdentifier {rx_packet.TransactionIdentifier}, expected {tx_packet.TransactionIdentifier}");
            }

            //
            // Length byte of payload should be quantity*2 
            //
            if (((int)rx_packet.Payload.GetByte(0)) != quantity * 2)
            {
                StringBuilder error = new StringBuilder();

                error.AppendLine($"Unexpected number of bytes received on 'ReadMultipleHoldingRegisters( Addres={startingAddress}, quantity={quantity}').  ");
                error.AppendLine($"Payload length byte = {rx_packet.Payload.GetByte(0)}, quantity={quantity}, packet length byte={rx_packet.LengthField}  ");
                error.AppendLine($"TX:");
                error.AppendLine("    " + tx_packet.ToString());
                error.AppendLine("    " + tx_packet.ToString("x"));

                error.AppendLine($"RX:");
                error.AppendLine("    " + rx_packet.ToString());
                error.AppendLine("    " + rx_packet.ToString("x"));

                throw new ModbusException(error.ToString());
            }



            return rx_packet.Payload;
        }


        public DataBuffer WriteMultipleHoldingRegisters(int startingAddress, int quantity, DataBuffer data)
        {
            if (!IsConnected)
                throw new ModbusException("Not connected");

            var stream = tcpClient?.GetStream();

            if (stream == null)
                throw new ModbusException("No stream");


            ModbusTcpPacket tx_packet = new ModbusTcpPacket(4 + data.Length);

            tx_packet.TransactionIdentifier = (++TransactionCounter);
            tx_packet.FunctionCode = FunctionCodes.WriteMultipleHoldingRegisters;
            tx_packet.UnitIdentifier = (byte)UnitIdentifier;


            tx_packet.Payload.PutShort(0, (ushort)startingAddress);
            tx_packet.Payload.PutShort(2, (ushort)quantity);
            tx_packet.Payload.PutData(4, data);



            tx_packet.WriteToStream(stream);

            var rx_packet = ModbusTcpPacket.ReadFromStream(stream);


            if (rx_packet.ExceptionCode != ExceptionCodes.Ok)
                throw new ModbusException(rx_packet.ExceptionCode);

            return rx_packet.Payload;
        }


        #endregion
    }



}


