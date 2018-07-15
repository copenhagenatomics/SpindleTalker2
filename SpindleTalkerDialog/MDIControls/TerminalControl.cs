using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using VFDcontrol;

namespace SpindleTalker2
{
    public partial class TerminalControl : UserControl
    {
        public TerminalControl(SettingsControl settingsControl)
        {
            this.commandBuilder1 = new SpindleTalker2.UserControls.CommandBuilder(this, settingsControl);
            InitializeComponent();
            Serial.OnWriteTerminalForm += Serial_WriteTerminalForm;
        }

        private static bool IsValidHexString(IEnumerable<char> hexString)
        {
            return hexString.Select(currentCharacter =>
                        (currentCharacter >= '0' && currentCharacter <= '9') ||
                        (currentCharacter >= 'a' && currentCharacter <= 'f') ||
                        (currentCharacter >= 'A' && currentCharacter <= 'F')).All(isHexCharacter => isHexCharacter);
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
