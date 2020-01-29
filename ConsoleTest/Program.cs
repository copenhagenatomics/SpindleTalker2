using System;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using VfdControl;

namespace ConsoleTest
{
    public class Program
    {
        static void Main(string[] args)
        {
            MotorControl _motorControl = null;
            try
            {
                _motorControl = InitializeVFD();

                if (_motorControl != null)
                {
                    // Get settings that from VFD and save them to a file
                    var settings = _motorControl.HYmodbus.GetRegisterValues();
                    using (StreamWriter writetext = new StreamWriter("settings.csv"))
                    {
                        foreach (var item in settings)
                        {
                            writetext.WriteLine(item.ToString(','));
                        }
                    }

                    _motorControl.SetRPM(500);

                    for (int i = 0; i < 20; i++)
                    {
                        Thread.Sleep(1000);
                        Console.WriteLine(_motorControl.HYmodbus.VFDData.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            if (_motorControl != null)
                StopAndCloseVFD(_motorControl);
            Console.WriteLine("bye...");
        }

        private static MotorControl InitializeVFD()
        {
            var ports = SerialPort.GetPortNames();
            var isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
            if ((!isWindows && !ports.Any(x => x.Contains("cu.usbserial"))) || (isWindows && !ports.Any(x => x.Contains("COM1"))))
            {
                Console.WriteLine("VFD controller not online");
                return null;
            }
            else
            {
                string _port;
                if (isWindows)
                    _port = ports.FirstOrDefault(c => c.Contains("COM1"));
                else
                    _port = ports.FirstOrDefault(c => c.Contains("cu.usbserial"));

                SerialPort sp = new SerialPort(_port, 38400);
                sp.Close();
                var MotorControl = new MotorControl(baudRate: 38400, portName: _port);
                MotorControl.HYmodbus.OnWriteTerminalForm += Serial_Write;
                MotorControl.HYmodbus.OnWriteLog += OnWriteLog;
                MotorControl.HYmodbus.InitialPoll();

                int i = 0;
                while (!MotorControl.HYmodbus.VFDData.InitDataOK())
                {
                    Thread.Sleep(100);
                    if (i++ > 50)
                    {
                        if (!MotorControl.HYmodbus.ComOpen)
                            throw new Exception($"Unable to open Serial port to VFD {MotorControl.HYmodbus.PortName}, Baud: {MotorControl.HYmodbus.BaudRate}");
                        else
                            throw new Exception($"VFD not initialized: {MotorControl.HYmodbus.VFDData.MotorSettings()}");
                    }
                }

                MotorControl.Start(SpindleDirection.Forward);
                return MotorControl;
            }
        }

        private static void StopAndCloseVFD(MotorControl motorControl)
        {
            Console.WriteLine("Waiting for motor to stop... then closing serial port");
            motorControl.Stop();
            Thread.Sleep(2000);
            motorControl.HYmodbus.Disconnect();
        }

        public static void Serial_Write(string message, bool send)
        {
            Console.WriteLine(message);
        }

        private static void OnWriteLog(string message, bool error)
        {
            Console.WriteLine(message);
        }
    }
}

