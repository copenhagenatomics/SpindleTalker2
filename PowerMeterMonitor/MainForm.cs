using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using VFDcontrol;

namespace PowerMeterMonitor
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            comboBoxComPort.Items.Clear();
            comboBoxComPort.Items.AddRange(SerialPort.GetPortNames());
            comboBoxComPort.SelectedIndex = 0;

            comboBoxBaudRate.SelectedIndex = 0;
            comboBoxSlaveID.SelectedIndex = 0;
            comboBoxCommandType.SelectedIndex = 0;

            textBoxData0.Text = "0";
            textBoxData1.Text = "0";
            textBoxData2.Text = "0";

            HYmodbus.OnWriteTerminalForm += HYmodbus_OnWriteTerminalForm;
            HYmodbus.OnWriteLog += HYmodbus_OnWriteLog;

            Connect();
        }

        private void HYmodbus_OnWriteLog(string message, bool error = false)
        {
            Debug.Print(message);
        }

        private void HYmodbus_OnWriteTerminalForm(string message, bool send)
        {
            if (textBoxRx.InvokeRequired)
            {
                textBoxRx.Invoke(new Action(() => HYmodbus_OnWriteTerminalForm(message, send)));
            }
            else if (send)
            {
                textBoxTx.Text += message + Environment.NewLine;
                textBoxTx.SelectionStart = textBoxTx.Text.Length;
                textBoxTx.ScrollToCaret();
            }
            else
            {
                textBoxRx.Text += message + Environment.NewLine;
                textBoxRx.SelectionStart = textBoxRx.Text.Length;
                textBoxRx.ScrollToCaret();
            }
        }

        private void Connect()
        {
            if (comboBoxComPort.Text.Length == 0)  return;
            if (comboBoxBaudRate.Text.Length == 0) return;
            if (comboBoxSlaveID.Text.Length == 0) return;

            VFDsettings.PortName = comboBoxComPort.Text;
            VFDsettings.BaudRate = int.Parse(comboBoxBaudRate.Text);
            VFDsettings.VFD_ModBusID = int.Parse(comboBoxSlaveID.Text);

            if(HYmodbus.ComOpen)
                HYmodbus.Disconnect();

            Thread.Sleep(100);

            HYmodbus.Connect();

            if (HYmodbus.ComOpen)
            {
                labelConnectionStatus.Text = "Connected";
                labelConnectionStatus.ForeColor = Color.Green;
            }
            else
            {
                labelConnectionStatus.Text = "Connected";
                labelConnectionStatus.ForeColor = Color.Green;
            }
        }

        private void buttonSend_Click(object sender, EventArgs e)
        {
            if (!HYmodbus.ComOpen)
            {
                MessageBox.Show("Connection not open");
                return;
            }

            VFDcontrol.CommandType selectedCommandType = (VFDcontrol.CommandType)Enum.Parse(typeof(VFDcontrol.CommandType), comboBoxCommandType.SelectedItem.ToString());
            HYmodbus.SendCommand((byte)selectedCommandType, (byte)3, Convert.ToByte(textBoxData0.Text, 16), Convert.ToByte(textBoxData1.Text, 16), Convert.ToByte(textBoxData2.Text, 16));
        }

        private void comboBoxComPort_SelectedIndexChanged(object sender, EventArgs e)
        {
            Connect();
        }

        private void comboBoxBaudRate_SelectedIndexChanged(object sender, EventArgs e)
        {
            Connect();
        }
    }
}
