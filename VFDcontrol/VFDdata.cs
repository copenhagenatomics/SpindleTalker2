using System;
using System.Collections.Generic;

namespace VFDcontrol
{
    public class VFDdata
    {
        public delegate void MaxValueChanged(VFDdata data);
        public delegate void SerialPortConnected(bool connected);
        public event MaxValueChanged OnChanged;
        public event SerialPortConnected OnSerialPortConnected;

        public  bool SerialConnected
        {
            get { return _serialConnected; }
            set
            {
                _serialConnected = value;
                OnSerialPortConnected?.Invoke(value);
            }
        }
        private  bool _serialConnected;


        public  int MinFreq
        {
            get { return _MinFreq; }
            set
            {
                _MinFreq = value;
                OnChanged?.Invoke(this);
            }
        }
        private  int _MinFreq;

        public  int MaxFreq
        {
            get { return _MaxFreq; }
            set
            {
                _MaxFreq = value;
                OnChanged?.Invoke(this);
            }
        }
        private  int _MaxFreq;

        public  int MaxRPM
        {
            get { return _MaxRPM; }
            set
            {
                _MaxRPM = value;
                OnChanged?.Invoke(this);
            }
        }
        private  int _MaxRPM;


        public  int MinRPM
        {
            get
            {
                if (MaxFreq > 0 && MaxRPM > 0)
                {
                    int minRPM = (int)(((double)MaxRPM / (double)MaxFreq) * (double)MinFreq);
                    return minRPM;
                }
                else return 0;
            }
        }

        public  int IntermediateFreq
        {
            get { return _IntermediateFreq; }
            set { _IntermediateFreq = value; }
        }
        private  int _IntermediateFreq;

        public  int MinimumFreq
        {
            get { return _MinimumFreq; }
            set
            {
                _MinimumFreq = value;
            }
        }
        private  int _MinimumFreq;

        public  double MaxVoltage
        {
            get { return _MaxVoltage; }
            set
            {
                _MaxVoltage = value;
                OnChanged?.Invoke(this);
            }
        }
        private  double _MaxVoltage;

        public  double IntermediateVoltage
        {
            get { return _IntermediateVoltage; }
            set
            {
                _IntermediateVoltage = value;
            }
        }
        private  double _IntermediateVoltage;

        public  double MinVoltage
        {
            get { return _MinVoltage; }
            set
            {
                _MinVoltage = value;
            }
        }
        private  double _MinVoltage;

        public  double RatedMotorVoltage
        {
            get { return _RatedMotorVoltage; }
            set
            {
                _RatedMotorVoltage = value;
                OnChanged?.Invoke(this);
            }
        }
        private  double _RatedMotorVoltage;

        public  double RatedMotorCurrent
        {
            get { return _RatedMotorCurrent; }
            set
            {
                _RatedMotorCurrent = value;
                OnChanged?.Invoke(this);
            }
        }
        private  double _RatedMotorCurrent;

        public double RatedMotorRPM
        {
            get { return _RatedMotorRPM; }
            set
            {
                _RatedMotorRPM = value;
                OnChanged?.Invoke(this);
            }
        }
        private double _RatedMotorRPM;

        public int NumberOfMotorPols
        {
            get { return _NumberOfMotorPols; }
            set
            {
                _NumberOfMotorPols = value;
                OnChanged?.Invoke(this);
            }
        }
        private  int _NumberOfMotorPols;

        public  int InverterFrequency
        {
            get { return _InverterFrequency; }
            set
            {
                _InverterFrequency = value;
            }
        }
        private  int _InverterFrequency;

        public  double SetFrequency
        {
            get { return _setFrequency; }
            set { _setFrequency = value; }
        }
        private  double _setFrequency;

        public  double OutFrequency
        {
            get { return _outFrequency; }
            set { _outFrequency = value; }
        }
        private  double _outFrequency;

        public  double OutRPM
        {
            get { return _outRPM; }
            set { _outRPM = value;  _timestamp = DateTime.UtcNow;   }
        }
        private  double _outRPM;

        public DateTime TimeStamp
        {
            get { return _timestamp; }
        }

        private DateTime _timestamp;

        public  double OutAmp
        {
            get { return _outAmp; }
            set { _outAmp = value; }
        }
        private  double _outAmp;

        public  double OutVoltDC
        {
            get { return _outVoltDC; }
            set { _outVoltDC = value; }
        }
        private  double _outVoltDC;

        public  double OutVoltAC
        {
            get { return _outVoltAC; }
            set { _outVoltAC = value; }
        }
        private  double _outVoltAC;

        public  double OutTemperature
        {
            get { return _outTemperature; }
            set { _outTemperature = value; }
        }
        private  double _outTemperature;

        public  void Clear()
        {
            MaxFreq = -1;
            MinFreq = -1;
            MaxRPM = -1;
        }

        public override string ToString()
        {
            return $"Set Freq: {SetFrequency}, Out Freq: {OutFrequency}, RPM: {OutRPM}, Amp: {OutAmp}, VoltDC: {OutVoltDC}, VoltAC: {OutVoltAC}, Temp: {OutTemperature}°C";
        }

        public string MotorSettings()
        {
            return $"MaxFreq {MaxFreq}, MinFreq {MinFreq}, MaxVoltage {MaxVoltage}, MaxRPM {MaxRPM}, RatedMotorVoltag {RatedMotorVoltage}, RatedMotorCurrent {_RatedMotorCurrent}, RatedMotorRPM {RatedMotorRPM}";
        }

        public List<double> GetValues()
        {
            return new List<double> { SetFrequency, OutFrequency, OutAmp, OutRPM, OutVoltDC, OutVoltAC, OutTemperature };
        }

        public bool InitDataOK()
        {
            return MaxFreq > 0 && MinFreq >= 0 && MaxRPM > 0 && OutRPM >= 0;
        }
    }
}
