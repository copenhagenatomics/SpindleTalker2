using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SpindleTalker2
{
    public partial class TerminalControl : UserControl
    {
        public TerminalControl(MainWindow mainWindow, char csvSeperator)
        {
            this.commandBuilder1 = new UserControls.CommandBuilder(this, mainWindow, csvSeperator);
            InitializeComponent();
            mainWindow._hyMotorControl._hyModbus.OnWriteTerminalForm += Serial_WriteTerminalForm;
        }

        private void Serial_WriteTerminalForm(string message, bool send)
        {
            if (textBoxResponse.InvokeRequired)
            {
                textBoxResponse.Invoke(new Action(() => Serial_WriteTerminalForm(message, send)));
            }
            else if (send)
            {
                textBoxSent.Text += message + Environment.NewLine;
                textBoxSent.SelectionStart = textBoxSent.Text.Length;
                textBoxSent.ScrollToCaret();
            }
            else
            {
                textBoxResponse.Text += message + Environment.NewLine;
                textBoxResponse.SelectionStart = textBoxResponse.Text.Length;
                textBoxResponse.ScrollToCaret();
            }
        }
    }
}
