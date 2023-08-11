namespace TempMonitor.Forms
{
    partial class frmNewFile
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmNewFile));
            this.butCancel = new System.Windows.Forms.Button();
            this.tbNewFileName = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.ckBins = new System.Windows.Forms.CheckBox();
            this.ckSensors = new System.Windows.Forms.CheckBox();
            this.ckRecords = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.butSave = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // butCancel
            // 
            this.butCancel.Image = ((System.Drawing.Image)(resources.GetObject("butCancel.Image")));
            this.butCancel.Location = new System.Drawing.Point(8, 148);
            this.butCancel.Name = "butCancel";
            this.butCancel.Size = new System.Drawing.Size(88, 61);
            this.butCancel.TabIndex = 14;
            this.butCancel.Text = "Cancel";
            this.butCancel.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.butCancel.UseVisualStyleBackColor = true;
            this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
            // 
            // tbNewFileName
            // 
            this.tbNewFileName.Location = new System.Drawing.Point(90, 12);
            this.tbNewFileName.Name = "tbNewFileName";
            this.tbNewFileName.Size = new System.Drawing.Size(117, 20);
            this.tbNewFileName.TabIndex = 0;
            this.tbNewFileName.Tag = "0";
            this.tbNewFileName.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tbNewFileName.TextChanged += new System.EventHandler(this.tbNewFileName_TextChanged);
            this.tbNewFileName.Validating += new System.ComponentModel.CancelEventHandler(this.tbNewFileName_Validating);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(5, 15);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(79, 13);
            this.label2.TabIndex = 15;
            this.label2.Text = "New File Name";
            this.label2.Click += new System.EventHandler(this.label2_Click);
            // 
            // ckBins
            // 
            this.ckBins.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.ckBins.Location = new System.Drawing.Point(8, 73);
            this.ckBins.Name = "ckBins";
            this.ckBins.Size = new System.Drawing.Size(199, 19);
            this.ckBins.TabIndex = 1;
            this.ckBins.Text = "Bins";
            this.ckBins.UseVisualStyleBackColor = true;
            this.ckBins.CheckedChanged += new System.EventHandler(this.ckBins_CheckedChanged);
            // 
            // ckSensors
            // 
            this.ckSensors.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.ckSensors.Location = new System.Drawing.Point(8, 98);
            this.ckSensors.Name = "ckSensors";
            this.ckSensors.Size = new System.Drawing.Size(199, 19);
            this.ckSensors.TabIndex = 2;
            this.ckSensors.Text = "Sensors";
            this.ckSensors.UseVisualStyleBackColor = true;
            this.ckSensors.CheckedChanged += new System.EventHandler(this.ckSensors_CheckedChanged);
            // 
            // ckRecords
            // 
            this.ckRecords.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.ckRecords.Location = new System.Drawing.Point(8, 123);
            this.ckRecords.Name = "ckRecords";
            this.ckRecords.Size = new System.Drawing.Size(199, 19);
            this.ckRecords.TabIndex = 3;
            this.ckRecords.Text = "Temperature Records";
            this.ckRecords.UseVisualStyleBackColor = true;
            this.ckRecords.CheckedChanged += new System.EventHandler(this.ckRecords_CheckedChanged);
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(12, 51);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(195, 13);
            this.label1.TabIndex = 20;
            this.label1.Text = "Copy From Current Database:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // butSave
            // 
            this.butSave.Image = ((System.Drawing.Image)(resources.GetObject("butSave.Image")));
            this.butSave.Location = new System.Drawing.Point(119, 149);
            this.butSave.Name = "butSave";
            this.butSave.Size = new System.Drawing.Size(88, 61);
            this.butSave.TabIndex = 21;
            this.butSave.Text = "OK";
            this.butSave.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.butSave.UseVisualStyleBackColor = true;
            this.butSave.Click += new System.EventHandler(this.butSave_Click);
            // 
            // frmNewFile
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(220, 222);
            this.Controls.Add(this.butSave);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.ckRecords);
            this.Controls.Add(this.ckSensors);
            this.Controls.Add(this.ckBins);
            this.Controls.Add(this.tbNewFileName);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.butCancel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmNewFile";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "New File";
            this.Load += new System.EventHandler(this.frmNewFile_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button butCancel;
        private System.Windows.Forms.TextBox tbNewFileName;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox ckBins;
        private System.Windows.Forms.CheckBox ckSensors;
        private System.Windows.Forms.CheckBox ckRecords;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button butSave;
    }
}