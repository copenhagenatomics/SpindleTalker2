using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace VFDcontrol
{
    public class RegisterValue
    {
        private const string QT = "\"";
        public int ID { get; private set; }
        public string Value { get; set; }

        public static string Header(char seperator) { return $"{QT}ID{QT}{seperator}{QT}Type{QT}{seperator}{QT}Description{QT}{seperator}{QT}Value{QT}{seperator}{QT}Unit{QT}{seperator}{QT}DefaultValue{QT}";  }
        public static int ColumnCount { get { return 6; } }
        public RegisterValue(string id)
        {
            ID = int.Parse(id);
        }

        public RegisterValue(byte id)
        {
            ID = id;
        }

        public override string ToString()
        {
            return ToString(',');
        }

        public string ToString(char seperator)
        {
            return $"{ID}{seperator}{Type}{seperator}{QT}{Description}{QT}{seperator}{Value}{seperator}{Unit}{seperator}{DefaultValue}";
        }


        public byte data0 { get { return ToByte(0); } }
        public byte data1 { get { return ToByte(8); } }
        public byte data2 { get { return ToByte(16); } }

        private byte ToByte(int shift)
        {
            switch (Type)
            {
                case "byte": return byte.Parse(Value);
                case "int": return (byte)(int.Parse(Value) >> shift);
                case "intX10": return (byte)(((byte)(int.Parse(Value) / 10.0)) >> shift);
                case "intX100": return (byte)(((byte)(int.Parse(Value) / 100.0)) >> shift);
                case "intX1000": return (byte)(((byte)(int.Parse(Value) / 1000.0)) >> shift);
                case "char": return (byte)Value[0];
                default: return 0;
            }
        }

    #region Paremeters From Booklet

        public string Type
        {
            get
            {
                switch (ID)
                {
                    case 3: return "intX100";
                    case 4: return "intX10";
                    case 5: return "intX100";
                    case 6: return "intX100";
                    case 7: return "intX100";
                    case 8: return "intX10";
                    case 9: return "intX10";
                    case 10: return "intX10";
                    case 11: return "intX100";
                    case 27: return "intX10";
                    case 28: return "intX10";
                    case 29: return "intX10";
                    case 30: return "intX10";
                    case 31: return "intX10";
                    case 32: return "intX10";
                    case 34: return "intX10";
                    default: return "byte";
                }
            }
        }

        public string Unit
        {
            get
            {
                switch (ID)
                {
                    case 0: return "enum";
                    case 1: return "enum";
                    case 2: return "enum";
                    case 3: return "Hz";
                    case 4: return "Hz";
                    case 5: return "Hz";
                    case 6: return "Hz";
                    case 7: return "Hz";
                    case 8: return "Volt";
                    case 9: return "Volt";
                    case 10: return "Volt";
                    case 11: return "Hz";
                    case 12: return "Reserved";
                    case 13: return "enum";
                    case 22: return "Reserved";
                    case 23: return "enum";
                    case 24: return "enum";
                    case 25: return "enum";
                    case 26: return "enum";
                    case 27: return "Hz";
                    case 28: return "Hz";
                    case 29: return "Sec";
                    case 30: return "Sec";
                    case 31: return "%";
                    case 32: return "Sec";
                    case 33: return "%";
                    case 34: return "?";
                    case 35: return "Reserved";
                    case 41: return "enum (kHz)";
                    case 42: return "Hz";
                    case 43: return "sec";
                    case 44: return "enum";
                    case 45: return "enum";
                    case 46: return "enum";
                    case 47: return "enum";
                    case 48: return "enum";
                    case 49: return "enum";
                    case 50: return "enum";
                    case 51: return "enum";
                    case 52: return "enum";
                    case 54: return "enum";
                    case 55: return "%";
                    case 56: return "Hz";
                    case 57: return "Hz";
                    case 58: return "Hz";
                    case 59: return "Hz";
                    case 60: return "Hz";
                    case 61: return "Hz";
                    case 62: return "Hz";
                    case 63: return "sec";
                    case 64: return "sec";
                    case 65: return "count";
                    case 66: return "count";
                    case 67: return "Not used";
                    case 70: return "enum";
                    case 71: return "?";
                    case 72: return "Hz";
                    case 73: return "Hz";
                    case 74: return "Hz";
                    case 75: return "Hz";
                    case 76: return "bool";
                    case 77: return "bool";
                    case 78: return "bool";
                    case 79: return "Not used";
                    case 80: return "enum";
                    case 81: return "enum";
                    case 82: return "enum";
                    case 83: return "enum";
                    case 84: return "sec";
                    case 85: return "sec";
                    case 86: return "Hz";
                    case 87: return "Hz";
                    case 88: return "Hz";
                    case 89: return "Hz";
                    case 90: return "Hz";
                    case 91: return "Hz";
                    case 92: return "Hz";
                    case 93: return "Hz";
                    case 94: return "Hz";
                    case 95: return "Hz";
                    case 96: return "Hz";
                    case 97: return "Hz";
                    case 98: return "Hz";
                    case 99: return "Hz";
                    case 100: return "Hz";
                    case 101: return "sec";
                    case 102: return "sec";
                    case 103: return "sec";
                    case 104: return "sec";
                    case 105: return "sec";
                    case 106: return "sec";
                    case 107: return "sec";
                    case 108: return "sec";
                    case 117: return "bool";
                    case 118: return "bool";
                    case 119: return "%";
                    case 120: return "%";
                    case 121: return "sec";
                    case 122: return "%";
                    case 123: return "enum";
                    case 124: return "%";
                    case 125: return "sec";
                    case 126: return "bool";
                    case 127: return "Not used";
                    case 128: return "Not used";
                    case 129: return "Not used";
                    case 130: return "enum";
                    case 131: return "sec";
                    case 132: return "sec";
                    case 133: return "sec";
                    case 134: return "sec";
                    case 135: return "%";
                    case 136: return "sec";
                    case 137: return "%";
                    case 138: return "Hz";
                    case 139: return "sec";
                    case 140: return "Not used";
                    case 141: return "Volt";
                    case 142: return "Amp";
                    case 143: return "count";
                    case 144: return "RPM";
                    case 145: return "%";
                    case 146: return "Amp";
                    case 147: return "?";
                    case 148: return "Not used";
                    case 149: return "Not used";
                    case 150: return "bool";
                    case 151: return "%";
                    case 152: return "count";
                    case 153: return "bool";
                    case 154: return "sec";
                    case 155: return "count";
                    case 156: return "%";
                    case 157: return "sec";
                    case 158: return "sec";
                    case 159: return "%";
                    case 160: return "bool";
                    case 161: return "%";
                    case 162: return "%";
                    case 163: return "address";
                    case 164: return "bit/sec";
                    case 173: return "Volt";
                    case 174: return "Amp";
                    case 175: return "enum";
                    case 176: return "enum";
                    default: return "Unknown";
                }
            }
        }

        public string DefaultValue
        {
            get
            {
                switch(ID)
                {
                    case 0: return "0";
                    case 1: return "0";
                    case 2: return "0";
                    case 3: return "0";
                    case 4: return "50";
                    case 5: return "50";
                    case 6: return "2.5";
                    case 7: return "0.5";
                    case 8: return "220";
                    case 9: return "0.15";
                    case 10: return "0";
                    case 11: return "0";
                    case 12: return "0";
                    case 13: return "0";
                    case 22: return "0";
                    case 23: return "1";
                    case 24: return "1";
                    case 25: return "0";
                    case 26: return "0";
                    case 27: return "0.5";
                    case 28: return "0.5";
                    case 29: return "0";
                    case 30: return "0";
                    case 31: return "2";
                    case 32: return "5";
                    case 33: return "150";
                    case 34: return "0.5";
                    case 35: return "0";
                    case 41: return "5 (4 kHz)";
                    case 42: return "5";
                    case 43: return "1";
                    case 44: return "2";
                    case 45: return "3";
                    case 46: return "14";
                    case 47: return "22";
                    case 48: return "24";
                    case 49: return "23";
                    case 50: return "1";
                    case 51: return "5";
                    case 52: return "3";
                    case 54: return "0";
                    case 55: return "100";
                    case 56: return "0";
                    case 57: return "0";
                    case 58: return "0";
                    case 59: return "0.5";
                    case 60: return "0";
                    case 61: return "0";
                    case 62: return "0.5";
                    case 63: return "0.1";
                    case 64: return "1";
                    case 65: return "0";
                    case 66: return "0";
                    case 67: return "Not used";
                    case 70: return "0";
                    case 71: return "20";
                    case 72: return "50";
                    case 73: return "0";
                    case 74: return "0";
                    case 75: return "0";
                    case 76: return "0";
                    case 77: return "0";
                    case 78: return "0";
                    case 79: return "Not used";
                    case 80: return "0";
                    case 81: return "0";
                    case 82: return "0";
                    case 83: return "0";
                    case 84: return "0";
                    case 85: return "0";
                    case 86: return "15";
                    case 87: return "20";
                    case 88: return "25";
                    case 89: return "30";
                    case 90: return "35";
                    case 91: return "40";
                    case 92: return "0.5";
                    case 93: return "10";
                    case 94: return "15";
                    case 95: return "20";
                    case 96: return "25";
                    case 97: return "30";
                    case 98: return "35";
                    case 99: return "40";
                    case 100: return "45";
                    case 101: return "10";
                    case 102: return "10";
                    case 103: return "0";
                    case 104: return "0";
                    case 105: return "0";
                    case 106: return "0";
                    case 107: return "0";
                    case 108: return "0";
                    case 117: return "0";
                    case 118: return "1";
                    case 119: return "150";
                    case 120: return "0";
                    case 121: return "5";
                    case 122: return "150";
                    case 123: return "0";
                    case 124: return "0";
                    case 125: return "1";
                    case 126: return "0";
                    case 127: return "0";
                    case 128: return "0";
                    case 129: return "0";
                    case 130: return "0";
                    case 131: return "60";
                    case 132: return "5";
                    case 133: return "60";
                    case 134: return "60";
                    case 135: return "95";
                    case 136: return "30";
                    case 137: return "80";
                    case 138: return "20";
                    case 139: return "20";
                    case 140: return "0";
                    case 141: return "?";
                    case 142: return "?";
                    case 143: return "4";
                    case 144: return "1440";
                    case 145: return "2";
                    case 146: return "40";
                    case 147: return "0";
                    case 148: return "0";
                    case 149: return "0";
                    case 150: return "1";
                    case 151: return "0";
                    case 152: return "1";
                    case 153: return "0";
                    case 154: return "0.5";
                    case 155: return "0";
                    case 156: return "100";
                    case 157: return "5";
                    case 158: return "0";
                    case 159: return "?";
                    case 160: return "0";
                    case 161: return "100";
                    case 162: return "0";
                    case 163: return "0";
                    case 164: return "1";
                    case 165: return "0";
                    case 166: return "0";
                    case 167: return "0";
                    case 168: return "0";
                    case 170: return "0";
                    case 171: return "0";
                    case 172: return "0";
                    case 173: return "0";
                    case 174: return "?";
                    case 175: return "0";
                    case 176: return "0";
                    case 177: return "0";
                    case 178: return "0";
                    case 179: return "0";
                    case 180: return "0";
                    default: return "Unknown";
                }
            }
        }

        public string Description
        {
            get
            {
                switch (ID)
                {
                    case 0: return "Parameter read only mode";
                    case 1: return "Operation mode (manual, input pin, RS485)";
                    case 2: return "Frequency from (manual, input pin, RS485)";
                    case 3: return "Main Frequency";
                    case 4: return "Base Frequency";
                    case 5: return "Motor Max operating frequency";
                    case 6: return "Frequency step size";
                    case 7: return "Motor minimum frequency while rotating, (see PD011)";
                    case 8: return "Motor Max Voltage";
                    case 9: return "Motor voltage step size";
                    case 10: return "Motor Min Voltage";
                    case 11: return "Frequency lower limit, (see PD007)";
                    case 12: return "Not used";
                    case 13: return "Reset VFD to factory settings";
                    case 22: return "Not used";
                    case 23: return "Reverse rotation select";
                    case 24: return "Enable stop key";
                    case 25: return "Startup mode (Freq ramp up curve)";
                    case 26: return "Break mode (freq ramp down curve)";
                    case 27: return "Starting frequency (see PD007, PD011)";
                    case 28: return "Stopping frequency";
                    case 29: return "DC breaking time before startup";
                    case 30: return "DC breaking time after stop";
                    case 31: return "DC breaking voltage level";
                    case 32: return "Time to measure frequency of motor";
                    case 33: return "Current level when measuring frequency of motor";
                    case 34: return "Inverter track time of up or down";
                    case 35: return "Not used";
                    case 41: return "PWM carrier frequency (see table)";
                    case 42: return "Jogging Frequency";
                    case 43: return "S-Curves time";
                    case 44: return "Function of input Pin D1 (see table)";
                    case 45: return "Function of input Pin D2 (see table)";
                    case 46: return "Function of input Pin D3 (see table)";
                    case 47: return "Function of input Pin D4 (see table)";
                    case 48: return "Function of input Pin D5 (see table)";
                    case 49: return "Function of input Pin D6 (see table)";
                    case 50: return "Function of output Pin Y1 (see table)";
                    case 51: return "Function of output Pin Y2 (see table)";
                    case 52: return "Function of output Pin Y3 (see table)";
                    case 54: return "Function of analog output pin (see table)";
                    case 55: return "Function of analog input pin (see table)";
                    case 56: return "Skip Frequency 1";
                    case 57: return "Skip Frequency 2";
                    case 58: return "Skip Frequency 3";
                    case 59: return "Skip Frequency Range";
                    case 60: return "Uniform Frequency 1";
                    case 61: return "Uniform Frequency 2";
                    case 62: return "Uniform Frequency Range";
                    case 63: return "Timer 1 time";
                    case 64: return "Timer 2 time";
                    case 65: return "counter value";
                    case 66: return "intermediate counter";
                    case 67: return "Not used";
                    case 70: return "Analog input pin";
                    case 71: return "Analog filtering constant";
                    case 72: return "Higher analog frequency";
                    case 73: return "Lower analog frequency";
                    case 74: return "Bias direction at higher frequency";
                    case 75: return "Bias direction at lower frequency";
                    case 76: return "Analog negative bias reverse";
                    case 77: return "Up/Down function";
                    case 78: return "Up/Down speed";
                    case 79: return "Not used";
                    case 80: return "PLC Operation";
                    case 81: return "Auto PLC";
                    case 82: return "PLC running direction 1 - 8";
                    case 83: return "PLC running direction 9 - 16";
                    case 84: return "PLC ramp time 1 - 8";
                    case 85: return "PLC ramp time 9 - 16";
                    case 86: return "PLC frequency 2";
                    case 87: return "PLC frequency 3";
                    case 88: return "PLC frequency 4";
                    case 89: return "PLC frequency 5";
                    case 90: return "PLC frequency 6";
                    case 91: return "PLC frequency 7";
                    case 92: return "PLC frequency 8";
                    case 93: return "PLC frequency 9";
                    case 94: return "PLC frequency 10";
                    case 95: return "PLC frequency 11";
                    case 96: return "PLC frequency 12";
                    case 97: return "PLC frequency 13";
                    case 98: return "PLC frequency 14";
                    case 99: return "PLC frequency 15";
                    case 100: return "PLC frequency 16";
                    case 101: return "PLC timer 1 and 9";
                    case 102: return "PLC timer 2 and 10";
                    case 103: return "PLC timer 3 and 11";
                    case 104: return "PLC timer 4 and 12";
                    case 105: return "PLC timer 5 and 13";
                    case 106: return "PLC timer 6 and 14";
                    case 107: return "PLC timer 7 and 15";
                    case 108: return "PLC timer 8 and 16";
                    case 117: return "Multi speed memory function";
                    case 118: return "Over voltage stall prevention";
                    case 119: return "Stall prevention level at ramp up";
                    case 120: return "Stall prevention level at constant speed";
                    case 121: return "Time for stall prevention at constant speed";
                    case 122: return "Stall prevention level at ramp down";
                    case 123: return "Over torque detect mode";
                    case 124: return "Over torque detect level";
                    case 125: return "Over torque detect time";
                    case 126: return "counter memory";
                    case 127: return "Not used";
                    case 128: return "Not used";
                    case 129: return "Not used";
                    case 130: return "Number of pump";
                    case 131: return "Continous running time of pump";
                    case 132: return "Interlocking time of pump";
                    case 133: return "High speed running time";
                    case 134: return "Low speed running time";
                    case 135: return "Stopping voltage level";
                    case 136: return "Lasting time at stopping voltage level";
                    case 137: return "Wakeup voltage level";
                    case 138: return "Sleep Frequency";
                    case 139: return "Lasting time of sleeping frequency";
                    case 140: return "Not used";
                    case 141: return "Rated Motor Voltage";
                    case 142: return "Raded Motor Current";
                    case 143: return "Number of Motor Poles";
                    case 144: return "Rated Motor speed (RPM)";
                    case 145: return "Auto torque compensation";
                    case 146: return "Motor no load current";
                    case 147: return "Motor slip compensation";
                    case 148: return "Not used";
                    case 149: return "Not used";
                    case 150: return "Auto voltage regulation";
                    case 151: return "Auto emergency saving";
                    case 152: return "Fault reset times";
                    case 153: return "Restart after instantaneous stop";
                    case 154: return "Allowable power breakdown time";
                    case 155: return "Number of abnormal restart";
                    case 156: return "Proportional constant (P)";
                    case 157: return "Integral time (I)";
                    case 158: return "Differential time (D)";
                    case 159: return "Target value";
                    case 160: return "PID target value";
                    case 161: return "PID upper limit";
                    case 162: return "PID lower limit";
                    case 163: return "RS485 address";
                    case 164: return "RS485 baudrate";
                    case 165: return "RS485 flow control";
                    case 166: return "not used";
                    case 167: return "not used";
                    case 168: return "not used";
                    case 170: return "Display name";
                    case 171: return "Display item open";
                    case 172: return "Fault clear";
                    case 173: return "Voltage rating of VFD";
                    case 174: return "current rating of VFD";
                    case 175: return "VFD model";
                    case 176: return "VFD frequency standard";
                    case 177: return "Fault record 1";
                    case 178: return "Fault record 2";
                    case 179: return "Fault record 3";
                    case 180: return "Fault record 4";
                    case 181: return "Software version";
                    case 182: return "Manufacture data";
                    case 183: return "Serial number";
                    default: return (ID >= 184 && ID <= 250) ? "Factory Setting" : "NA";
                }
            }
        }

        public int CommandLength
        {
            get
            {
                switch (ID)
                {
                    case 0: 
                    case 1: 
                    case 2: return 1;
                    case 3:
                    case 4:
                    case 5:
                    case 6:
                    case 7:
                    case 8:
                    case 9:
                    case 10:
                    case 11: return 3;
                    case 12:
                    case 13:
                    case 22:
                    case 23:
                    case 24:
                    case 25:
                    case 26: return 1;
                    case 27:
                    case 28: return 3;
                    case 29:
                    case 30:
                    case 31:
                    case 32:
                    case 33:
                    case 34:
                    case 35:
                    case 41: return 1;
                    case 42: return 2;
                    case 43: return 2;
                    case 44:
                    case 45:
                    case 46:
                    case 47:
                    case 48:
                    case 49:
                    case 50:
                    case 51:
                    case 52:
                    case 53:
                    case 54: return 1;
                    case 55:
                    case 56:
                    case 57:
                    case 58:
                    case 59:
                    case 60:
                    case 61:
                    case 62: return 3;
                    case 63: return 1;
                    case 64: return 1;
                    case 65: return 2;
                    case 66: return 2;
                    case 67:
                    case 68:
                    case 69:
                    case 70:
                    case 71: return 1;
                    case 72:
                    case 73: return 3;
                    case 74:
                    case 75:
                    case 76:
                    case 77:
                    case 78:
                    case 79:
                    case 80:
                    case 81:
                    case 82:
                    case 83: return 1;
                    case 84:
                    case 85: return 2;
                    case 86:
                    case 87:
                    case 88:
                    case 89:
                    case 90:
                    case 91:
                    case 92:
                    case 93:
                    case 94:
                    case 95:
                    case 96:
                    case 97:
                    case 98:
                    case 99:
                    case 100: return 3;
                    case 101:
                    case 102:
                    case 103:
                    case 104:
                    case 105:
                    case 106:
                    case 107:
                    case 108:return 2;
                    default: return 1;
                }

                throw new NotImplementedException();
            }
        }

        public string ValueRange
        {
            get
            {
                switch (ID)
                {
                    case 0: return "0,1";
                    case 1: return "0,1,2";
                    case 2:return "0,1,2";
                    case 3: return "0-400 Hz";
                    case 4: return "0-400 Hz";
                    case 5: return "0-400 Hz";
                    case 6: return "10-400 Hz";
                    case 7: return "0.12-100 Hz";
                    case 8: return "0.1-380 V";
                    case 9: return "0.15-10 V";
                    case 10: return "0.1-50 V";
                    case 11: return "0-400 V";
                    case 12: return "0";
                    case 13: return "8 = reset";
                    case 22: return "0";
                    case 23: return "0,1";
                    case 24: return "0,1";
                    case 25: return "0,1";
                    case 26: return "0,1";
                    case 27: return "0.1-10 Hz";
                    case 28: return "0.1-10 Hz";
                    case 29: return "0.02-5 sec";
                    case 30: return "0-25 sec";
                    case 31: return "0-20 %";
                    case 32: return "0.12-10 sec";
                    case 33: return "0-200 %";
                    case 34: return "?";
                    case 35: return "0";
                    case 41: return "0-15 (0.1 - 20kHz)";
                    case 42: return "0-400";
                    case 43: return "0-6500";
                    case 44: return "0-32";
                    case 45: return "0-32";
                    case 46: return "0-32";
                    case 47: return "0-32";
                    case 48: return "0-32";
                    case 49: return "0-32";
                    case 50: return "0-32";
                    case 51: return "0-32";
                    case 52: return "0-32";
                    case 54: return "0-7";
                    case 55: return "0-100";
                    case 56: return "0-400";
                    case 57: return "0-400";
                    case 58: return "0-400";
                    case 59: return "0.1-10";
                    case 60: return "0-400";
                    case 61: return "0-400";
                    case 62: return "0.1-10";
                    case 63: return "0.1-10";
                    case 64: return "1-100";
                    case 65: return "0-65500";
                    case 66: return "0-65500";
                    case 67: return "Not used";
                    case 70: return "0-4";
                    case 71: return "0-50";
                    case 72: return "0-400";
                    case 73: return "0-400";
                    case 74: return "0,1";
                    case 75: return "0,1";
                    case 76: return "0,1";
                    case 77: return "0,1";
                    case 78: return "0,1";
                    case 79: return "Not used";
                    case 80: return "0-5";
                    case 81: return "0-3";
                    case 82: return "0-255";
                    case 83: return "0-255";
                    case 84: return "0-65535";
                    case 85: return "0-65535";
                    case 86: return "0-400";
                    case 87: return "0-400";
                    case 88: return "0-400";
                    case 89: return "0-400";
                    case 90: return "0-400";
                    case 91: return "0-400";
                    case 92: return "0-400";
                    case 93: return "0-400";
                    case 94: return "0-400";
                    case 95: return "0-400";
                    case 96: return "0-400";
                    case 97: return "0-400";
                    case 98: return "0-400";
                    case 99: return "0-400";
                    case 100: return "0-400";
                    case 101: return "0-6500";
                    case 102: return "0-6500";
                    case 103: return "0-6500";
                    case 104: return "0-6500";
                    case 105: return "0-6500";
                    case 106: return "0-6500";
                    case 107: return "0-6500";
                    case 108: return "0-6500";
                    case 117: return "0,1";
                    case 118: return "0,1";
                    case 119: return "0-200";
                    case 120: return "0-200";
                    case 121: return "?";
                    case 122: return "0-200";
                    case 123: return "0-3";
                    case 124: return "0-200";
                    case 125: return "0.1-20";
                    case 126: return "0,1";
                    case 127: return "Not used";
                    case 128: return "Not used";
                    case 129: return "Not used";
                    case 130: return "0,1,2";
                    case 131: return "0-65535";
                    case 132: return "0-250";
                    case 133: return "0-250";
                    case 134: return "0-250";
                    case 135: return "1-150";
                    case 136: return "0-250";
                    case 137: return "1-150";
                    case 138: return "0-400";
                    case 139: return "0-250";
                    case 140: return "Not used";
                    case 141: return "0-400";
                    case 142: return "0-32";
                    case 143: return "2-10";
                    case 144: return "0-9999";
                    case 145: return "0.1-10";
                    case 146: return "0-99";
                    case 147: return "0-10";
                    case 148: return "Not used";
                    case 149: return "Not used";
                    case 150: return "0,1";
                    case 151: return "0-10";
                    case 152: return "0-255";
                    case 153: return "0,1";
                    case 154: return "0.1-5";
                    case 155: return "0-10";
                    case 156: return "0-1000";
                    case 157: return "0.1-3600";
                    case 158: return "0.01-10";
                    case 159: return "0-100";
                    case 160: return "0,1";
                    case 161: return "0-100";
                    case 162: return "0-100";
                    case 163: return "0-250";
                    case 164: return "0-3";
                    case 165: return "0-5";
                    case 166: return "Not used";
                    case 167: return "Not used";
                    case 168: return "Not used";
                    case 170: return "0-5";
                    case 171: return "0-15";
                    case 172: return "0-10";
                    case 173: return "0-400";
                    case 174: return "0-32";
                    case 175: return "0,1";
                    case 176: return "0,1";
                    default: return "NA";
                }

                throw new NotImplementedException();
            }
        }

        #endregion

        public static List<RegisterValue> LoadCsv(string filename, char seperator = ',')
        {
            var lines = File.ReadAllLines(filename).Select(x => x.Replace(seperator, ',')).ToList();
            if (lines[0] == RegisterValue.Header(seperator))
            {
                var result = new List<RegisterValue>();
                foreach (var line in lines.Skip(1))
                {
                    var row = line.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).ToList();
                    if (row.Count() != RegisterValue.ColumnCount)
                    {
                        MessageBox.Show("Invalid column count in row:" + line);
                        return null;
                    }

                    var item = new RegisterValue(row[0]);
                    item.Value = row[3];
                    if (item.Type != row[1]) MessageBox.Show($"ID: {item.ID} has wrong type: {item.Type} <> {row[1]}");
                    if (item.Unit != row[4]) MessageBox.Show($"ID: {item.ID} has wrong type: {item.Unit} <> {row[4]}");
                    result.Add(item);
                }

                return result;
            }

            MessageBox.Show("Invalid column headers");
            return null;
        }
    }
}
