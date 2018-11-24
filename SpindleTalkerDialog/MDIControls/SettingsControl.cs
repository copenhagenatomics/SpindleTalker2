using System;
using System.IO.Ports;
using System.Linq;
using System.Windows.Forms;
using VFDcontrol;

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
            HYmodbus.VFDData.OnFreqChanged += VFDsettings_OnFreqChanged;
            HYmodbus.VFDData.OnRpmChanged += VFDsettings_OnRpmChanged;
        }

        private void SettingsForm_Load(object sender, EventArgs e)
        {
            textBoxQuickset.Text = VFDsettings.QuickSets;
        }

        private void VFDsettings_OnFreqChanged(int minValue, int maxValue)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => VFDsettings_OnFreqChanged(minValue, maxValue)));
            }
            else
            {
                labelMinMaxFreq.Text = string.Format("Min/Max Frequency = {0}Hz/{1}Hz", minValue, maxValue);
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
                labelMaxRPM.Text = string.Format("Maximum speed = {0:#,##0}RPM", maxValue);
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
            _mainWindow.COMPortStatus(HYmodbus.VFDData.SerialConnected);
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
            byte[] factoryReset = new byte[] { (byte)VFDsettings.VFD_ModBusID, (byte)VFDcontrol.CommandType.FunctionWrite, (byte)CommandLength.TwoBytes, 0x13, 0x08 };
            HYmodbus.SendDataAsync(factoryReset);
        }

        private void comboBoxCSV_SelectedIndexChanged(object sender, EventArgs e)
        {
            csvSeperator = comboBoxCSV.SelectedItem.ToString()[0];
        }
    }
}
