namespace BinWatch
{
    partial class ModuleEditForm
    {
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.Label lblMac;
        private System.Windows.Forms.Label lblMacValue;
        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.Label lblId;
        private System.Windows.Forms.NumericUpDown nudId;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnCancel;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components  = new System.ComponentModel.Container();
            this.lblMac      = new System.Windows.Forms.Label();
            this.lblMacValue = new System.Windows.Forms.Label();
            this.lblName     = new System.Windows.Forms.Label();
            this.txtName     = new System.Windows.Forms.TextBox();
            this.lblId       = new System.Windows.Forms.Label();
            this.nudId       = new System.Windows.Forms.NumericUpDown();
            this.btnSave     = new System.Windows.Forms.Button();
            this.btnCancel   = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)this.nudId).BeginInit();
            this.SuspendLayout();

            // col1=12  col2=170  row0=14  row1=47  row2=80  row3=122

            // lblMac
            this.lblMac.AutoSize = true;
            this.lblMac.Location = new System.Drawing.Point(12, 17);
            this.lblMac.Name     = "lblMac";
            this.lblMac.Text     = "MAC:";

            // lblMacValue
            this.lblMacValue.AutoSize = true;
            this.lblMacValue.Location = new System.Drawing.Point(170, 17);
            this.lblMacValue.Name     = "lblMacValue";
            this.lblMacValue.Font     = new System.Drawing.Font("Consolas", 11F);

            // lblName
            this.lblName.AutoSize = true;
            this.lblName.Location = new System.Drawing.Point(12, 50);
            this.lblName.Name     = "lblName";
            this.lblName.Text     = "Name (max 10 chars):";

            // txtName
            this.txtName.Location  = new System.Drawing.Point(170, 47);
            this.txtName.MaxLength = 10;
            this.txtName.Name      = "txtName";
            this.txtName.Size      = new System.Drawing.Size(150, 23);

            // lblId
            this.lblId.AutoSize = true;
            this.lblId.Location = new System.Drawing.Point(12, 83);
            this.lblId.Name     = "lblId";
            this.lblId.Text     = "ID (0 = unregistered):";

            // nudId
            this.nudId.Location = new System.Drawing.Point(170, 80);
            this.nudId.Maximum  = 255;
            this.nudId.Minimum  = 0;
            this.nudId.Name     = "nudId";
            this.nudId.Size     = new System.Drawing.Size(60, 23);

            // btnSave
            this.btnSave.Location = new System.Drawing.Point(170, 122);
            this.btnSave.Name     = "btnSave";
            this.btnSave.Size     = new System.Drawing.Size(90, 28);
            this.btnSave.Text     = "Save";
            this.btnSave.Click   += new System.EventHandler(this.btnSave_Click);

            // btnCancel
            this.btnCancel.Location     = new System.Drawing.Point(270, 122);
            this.btnCancel.Name         = "btnCancel";
            this.btnCancel.Size         = new System.Drawing.Size(90, 28);
            this.btnCancel.Text         = "Cancel";
            this.btnCancel.DialogResult  = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Click        += new System.EventHandler(this.btnCancel_Click);

            // ModuleEditForm
            this.AcceptButton        = this.btnSave;
            this.CancelButton        = this.btnCancel;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode       = System.Windows.Forms.AutoScaleMode.Font;
            this.Font                = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ClientSize          = new System.Drawing.Size(380, 168);
            this.Controls.AddRange(new System.Windows.Forms.Control[] {
                lblMac, lblMacValue, lblName, txtName, lblId, nudId, btnSave, btnCancel });
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox     = false;
            this.MinimizeBox     = false;
            this.Name            = "ModuleEditForm";
            this.StartPosition   = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text            = "Edit Module";
            this.ShowInTaskbar   = false;

            ((System.ComponentModel.ISupportInitialize)this.nudId).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}
