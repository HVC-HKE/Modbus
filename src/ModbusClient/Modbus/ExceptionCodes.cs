


namespace Modbus
{
    public enum ExceptionCodes
    {
        //
        // No exception
        //
        Ok = 0,
        //
        //  Function code received in the query is not recognized or allowed by slave
        //
        Illegal_Function = 1,
        //
        //  Data address of some or all the required entities are not allowed or do not exist in slave
        //
        Illegal_Data_Address = 2,
        //
        //  Value is not accepted by slave
        //
        Illegal_Data_Value = 3,
        //
        //  Unrecoverable error occurred while slave was attempting to perform requested action
        //
        Slave_Device_Failure = 4,
        //
        //  Slave has accepted request and is processing it, but a long duration of time is required.This response is returned to prevent a timeout error from occurring in the master.Master can next issue a Poll Program Complete message to determine whether processing is completed
        //
        Acknowledge = 5,
        //
        //  Slave is engaged in processing a long-duration command. Master should retry later
        //
        Slave_Device_Busy = 6,
        //
        //  Slave cannot perform the programming functions. Master should request diagnostic or error information from slave
        //
        Negative_Acknowledge = 7,
        //
        //  Slave detected a parity error in memory.Master can retry the request, but service may be required on the slave device
        //
        Memory_Parity_Error = 8,
        //
        //  Specialized for Modbus gateways. Indicates a misconfigured gateway
        //
        Gateway_Path_Unavailable = 10,
        //
        //  Failed to Respond     Specialized for Modbus gateways. Sent when slave fails to respond
        //
        Gateway_Target_Device = 11,

        //
        //  Packet out of sequence. Did not receive trhe expected TransactionIdentifier
        //
        Protocol_Error_Wrong_TransactionIdentifier = 20,
        //
        //  Packet out of sequence. Did not receive the expected number of bytes
        //
        Protocol_Error_Wrong_NumberOfBytes = 21,

    }

}
