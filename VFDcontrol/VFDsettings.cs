using System;
using System.IO;
using System.IO.Ports;
using VFDcontrol.Settings4Net;

namespace VFDcontrol
{
    public static class VFDsettings
    {
        private static Setting4Net _settings = new Setting4Net();
        private static string settingsFile;

        public delegate void ValueChanged(int minValue, int maxValue);
        public delegate void MaxValueChanged(int maxValue);
        public delegate void SerialPortConnected(bool connected);
        public static event ValueChanged OnFreqChanged;
        public static event MaxValueChanged OnMaxFreqChanged;
        public static event MaxValueChanged OnRpmChanged;
        public static event SerialPortConnected OnSerialPortConnected;

        static VFDsettings()
        {
            string settingsDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SpindleTalker2");
            if (!Directory.Exists(settingsDir)) Directory.CreateDirectory(settingsDir);

            settingsFile = Path.Combine(settingsDir, "settings.xml");
            Console.WriteLine(settingsFile);

            if (File.Exists(settingsFile))
                _settings.Open(settingsFile);

            _VFD_MaxFreq = -1;
            _VFD_MinFreq = -1;
            _VFD_MaxRPM = -1;
        }

        public static bool SerialConnected
        {
            get { return _serialConnected;}
            set
            {
                _serialConnected = value;
                OnSerialPortConnected?.Invoke(value);
            }
        }
        private static bool _serialConnected;

        public static string PortName
        {
            get { return _settings.GetItemOrDefaultValue("PortName", "COM1").ToString(); }
            set { _settings.Settings["PortName"] = value; }
        }

        public static int BaudRate
        {
            get { return Convert.ToInt32(_settings.GetItemOrDefaultValue("BaudRate", 9600)); }
            set { _settings.Settings["BaudRate"] = value.ToString(); }
        }

        public static int DataBits
        {
            get { return Convert.ToInt32(_settings.GetItemOrDefaultValue("DataBits", 8)); }
            set { _settings.Settings["DataBits"] = value.ToString(); }
        }

        public static Parity Parity
        {
            get { return (Parity)Enum.Parse(typeof(Parity), _settings.GetItemOrDefaultValue("Parity", "None").ToString()); }
            set { _settings.Settings["Parity"] = value.ToString(); }
        }

        public static StopBits StopBits
        {
            get { return (StopBits)Enum.Parse(typeof(StopBits), _settings.GetItemOrDefaultValue("StopBits", 1).ToString()); }
            set { _settings.Settings["StopBits"] = value.ToString(); }
        }

        public static bool AutoConnectAtStartup
        {
            get { return Convert.ToBoolean(_settings.GetItemOrDefaultValue("AutoConnectAtStartup", false)); }
            set { _settings.Settings["AutoConnectAtStartup"] = value.ToString(); }
        }

        public static string LastMDIChild
        {
            get { return _settings.GetItemOrDefaultValue("LastMDIChild", "Graphs").ToString(); }
            set { _settings.Settings["LastMDIChild"] = value; }
        }

        public static int VFD_MinFreq
        {
            get { return _VFD_MinFreq; }
            set
            {
                _VFD_MinFreq = value;
                OnFreqChanged?.Invoke(_VFD_MinFreq, _VFD_MaxFreq);
            }
        }
        private static int _VFD_MinFreq;

        public static int VFD_MaxFreq
        {
            get { return _VFD_MaxFreq; }
            set
            {
                _VFD_MaxFreq = value;
                OnFreqChanged?.Invoke(_VFD_MinFreq, _VFD_MaxFreq);
                OnMaxFreqChanged?.Invoke(_VFD_MaxFreq);
            }
        }
        private static int _VFD_MaxFreq;

        public static int VFD_MaxRPM
        {
            get { return _VFD_MaxRPM; }
            set
            {
                _VFD_MaxRPM = value;
                OnRpmChanged?.Invoke(_VFD_MaxRPM);   
            }
        }
        private static int _VFD_MaxRPM;


        public static int VFD_MinRPM
        {
            get
            {
                if (VFD_MaxFreq > 0 && VFD_MaxRPM > 0)
                {
                    int minRPM = (int)(((double)VFD_MaxRPM / (double)VFD_MaxFreq) * (double)VFD_MinFreq);
                    return minRPM;
                }
                else return 0;
            }
            set { ; }
        }

        public static int VFD_IntermediateFreq
        {
            get { return _VFD_IntermediateFreq; }
            set
            {
                _VFD_IntermediateFreq = value;
            }
        }
        private static int _VFD_IntermediateFreq;

        public static int VFD_MinimumFreq
        {
            get { return _VFD_MinimumFreq; }
            set
            {
                _VFD_MinimumFreq = value;
            }
        }
        private static int _VFD_MinimumFreq;

        public static double VFD_MaxVoltage
        {
            get { return _VFD_MaxVoltage; }
            set
            {
                _VFD_MaxVoltage = value;
            }
        }
        private static double _VFD_MaxVoltage;

        public static double VFD_IntermediateVoltage
        {
            get { return _VFD_IntermediateVoltage; }
            set
            {
                _VFD_IntermediateVoltage = value;
            }
        }
        private static double _VFD_IntermediateVoltage;

        public static double VFD_MinVoltage
        {
            get { return _VFD_MinVoltage; }
            set
            {
                _VFD_MinVoltage = value;
            }
        }
        private static double _VFD_MinVoltage;

        public static double VFD_RatedMotorVoltage
        {
            get { return _VFD_RatedMotorVoltage; }
            set
            {
                _VFD_RatedMotorVoltage = value;
            }
        }
        private static double _VFD_RatedMotorVoltage;

        public static double VFD_RatedMotorCurrent
        {
            get { return _VFD_RatedMotorCurrent; }
            set
            {
                _VFD_RatedMotorCurrent = value;
            }
        }
        private static double _VFD_RatedMotorCurrent;

        public static int VFD_NumberOfMotorPols
        {
            get { return _VFD_NumberOfMotorPols; }
            set
            {
                _VFD_NumberOfMotorPols = value;
            }
        }
        private static int _VFD_NumberOfMotorPols;

        public static int VFD_InverterFrequency
        {
            get { return _VFD_InverterFrequency; }
            set
            {
                _VFD_InverterFrequency = value;
            }
        }
        private static int _VFD_InverterFrequency;

        public static int VFD_ModBusID
        {
            get { return Convert.ToInt32(_settings.GetItemOrDefaultValue("VFD_ModBusID", 1)); }
            set { _settings.Settings["VFD_ModBusID"] = value.ToString(); }
        }

        public static string QuickSets
        {
            get { return _settings.GetItemOrDefaultValue("QuickSets", "50;100;500;1500;3000").ToString(); }
            set { _settings.Settings["QuickSets"] = value; }
        }

        public static void Save()
        {
            _settings.Save(settingsFile);
        }

        public static void ClearVFDSettings()
        {
            VFD_MaxFreq = -1;
            VFD_MinFreq = -1;
            VFD_MaxRPM = -1;
        }
    }
}
