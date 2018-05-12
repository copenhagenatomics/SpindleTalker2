﻿namespace SpindleTalker2.UserControls
{
    partial class CommandBuilder
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CommandBuilder));
            this.LegendSlaveID = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.labelSlaveID = new System.Windows.Forms.Label();
            this.cbCommandType = new System.Windows.Forms.ComboBox();
            this.cbCommandLength = new System.Windows.Forms.ComboBox();
            this.groupBoxBase = new System.Windows.Forms.GroupBox();
            this.ButtonDownload = new System.Windows.Forms.PictureBox();
            this.ButtonUpload = new System.Windows.Forms.PictureBox();
            this.buttonClearText = new System.Windows.Forms.Button();
            this.data0 = new System.Windows.Forms.NumericUpDown();
            this.buttonSend = new System.Windows.Forms.Button();
            this.data2 = new System.Windows.Forms.TextBox();
            this.data1 = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBoxBase.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ButtonDownload)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ButtonUpload)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.data0)).BeginInit();
            this.SuspendLayout();
            // 
            // LegendSlaveID
            // 
            this.LegendSlaveID.AutoSize = true;
            this.LegendSlaveID.Location = new System.Drawing.Point(11, 14);
            this.LegendSlaveID.Name = "LegendSlaveID";
            this.LegendSlaveID.Size = new System.Drawing.Size(48, 13);
            this.LegendSlaveID.TabIndex = 0;
            this.LegendSlaveID.Text = "Slave ID";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(93, 14);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(81, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Command Type";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(218, 14);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(81, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Payload Length";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(312, 11);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(42, 13);
            this.label4.TabIndex = 3;
            this.label4.Text = "Data[0]";
            // 
            // labelSlaveID
            // 
            this.labelSlaveID.Location = new System.Drawing.Point(11, 27);
            this.labelSlaveID.Name = "labelSlaveID";
            this.labelSlaveID.Size = new System.Drawing.Size(48, 20);
            this.labelSlaveID.TabIndex = 4;
            this.labelSlaveID.Text = "0";
            this.labelSlaveID.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // cbCommandType
            // 
            this.cbCommandType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbCommandType.FormattingEnabled = true;
            this.cbCommandType.Location = new System.Drawing.Point(65, 27);
            this.cbCommandType.Name = "cbCommandType";
            this.cbCommandType.Size = new System.Drawing.Size(139, 21);
            this.cbCommandType.TabIndex = 0;
            // 
            // cbCommandLength
            // 
            this.cbCommandLength.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbCommandLength.FormattingEnabled = true;
            this.cbCommandLength.Location = new System.Drawing.Point(221, 27);
            this.cbCommandLength.Name = "cbCommandLength";
            this.cbCommandLength.Size = new System.Drawing.Size(78, 21);
            this.cbCommandLength.TabIndex = 1;
            // 
            // groupBoxBase
            // 
            this.groupBoxBase.Controls.Add(this.ButtonDownload);
            this.groupBoxBase.Controls.Add(this.ButtonUpload);
            this.groupBoxBase.Controls.Add(this.buttonClearText);
            this.groupBoxBase.Controls.Add(this.data0);
            this.groupBoxBase.Controls.Add(this.buttonSend);
            this.groupBoxBase.Controls.Add(this.data2);
            this.groupBoxBase.Controls.Add(this.data1);
            this.groupBoxBase.Controls.Add(this.label5);
            this.groupBoxBase.Controls.Add(this.label1);
            this.groupBoxBase.Controls.Add(this.cbCommandLength);
            this.groupBoxBase.Controls.Add(this.LegendSlaveID);
            this.groupBoxBase.Controls.Add(this.label4);
            this.groupBoxBase.Controls.Add(this.cbCommandType);
            this.groupBoxBase.Controls.Add(this.label3);
            this.groupBoxBase.Controls.Add(this.labelSlaveID);
            this.groupBoxBase.Controls.Add(this.label2);
            this.groupBoxBase.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBoxBase.Location = new System.Drawing.Point(0, 0);
            this.groupBoxBase.Name = "groupBoxBase";
            this.groupBoxBase.Size = new System.Drawing.Size(850, 58);
            this.groupBoxBase.TabIndex = 1;
            this.groupBoxBase.TabStop = false;
            this.groupBoxBase.Text = "Command Builder";
            // 
            // ButtonDownload
            // 
            this.ButtonDownload.Image = ((System.Drawing.Image)(resources.GetObject("ButtonDownload.Image")));
            this.ButtonDownload.InitialImage = ((System.Drawing.Image)(resources.GetObject("ButtonDownload.InitialImage")));
            this.ButtonDownload.Location = new System.Drawing.Point(609, 15);
            this.ButtonDownload.Name = "ButtonDownload";
            this.ButtonDownload.Size = new System.Drawing.Size(116, 33);
            this.ButtonDownload.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.ButtonDownload.TabIndex = 14;
            this.ButtonDownload.TabStop = false;
            this.ButtonDownload.Click += new System.EventHandler(this.ButtonDownload_Click);
            // 
            // ButtonUpload
            // 
            this.ButtonUpload.Image = ((System.Drawing.Image)(resources.GetObject("ButtonUpload.Image")));
            this.ButtonUpload.InitialImage = ((System.Drawing.Image)(resources.GetObject("ButtonUpload.InitialImage")));
            this.ButtonUpload.Location = new System.Drawing.Point(731, 15);
            this.ButtonUpload.Name = "ButtonUpload";
            this.ButtonUpload.Size = new System.Drawing.Size(116, 33);
            this.ButtonUpload.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.ButtonUpload.TabIndex = 13;
            this.ButtonUpload.TabStop = false;
            this.ButtonUpload.Click += new System.EventHandler(this.ButtonUpload_Click);
            // 
            // buttonClearText
            // 
            this.buttonClearText.Location = new System.Drawing.Point(538, 24);
            this.buttonClearText.Name = "buttonClearText";
            this.buttonClearText.Size = new System.Drawing.Size(65, 23);
            this.buttonClearText.TabIndex = 12;
            this.buttonClearText.Text = "Clear Text";
            this.buttonClearText.UseVisualStyleBackColor = true;
            this.buttonClearText.Click += new System.EventHandler(this.buttonClearText_Click);
            // 
            // data0
            // 
            this.data0.Location = new System.Drawing.Point(315, 28);
            this.data0.Maximum = new decimal(new int[] {
            300,
            0,
            0,
            0});
            this.data0.Name = "data0";
            this.data0.Size = new System.Drawing.Size(38, 20);
            this.data0.TabIndex = 10;
            this.data0.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // buttonSend
            // 
            this.buttonSend.Location = new System.Drawing.Point(477, 24);
            this.buttonSend.Name = "buttonSend";
            this.buttonSend.Size = new System.Drawing.Size(55, 23);
            this.buttonSend.TabIndex = 5;
            this.buttonSend.Text = "Send";
            this.buttonSend.UseVisualStyleBackColor = true;
            this.buttonSend.Click += new System.EventHandler(this.buttonSend_Click);
            // 
            // data2
            // 
            this.data2.Location = new System.Drawing.Point(414, 27);
            this.data2.Name = "data2";
            this.data2.Size = new System.Drawing.Size(39, 20);
            this.data2.TabIndex = 4;
            this.data2.Text = "00";
            this.data2.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.data2.Enter += new System.EventHandler(this.textBoxEnter);
            // 
            // data1
            // 
            this.data1.Location = new System.Drawing.Point(364, 27);
            this.data1.Name = "data1";
            this.data1.Size = new System.Drawing.Size(39, 20);
            this.data1.TabIndex = 3;
            this.data1.Text = "00";
            this.data1.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.data1.Enter += new System.EventHandler(this.textBoxEnter);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(412, 11);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(42, 13);
            this.label5.TabIndex = 9;
            this.label5.Text = "Data[2]";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(362, 11);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(42, 13);
            this.label1.TabIndex = 8;
            this.label1.Text = "Data[1]";
            // 
            // CommandBuilder
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBoxBase);
            this.Name = "CommandBuilder";
            this.Size = new System.Drawing.Size(850, 58);
            this.groupBoxBase.ResumeLayout(false);
            this.groupBoxBase.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ButtonDownload)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ButtonUpload)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.data0)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox cbCommandLength;
        private System.Windows.Forms.ComboBox cbCommandType;
        private System.Windows.Forms.Label labelSlaveID;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label LegendSlaveID;
        private System.Windows.Forms.GroupBox groupBoxBase;
        private System.Windows.Forms.Button buttonSend;
        private System.Windows.Forms.TextBox data2;
        private System.Windows.Forms.TextBox data1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown data0;
        private System.Windows.Forms.Button buttonClearText;
        private System.Windows.Forms.PictureBox ButtonUpload;
        private System.Windows.Forms.PictureBox ButtonDownload;
    }
}
