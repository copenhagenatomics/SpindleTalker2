using System;
using System.Drawing;
using System.Windows.Forms;
using SpindleTalker2.Properties;
using VFDcontrol;

namespace SpindleTalker2
{
    public partial class MeterControl : UserControl  
    {
        private MainWindow _mainWindow;

        public MeterControl(MainWindow spindleTalkerBase)
        {
            InitializeComponent();
            _mainWindow = spindleTalkerBase;
            HYmodbus.OnProcessPollPacket += Serial_ProcessPollPacket;
            HYmodbus.VFDData.OnMaxFreqChanged += VFDsettings_OnMaxFreqChanged;
            HYmodbus.VFDData.OnRpmChanged += VFDsettings_OnRpmChanged;
            Spindle.OnSpindleShuttingDown += Spindle_OnSpindleShuttingDown;
        }

        private void Serial_ProcessPollPacket(VFDdata data)
        {
            if (this.InvokeRequired)
            {
                try { this.Invoke(new Action(() => Serial_ProcessPollPacket(data))); } catch { }
            }
            else
            {
                MeterSetF.Value = SpindleShuttingDown ? 0 : data.SetFrequency;
                MeterOutF.Value = data.OutFrequency;
                MeterRPM.Value = data.OutRPM;
                Image toolstripImage = (data.OutRPM > 0 ? Resources.greenLED : Resources.redLED);
                string status = (data.OutRPM > 0 ? string.Format("Current RPM = {0:#,##0}", data.OutRPM) : "Spindle is stopped");
                _mainWindow.toolStripStatusRPM.Text = status;
                _mainWindow.toolStripStatusRPM.Image = toolstripImage;
                MeterAmps.Value = data.OutAmp;
                MeterVDC.Value = data.OutVoltDC;
                MeterVAC.Value = HYmodbus.VFDData.OutVoltAC;
            }
        }

        private void VFDsettings_OnMaxFreqChanged(int maxValue)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => VFDsettings_OnMaxFreqChanged(maxValue)));
            }
            else
            {
                MeterOutF.ScaleMaxValue = maxValue;
                MeterSetF.ScaleMaxValue = maxValue;
            }
        }

        private void VFDsettings_OnRpmChanged(int maxValue)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => VFDsettings_OnRpmChanged(maxValue)));
            }
            else
            {
                MeterRPM.ScaleMaxValue = maxValue;
                MeterRPM.ScaleMaxValue = maxValue;  // why do we set this twice?
            }
        }

        private void Spindle_OnSpindleShuttingDown(bool stop)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => Spindle_OnSpindleShuttingDown(stop)));
            }
            else
            {
                SpindleShuttingDown = stop;
            }
        }

        public void ZeroAll()
        {
            MeterSetF.Value = -1;
            MeterOutF.Value = -1;
            MeterRPM.Value = -1;
            MeterVDC.Value = -1;
            MeterVAC.Value = -1;
        }

        public bool SpindleShuttingDown = false;

    }
}
