using System;
using System.Collections.Generic;
using System.Text;

namespace Modbus
{
    public interface IModbusClient : IDisposable
    {
        /// <summary>
        /// Establish connection
        /// </summary>
        public void Connect();

        /// <summary>
        /// Break connection
        /// </summary>
        public void Disconnect();

        public bool IsConnected { get; }



        /// <summary>
        /// Read multiple holding regeisters
        /// </summary>
        /// <param name="startingAddress">Address of first holding register to read</param>
        /// <param name="quantity">Number of 16bits holding registers to read </param>
        /// <returns>DataBuffer containing read regiosters</returns>
        /// <exception cref="Modbus.ModbusException">Thrown when modbus devices replied with exception code.</exception>
        public DataBuffer ReadMultipleHoldingRegisters(int startingAddress, int quantity);


        /// <summary>
        /// Write multiple holding regeisters
        /// </summary>
        /// <param name="startingAddress">Address of first holding register to read</param>
        /// <param name="quantity">Number of 16bits holding registers to read </param>
        /// <param name="data">Databuffer containing new values for registers</param>
        /// <returns>DataBuffer containing read regiosters</returns>
        /// <exception cref="Modbus.ModbusException">Thrown when modbus devices replied with exception code.</exception>
        public DataBuffer WriteMultipleHoldingRegisters(int startingAddress, int quantity, DataBuffer data);


    }
}
