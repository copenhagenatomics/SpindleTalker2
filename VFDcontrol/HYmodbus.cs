﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;

namespace VfdControl
{
    public class HYmodbus
    {
        private Queue<byte[]> _commandQueue = new Queue<byte[]>();
        private Queue<int> _receivedQueue = new Queue<int>();
        private ManualResetEvent _spindleActive = new ManualResetEvent(true);
        private ManualResetEvent _dataReadyToRead = new ManualResetEvent(true);
        public int ResponseWaitTimeout { get; set; }
        public bool DownloadUploadMode;
        public VFDdata VFDData = new VFDdata();
        public bool ComOpen { get; private set; }
        #region Events
        public delegate void ProcessPollPacket(VFDdata data);
        public delegate void WriteTerminalForm(string message, bool send);
        public delegate void WriteLog(string message, bool error = false);
        public event ProcessPollPacket OnProcessPollPacket;
        public event WriteTerminalForm OnWriteTerminalForm;
        public event WriteLog OnWriteLog;
        #endregion
        #region Profibus connection settings
        /// <summary>Name of serial port to use.</summary>
        public string PortName { get; set; }
        /// <summary>Boud rate. Default is set to 38400.</summary>
        public int BaudRate { get; set; }
        /// <summary>Data bits. Default is set to 8.</summary>
        public int DataBits { get; set; }
        Parity _parity;
        /// <summary>Parity. Default is set to None.</summary>
        public int Parity { get { return (int)_parity; } set { _parity = (Parity)value; } }
        StopBits _stopBits;
        /// <summary>Stop bits. Default is set to One.</summary>
        public int StopBits { get { return (int)_stopBits; } set { _stopBits = (StopBits)value; } }
        /// <summary>Modbus id. Default is set to 1.</summary>
        public int ModBusID { get; set; }
        #endregion
        /// <summary>
        /// Constructor that setup Modbus and starts thread.
        /// </summary>
        /// <param name="portName">Name of serial port to use.</param>
        /// <param name="baudRate">Boud rate. Default is set to 38400.</param>
        /// <param name="dataBits">Data bits. Default is set to 8.</param>
        /// <param name="parity">Parity. Default is set to None.</param>
        /// <param name="stopBits">Stop bits. Default is set to One.</param>
        /// <param name="modBusID">Modbus id. Default is set to 1.</param>
        /// <param name="responseWaitTimeout">In milliseconds. Default is 100.</param>
        public HYmodbus(string portName, int baudRate = 38400, int dataBits = 8, int parity = 0, int stopBits = 1, int modBusID = 1, int responseWaitTimeout = 100)
        {
            ComOpen = false;
            populateCRCTable();
            this.PortName = portName;
            this.BaudRate = baudRate;
            this.DataBits = dataBits;
            this.Parity = parity;
            this.StopBits = stopBits;
            this.ModBusID = modBusID;
            this.ResponseWaitTimeout = responseWaitTimeout;
        }

        

        #region Public Methods
        /// <summary>
        /// Get list of settings in VFD.
        /// </summary>
        /// <returns>List of settings in VFD</returns>
        public List<RegisterValue> GetRegisterValues()
        {
            var res = new List<RegisterValue>();
            for (int i = 0; i < 200; i++)
            {
                try
                {
                    var result = this.SendCommand((byte)CommandType.FunctionRead, 1, (byte)i, 0, 0);
                    if (result != null)
                    {
                        result.Value = result.ToValue();
                        res.Add(result);
                    }
                }
                catch (Exception ex)
                {
                    OnWriteLog(ex.Message, true);
                }
            }
            return res;
        }
        /// <summary>
        /// Set settings values in VFD.
        /// </summary>
        /// <param name="values">Vales to set in VFD.</param>
        public void SetRegisterValues(List<RegisterValue> values)
        {
            foreach (var line in values.Where(x => x.DefaultValue != "Unknown"))
            {
                try
                {
                    var result = this.SendCommand((byte)CommandType.FunctionWrite, (byte)line.CommandLength, line.ID, line.data0, line.data1);
                    OnWriteLog(result.ToString());
                }
                catch (Exception ex)
                {
                    OnWriteLog(ex.Message, true);
                }
            }
        }
        /// <summary>
        /// Connect to VFD.
        /// </summary>
        public void Connect()
        {
            if(!ComOpen)
            {
                new Thread(() => DoWork()).Start();
            }

            DownloadUploadMode = false;
            _spindleActive.Reset();   
            InitialPoll();
        }

        public void InitialPollPowerMeter()
        {
            byte[] packet = new byte[8];
            packet[0] = (byte)this.ModBusID;
            packet[1] = (byte)CommandType.ReadControlData;
            packet[2] = (byte)CommandLength.Float;
            packet[4] = 0x00;
            packet[5] = 0x00;
            packet[6] = 0x00;
            packet[7] = 0x00;
            VFDData.Clear();

            for (byte i = 0; i < 20; i++)
            {
                packet[3] = i;
                SendDataAsync(packet);
            }
        }

        public void InitialPoll()
        {
            byte[] packet = new byte[6];
            packet[0] = (byte)this.ModBusID;
            packet[1] = (byte)CommandType.ReadControlData;
            packet[2] = (byte)CommandLength.ThreeBytes;
            packet[3] = (byte)ControlDataType.OutF;
            packet[4] = 0x00;
            packet[5] = 0x00;
            VFDData.Clear();

            // Request current RPM to see if the spindle is actually still running
            SendDataAsync(packet);

            // Request base frequency
            packet[1] = (byte)CommandType.FunctionRead;
            packet[3] = (byte)ModbusRegisters.BaseFreq;
            SendDataAsync(packet);

            // Request max frequency
            packet[3] = (byte)ModbusRegisters.MaxFreq;
            SendDataAsync(packet);

            // Request intermediate frequency
            packet[3] = (byte)ModbusRegisters.IntermediateFreq;
            SendDataAsync(packet);

            // Request minimum frequency
            packet[3] = (byte)ModbusRegisters.MinimumFreq;
            SendDataAsync(packet);

            // Request max voltage
            packet[3] = (byte)ModbusRegisters.MaxVoltage;
            SendDataAsync(packet);

            // Request intermediate voltage
            packet[3] = (byte)ModbusRegisters.IntermediateVoltage;
            SendDataAsync(packet);

            // Request minimum voltage
            packet[3] = (byte)ModbusRegisters.MinVoltage;
            SendDataAsync(packet);

            // Request minimum frequency
            packet[3] = (byte)ModbusRegisters.LowerLimitFreq;
            SendDataAsync(packet);

            // Request rated motor voltage
            packet[3] = (byte)ModbusRegisters.RatedMotorVoltage;
            SendDataAsync(packet);

            // Request rated motor current
            packet[3] = (byte)ModbusRegisters.RatedMotorCurrent;
            SendDataAsync(packet);

            // Request number of motor pols
            packet[3] = (byte)ModbusRegisters.NumberOfMotorPols;
            SendDataAsync(packet);

            // Request max RPM
            packet[3] = (byte)ModbusRegisters.MaxRPM;
            SendDataAsync(packet);

            // Request inverter frequency
            packet[3] = (byte)ModbusRegisters.InverterFrequency;
            SendDataAsync(packet);

            // Request VFD voltage rating.
            packet[3] = (byte)ModbusRegisters.VFDVoltageRating;
            SendDataAsync(packet);

            // Request motor acceleration.
            packet[3] = (byte)ModbusRegisters.Acceleration;
            SendDataAsync(packet);

            // Request motor deceleration.
            packet[3] = (byte)ModbusRegisters.Deceleration;
            SendDataAsync(packet);

        }

        public void Disconnect()
        {
            _commandQueue.Clear();
            VFDData.SerialConnected = false;
            SendDataAsync(new byte[] { 0xff, 0xff, 0xff, 0xff, 0xff });
            Thread.Sleep(100);
        }

        public void StartPolling()
        {
            DownloadUploadMode = false;
            _spindleActive.Set();
        }

        public RegisterValue SendCommand(CommandType commandType, byte register, int value)
        {
            return SendCommand((byte)commandType, 3, register, (byte)(value & 0xFF), (byte)(value >> 8));
        }

        public RegisterValue SendCommand(byte selectedCommandType, byte selectedCommandLength, int register, byte _data1, byte _data2)
        {
            int packetLength = selectedCommandLength + 3;
            byte[] command = new byte[packetLength];
            command[0] = (byte)this.ModBusID;
            command[1] = selectedCommandType;
            command[2] = selectedCommandLength;
            command[3] = (byte)register;
            if (packetLength > 4) command[4] = _data1;
            if (packetLength > 5) command[5] = _data2;

            return new RegisterValue(register)
            {
                Value = SendData(command).ToString()
            };
        }

        public void SendDataAsync(byte[] dataToSend)
        {
            lock (_commandQueue)
            {
                _commandQueue.Enqueue(crc16byte(dataToSend));
            }
            _spindleActive.Set();
        }

        public int SendData(byte[] dataToSend)
        {
            DownloadUploadMode = true;
            lock (_commandQueue)
            {
                _commandQueue.Enqueue(crc16byte(dataToSend));
            }

            _spindleActive.Set();
            for (int i = 0; i < 8; i++)
            {
                //Application.DoEvents(); Code disabled for .net core
                if (_receivedQueue.Any())
                    return _receivedQueue.Dequeue();

                Thread.Sleep(50);
            }

            return 0;
        }
        #endregion

        #region Background Worker
        private void ProcessReceivedPacket(byte[] receivedPacket)
        {
            if (receivedPacket.Length == 0)
            {
                VFDData.ReadError = true;
                OnWriteTerminalForm?.Invoke($"no data received..", false);
                return;
            }

            if (receivedPacket[0] != (byte)this.ModBusID)
            {
                VFDData.ReadError = true;
                return;
            }

            if (receivedPacket.Length < 4)
            {
                VFDData.ReadError = true;
                return;
            }

            string hexString = ByteArrayToHexString(receivedPacket);
            if (!CRCCheck(receivedPacket))
            {
                VFDData.ReadError = true;
                OnWriteLog?.Invoke($"{DateTime.Now.ToString("H:mm:ss.ff")} - CRC Failed : {hexString}", true);
                return;
            }

            VFDData.ReadError = false;

            int receivedValue = Convert.ToInt32(receivedPacket[receivedPacket.Length - 3]);
            if (receivedPacket.Length == 8)
            {
                receivedValue += Convert.ToInt32(receivedPacket[receivedPacket.Length - 4] << 8);
            }

            if (receivedPacket[1] == (byte)CommandType.ReadControlData && receivedPacket[2] == (byte)CommandLength.ThreeBytes)
            {
                ProcessControlData(receivedValue, receivedPacket[3]);
                //  Debug.Print(VFDData.GetControlDataString());
                OnProcessPollPacket?.Invoke(VFDData);
            }
            else if(receivedPacket[1] == (byte)CommandType.FunctionRead || receivedPacket[1] == (byte)CommandType.FunctionWrite)
            {
                if (DownloadUploadMode)
                {
                    _receivedQueue.Enqueue(receivedValue);
                    string message = $"{DateTime.Now.ToString("H:mm:ss.fff")} - Data received : {hexString} ({receivedValue})";
                    Debug.Print(message);
                    OnWriteTerminalForm?.Invoke(message, false);
                }
                else
                {
                    ProcessInitData(receivedValue, receivedPacket[3], hexString);
                    if (VFDData.InitDataOK())
                        StartPolling();
                }
            }
        }

        private void PrintReceivedData(string text, double value)
        {
            string message = $"{DateTime.Now.ToString("H:mm:ss.ff")} - {text} = {value}";
            OnWriteLog?.Invoke(message, false);
            OnWriteTerminalForm?.Invoke(message, false);
        }

        private void PrintSendData(byte[] buffer)
        {
            int rawValue = Convert.ToInt32(buffer[buffer.Length - 3]);
            if (buffer.Length == 8)
            {
                rawValue += Convert.ToInt32(buffer[buffer.Length - 4] << 8);
            }

            string message = $"{DateTime.Now.ToString("H:mm:ss.ff")} - Data sent : {ByteArrayToHexString(buffer)} ({rawValue})";
            OnWriteLog?.Invoke(message, false);
            OnWriteTerminalForm?.Invoke(message, true);
        }

        void DoWork()
        {
            if (this.BaudRate == 0)
                return; // VFD not connected

            SerialPort comPort = new SerialPort();
            comPort.BaudRate = this.BaudRate;
            comPort.DataBits = this.DataBits;
            comPort.StopBits = _stopBits;
            comPort.Parity = _parity;
            comPort.PortName = this.PortName;
            //comPort.Handshake = Handshake.None;  // default
            //comPort.ReadTimeout = 2000;
            //comPort.WriteTimeout = 2000;

            try
            {
                comPort.Open();
            }
            catch (Exception)
            {
                OnWriteLog?.Invoke($"Unable to open VFD serial port {comPort.PortName}, {comPort.BaudRate}", true);
                VFDData.SerialConnected = false;
                return;
            }

            if (comPort.IsOpen)
            {
                ComOpen = true;
                OnWriteLog?.Invoke($"Motor controller serial port is open: {comPort.PortName}, {comPort.BaudRate}", false);
                comPort.DataReceived += comPort_DataReceived;
                VFDData.SerialConnected = true; // Report that the COM port has opened successfully
            }

            byte[] statusRequestPacket = new byte[6];
            statusRequestPacket[0] = (byte)this.ModBusID; // Slave address
            statusRequestPacket[1] = (byte)CommandType.ReadControlData; // Huanyang VFD Read Control Data
            statusRequestPacket[2] = (byte)CommandLength.ThreeBytes; // Number of bytes in request field
            statusRequestPacket[3] = 0x00; // Register byte - 0x00 = Set Frequency, 0x01 = Output Frequency, 0x02 = Output Amps, 0x03 = RPM
            statusRequestPacket[4] = 0x00; // Padding
            statusRequestPacket[5] = 0x00; // Padding

            while (_spindleActive.WaitOne())
            {
                // ModBus packet format
                //      [xx]   |     [xx]     |      [xx]      | [xx] [xx] [..] | [xx][xx]
                //    Slave ID | Command Type | Request Length |     Request    |   CRC   
                //
                var dataToSend = GetData(statusRequestPacket);
                if (dataToSend != null)
                {
                    if (dataToSend[0] == 0xff && dataToSend[1] == 0xff) // everyone shutdown
                    {
                        comPort.Close();
                        VFDData.SerialConnected = false;
                        VFDData.ReceivingValues = false;
                        ComOpen = false;
                        return;
                    }

                    try
                    {
                        comPort.Write(dataToSend, 0, dataToSend.Length);
                        if (dataToSend[1] != (byte)CommandType.ReadControlData)
                        {
                            //PrintSendData(dataToSend);
                        }

                        if (_dataReadyToRead.WaitOne(500)) // Wait for a notification from comPort.dataReceived, timeout after 500ms
                        {
                            VFDData.ReceivingValues = true;
                            var dataReceived = ReadData(comPort, GetResponseLength(dataToSend[1]));
                            ProcessReceivedPacket(dataReceived);
                        }
                        else
                        {
                            VFDData.ReceivingValues = false;
                        }
                    }
                    catch (Exception ex)
                    {
                        OnWriteLog?.Invoke("VFD Read / Write error: " + ex.ToString(), true);
                        break; // exit. 
                    }
                }
            }

            try { comPort.Close(); } catch { }
            VFDData.SerialConnected = false;
            VFDData.ReceivingValues = false;
            ComOpen = false;
        }

        private byte[] CRCSign(byte[] byteArrayToSign) { return crc16byte(byteArrayToSign); }

        private bool CRCCheck(byte[] byteArrayToCheck)
        {
            var rawMessage = new byte[byteArrayToCheck.Length - 2];
            Buffer.BlockCopy(byteArrayToCheck, 0, rawMessage, 0, byteArrayToCheck.Length - 2); // Get the packet without the last two bytes (the existing CRC)

            bool validCRC = byteArrayToCheck.SequenceEqual(CRCSign(rawMessage));
            return validCRC;
        }

        private byte[] GetData(byte[] statusRequestPacket)
        {
            lock (_commandQueue)
            {
                if (_commandQueue.Count > 0)
                {
                    return _commandQueue.Dequeue();
                }
                else if(!DownloadUploadMode) // If there is no command in the queue, use the time for polling
                {
                    
                    // loop through the registers and poll one at a time. 
                    if (statusRequestPacket[3] < 0x07) statusRequestPacket[3] += 1;
                    else statusRequestPacket[3] = 0x00;

                    return crc16byte(statusRequestPacket);
                }

                return null;
            }
        }

        private byte[] ReadData(SerialPort comPort, int expectedResponseLength)
        {
            byte[] dataReceived;
            // Wait for the expected number of bytes or timeout.
            int responseLoopTimeoutCount = 0;
            while (comPort.BytesToRead < expectedResponseLength && responseLoopTimeoutCount < ResponseWaitTimeout / 10)
            {
                Thread.Sleep(10);
                responseLoopTimeoutCount++;
            }

            if (comPort.BytesToRead < expectedResponseLength) expectedResponseLength = comPort.BytesToRead;

            dataReceived = new byte[expectedResponseLength];
            comPort.Read(dataReceived, 0, expectedResponseLength);
            return dataReceived;
        }

        private int GetResponseLength(byte secondByte)
        {
            switch (secondByte)
            {
                case 0x03: return 6;
                case 0x05: return 7;
                default: return 8;
            }
        }

        private void ProcessControlData(int rawValue, byte cmdType)
        {
            switch (cmdType)
            {
                case (byte)ControlDataType.SetF:
                    VFDData.SetFrequency = rawValue / 100.0;
                    return;
                case (byte)ControlDataType.OutF:
                    VFDData.OutFrequency = rawValue / 100.0;
                    return;
                case (byte)ControlDataType.RoTT:
                    VFDData.OutRPM = rawValue;
                    return;
                case (byte)ControlDataType.OutA:
                    VFDData.OutAmp = rawValue / 10.0;
                    return;
                case (byte)ControlDataType.DCV:
                    VFDData.OutVoltDC = rawValue / 10.0;
                    return;
                case (byte)ControlDataType.ACV:
                    VFDData.OutVoltAC = rawValue / 10.0;
                    return;
                case (byte)ControlDataType.Tmp:
                    VFDData.OutTemperature = rawValue / 10.0;
                    return;
            }
        }

        private void ProcessInitData(int rawValue, byte cmdType, string hexString)
        {
            switch (cmdType)
            {
                case (byte)ModbusRegisters.BaseFreq:
                    VFDData.BaseFreq = rawValue / 100D;
                    PrintReceivedData("Net Frequency (Hz)", VFDData.BaseFreq);
                    return;
                case (byte)ModbusRegisters.MaxFreq:
                    VFDData.MaxFreq = rawValue / 100D;
                    PrintReceivedData("Maximum Frequency (Hz)", VFDData.MaxFreq);
                    return;
                case (byte)ModbusRegisters.LowerLimitFreq:
                    VFDData.LowerLevelFreq = rawValue / 100D;
                    PrintReceivedData("Lower Limit Frequency (Hz)", VFDData.LowerLevelFreq);
                    return;
                case (byte)ModbusRegisters.MaxRPM:
                    VFDData.RatedMotorRPM = rawValue;
                    VFDData.MaxRPM = (int)(VFDData.RatedMotorRPM * VFDData.MaxFreq / 50.0); // adjust motor value to what is possible with current VFD settings. 
                    PrintReceivedData("Motor rated RPM (@50 Hz)", VFDData.RatedMotorRPM); 
                    PrintReceivedData("Maximum RPM", VFDData.MaxRPM);
                    return;
                case (byte)ModbusRegisters.IntermediateFreq:
                    VFDData.IntermediateFreq = rawValue / 100D;
                    PrintReceivedData("Intermediate Frequency", VFDData.IntermediateFreq);
                    return;
                case (byte)ModbusRegisters.MinimumFreq:
                    VFDData.MinimumFreq = rawValue / 100D;
                    PrintReceivedData("Minimum Frequency", VFDData.MinimumFreq);
                    return;
                case (byte)ModbusRegisters.MaxVoltage:
                    VFDData.MaxVoltage = rawValue / 10.0;
                    PrintReceivedData("Maximum Output Voltage", VFDData.MaxVoltage);
                    return;
                case (byte)ModbusRegisters.IntermediateVoltage:
                    VFDData.IntermediateVoltage = rawValue / 10.0;
                    PrintReceivedData("Intermediate Voltage", VFDData.IntermediateVoltage);
                    return;
                case (byte)ModbusRegisters.MinVoltage:
                    VFDData.MinVoltage = rawValue / 10;
                    PrintReceivedData("Minimum Voltage", VFDData.MinVoltage);
                    return;
                case (byte)ModbusRegisters.RatedMotorVoltage:
                    VFDData.RatedMotorVoltage = rawValue / 10.0;
                    PrintReceivedData("Rated Motor Voltage", VFDData.RatedMotorVoltage);
                    return;
                case (byte)ModbusRegisters.RatedMotorCurrent:
                    VFDData.RatedMotorCurrent = rawValue / 10.0;
                    PrintReceivedData("Rated Motor Current", VFDData.RatedMotorCurrent);
                    return;
                case (byte)ModbusRegisters.NumberOfMotorPols:
                    VFDData.NumberOfMotorPols = rawValue;
                    PrintReceivedData("Number Of Motor Pols", VFDData.NumberOfMotorPols);
                    return;
                case (byte)ModbusRegisters.InverterFrequency:
                    VFDData.InverterFrequency = rawValue == 1 ? 60 : 50;
                    PrintReceivedData("Inverter Frequency (Hz)", VFDData.InverterFrequency);
                    return;
                case (byte)ModbusRegisters.VFDVoltageRating:
                    VFDData.VFDVoltageRating = rawValue / 10;
                    PrintReceivedData("VFD Voltage rating (V)", VFDData.VFDVoltageRating);
                    return;
                case (byte)ModbusRegisters.Acceleration:
                    VFDData.Acceleration = rawValue / 10.0;
                    PrintReceivedData("Acceleration ", VFDData.Acceleration);
                    return;
                case (byte)ModbusRegisters.Deceleration:
                    VFDData.Deceleration = rawValue / 10.0;
                    PrintReceivedData("Deceleration", VFDData.Deceleration);
                    return;
            }

            // if unknown command then print:
            OnWriteLog?.Invoke($"{DateTime.Now.ToString("H:mm:ss.ff")} - Initial poll packet = {hexString} = {rawValue}", false);
        }

        private void comPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            _dataReadyToRead.Set();
        }

        #endregion

        #region Data Manipulation

        private byte[] HexStringToByteArray(string s)
        {
            s = s.Replace(" ", "");
            byte[] buffer = new byte[s.Length / 2];
            for (int i = 0; i < s.Length; i += 2)
                buffer[i / 2] = (byte)Convert.ToByte(s.Substring(i, 2), 16);
            return buffer;
        }

        private string ByteArrayToHexString(byte[] data)
        {
            StringBuilder sb = new StringBuilder(data.Length * 3);
            foreach (byte b in data)
                sb.Append(Convert.ToString(b, 16).PadLeft(2, '0').PadRight(3, ' '));
            return sb.ToString().ToUpper();
        }

        #endregion

        #region CRC Calculation

        // Taken from http://www.codeproject.com/Articles/19214/CRC-Calculation
        // Credit to Ranjan.D

        private byte[] crc_table { get; set; } = new byte[512];

        #region Lookup Table
        private void populateCRCTable()
        {
            crc_table[0] = 0x0;
            crc_table[1] = 0xC1;
            crc_table[2] = 0x81;
            crc_table[3] = 0x40;
            crc_table[4] = 0x1;
            crc_table[5] = 0xC0;
            crc_table[6] = 0x80;
            crc_table[7] = 0x41;
            crc_table[8] = 0x1;
            crc_table[9] = 0xC0;
            crc_table[10] = 0x80;
            crc_table[11] = 0x41;
            crc_table[12] = 0x0;
            crc_table[13] = 0xC1;
            crc_table[14] = 0x81;
            crc_table[15] = 0x40;
            crc_table[16] = 0x1;
            crc_table[17] = 0xC0;
            crc_table[18] = 0x80;
            crc_table[19] = 0x41;
            crc_table[20] = 0x0;
            crc_table[21] = 0xC1;
            crc_table[22] = 0x81;
            crc_table[23] = 0x40;
            crc_table[24] = 0x0;
            crc_table[25] = 0xC1;
            crc_table[26] = 0x81;
            crc_table[27] = 0x40;
            crc_table[28] = 0x1;
            crc_table[29] = 0xC0;
            crc_table[30] = 0x80;
            crc_table[31] = 0x41;
            crc_table[32] = 0x1;
            crc_table[33] = 0xC0;
            crc_table[34] = 0x80;
            crc_table[35] = 0x41;
            crc_table[36] = 0x0;
            crc_table[37] = 0xC1;
            crc_table[38] = 0x81;
            crc_table[39] = 0x40;
            crc_table[40] = 0x0;
            crc_table[41] = 0xC1;
            crc_table[42] = 0x81;
            crc_table[43] = 0x40;
            crc_table[44] = 0x1;
            crc_table[45] = 0xC0;
            crc_table[46] = 0x80;
            crc_table[47] = 0x41;
            crc_table[48] = 0x0;
            crc_table[49] = 0xC1;
            crc_table[50] = 0x81;
            crc_table[51] = 0x40;
            crc_table[52] = 0x1;
            crc_table[53] = 0xC0;
            crc_table[54] = 0x80;
            crc_table[55] = 0x41;
            crc_table[56] = 0x1;
            crc_table[57] = 0xC0;
            crc_table[58] = 0x80;
            crc_table[59] = 0x41;
            crc_table[60] = 0x0;
            crc_table[61] = 0xC1;
            crc_table[62] = 0x81;
            crc_table[63] = 0x40;
            crc_table[64] = 0x1;
            crc_table[65] = 0xC0;
            crc_table[66] = 0x80;
            crc_table[67] = 0x41;
            crc_table[68] = 0x0;
            crc_table[69] = 0xC1;
            crc_table[70] = 0x81;
            crc_table[71] = 0x40;
            crc_table[72] = 0x0;
            crc_table[73] = 0xC1;
            crc_table[74] = 0x81;
            crc_table[75] = 0x40;
            crc_table[76] = 0x1;
            crc_table[77] = 0xC0;
            crc_table[78] = 0x80;
            crc_table[79] = 0x41;
            crc_table[80] = 0x0;
            crc_table[81] = 0xC1;
            crc_table[82] = 0x81;
            crc_table[83] = 0x40;
            crc_table[84] = 0x1;
            crc_table[85] = 0xC0;
            crc_table[86] = 0x80;
            crc_table[87] = 0x41;
            crc_table[88] = 0x1;
            crc_table[89] = 0xC0;
            crc_table[90] = 0x80;
            crc_table[91] = 0x41;
            crc_table[92] = 0x0;
            crc_table[93] = 0xC1;
            crc_table[94] = 0x81;
            crc_table[95] = 0x40;
            crc_table[96] = 0x0;
            crc_table[97] = 0xC1;
            crc_table[98] = 0x81;
            crc_table[99] = 0x40;
            crc_table[100] = 0x1;
            crc_table[101] = 0xC0;
            crc_table[102] = 0x80;
            crc_table[103] = 0x41;
            crc_table[104] = 0x1;
            crc_table[105] = 0xC0;
            crc_table[106] = 0x80;
            crc_table[107] = 0x41;
            crc_table[108] = 0x0;
            crc_table[109] = 0xC1;
            crc_table[110] = 0x81;
            crc_table[111] = 0x40;
            crc_table[112] = 0x1;
            crc_table[113] = 0xC0;
            crc_table[114] = 0x80;
            crc_table[115] = 0x41;
            crc_table[116] = 0x0;
            crc_table[117] = 0xC1;
            crc_table[118] = 0x81;
            crc_table[119] = 0x40;
            crc_table[120] = 0x0;
            crc_table[121] = 0xC1;
            crc_table[122] = 0x81;
            crc_table[123] = 0x40;
            crc_table[124] = 0x1;
            crc_table[125] = 0xC0;
            crc_table[126] = 0x80;
            crc_table[127] = 0x41;
            crc_table[128] = 0x1;
            crc_table[129] = 0xC0;
            crc_table[130] = 0x80;
            crc_table[131] = 0x41;
            crc_table[132] = 0x0;
            crc_table[133] = 0xC1;
            crc_table[134] = 0x81;
            crc_table[135] = 0x40;
            crc_table[136] = 0x0;
            crc_table[137] = 0xC1;
            crc_table[138] = 0x81;
            crc_table[139] = 0x40;
            crc_table[140] = 0x1;
            crc_table[141] = 0xC0;
            crc_table[142] = 0x80;
            crc_table[143] = 0x41;
            crc_table[144] = 0x0;
            crc_table[145] = 0xC1;
            crc_table[146] = 0x81;
            crc_table[147] = 0x40;
            crc_table[148] = 0x1;
            crc_table[149] = 0xC0;
            crc_table[150] = 0x80;
            crc_table[151] = 0x41;
            crc_table[152] = 0x1;
            crc_table[153] = 0xC0;
            crc_table[154] = 0x80;
            crc_table[155] = 0x41;
            crc_table[156] = 0x0;
            crc_table[157] = 0xC1;
            crc_table[158] = 0x81;
            crc_table[159] = 0x40;
            crc_table[160] = 0x0;
            crc_table[161] = 0xC1;
            crc_table[162] = 0x81;
            crc_table[163] = 0x40;
            crc_table[164] = 0x1;
            crc_table[165] = 0xC0;
            crc_table[166] = 0x80;
            crc_table[167] = 0x41;
            crc_table[168] = 0x1;
            crc_table[169] = 0xC0;
            crc_table[170] = 0x80;
            crc_table[171] = 0x41;
            crc_table[172] = 0x0;
            crc_table[173] = 0xC1;
            crc_table[174] = 0x81;
            crc_table[175] = 0x40;
            crc_table[176] = 0x1;
            crc_table[177] = 0xC0;
            crc_table[178] = 0x80;
            crc_table[179] = 0x41;
            crc_table[180] = 0x0;
            crc_table[181] = 0xC1;
            crc_table[182] = 0x81;
            crc_table[183] = 0x40;
            crc_table[184] = 0x0;
            crc_table[185] = 0xC1;
            crc_table[186] = 0x81;
            crc_table[187] = 0x40;
            crc_table[188] = 0x1;
            crc_table[189] = 0xC0;
            crc_table[190] = 0x80;
            crc_table[191] = 0x41;
            crc_table[192] = 0x0;
            crc_table[193] = 0xC1;
            crc_table[194] = 0x81;
            crc_table[195] = 0x40;
            crc_table[196] = 0x1;
            crc_table[197] = 0xC0;
            crc_table[198] = 0x80;
            crc_table[199] = 0x41;
            crc_table[200] = 0x1;
            crc_table[201] = 0xC0;
            crc_table[202] = 0x80;
            crc_table[203] = 0x41;
            crc_table[204] = 0x0;
            crc_table[205] = 0xC1;
            crc_table[206] = 0x81;
            crc_table[207] = 0x40;
            crc_table[208] = 0x1;
            crc_table[209] = 0xC0;
            crc_table[210] = 0x80;
            crc_table[211] = 0x41;
            crc_table[212] = 0x0;
            crc_table[213] = 0xC1;
            crc_table[214] = 0x81;
            crc_table[215] = 0x40;
            crc_table[216] = 0x0;
            crc_table[217] = 0xC1;
            crc_table[218] = 0x81;
            crc_table[219] = 0x40;
            crc_table[220] = 0x1;
            crc_table[221] = 0xC0;
            crc_table[222] = 0x80;
            crc_table[223] = 0x41;
            crc_table[224] = 0x1;
            crc_table[225] = 0xC0;
            crc_table[226] = 0x80;
            crc_table[227] = 0x41;
            crc_table[228] = 0x0;
            crc_table[229] = 0xC1;
            crc_table[230] = 0x81;
            crc_table[231] = 0x40;
            crc_table[232] = 0x0;
            crc_table[233] = 0xC1;
            crc_table[234] = 0x81;
            crc_table[235] = 0x40;
            crc_table[236] = 0x1;
            crc_table[237] = 0xC0;
            crc_table[238] = 0x80;
            crc_table[239] = 0x41;
            crc_table[240] = 0x0;
            crc_table[241] = 0xC1;
            crc_table[242] = 0x81;
            crc_table[243] = 0x40;
            crc_table[244] = 0x1;
            crc_table[245] = 0xC0;
            crc_table[246] = 0x80;
            crc_table[247] = 0x41;
            crc_table[248] = 0x1;
            crc_table[249] = 0xC0;
            crc_table[250] = 0x80;
            crc_table[251] = 0x41;
            crc_table[252] = 0x0;
            crc_table[253] = 0xC1;
            crc_table[254] = 0x81;
            crc_table[255] = 0x40;
            crc_table[256] = 0x0;
            crc_table[257] = 0xC0;
            crc_table[258] = 0xC1;
            crc_table[259] = 0x1;
            crc_table[260] = 0xC3;
            crc_table[261] = 0x3;
            crc_table[262] = 0x2;
            crc_table[263] = 0xC2;
            crc_table[264] = 0xC6;
            crc_table[265] = 0x6;
            crc_table[266] = 0x7;
            crc_table[267] = 0xC7;
            crc_table[268] = 0x5;
            crc_table[269] = 0xC5;
            crc_table[270] = 0xC4;
            crc_table[271] = 0x4;
            crc_table[272] = 0xCC;
            crc_table[273] = 0xC;
            crc_table[274] = 0xD;
            crc_table[275] = 0xCD;
            crc_table[276] = 0xF;
            crc_table[277] = 0xCF;
            crc_table[278] = 0xCE;
            crc_table[279] = 0xE;
            crc_table[280] = 0xA;
            crc_table[281] = 0xCA;
            crc_table[282] = 0xCB;
            crc_table[283] = 0xB;
            crc_table[284] = 0xC9;
            crc_table[285] = 0x9;
            crc_table[286] = 0x8;
            crc_table[287] = 0xC8;
            crc_table[288] = 0xD8;
            crc_table[289] = 0x18;
            crc_table[290] = 0x19;
            crc_table[291] = 0xD9;
            crc_table[292] = 0x1B;
            crc_table[293] = 0xDB;
            crc_table[294] = 0xDA;
            crc_table[295] = 0x1A;
            crc_table[296] = 0x1E;
            crc_table[297] = 0xDE;
            crc_table[298] = 0xDF;
            crc_table[299] = 0x1F;
            crc_table[300] = 0xDD;
            crc_table[301] = 0x1D;
            crc_table[302] = 0x1C;
            crc_table[303] = 0xDC;
            crc_table[304] = 0x14;
            crc_table[305] = 0xD4;
            crc_table[306] = 0xD5;
            crc_table[307] = 0x15;
            crc_table[308] = 0xD7;
            crc_table[309] = 0x17;
            crc_table[310] = 0x16;
            crc_table[311] = 0xD6;
            crc_table[312] = 0xD2;
            crc_table[313] = 0x12;
            crc_table[314] = 0x13;
            crc_table[315] = 0xD3;
            crc_table[316] = 0x11;
            crc_table[317] = 0xD1;
            crc_table[318] = 0xD0;
            crc_table[319] = 0x10;
            crc_table[320] = 0xF0;
            crc_table[321] = 0x30;
            crc_table[322] = 0x31;
            crc_table[323] = 0xF1;
            crc_table[324] = 0x33;
            crc_table[325] = 0xF3;
            crc_table[326] = 0xF2;
            crc_table[327] = 0x32;
            crc_table[328] = 0x36;
            crc_table[329] = 0xF6;
            crc_table[330] = 0xF7;
            crc_table[331] = 0x37;
            crc_table[332] = 0xF5;
            crc_table[333] = 0x35;
            crc_table[334] = 0x34;
            crc_table[335] = 0xF4;
            crc_table[336] = 0x3C;
            crc_table[337] = 0xFC;
            crc_table[338] = 0xFD;
            crc_table[339] = 0x3D;
            crc_table[340] = 0xFF;
            crc_table[341] = 0x3F;
            crc_table[342] = 0x3E;
            crc_table[343] = 0xFE;
            crc_table[344] = 0xFA;
            crc_table[345] = 0x3A;
            crc_table[346] = 0x3B;
            crc_table[347] = 0xFB;
            crc_table[348] = 0x39;
            crc_table[349] = 0xF9;
            crc_table[350] = 0xF8;
            crc_table[351] = 0x38;
            crc_table[352] = 0x28;
            crc_table[353] = 0xE8;
            crc_table[354] = 0xE9;
            crc_table[355] = 0x29;
            crc_table[356] = 0xEB;
            crc_table[357] = 0x2B;
            crc_table[358] = 0x2A;
            crc_table[359] = 0xEA;
            crc_table[360] = 0xEE;
            crc_table[361] = 0x2E;
            crc_table[362] = 0x2F;
            crc_table[363] = 0xEF;
            crc_table[364] = 0x2D;
            crc_table[365] = 0xED;
            crc_table[366] = 0xEC;
            crc_table[367] = 0x2C;
            crc_table[368] = 0xE4;
            crc_table[369] = 0x24;
            crc_table[370] = 0x25;
            crc_table[371] = 0xE5;
            crc_table[372] = 0x27;
            crc_table[373] = 0xE7;
            crc_table[374] = 0xE6;
            crc_table[375] = 0x26;
            crc_table[376] = 0x22;
            crc_table[377] = 0xE2;
            crc_table[378] = 0xE3;
            crc_table[379] = 0x23;
            crc_table[380] = 0xE1;
            crc_table[381] = 0x21;
            crc_table[382] = 0x20;
            crc_table[383] = 0xE0;
            crc_table[384] = 0xA0;
            crc_table[385] = 0x60;
            crc_table[386] = 0x61;
            crc_table[387] = 0xA1;
            crc_table[388] = 0x63;
            crc_table[389] = 0xA3;
            crc_table[390] = 0xA2;
            crc_table[391] = 0x62;
            crc_table[392] = 0x66;
            crc_table[393] = 0xA6;
            crc_table[394] = 0xA7;
            crc_table[395] = 0x67;
            crc_table[396] = 0xA5;
            crc_table[397] = 0x65;
            crc_table[398] = 0x64;
            crc_table[399] = 0xA4;
            crc_table[400] = 0x6C;
            crc_table[401] = 0xAC;
            crc_table[402] = 0xAD;
            crc_table[403] = 0x6D;
            crc_table[404] = 0xAF;
            crc_table[405] = 0x6F;
            crc_table[406] = 0x6E;
            crc_table[407] = 0xAE;
            crc_table[408] = 0xAA;
            crc_table[409] = 0x6A;
            crc_table[410] = 0x6B;
            crc_table[411] = 0xAB;
            crc_table[412] = 0x69;
            crc_table[413] = 0xA9;
            crc_table[414] = 0xA8;
            crc_table[415] = 0x68;
            crc_table[416] = 0x78;
            crc_table[417] = 0xB8;
            crc_table[418] = 0xB9;
            crc_table[419] = 0x79;
            crc_table[420] = 0xBB;
            crc_table[421] = 0x7B;
            crc_table[422] = 0x7A;
            crc_table[423] = 0xBA;
            crc_table[424] = 0xBE;
            crc_table[425] = 0x7E;
            crc_table[426] = 0x7F;
            crc_table[427] = 0xBF;
            crc_table[428] = 0x7D;
            crc_table[429] = 0xBD;
            crc_table[430] = 0xBC;
            crc_table[431] = 0x7C;
            crc_table[432] = 0xB4;
            crc_table[433] = 0x74;
            crc_table[434] = 0x75;
            crc_table[435] = 0xB5;
            crc_table[436] = 0x77;
            crc_table[437] = 0xB7;
            crc_table[438] = 0xB6;
            crc_table[439] = 0x76;
            crc_table[440] = 0x72;
            crc_table[441] = 0xB2;
            crc_table[442] = 0xB3;
            crc_table[443] = 0x73;
            crc_table[444] = 0xB1;
            crc_table[445] = 0x71;
            crc_table[446] = 0x70;
            crc_table[447] = 0xB0;
            crc_table[448] = 0x50;
            crc_table[449] = 0x90;
            crc_table[450] = 0x91;
            crc_table[451] = 0x51;
            crc_table[452] = 0x93;
            crc_table[453] = 0x53;
            crc_table[454] = 0x52;
            crc_table[455] = 0x92;
            crc_table[456] = 0x96;
            crc_table[457] = 0x56;
            crc_table[458] = 0x57;
            crc_table[459] = 0x97;
            crc_table[460] = 0x55;
            crc_table[461] = 0x95;
            crc_table[462] = 0x94;
            crc_table[463] = 0x54;
            crc_table[464] = 0x9C;
            crc_table[465] = 0x5C;
            crc_table[466] = 0x5D;
            crc_table[467] = 0x9D;
            crc_table[468] = 0x5F;
            crc_table[469] = 0x9F;
            crc_table[470] = 0x9E;
            crc_table[471] = 0x5E;
            crc_table[472] = 0x5A;
            crc_table[473] = 0x9A;
            crc_table[474] = 0x9B;
            crc_table[475] = 0x5B;
            crc_table[476] = 0x99;
            crc_table[477] = 0x59;
            crc_table[478] = 0x58;
            crc_table[479] = 0x98;
            crc_table[480] = 0x88;
            crc_table[481] = 0x48;
            crc_table[482] = 0x49;
            crc_table[483] = 0x89;
            crc_table[484] = 0x4B;
            crc_table[485] = 0x8B;
            crc_table[486] = 0x8A;
            crc_table[487] = 0x4A;
            crc_table[488] = 0x4E;
            crc_table[489] = 0x8E;
            crc_table[490] = 0x8F;
            crc_table[491] = 0x4F;
            crc_table[492] = 0x8D;
            crc_table[493] = 0x4D;
            crc_table[494] = 0x4C;
            crc_table[495] = 0x8C;
            crc_table[496] = 0x44;
            crc_table[497] = 0x84;
            crc_table[498] = 0x85;
            crc_table[499] = 0x45;
            crc_table[500] = 0x87;
            crc_table[501] = 0x47;
            crc_table[502] = 0x46;
            crc_table[503] = 0x86;
            crc_table[504] = 0x82;
            crc_table[505] = 0x42;
            crc_table[506] = 0x43;
            crc_table[507] = 0x83;
            crc_table[508] = 0x41;
            crc_table[509] = 0x81;
            crc_table[510] = 0x80;
            crc_table[511] = 0x40;
        } 
        #endregion

        private byte[] crc16byte(byte[] modbusframe_noCRC)
        {
            int i;
            int index;
            int length = modbusframe_noCRC.Length;
            int crc_Low = 0xFF;
            int crc_High = 0xFF;
            byte[] modbusframe_withCRC = new byte[length + 2];

            for (i = 0; i < length; i++)
            {
                modbusframe_withCRC[i] = modbusframe_noCRC[i];
                index = crc_High ^ (char)modbusframe_noCRC[i];
                crc_High = crc_Low ^ crc_table[index];
                crc_Low = (byte)crc_table[index + 256];
            }

            modbusframe_withCRC[length] = (byte)crc_High;
            modbusframe_withCRC[length + 1] = (byte)crc_Low;

            return modbusframe_withCRC;
        }


        #endregion
    }


}
