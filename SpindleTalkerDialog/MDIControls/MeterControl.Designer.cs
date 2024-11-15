﻿namespace SpindleTalker2
{
    partial class MeterControl
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.MeterSetF = new SpindleTalker2.GCSMeter();
            this.MeterOutF = new SpindleTalker2.GCSMeter();
            this.MeterRPM = new SpindleTalker2.GCSMeter();
            this.MeterAmps = new SpindleTalker2.GCSMeter();
            this.MeterVAC = new SpindleTalker2.GCSMeter();
            this.MeterVDC = new SpindleTalker2.GCSMeter();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 6;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 16F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 16F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 16F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 16F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 16F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 16F));
            this.tableLayoutPanel1.Controls.Add(this.MeterSetF, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.MeterOutF, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.MeterRPM, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.MeterAmps, 3, 0);
            this.tableLayoutPanel1.Controls.Add(this.MeterVDC, 4, 0);
            this.tableLayoutPanel1.Controls.Add(this.MeterVAC, 5, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 214F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(900, 214);
            this.tableLayoutPanel1.TabIndex = 4;
            // 
            // MeterSetF
            // 
            this.MeterSetF.Colour = System.Drawing.Color.Firebrick;
            this.MeterSetF.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MeterSetF.Location = new System.Drawing.Point(3, 3);
            this.MeterSetF.Name = "MeterSetF";
            this.MeterSetF.ScaleDivisions = 6;
            this.MeterSetF.ScaleMaxValue = 1D;
            this.MeterSetF.ScaleMinValue = -1D;
            this.MeterSetF.ScaleSubDivisions = 3;
            this.MeterSetF.Size = new System.Drawing.Size(140, 208);
            this.MeterSetF.TabIndex = 7;
            this.MeterSetF.Title = "Set Frequency";
            this.MeterSetF.Units = "Hz";
            this.MeterSetF.Value = -1D;
            // 
            // MeterOutF
            // 
            this.MeterOutF.Colour = System.Drawing.Color.DarkOliveGreen;
            this.MeterOutF.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MeterOutF.Location = new System.Drawing.Point(150, 3);
            this.MeterOutF.Name = "MeterOutF";
            this.MeterOutF.ScaleDivisions = 6;
            this.MeterOutF.ScaleMaxValue = 1D;
            this.MeterOutF.ScaleMinValue = -1D;
            this.MeterOutF.ScaleSubDivisions = 3;
            this.MeterOutF.Size = new System.Drawing.Size(140, 208);
            this.MeterOutF.TabIndex = 8;
            this.MeterOutF.Title = "Output Frequency";
            this.MeterOutF.Units = "Hz";
            this.MeterOutF.Value = -1D;
            // 
            // MeterRPM
            // 
            this.MeterRPM.Colour = System.Drawing.Color.DarkCyan;
            this.MeterRPM.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MeterRPM.Location = new System.Drawing.Point(300, 3);
            this.MeterRPM.Name = "MeterRPM";
            this.MeterRPM.ScaleDivisions = 6;
            this.MeterRPM.ScaleMaxValue = 1D;
            this.MeterRPM.ScaleMinValue = -1D;
            this.MeterRPM.ScaleSubDivisions = 3;
            this.MeterRPM.Size = new System.Drawing.Size(140, 208);
            this.MeterRPM.TabIndex = 9;
            this.MeterRPM.Title = "RPM";
            this.MeterRPM.Units = "RPM";
            this.MeterRPM.Value = -1D;
            // 
            // MeterAmps
            // 
            this.MeterAmps.Colour = System.Drawing.Color.DarkBlue;
            this.MeterAmps.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MeterAmps.Location = new System.Drawing.Point(450, 3);
            this.MeterAmps.Name = "MeterAmps";
            this.MeterAmps.ScaleDivisions = 6;
            this.MeterAmps.ScaleMaxValue = 20D;
            this.MeterAmps.ScaleMinValue = 0D;
            this.MeterAmps.ScaleSubDivisions = 3;
            this.MeterAmps.Size = new System.Drawing.Size(140, 208);
            this.MeterAmps.TabIndex = 10;
            this.MeterAmps.Title = "Output Current";
            this.MeterAmps.Units = "A";
            this.MeterAmps.Value = 0D;
            // 
            // MeterVDC
            // 
            this.MeterVDC.Colour = System.Drawing.Color.DarkMagenta;
            this.MeterVDC.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MeterVDC.Location = new System.Drawing.Point(600, 3);
            this.MeterVDC.Name = "MeterVDC";
            this.MeterVDC.ScaleDivisions = 6;
            this.MeterVDC.ScaleMaxValue = 600D;
            this.MeterVDC.ScaleMinValue = -1D;
            this.MeterVDC.ScaleSubDivisions = 3;
            this.MeterVDC.Size = new System.Drawing.Size(140, 208);
            this.MeterVDC.TabIndex = 11;
            this.MeterVDC.Title = "Input Volt DC";
            this.MeterVDC.Units = "V";
            this.MeterVDC.Value = 0D;
            // 
            // MeterVAC
            // 
            this.MeterVAC.Colour = System.Drawing.Color.DarkOrange;
            this.MeterVAC.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MeterVAC.Location = new System.Drawing.Point(750, 3);
            this.MeterVAC.Name = "MeterVAC";
            this.MeterVAC.ScaleDivisions = 6;
            this.MeterVAC.ScaleMaxValue = 380D;
            this.MeterVAC.ScaleMinValue = 0D;
            this.MeterVAC.ScaleSubDivisions = 3;
            this.MeterVAC.Size = new System.Drawing.Size(140, 208);
            this.MeterVAC.TabIndex = 12;
            this.MeterVAC.Title = "Output Volt AC";
            this.MeterVAC.Units = "V";
            this.MeterVAC.Value = 0D;
            // 
            // MeterControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "MeterControl";
            this.Size = new System.Drawing.Size(900, 214);
            this.Tag = "Graphs";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        public GCSMeter MeterOutF;
        public GCSMeter MeterSetF;
        public GCSMeter MeterRPM;
        public GCSMeter MeterAmps;
        public GCSMeter MeterVDC;
        public GCSMeter MeterVAC;


    }
}