using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Linq;

namespace SpindleTalker2
{
    public static class VFDsettings
    {
        private static Dictionary<string, string> _settings = new Dictionary<string, string>();
        private static string settingsFile;

        static VFDsettings()
        {
            settingsFile = Path.GetFullPath("settings.txt");
            Console.WriteLine($"Motor controller settings file: {settingsFile}");

            if (File.Exists(settingsFile))
            {
                var list = File.ReadAllLines(settingsFile).ToList();
                foreach(var line in list)
                {
                    var row = line.Split(",".ToCharArray());
                    _settings.Add(row[0], row[1]);
                }
            }
        }

        public static string PortName
        {
            get
            {
                var list = OrderedPortNames();
                if (!list.Any()) return "COM1";
                var port = GetItemOrDefaultValue("PortName", list.First()).ToString();
                return list.Contains(port) ? port : list.First();
            }
            set { _settings["PortName"] = value; }
        }

        public static int BaudRate
        {
            get { return Convert.ToInt32(GetItemOrDefaultValue("BaudRate", "38400")); }
            set { _settings["BaudRate"] = value.ToString(); }
        }

        public static int DataBits
        {
            get { return Convert.ToInt32(GetItemOrDefaultValue("DataBits", "8")); }
            set { _settings["DataBits"] = value.ToString(); }
        }

        public static Parity Parity
        {
            get { return (Parity)Enum.Parse(typeof(Parity), GetItemOrDefaultValue("Parity", "None").ToString()); }
            set { _settings["Parity"] = value.ToString(); }
        }

        public static StopBits StopBits
        {
            get { return (StopBits)Enum.Parse(typeof(StopBits), GetItemOrDefaultValue("StopBits", "1").ToString()); }
            set { _settings["StopBits"] = value.ToString(); }
        }

        public static bool AutoConnectAtStartup
        {
            get { return Convert.ToBoolean(GetItemOrDefaultValue("AutoConnectAtStartup", "true")); }
            set { _settings["AutoConnectAtStartup"] = value.ToString(); }
        }

        public static string LastMDIChild
        {
            get { return GetItemOrDefaultValue("LastMDIChild", "Graphs").ToString(); }
            set { _settings["LastMDIChild"] = value; }
        }


        public static int VFD_ModBusID
        {
            get { return Convert.ToInt32(GetItemOrDefaultValue("VFD_ModBusID", "1")); }
            set { _settings["VFD_ModBusID"] = value.ToString(); }
        }

        public static string QuickSets
        {
            get { return GetItemOrDefaultValue("QuickSets", "100;200;500;1000;1500;3000").ToString(); }
            set { _settings["QuickSets"] = value; }
        }

        private static string GetItemOrDefaultValue(string key, string defaultValue)
        {
            if (_settings.ContainsKey(key))
                return _settings[key];

            return defaultValue;
        }

        public static void Save()
        {
            File.WriteAllLines(settingsFile, _settings.Select(x => x.Key + "," + x.Value));
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
