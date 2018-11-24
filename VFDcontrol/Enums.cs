// adapted from http://www.cnczone.com/forums/phase-converters/91847-huanyang-vfd-rs485-modbus-7.html
// Credit - user ScottA

using System;

namespace VFDcontrol
{
    [Flags]
    public enum CommandType
    {
        FunctionRead = 0x01,  // response length unknown
        FunctionWrite = 0x02,  // response length 8
        WriteControlData = 0x03,  // response length 6
        ReadControlData = 0x04,  // response length 8
        WriteInverterFrequencyData = 0x05,  // response length 7
        Recerved1 = 0x06, 
        Reserved2 = 0x07,
        LoopTest = 0x08  
    }

    [Flags]
    public enum ControlDataType
    {
        SetF = 0x00,
        OutF = 0x01,
        OutA = 0x02,
        RoTT = 0x03,
        DCV = 0x04,
        ACV = 0x05,
        Cont = 0x06,
        Tmp = 0x07,
    }

    [Flags]
    public enum ControlCommands
    {
        Run_Fwd = 0x01,
        Stop = 0x08,
        Run_Rev = 0x11, // Note that this must be enabled on the VFD - PD??? set to '1'
    }

    [Flags]
    public enum CommandLength
    {
        OneByte = 0x01,
        TwoBytes = 0x02,
        ThreeBytes = 0x03,
    }

    public enum ControlResponse
    {
        Run = 0x01,
        Jog = 0x02,
        Command_rf = 0x04,
        Running = 0x08,
        Jogging = 0x10,
        Running_rf = 0x20,
        Braking = 0x40,
        Track_Start = 0x80,
    }

    public enum SpindleDirection
    {
        Forward,
        Backwards,
    }

    public enum ModbusRegisters
    {
        MaxFreq = 0x05, //PD005
        IntermediateFreq = 0x06, //PD006
        MinimumFreq = 0x07, //PD007
        MaxVoltage = 0x08, //PD008
        IntermediateVoltage = 0x09, //PD009
        MinVoltage = 0x0A, //PD010
        MinFreq = 0x0B, //PD011
        RatedMotorVoltage = 0x8D,  //PD141
        RatedMotorCurrent = 0x8E,  //PD142
        NumberOfMotorPols = 0x8F,  //PD143
        MaxRPM = 0x90,  //PD144
        InverterFrequency = 0xB0,  //PD176

    }

}
