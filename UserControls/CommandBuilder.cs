using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using System.Diagnostics;

namespace SpindleTalker2.UserControls
{
    public partial class CommandBuilder : UserControl
    {
        public CommandBuilder()
        {
            InitializeComponent();

            cbCommandType.Items.Clear(); cbCommandType.Items.AddRange(Enum.GetNames(typeof(CommandType)));
            cbCommandLength.Items.Clear(); cbCommandLength.Items.AddRange(Enum.GetNames(typeof(CommandLength)));
            labelSlaveID.Text = Settings.VFD_ModBusID.ToString();

            cbCommandType.SelectedIndex = 0;
            cbCommandLength.SelectedIndex = cbCommandLength.Items.Count - 1;

        }

        private void buttonSend_Click(object sender, EventArgs e)
        {
            CommandType selectedCommandType = (CommandType)Enum.Parse(typeof(CommandType), cbCommandType.SelectedItem.ToString());
            CommandLength selectedCommandLength = (CommandLength)Enum.Parse(typeof(CommandLength), cbCommandLength.SelectedItem.ToString());

            SendCommand((byte)selectedCommandType, (byte)selectedCommandLength, (byte)data0.Value, Convert.ToByte(data1.Text, 16), Convert.ToByte(data2.Text, 16));
        }

        private RegisterValue SendCommand(byte selectedCommandType, byte selectedCommandLength, byte _data0, byte _data1, byte _data2)
        {
            int packetLength = selectedCommandLength + 3;
            byte[] command = new byte[packetLength];
            command[0] = (byte)Settings.VFD_ModBusID;
            command[1] = selectedCommandType;
            command[2] = selectedCommandLength;
            command[3] = _data0;
            if (packetLength > 4) command[4] = _data1;
            if (packetLength > 5) command[5] = _data2;

            return new RegisterValue(_data0)
            {
                Value = Serial.SendData(command).ToString()
            };
        }

        private void textBoxEnter(object sender, EventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            textBox.SelectionStart = textBox.Text.Length;
        }

        private void buttonTick_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Currently disabled");
            //Settings.terminalForm.textBoxResponse.Text = string.Empty;
            //Settings.terminalForm.textBoxSent.Text = string.Empty;
            //Settings.PIDRunner = true;
            //Timer tick = new Timer();
            //tick.Interval = 50;
            //tick.Tick += tick_Tick;
            //tick.Start();
        }

        void tick_Tick(object sender, EventArgs e)
        {
            //if (data0.Value < 250)
            //{
            //    data0.Value += 1;
            //    buttonSend_Click(null, null);
            //}
            //else
            //{
            //    (sender as Timer).Stop();
            //    Settings.PIDRunner = false;
            //}
        }

        private void buttonClearText_Click(object sender, EventArgs e)
        {
            Settings.terminalForm.textBoxResponse.Clear();
            Settings.terminalForm.textBoxSent.Clear();
        }

        private void ButtonDownload_Click(object sender, EventArgs e)
        {
            var dialog = new SaveFileDialog();
            dialog.CheckPathExists = true;
            dialog.Filter = "csv file |*.csv";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                var lines = new List<string>();
                lines.Add(RegisterValue.Header(Settings.settingsForm.csvSeperator));
                for (int i=1; i<200; i++)
                {
                    try
                    {
                        var result = SendCommand((byte)CommandType.FunctionRead, 1, (byte)i, 0, 0);
                        if(result != null)
                            lines.Add(result.ToString(Settings.settingsForm.csvSeperator));
                    }
                    catch (Exception ex)
                    {
                        Debug.Print(ex.ToString());
                    }
                }

                File.WriteAllLines(dialog.FileName, lines);
                MessageBox.Show("Finished downloading all values to file");
            }
        }

        private void ButtonUpload_Click(object sender, EventArgs e)
        {
            var dialog = new OpenFileDialog();
            dialog.Multiselect = false;
            dialog.CheckFileExists = true;
            dialog.CheckPathExists = true;
            dialog.Filter = "csv file |*.csv";
            if(dialog.ShowDialog() == DialogResult.OK)
            {
                var lines = RegisterValue.LoadCsv(dialog.FileName, Settings.settingsForm.csvSeperator);
                if(lines != null)
                {
                    foreach(var line in lines)
                    {
                        try
                        {
                            SendCommand((byte)CommandType.FunctionWrite, (byte)line.CommandLength, line.data0, line.data1, line.data2);
                            Thread.Sleep(10);
                        }
                        catch (Exception ex)
                        {
                            Debug.Print(ex.ToString());
                        }
                    }

                    MessageBox.Show("Finished uploading all values to VFD");
                }
            }
        }
    }
}
