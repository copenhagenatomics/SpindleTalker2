﻿using System;
using System.Threading;
using System.Windows.Forms;
using VfdControl;
using SpindleTalker2.VfdSettings;

namespace SpindleTalker2.UserControls
{
    public partial class CommandBuilder : UserControl
    {
        private TerminalControl _terminalForm;
        private MainWindow _mainWindow;
        private char _csvSeperator;

        public CommandBuilder(TerminalControl terminalControl, MainWindow mainWindow, char csvSeperator)
        {
            _terminalForm = terminalControl;
            _mainWindow = mainWindow;
            _csvSeperator = csvSeperator;
            InitializeComponent();

            cbCommandType.Items.Clear(); cbCommandType.Items.AddRange(Enum.GetNames(typeof(CommandType)));
            cbCommandLength.Items.Clear(); cbCommandLength.Items.AddRange(Enum.GetNames(typeof(CommandLength)));
            labelSlaveID.Text = VFDsettings.VFD_ModBusID.ToString();

            cbCommandType.SelectedIndex = 0;
            cbCommandLength.SelectedIndex = cbCommandLength.Items.Count - 1;

        }

        private void buttonSend_Click(object sender, EventArgs e)
        {
            CommandType selectedCommandType = (CommandType)Enum.Parse(typeof(CommandType), cbCommandType.SelectedItem.ToString());
            CommandLength selectedCommandLength = (CommandLength)Enum.Parse(typeof(CommandLength), cbCommandLength.SelectedItem.ToString());

            _mainWindow._hyMotorControl._hyModbus.SendCommand((byte)selectedCommandType, (byte)selectedCommandLength, (byte)data0.Value, Convert.ToByte(data1.Text, 16), Convert.ToByte(data2.Text, 16));
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
                new Thread(() =>
                {
                    var settings = _mainWindow._hyMotorControl.ReadSettingsFromVfd(_csvSeperator);
                    var vfdSettingsHandler = new VfdSettingsHandler();
                    vfdSettingsHandler.SaveCsv(dialog.FileName, _csvSeperator, settings);
                    _mainWindow._hyMotorControl._hyModbus.StartPolling();
                    MessageBox.Show("Finished downloading all values to file");
                }).Start();
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
                new Thread(() => {
                    var vfdSettingsHandler = new VfdSettingsHandler();
                    var lines = vfdSettingsHandler.OpenCsv(dialog.FileName, _csvSeperator);
                    if (_mainWindow._hyMotorControl.WriteSettingsToVfd(lines, _csvSeperator))
                    {
                        MessageBox.Show("Finished uploading all values to VFD");
                    }
                    _mainWindow._hyMotorControl._hyModbus.StartPolling();
                }).Start();
            }
        }
    }
}
