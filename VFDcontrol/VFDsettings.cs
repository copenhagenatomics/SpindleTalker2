using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Linq;
using VFDcontrol.Settings4Net;

namespace VFDcontrol
{
    public static class VFDsettings
    {
        private static Setting4Net _settings = new Setting4Net();
        private static string settingsFile;

        static VFDsettings()
        {
            settingsFile = "settings.xml";
            Console.WriteLine($"Motor controller settings file: {settingsFile}");

            if (File.Exists(settingsFile))
                _settings.Open(settingsFile);
        }

        public static string PortName
        {
            get
            {
                var list = OrderedPortNames();
                var port = _settings.GetItemOrDefaultValue("PortName", list.First()).ToString();
                return list.Contains(port) ? port : list.First();
            }
            set { _settings.Settings["PortName"] = value; }
        }

        public static int BaudRate
        {
            get { return Convert.ToInt32(_settings.GetItemOrDefaultValue("BaudRate", 38400)); }
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
            get { return Convert.ToBoolean(_settings.GetItemOrDefaultValue("AutoConnectAtStartup", true)); }
            set { _settings.Settings["AutoConnectAtStartup"] = value.ToString(); }
        }

        public static string LastMDIChild
        {
            get { return _settings.GetItemOrDefaultValue("LastMDIChild", "Graphs").ToString(); }
            set { _settings.Settings["LastMDIChild"] = value; }
        }


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

        public static List<string> OrderedPortNames()
        {
            // Just a placeholder for a successful parsing of a string to an integer
            int num;

            // Order the serial port names in numberic order (if possible)
            return SerialPort.GetPortNames().OrderBy(a => a.Length > 3 && int.TryParse(a.Substring(3), out num) ? num : 0).ToList();
        }
    }
}
