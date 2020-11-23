namespace TempMonitor
{
    partial class frmAbout
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmAbout));
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.lbVersion = new System.Windows.Forms.Label();
            this.lbDate = new System.Windows.Forms.Label();
            this.lbDBver = new System.Windows.Forms.Label();
            this.butSave = new System.Windows.Forms.Button();
            this.lbFolder = new System.Windows.Forms.Label();
            this.lbFileDate = new System.Windows.Forms.Label();
            this.lbSize = new System.Windows.Forms.Label();
            this.lbName = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(13, 13);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(128, 128);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(147, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(224, 25);
            this.label1.TabIndex = 1;
            this.label1.Text = "TemperatureMonitor";
            // 
            // lbVersion
            // 
            this.lbVersion.Location = new System.Drawing.Point(147, 38);
            this.lbVersion.Name = "lbVersion";
            this.lbVersion.Size = new System.Drawing.Size(222, 23);
            this.lbVersion.TabIndex = 2;
            this.lbVersion.Text = "Version";
            this.lbVersion.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbDate
            // 
            this.lbDate.Location = new System.Drawing.Point(147, 90);
            this.lbDate.Name = "lbDate";
            this.lbDate.Size = new System.Drawing.Size(222, 23);
            this.lbDate.TabIndex = 5;
            this.lbDate.Text = "Date";
            this.lbDate.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbDBver
            // 
            this.lbDBver.Location = new System.Drawing.Point(147, 64);
            this.lbDBver.Name = "lbDBver";
            this.lbDBver.Size = new System.Drawing.Size(222, 23);
            this.lbDBver.TabIndex = 7;
            this.lbDBver.Text = "DB version";
            this.lbDBver.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // butSave
            // 
            this.butSave.Image = ((System.Drawing.Image)(resources.GetObject("butSave.Image")));
            this.butSave.Location = new System.Drawing.Point(281, 279);
            this.butSave.Name = "butSave";
            this.butSave.Size = new System.Drawing.Size(88, 61);
            this.butSave.TabIndex = 22;
            this.butSave.Text = "OK";
            this.butSave.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.butSave.UseVisualStyleBackColor = true;
            this.butSave.Click += new System.EventHandler(this.butSave_Click);
            // 
            // lbFolder
            // 
            this.lbFolder.BackColor = System.Drawing.SystemColors.Info;
            this.lbFolder.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbFolder.Location = new System.Drawing.Point(12, 290);
            this.lbFolder.Name = "lbFolder";
            this.lbFolder.Size = new System.Drawing.Size(260, 50);
            this.lbFolder.TabIndex = 30;
            // 
            // lbFileDate
            // 
            this.lbFileDate.BackColor = System.Drawing.SystemColors.Info;
            this.lbFileDate.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbFileDate.Location = new System.Drawing.Point(123, 246);
            this.lbFileDate.Name = "lbFileDate";
            this.lbFileDate.Size = new System.Drawing.Size(149, 16);
            this.lbFileDate.TabIndex = 29;
            // 
            // lbSize
            // 
            this.lbSize.BackColor = System.Drawing.SystemColors.Info;
            this.lbSize.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbSize.Location = new System.Drawing.Point(123, 220);
            this.lbSize.Name = "lbSize";
            this.lbSize.Size = new System.Drawing.Size(149, 16);
            this.lbSize.TabIndex = 28;
            // 
            // lbName
            // 
            this.lbName.BackColor = System.Drawing.SystemColors.Info;
            this.lbName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbName.Location = new System.Drawing.Point(123, 194);
            this.lbName.Name = "lbName";
            this.lbName.Size = new System.Drawing.Size(149, 16);
            this.lbName.TabIndex = 27;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(14, 271);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(91, 13);
            this.label4.TabIndex = 26;
            this.label4.Text = "Database Folder :";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(14, 248);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(79, 13);
            this.label3.TabIndex = 25;
            this.label3.Text = "Database Date";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(14, 222);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(99, 13);
            this.label5.TabIndex = 24;
            this.label5.Text = "Database Size (KB)";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(14, 196);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(84, 13);
            this.label6.TabIndex = 23;
            this.label6.Text = "Database Name";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(12, 169);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(87, 16);
            this.label7.TabIndex = 31;
            this.label7.Text = "Current File";
            // 
            // frmAbout
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(377, 349);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.lbFolder);
            this.Controls.Add(this.lbFileDate);
            this.Controls.Add(this.lbSize);
            this.Controls.Add(this.lbName);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.butSave);
            this.Controls.Add(this.lbDBver);
            this.Controls.Add(this.lbDate);
            this.Controls.Add(this.lbVersion);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pictureBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmAbout";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "About";
            this.Load += new System.EventHandler(this.frmAbout_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lbVersion;
        private System.Windows.Forms.Label lbDate;
        private System.Windows.Forms.Label lbDBver;
        private System.Windows.Forms.Button butSave;
        private System.Windows.Forms.Label lbFolder;
        private System.Windows.Forms.Label lbFileDate;
        private System.Windows.Forms.Label lbSize;
        private System.Windows.Forms.Label lbName;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
    }
}