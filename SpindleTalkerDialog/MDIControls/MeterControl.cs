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
            Serial.OnProcessPollPacket += Serial_ProcessPollPacket;
            VFDsettings.OnMaxFreqChanged += VFDsettings_OnMaxFreqChanged;
            VFDsettings.OnRpmChanged += VFDsettings_OnRpmChanged;
            Spindle.OnSpindleShuttingDown += Spindle_OnSpindleShuttingDown;
        }

        private void Serial_ProcessPollPacket(byte[] pollPacket)
        {
            if (this.InvokeRequired)
            {
                try { this.Invoke(new Action(() => Serial_ProcessPollPacket(pollPacket))); } catch { }
            }
            else
            {
                int value = Convert.ToInt32((pollPacket[4] << 8) + pollPacket[5]);
                switch (pollPacket[3])
                {
                    case (byte)Status.SetF:
                        MeterSetF.Value = (SpindleShuttingDown ? 0 : (double)(value / 100));
                        break;
                    case (byte)Status.OutF:
                        MeterOutF.Value = (double)(value / 100);
                        break;
                    case (byte)Status.RoTT:
                        MeterRPM.Value = (double)(value);
                        Image toolstripImage = (value > 0 ? Resources.greenLED : Resources.redLED);
                        string status = (value > 0 ? string.Format("Current RPM = {0:#,##0}", value) : "Spindle is stopped");
                        _mainWindow.toolStripStatusRPM.Text = status;
                        _mainWindow.toolStripStatusRPM.Image = toolstripImage;
                        break;
                    case (byte)Status.OutA:
                        MeterAmps.Value = (double)((double)value / 10);
                        break;
                    case (byte)Status.DCV:
                        MeterVDC.Value = (double)(value/10.0);
                        Console.WriteLine("DC voltage: " + value/10.0);
                        break;
                    case (byte)Status.ACV:
                        MeterVAC.Value = (double)(value / 10.0);
                        Console.WriteLine("AC voltage: " + value/10.0);
                        break;
                    case (byte)Status.Tmp:
                        Console.WriteLine("VFD temperature: " + value);
                        break;
                }
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
