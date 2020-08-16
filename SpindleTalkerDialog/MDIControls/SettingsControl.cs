using System;
using System.IO.Ports;
using System.Linq;
using System.Windows.Forms;
using VfdControl;

namespace SpindleTalker2
{
    public partial class SettingsControl : UserControl
    {
        public char csvSeperator { get; private set; }
        private MainWindow _mainWindow;

        public SettingsControl(MainWindow mainWindow)
        {
            InitializeComponent();
            csvSeperator = ';';
            _mainWindow = mainWindow;
            comboBoxCSV.SelectedText = csvSeperator.ToString();
            mainWindow._hyMotorControl._hyModbus.OnProcessPollPacket += HYmodbus_ProcessPollPacket;
            mainWindow._hyMotorControl._hyModbus.VFDData.OnChanged += VFDData_OnChanged;
        }

        private void SettingsForm_Load(object sender, EventArgs e)
        {
            textBoxQuickset.Text = VFDsettings.QuickSets;
            FreqVoltChart.width = pictureBoxFreqVolt.Width;
            FreqVoltChart.height = pictureBoxFreqVolt.Height;
        }

        private void VFDData_OnChanged(VFDdata data)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => VFDData_OnChanged(data)));
            }
            else
            {
                labelMinMaxFreq.Text = $"Min/Max Frequency = {data.LowerLevelFreq} Hz/{data.MaxFreq} Hz   (Net freq: {data.BaseFreq} Hz)";
                labelMaxRPM.Text = $"Rated motor speed (@50 Hz) = {data.RatedMotorRPM} RPM";
                labelMotorSettings.Text = $"Motor settings: {data.NumberOfMotorPols} poles, Volt: {data.RatedMotorVoltage} VAC, Amps: {data.RatedMotorCurrent} A";
                pictureBoxFreqVolt.Image = FreqVoltChart.Draw(data);
            }
        }

        private void HYmodbus_ProcessPollPacket(VFDdata data)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => VFDData_OnChanged(data)));
            }
            else
            {
                pictureBoxFreqVolt.Image = FreqVoltChart.Draw(data);
            }
        }

        public bool FillComPortList()
        {
            var selected = cmbPortName.SelectedItem?.ToString();
            var list = VFDsettings.OrderedPortNames();
            if (selected == null || !list.Contains(selected))
                selected = VFDsettings.PortName;

            cmbPortName.Items.Clear();
            cmbPortName.Items.AddRange(list.ToArray());
            cmbPortName.SelectedItem = selected;

            return list.Any();
        }

        /// <summary> Populate the form's controls with default settings. </summary>
        public bool InitializeControlValues()
        {
            cmbParity.Items.Clear(); cmbParity.Items.AddRange(Enum.GetNames(typeof(Parity)));
            cmbStopBits.Items.Clear(); cmbStopBits.Items.AddRange(Enum.GetNames(typeof(StopBits)));

            cmbParity.Text = VFDsettings.Parity.ToString();
            cmbStopBits.Text = VFDsettings.StopBits.ToString();
            cmbDataBits.Text = VFDsettings.DataBits.ToString();
            cmbParity.Text = VFDsettings.Parity.ToString();
            cmbBaudRate.Text = VFDsettings.BaudRate.ToString();
            checkBoxAutoConnectAtStartup.Checked = VFDsettings.AutoConnectAtStartup;

            if(!FillComPortList())
            {
                MessageBox.Show(this, "There are no COM Ports detected on this computer.\nPlease install a COM Port and restart this app.", "No COM Ports Installed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return true;
        }

        private void SettingsForm_ResizeEnd(object sender, EventArgs e)
        {
        }

        private void cmbPortName_SelectedIndexChanged(object sender, EventArgs e)
        {
            VFDsettings.PortName = cmbPortName.SelectedItem.ToString();
            _mainWindow.COMPortStatus(_mainWindow._hyMotorControl._hyModbus.VFDData.SerialConnected);
        }

        private void checkBoxAutoConnectAtStartup_CheckedChanged(object sender, EventArgs e)
        {
            VFDsettings.AutoConnectAtStartup = checkBoxAutoConnectAtStartup.Checked;
        }

        private void ButtonSaveQuickSet_Click(object sender, EventArgs e)
        {
            VFDsettings.QuickSets = textBoxQuickset.Text;
            VFDsettings.Save();
            _mainWindow.PopulateQuickSets();
        }

        private void buttonResetVFD_Click(object sender, EventArgs e)
        {
            byte[] factoryReset = new byte[] { (byte)VFDsettings.VFD_ModBusID, (byte)CommandType.FunctionWrite, (byte)CommandLength.TwoBytes, 0x13, 0x08 };
            _mainWindow._hyMotorControl._hyModbus.SendDataAsync(factoryReset);
        }

        private void comboBoxCSV_SelectedIndexChanged(object sender, EventArgs e)
        {
            csvSeperator = comboBoxCSV.SelectedItem.ToString()[0];
        }
    }
}
