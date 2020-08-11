using System;
using System.Diagnostics;
using System.Drawing;
using System.IO.Ports;
using System.Threading;
using System.Windows.Forms;
using VfdControl;

namespace PowerMeterMonitor
{
    public partial class MainForm : Form
    {
        private MotorControl _hyMotorControl;

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

            _hyMotorControl = new MotorControl(baudRate: 38400, portName: comboBoxComPort.Text);
            _hyMotorControl._hyModbus.PortName = comboBoxComPort.Text;
            _hyMotorControl._hyModbus.BaudRate = int.Parse(comboBoxBaudRate.Text);
            _hyMotorControl._hyModbus.ModBusID = int.Parse(comboBoxSlaveID.Text);
            _hyMotorControl._hyModbus.OnWriteTerminalForm += HYmodbus_OnWriteTerminalForm;
            _hyMotorControl._hyModbus.OnWriteLog += HYmodbus_OnWriteLog;

            if (_hyMotorControl._hyModbus.ComOpen)
                _hyMotorControl._hyModbus.Disconnect();

            Thread.Sleep(100);

            _hyMotorControl._hyModbus.Connect();

            if (_hyMotorControl._hyModbus.ComOpen)
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
            if (!_hyMotorControl._hyModbus.ComOpen)
            {
                MessageBox.Show("Connection not open");
                return;
            }

            CommandType selectedCommandType = (CommandType)Enum.Parse(typeof(CommandType), comboBoxCommandType.SelectedItem.ToString());
            _hyMotorControl._hyModbus.SendCommand((byte)selectedCommandType, (byte)3, Convert.ToByte(textBoxData0.Text, 16), Convert.ToByte(textBoxData1.Text, 16), Convert.ToByte(textBoxData2.Text, 16));
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
