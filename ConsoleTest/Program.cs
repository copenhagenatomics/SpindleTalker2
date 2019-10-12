using CA_DataUploaderLib;
using System;
using System.Linq;
using System.Threading;
using VFDcontrol;

namespace ConsoleTest
{
    public class Program
    {
        static void Main(string[] args)
        {
            try
            {
                InitializeVFD();

                MotorControl.SetRPM(500);

                for (int i = 0; i < 20; i++)
                {
                    Thread.Sleep(1000);
                    Console.WriteLine(HYmodbus.VFDData.ToString());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            StopAndCloseVFD();
            Console.WriteLine("bye...");
        }

        private static void InitializeVFD()
        {
            var serial = new SerialNumberMapper(false);
            if (!serial.McuBoards.Any(x => x.serialNumber == "unknown1"))
            {
                Console.WriteLine("VFD controller not online");
            }
            else
            {
                serial.McuBoards.First(x => x.serialNumber == "unknown1").Close();
                var VFD = serial.McuBoards.First(x => x.serialNumber.StartsWith("unknown"));
                VFDsettings.PortName = VFD.PortName;
                VFDsettings.BaudRate = 38400;
                HYmodbus.OnWriteTerminalForm += Serial_Write;
                HYmodbus.OnWriteLog += OnWriteLog;
                HYmodbus.InitialPoll();

                int i = 0;
                while (!HYmodbus.VFDData.InitDataOK())
                {
                    Thread.Sleep(100);
                    if (i++ > 50)
                    {
                        if (!HYmodbus.ComOpen)
                            throw new Exception($"Unable to open Serial port to VFD {VFDsettings.PortName}, Baud: {VFDsettings.BaudRate}");
                        else
                            throw new Exception($"VFD not initialized: {HYmodbus.VFDData.MotorSettings()}");
                    }
                }

                MotorControl.Start(SpindleDirection.Forward);
            }
        }

        private static void StopAndCloseVFD()
        {
            VFDsettings.Save();
            Console.WriteLine("Waiting for motor to stop... then closing serial port");
            MotorControl.Stop();
            Thread.Sleep(2000);
            HYmodbus.Disconnect();
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
