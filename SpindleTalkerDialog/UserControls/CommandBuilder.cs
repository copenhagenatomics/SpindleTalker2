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
using VFDcontrol;

namespace SpindleTalker2.UserControls
{
    public partial class CommandBuilder : UserControl
    {
        private TerminalControl _terminalForm;
        private SettingsControl _settingsForm;

        public CommandBuilder(TerminalControl terminalControl, SettingsControl settingsControl)
        {
            _terminalForm = terminalControl;
            _settingsForm = settingsControl;
            InitializeComponent();

            cbCommandType.Items.Clear(); cbCommandType.Items.AddRange(Enum.GetNames(typeof(VFDcontrol.CommandType)));
            cbCommandLength.Items.Clear(); cbCommandLength.Items.AddRange(Enum.GetNames(typeof(CommandLength)));
            labelSlaveID.Text = VFDsettings.VFD_ModBusID.ToString();

            cbCommandType.SelectedIndex = 0;
            cbCommandLength.SelectedIndex = cbCommandLength.Items.Count - 1;

        }

        private void buttonSend_Click(object sender, EventArgs e)
        {
            VFDcontrol.CommandType selectedCommandType = (VFDcontrol.CommandType)Enum.Parse(typeof(VFDcontrol.CommandType), cbCommandType.SelectedItem.ToString());
            CommandLength selectedCommandLength = (CommandLength)Enum.Parse(typeof(CommandLength), cbCommandLength.SelectedItem.ToString());

            HYmodbus.SendCommand((byte)selectedCommandType, (byte)selectedCommandLength, (byte)data0.Value, Convert.ToByte(data1.Text, 16), Convert.ToByte(data2.Text, 16));
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
            _terminalForm.textBoxResponse.Clear();
            _terminalForm.textBoxSent.Clear();
        }

        private void ButtonDownload_Click(object sender, EventArgs e)
        {
            var dialog = new SaveFileDialog();
            dialog.CheckPathExists = true;
            dialog.Filter = "csv file |*.csv";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                RegisterValue.Download(dialog.FileName, _settingsForm.csvSeperator);
                HYmodbus.StartPolling();
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
                try
                {
                    if(RegisterValue.Upload(dialog.FileName, _settingsForm.csvSeperator))
                    { 
                        MessageBox.Show("Finished uploading all values to VFD");
                    }

                    HYmodbus.StartPolling();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
        }
    }
}
