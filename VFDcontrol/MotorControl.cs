using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Threading;

namespace VfdControl
{
    public class MotorControl
    {
        public delegate void SpindleShuttingDown(bool stop);
        public event SpindleShuttingDown OnSpindleShuttingDown;
        /// <summary>Get array with Parity names.</summary>
        public string[] ParityNames { get { return Enum.GetNames(typeof(Parity)); } private set { } }
        /// <summary>Get array with StopBit names.</summary>
        public string[] StopBitNames { get { return Enum.GetNames(typeof(StopBits)); } private set { } }
        public HYmodbus _hyModbus { get; set; }
        #region Control packet definitions
        private byte[] ReadCurrentSetF { get; set; }
        private byte[] RunForward { get; set; }
        private byte[] RunBack { get; set; }
        private byte[] StopSpindle { get; set; }
        #endregion

        /// <summary>
        /// Setting up motor and HYmodbus class.
        /// </summary>
        /// <param name="portName">Name of serial port to use. Note that this property are stored in HYmodbus property/class.</param>
        /// <param name="baudRate">Boud rate. Default is set to 38400. Note that this property are stored in HYmodbus property/class.</param>
        /// <param name="dataBits">Data bits. Default is set to 8. Note that this property are stored in HYmodbus property/class.</param>
        /// <param name="parity">Parity. Default is set to None. Note that this property are stored in HYmodbus property/class. 0=None, 1=Odd, 2=Even, 3=Mark, 4=Space</param>
        /// <param name="stopBits">Stop bits. Default is set to One. Note that this property are stored in HYmodbus property/class. 0=None, 1=One, 2=Two, 3=OnePointFive</param>
        /// <param name="modBusID">Modbus id. Default is set to 1. Note that this property are stored in HYmodbus property/class.</param>
        /// <param name="responseWaitTimeout">In milliseconds. Default is 100. Note that this property are stored in HYmodbus property/class.</param>
        public MotorControl(string portName, int baudRate = 38400, int dataBits = 8, int parity = 0, int stopBits = 1, int modBusID = 1, int responseWaitTimeout = 100)
        {
            if (parity < 0 || parity > 4)
            {
                throw new Exception("Parity must be 0, 1, 2, 3 or 4");
            }
            if (stopBits < 0 || parity > 3)
            {
                throw new Exception("StopBits must be 0, 1, 2 or 3");
            }
            this.ReadCurrentSetF = new byte[] { (byte)modBusID, (byte)CommandType.ReadControlData, (byte)CommandLength.OneByte, (byte)ControlDataType.SetF };
            this.RunForward = new byte[] { (byte)modBusID, (byte)CommandType.WriteControlData, (byte)CommandLength.OneByte, (byte)ControlCommands.Run_Fwd };
            this.RunBack = new byte[] { (byte)modBusID, (byte)CommandType.WriteControlData, (byte)CommandLength.OneByte, (byte)ControlCommands.Run_Rev };
            this.StopSpindle = new byte[] { (byte)modBusID, (byte)CommandType.WriteControlData, (byte)CommandLength.OneByte, (byte)ControlCommands.Stop };
            _hyModbus = new HYmodbus(portName, baudRate, dataBits, parity, stopBits, modBusID, responseWaitTimeout);
        }
        /// <summary>
        /// Start the motor.
        /// </summary>
        /// <param name="direction">Direction to run.</param>
        public void Start(SpindleDirection direction)
        {
            //    ModBus packet format [xx] = one byte i.e. 0x1E
            //
            //      [xx]   |     [xx]     |      [xx]      | [xx] [xx] [..] | [xx][xx]
            //    Slave ID | Command Type | Request Length |     Request    |   CRC   
            //
            //    The casting of Enum's below is way overkill but it's to help anyone trying
            //    to get their head around the format of the 'ModBus' protocol used by these
            //    spindles. The CRC is added as part of the SendData() method. 
            //

            _hyModbus.SendDataAsync(ReadCurrentSetF); // I'm not sure why this is needed but it seems to be
            SetFrequency(_hyModbus.VFDData.LowerLevelFreq); 

            // For future testing, the spindle reverse function doesn't appear to be working
            _hyModbus.SendDataAsync(direction == SpindleDirection.Forward ? RunForward : RunBack);

            _hyModbus.StartPolling();
        }
        /// <summary>
        /// Stop the motor.
        /// </summary>
        public void Stop()
        {
            _hyModbus.SendDataAsync(StopSpindle);
            OnSpindleShuttingDown?.Invoke(true);
            Thread.Yield();
            SetRPM(0);
        }
        /// <summary>
        /// Set motor speed in RPM.
        /// This function assumes a linear correlation between frequency and spindle speed. This isn't correct but
        /// is a close enough approximation for my purposes. This is a possible area for future development.
        /// 
        /// Calculate the frequency that equates to the target RPM by working out the target RPM as
        /// a fraction of the max RPM and then multiplying that by the max Frequency.
        /// </summary>
        /// <param name="targetRPM"></param>
        public void SetRPM(int targetRPM)
        {
            double targetFrequency = (double)targetRPM / _hyModbus.VFDData.MaxRPM * _hyModbus.VFDData.MaxFreq;
            SetFrequency(targetFrequency);
        }
        /// <summary>
        /// Set motor speed as frequency.
        /// </summary>
        /// <param name="targetFrequency">Target frequency.</param>
        private void SetFrequency(double targetFrequency)
        {
            //   Check that the target frequency does not exceed the maximum or minumum values for the VFD and/or
            //   spindle. I assume that the VFD will ignore values above max (haven't tested) but values below the
            //   minumum recommended frequency for air-cooled spindles can cause major overheating issues.
            if (targetFrequency < _hyModbus.VFDData.LowerLevelFreq) targetFrequency = _hyModbus.VFDData.LowerLevelFreq;
            else if (targetFrequency > _hyModbus.VFDData.MaxFreq) targetFrequency = _hyModbus.VFDData.MaxFreq;

            int frequency = (int)targetFrequency * 100; // VFD expects target frequency in hundredths of Hertz

            // Construct the control packet
            byte[] controlPacket = new byte[5];
            controlPacket[0] = (byte)_hyModbus.ModBusID;
            controlPacket[1] = (byte)CommandType.WriteInverterFrequencyData;
            controlPacket[2] = (byte)CommandLength.TwoBytes;
            controlPacket[3] = (byte)(frequency >> 8); // Bitshift right to get bits nine to 16 of the int32 value
            controlPacket[4] = (byte)frequency; // returns the eight Least Significant Bits (LSB) of the int32 value

            _hyModbus.SendDataAsync(controlPacket);
        }

        public bool Upload(string fileName, char csvSeperator)
        {
            var lines = RegisterValue.LoadCsv(fileName, csvSeperator);
            if (lines != null)
            {
                Console.WriteLine("================== Startin upload ======================");
                foreach (var line in lines.Where(x => x.DefaultValue != "Unknown"))
                {
                    try
                    {
                        Console.WriteLine(line.ToString());
                        var result = _hyModbus.SendCommand((byte)CommandType.FunctionWrite, (byte)line.CommandLength, line.ID, line.data0, line.data1);
                        // Console.WriteLine(result.ToString());
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                    }
                }

                return true;
            }

            return false;
        }

        public void Download(string fileName, char seperator)
        {
            var lines = new List<string>();
            lines.Add(RegisterValue.Header(seperator));
            for (int i = 0; i < 200; i++)
            {
                try
                {
                    var result = _hyModbus.SendCommand((byte)CommandType.FunctionRead, 1, (byte)i, 0, 0);
                    if (result != null)
                    {
                        result.Value = result.ToValue();
                        lines.Add(result.ToString(seperator));
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }

            File.WriteAllLines(fileName, lines);
        }


        /// <summary>
        /// Return names for com ports
        /// </summary>
        /// <returns>Array of ports on the computer.</returns>
        public string[] GetLocalComPorts()
        {
            return SerialPort.GetPortNames();
        }
    }
}
