namespace TempMonitor.Forms
{
    partial class frmOptions
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmOptions));
            this.label1 = new System.Windows.Forms.Label();
            this.ckRecording = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.tbRecordInterval = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.tbDelay = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.ckAutoSave = new System.Windows.Forms.CheckBox();
            this.tbSaveInterval = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.tbSaveLocation = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.butDefaults = new System.Windows.Forms.Button();
            this.butLocation = new System.Windows.Forms.Button();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.butCancel = new System.Windows.Forms.Button();
            this.butSave = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.tbMaxBoxes = new System.Windows.Forms.TextBox();
            this.ckListen = new System.Windows.Forms.CheckBox();
            this.tbSleep = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(128, 16);
            this.label1.TabIndex = 0;
            this.label1.Text = "Reading Sensors";
            // 
            // ckRecording
            // 
            this.ckRecording.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.ckRecording.Location = new System.Drawing.Point(9, 128);
            this.ckRecording.Name = "ckRecording";
            this.ckRecording.Size = new System.Drawing.Size(160, 19);
            this.ckRecording.TabIndex = 4;
            this.ckRecording.Text = "Save To Database";
            this.ckRecording.UseVisualStyleBackColor = true;
            this.ckRecording.CheckedChanged += new System.EventHandler(this.ckRecording_CheckedChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 56);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(116, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Read Interval (minutes)";
            // 
            // tbRecordInterval
            // 
            this.tbRecordInterval.Location = new System.Drawing.Point(155, 52);
            this.tbRecordInterval.Name = "tbRecordInterval";
            this.tbRecordInterval.Size = new System.Drawing.Size(117, 20);
            this.tbRecordInterval.TabIndex = 1;
            this.tbRecordInterval.Tag = "0";
            this.tbRecordInterval.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tbRecordInterval.TextChanged += new System.EventHandler(this.tbRecordInterval_TextChanged);
            this.tbRecordInterval.Validating += new System.ComponentModel.CancelEventHandler(this.tbRecordInterval_Validating);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 81);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(136, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Controlbox Delay (seconds)";
            // 
            // tbDelay
            // 
            this.tbDelay.Location = new System.Drawing.Point(155, 77);
            this.tbDelay.Name = "tbDelay";
            this.tbDelay.Size = new System.Drawing.Size(117, 20);
            this.tbDelay.TabIndex = 2;
            this.tbDelay.Tag = "1";
            this.tbDelay.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tbDelay.TextChanged += new System.EventHandler(this.tbDelay_TextChanged);
            this.tbDelay.Validating += new System.ComponentModel.CancelEventHandler(this.tbDelay_Validating);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(9, 192);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(112, 16);
            this.label4.TabIndex = 6;
            this.label4.Text = "Sensor Report:";
            // 
            // ckAutoSave
            // 
            this.ckAutoSave.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.ckAutoSave.Location = new System.Drawing.Point(9, 211);
            this.ckAutoSave.Name = "ckAutoSave";
            this.ckAutoSave.Size = new System.Drawing.Size(160, 19);
            this.ckAutoSave.TabIndex = 6;
            this.ckAutoSave.Text = "Save to file";
            this.ckAutoSave.UseVisualStyleBackColor = true;
            this.ckAutoSave.CheckedChanged += new System.EventHandler(this.ckAutoSave_CheckedChanged);
            // 
            // tbSaveInterval
            // 
            this.tbSaveInterval.Location = new System.Drawing.Point(155, 233);
            this.tbSaveInterval.Name = "tbSaveInterval";
            this.tbSaveInterval.Size = new System.Drawing.Size(117, 20);
            this.tbSaveInterval.TabIndex = 7;
            this.tbSaveInterval.Tag = "2";
            this.tbSaveInterval.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tbSaveInterval.TextChanged += new System.EventHandler(this.tbSaveInterval_TextChanged);
            this.tbSaveInterval.Validating += new System.ComponentModel.CancelEventHandler(this.tbSaveInterval_Validating);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(9, 236);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(105, 13);
            this.label5.TabIndex = 8;
            this.label5.Text = "Save Interval (hours)";
            // 
            // tbSaveLocation
            // 
            this.tbSaveLocation.Location = new System.Drawing.Point(8, 281);
            this.tbSaveLocation.Multiline = true;
            this.tbSaveLocation.Name = "tbSaveLocation";
            this.tbSaveLocation.Size = new System.Drawing.Size(370, 56);
            this.tbSaveLocation.TabIndex = 8;
            this.tbSaveLocation.Tag = "3";
            this.tbSaveLocation.TextChanged += new System.EventHandler(this.tbSaveLocation_TextChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(9, 261);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(79, 13);
            this.label6.TabIndex = 11;
            this.label6.Text = "Save Location:";
            // 
            // butDefaults
            // 
            this.butDefaults.Image = ((System.Drawing.Image)(resources.GetObject("butDefaults.Image")));
            this.butDefaults.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.butDefaults.Location = new System.Drawing.Point(102, 356);
            this.butDefaults.Name = "butDefaults";
            this.butDefaults.Size = new System.Drawing.Size(88, 61);
            this.butDefaults.TabIndex = 14;
            this.butDefaults.Text = "Load Defaults";
            this.butDefaults.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.butDefaults.UseVisualStyleBackColor = true;
            this.butDefaults.Click += new System.EventHandler(this.butDefaults_Click);
            // 
            // butLocation
            // 
            this.butLocation.Image = ((System.Drawing.Image)(resources.GetObject("butLocation.Image")));
            this.butLocation.ImageAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.butLocation.Location = new System.Drawing.Point(8, 356);
            this.butLocation.Name = "butLocation";
            this.butLocation.Size = new System.Drawing.Size(88, 61);
            this.butLocation.TabIndex = 15;
            this.butLocation.Text = "New Location";
            this.butLocation.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.butLocation.UseVisualStyleBackColor = true;
            this.butLocation.Click += new System.EventHandler(this.butLocation_Click);
            // 
            // butCancel
            // 
            this.butCancel.Image = ((System.Drawing.Image)(resources.GetObject("butCancel.Image")));
            this.butCancel.Location = new System.Drawing.Point(196, 356);
            this.butCancel.Name = "butCancel";
            this.butCancel.Size = new System.Drawing.Size(88, 61);
            this.butCancel.TabIndex = 17;
            this.butCancel.Text = "Cancel";
            this.butCancel.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.butCancel.UseVisualStyleBackColor = true;
            this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
            // 
            // butSave
            // 
            this.butSave.Image = ((System.Drawing.Image)(resources.GetObject("butSave.Image")));
            this.butSave.Location = new System.Drawing.Point(290, 356);
            this.butSave.Name = "butSave";
            this.butSave.Size = new System.Drawing.Size(88, 61);
            this.butSave.TabIndex = 9;
            this.butSave.Text = "Save";
            this.butSave.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.butSave.UseVisualStyleBackColor = true;
            this.butSave.Click += new System.EventHandler(this.butSave_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(11, 106);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(137, 13);
            this.label7.TabIndex = 18;
            this.label7.Text = "Maximum # of Controlboxes";
            // 
            // tbMaxBoxes
            // 
            this.tbMaxBoxes.Location = new System.Drawing.Point(155, 102);
            this.tbMaxBoxes.Name = "tbMaxBoxes";
            this.tbMaxBoxes.Size = new System.Drawing.Size(117, 20);
            this.tbMaxBoxes.TabIndex = 3;
            this.tbMaxBoxes.Tag = "1";
            this.tbMaxBoxes.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tbMaxBoxes.TextChanged += new System.EventHandler(this.tbMaxBoxes_TextChanged);
            this.tbMaxBoxes.Validating += new System.ComponentModel.CancelEventHandler(this.tbMaxBoxes_Validating);
            // 
            // ckListen
            // 
            this.ckListen.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.ckListen.Location = new System.Drawing.Point(12, 31);
            this.ckListen.Name = "ckListen";
            this.ckListen.Size = new System.Drawing.Size(157, 19);
            this.ckListen.TabIndex = 0;
            this.ckListen.Text = "Listen Only";
            this.ckListen.UseVisualStyleBackColor = true;
            this.ckListen.CheckedChanged += new System.EventHandler(this.ckListen_CheckedChanged);
            // 
            // tbSleep
            // 
            this.tbSleep.Location = new System.Drawing.Point(155, 150);
            this.tbSleep.Name = "tbSleep";
            this.tbSleep.Size = new System.Drawing.Size(117, 20);
            this.tbSleep.TabIndex = 5;
            this.tbSleep.Tag = "1";
            this.tbSleep.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tbSleep.TextChanged += new System.EventHandler(this.tbSleep_TextChanged);
            this.tbSleep.Validating += new System.ComponentModel.CancelEventHandler(this.tbSleep_Validating);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(8, 157);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(107, 13);
            this.label8.TabIndex = 20;
            this.label8.Text = "Sleep Interval (hours)";
            // 
            // frmOptions
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(386, 429);
            this.Controls.Add(this.tbSleep);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.ckListen);
            this.Controls.Add(this.tbMaxBoxes);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.butCancel);
            this.Controls.Add(this.butSave);
            this.Controls.Add(this.butLocation);
            this.Controls.Add(this.butDefaults);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.tbSaveLocation);
            this.Controls.Add(this.tbSaveInterval);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.ckAutoSave);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.tbDelay);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.tbRecordInterval);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.ckRecording);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmOptions";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Options";
            this.Load += new System.EventHandler(this.frmOptions_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox ckRecording;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tbRecordInterval;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tbDelay;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckBox ckAutoSave;
        private System.Windows.Forms.TextBox tbSaveInterval;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox tbSaveLocation;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button butDefaults;
        private System.Windows.Forms.Button butLocation;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.Button butCancel;
        private System.Windows.Forms.Button butSave;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox tbMaxBoxes;
        private System.Windows.Forms.CheckBox ckListen;
        private System.Windows.Forms.TextBox tbSleep;
        private System.Windows.Forms.Label label8;
    }
}