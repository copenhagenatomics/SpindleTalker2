using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SpindleTalker2.Properties;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
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
            VFDsettings.OnFreqChanged += VFDsettings_OnFreqChanged;
            VFDsettings.OnRpmChanged += VFDsettings_OnRpmChanged;
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

        #region ComPort Initialisation

        // Taken from 
        /* 
         * Project:    SerialPort Terminal
         * Company:    Coad .NET, http://coad.net
         * Author:     Noah Coad, http://coad.net/noah
         * Created:    March 2005
         * 
         * Notes:      This was created to demonstrate how to use the SerialPort control for
         *             communicating with your PC's Serial RS-232 COM Port
         * 
         *             It is for educational purposes only and not sanctified for industrial use. :)
         *             Written to support the blog post article at: http://msmvps.com/blogs/coad/archive/2005/03/23/39466.aspx
         * 
         */
        public void RefreshComPortList()
        {
            // Determain if the list of com port names has changed since last checked
            string selected = RefreshComPortList(cmbPortName.Items.Cast<string>(), cmbPortName.SelectedItem as string, false);

            // If there was an update, then update the control showing the user the list of port names
            if (!String.IsNullOrEmpty(selected))
            {
                cmbPortName.Items.Clear();
                cmbPortName.Items.AddRange(OrderedPortNames());
                cmbPortName.SelectedItem = selected;
            }
        }

        public string RefreshComPortList(IEnumerable<string> PreviousPortNames, string CurrentSelection, bool PortOpen)
        {
            // Create a new return report to populate
            string selected = null;

            // Retrieve the list of ports currently mounted by the operating system (sorted by name)
            string[] ports = SerialPort.GetPortNames();

            // First determain if there was a change (any additions or removals)
            bool updated = PreviousPortNames.Except(ports).Count() > 0 || ports.Except(PreviousPortNames).Count() > 0;

            // If there was a change, then select an appropriate default port
            if (updated)
            {
                // Use the correctly ordered set of port names
                ports = OrderedPortNames();

                // Find newest port if one or more were added
                string newest = SerialPort.GetPortNames().Except(PreviousPortNames).OrderBy(a => a).LastOrDefault();

                // If the port was already open... (see logic notes and reasoning in Notes.txt)
                if (PortOpen)
                {
                    if (ports.Contains(CurrentSelection)) selected = CurrentSelection;
                    else if (!String.IsNullOrEmpty(newest)) selected = newest;
                    else selected = ports.LastOrDefault();
                }
                else
                {
                    if (!String.IsNullOrEmpty(newest)) selected = newest;
                    else if (ports.Contains(CurrentSelection)) selected = CurrentSelection;
                    else selected = ports.LastOrDefault();
                }
            }

            // If there was a change to the port list, return the recommended default selection
            return selected;
        }

        private string[] OrderedPortNames()
        {
            // Just a placeholder for a successful parsing of a string to an integer
            int num;

            // Order the serial port names in numberic order (if possible)
            return SerialPort.GetPortNames().OrderBy(a => a.Length > 3 && int.TryParse(a.Substring(3), out num) ? num : 0).ToArray();
        }

        /// <summary> Populate the form's controls with default settings. </summary>
        public bool InitializeControlValues()
        {
            bool isCOMPortAvailable = true;

            cmbParity.Items.Clear(); cmbParity.Items.AddRange(Enum.GetNames(typeof(Parity)));
            cmbStopBits.Items.Clear(); cmbStopBits.Items.AddRange(Enum.GetNames(typeof(StopBits)));

            cmbParity.Text = VFDsettings.Parity.ToString();
            cmbStopBits.Text = VFDsettings.StopBits.ToString();
            cmbDataBits.Text = VFDsettings.DataBits.ToString();
            cmbParity.Text = VFDsettings.Parity.ToString();
            cmbBaudRate.Text = VFDsettings.BaudRate.ToString();
            checkBoxAutoConnectAtStartup.Checked = VFDsettings.AutoConnectAtStartup;

            RefreshComPortList();

            // If it is still avalible, select the last com port used
            if (cmbPortName.Items.Contains(VFDsettings.PortName)) cmbPortName.Text = VFDsettings.PortName;
            else if (cmbPortName.Items.Count > 0) cmbPortName.SelectedIndex = cmbPortName.Items.Count - 1;
            else
            {
                MessageBox.Show(this, "There are no COM Ports detected on this computer.\nPlease install a COM Port and restart this app.", "No COM Ports Installed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                isCOMPortAvailable = false;
            }

            return isCOMPortAvailable;
        }

        #endregion

        private void SettingsForm_ResizeEnd(object sender, EventArgs e)
        {
        }

        private void cmbPortName_SelectedIndexChanged(object sender, EventArgs e)
        {
            VFDsettings.PortName = cmbPortName.SelectedItem.ToString() ;
            _mainWindow.COMPortStatus(VFDsettings.SerialConnected);
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
            Serial.SendDataAsync(factoryReset);
        }

        private void comboBoxCSV_SelectedIndexChanged(object sender, EventArgs e)
        {
            csvSeperator = comboBoxCSV.SelectedItem.ToString()[0];
        }
    }
}
