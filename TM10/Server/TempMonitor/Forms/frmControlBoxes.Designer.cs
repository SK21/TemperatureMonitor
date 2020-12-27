
namespace TempMonitor
{
    partial class frmControlBoxes
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmControlBoxes));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.btnNew = new System.Windows.Forms.Button();
            this.butDelete = new System.Windows.Forms.Button();
            this.btPrint = new System.Windows.Forms.Button();
            this.Edit = new System.Windows.Forms.GroupBox();
            this.ckSleep = new System.Windows.Forms.CheckBox();
            this.tbDescription = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tbNumber = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.DGV = new System.Windows.Forms.DataGridView();
            this.dataSet1 = new System.Data.DataSet();
            this.dataTable1 = new System.Data.DataTable();
            this.dataColumn1 = new System.Data.DataColumn();
            this.dataColumn2 = new System.Data.DataColumn();
            this.dataColumn3 = new System.Data.DataColumn();
            this.dataColumn4 = new System.Data.DataColumn();
            this.butSaveEdit = new System.Windows.Forms.Button();
            this.butCancelEdit = new System.Windows.Forms.Button();
            this.iDDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Number = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Sleep = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.Description = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Edit.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.DGV)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataSet1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataTable1)).BeginInit();
            this.SuspendLayout();
            // 
            // btnNew
            // 
            this.btnNew.Image = ((System.Drawing.Image)(resources.GetObject("btnNew.Image")));
            this.btnNew.Location = new System.Drawing.Point(337, 297);
            this.btnNew.Name = "btnNew";
            this.btnNew.Size = new System.Drawing.Size(88, 61);
            this.btnNew.TabIndex = 52;
            this.btnNew.Text = "New";
            this.btnNew.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.btnNew.UseVisualStyleBackColor = true;
            this.btnNew.Click += new System.EventHandler(this.btnNew_Click);
            // 
            // butDelete
            // 
            this.butDelete.Image = ((System.Drawing.Image)(resources.GetObject("butDelete.Image")));
            this.butDelete.Location = new System.Drawing.Point(337, 198);
            this.butDelete.Name = "butDelete";
            this.butDelete.Size = new System.Drawing.Size(88, 61);
            this.butDelete.TabIndex = 51;
            this.butDelete.Text = "Delete";
            this.butDelete.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.butDelete.UseVisualStyleBackColor = true;
            this.butDelete.Click += new System.EventHandler(this.butDelete_Click);
            // 
            // btPrint
            // 
            this.btPrint.Image = ((System.Drawing.Image)(resources.GetObject("btPrint.Image")));
            this.btPrint.Location = new System.Drawing.Point(337, 99);
            this.btPrint.Name = "btPrint";
            this.btPrint.Size = new System.Drawing.Size(88, 61);
            this.btPrint.TabIndex = 50;
            this.btPrint.Text = "Print";
            this.btPrint.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.btPrint.UseVisualStyleBackColor = true;
            this.btPrint.Click += new System.EventHandler(this.btPrint_Click);
            // 
            // Edit
            // 
            this.Edit.Controls.Add(this.ckSleep);
            this.Edit.Controls.Add(this.tbDescription);
            this.Edit.Controls.Add(this.label1);
            this.Edit.Controls.Add(this.tbNumber);
            this.Edit.Controls.Add(this.label4);
            this.Edit.Location = new System.Drawing.Point(12, 12);
            this.Edit.Name = "Edit";
            this.Edit.Size = new System.Drawing.Size(413, 77);
            this.Edit.TabIndex = 49;
            this.Edit.TabStop = false;
            this.Edit.Text = "Edit";
            // 
            // ckSleep
            // 
            this.ckSleep.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.ckSleep.Location = new System.Drawing.Point(179, 15);
            this.ckSleep.Name = "ckSleep";
            this.ckSleep.Size = new System.Drawing.Size(78, 24);
            this.ckSleep.TabIndex = 1;
            this.ckSleep.Text = "Use Sleep";
            this.ckSleep.UseVisualStyleBackColor = true;
            this.ckSleep.CheckedChanged += new System.EventHandler(this.ckSleep_CheckedChanged);
            // 
            // tbDescription
            // 
            this.tbDescription.Location = new System.Drawing.Point(70, 47);
            this.tbDescription.Name = "tbDescription";
            this.tbDescription.Size = new System.Drawing.Size(187, 20);
            this.tbDescription.TabIndex = 2;
            this.tbDescription.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tbDescription.TextChanged += new System.EventHandler(this.tbDescription_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(4, 50);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(60, 13);
            this.label1.TabIndex = 35;
            this.label1.Text = "Description";
            // 
            // tbNumber
            // 
            this.tbNumber.Location = new System.Drawing.Point(70, 19);
            this.tbNumber.Name = "tbNumber";
            this.tbNumber.Size = new System.Drawing.Size(90, 20);
            this.tbNumber.TabIndex = 0;
            this.tbNumber.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tbNumber.TextChanged += new System.EventHandler(this.tbNumber_TextChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(4, 22);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(44, 13);
            this.label4.TabIndex = 33;
            this.label4.Text = "Number";
            // 
            // DGV
            // 
            this.DGV.AllowUserToAddRows = false;
            this.DGV.AllowUserToDeleteRows = false;
            this.DGV.AllowUserToResizeColumns = false;
            this.DGV.AllowUserToResizeRows = false;
            this.DGV.AutoGenerateColumns = false;
            this.DGV.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.DGV.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.iDDataGridViewTextBoxColumn,
            this.Number,
            this.Sleep,
            this.Description});
            this.DGV.DataMember = "Table1";
            this.DGV.DataSource = this.dataSet1;
            this.DGV.Location = new System.Drawing.Point(12, 99);
            this.DGV.MultiSelect = false;
            this.DGV.Name = "DGV";
            this.DGV.ReadOnly = true;
            this.DGV.RowHeadersVisible = false;
            this.DGV.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.DGV.Size = new System.Drawing.Size(319, 460);
            this.DGV.TabIndex = 48;
            this.DGV.CellEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.DGV_CellEnter);
            // 
            // dataSet1
            // 
            this.dataSet1.DataSetName = "NewDataSet";
            this.dataSet1.Tables.AddRange(new System.Data.DataTable[] {
            this.dataTable1});
            // 
            // dataTable1
            // 
            this.dataTable1.Columns.AddRange(new System.Data.DataColumn[] {
            this.dataColumn1,
            this.dataColumn2,
            this.dataColumn3,
            this.dataColumn4});
            this.dataTable1.TableName = "Table1";
            // 
            // dataColumn1
            // 
            this.dataColumn1.ColumnName = "ID";
            this.dataColumn1.DataType = typeof(short);
            // 
            // dataColumn2
            // 
            this.dataColumn2.ColumnName = "Number";
            // 
            // dataColumn3
            // 
            this.dataColumn3.ColumnName = "Sleep";
            // 
            // dataColumn4
            // 
            this.dataColumn4.ColumnName = "Description";
            // 
            // butSaveEdit
            // 
            this.butSaveEdit.Image = ((System.Drawing.Image)(resources.GetObject("butSaveEdit.Image")));
            this.butSaveEdit.Location = new System.Drawing.Point(337, 498);
            this.butSaveEdit.Name = "butSaveEdit";
            this.butSaveEdit.Size = new System.Drawing.Size(88, 61);
            this.butSaveEdit.TabIndex = 0;
            this.butSaveEdit.Text = "Close";
            this.butSaveEdit.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.butSaveEdit.UseVisualStyleBackColor = true;
            this.butSaveEdit.Click += new System.EventHandler(this.butSaveEdit_Click);
            // 
            // butCancelEdit
            // 
            this.butCancelEdit.Enabled = false;
            this.butCancelEdit.Image = ((System.Drawing.Image)(resources.GetObject("butCancelEdit.Image")));
            this.butCancelEdit.Location = new System.Drawing.Point(337, 396);
            this.butCancelEdit.Name = "butCancelEdit";
            this.butCancelEdit.Size = new System.Drawing.Size(88, 61);
            this.butCancelEdit.TabIndex = 46;
            this.butCancelEdit.Text = "Cancel";
            this.butCancelEdit.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.butCancelEdit.UseVisualStyleBackColor = true;
            this.butCancelEdit.Click += new System.EventHandler(this.butCancelEdit_Click);
            // 
            // iDDataGridViewTextBoxColumn
            // 
            this.iDDataGridViewTextBoxColumn.DataPropertyName = "ID";
            this.iDDataGridViewTextBoxColumn.HeaderText = "ID";
            this.iDDataGridViewTextBoxColumn.Name = "iDDataGridViewTextBoxColumn";
            this.iDDataGridViewTextBoxColumn.ReadOnly = true;
            this.iDDataGridViewTextBoxColumn.Visible = false;
            // 
            // Number
            // 
            this.Number.DataPropertyName = "Number";
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.Number.DefaultCellStyle = dataGridViewCellStyle1;
            this.Number.HeaderText = "Number";
            this.Number.Name = "Number";
            this.Number.ReadOnly = true;
            // 
            // Sleep
            // 
            this.Sleep.DataPropertyName = "Sleep";
            this.Sleep.HeaderText = "Use Sleep";
            this.Sleep.Name = "Sleep";
            this.Sleep.ReadOnly = true;
            // 
            // Description
            // 
            this.Description.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Description.DataPropertyName = "Description";
            this.Description.HeaderText = "Description";
            this.Description.Name = "Description";
            this.Description.ReadOnly = true;
            // 
            // frmControlBoxes
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(437, 573);
            this.Controls.Add(this.btnNew);
            this.Controls.Add(this.butDelete);
            this.Controls.Add(this.btPrint);
            this.Controls.Add(this.Edit);
            this.Controls.Add(this.DGV);
            this.Controls.Add(this.butSaveEdit);
            this.Controls.Add(this.butCancelEdit);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmControlBoxes";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Controlboxes";
            this.Load += new System.EventHandler(this.frmControlBoxes_Load);
            this.Edit.ResumeLayout(false);
            this.Edit.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.DGV)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataSet1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataTable1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnNew;
        private System.Windows.Forms.Button butDelete;
        private System.Windows.Forms.Button btPrint;
        private System.Windows.Forms.GroupBox Edit;
        private System.Windows.Forms.TextBox tbDescription;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbNumber;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.DataGridView DGV;
        private System.Windows.Forms.Button butSaveEdit;
        private System.Windows.Forms.Button butCancelEdit;
        private System.Data.DataSet dataSet1;
        private System.Data.DataTable dataTable1;
        private System.Data.DataColumn dataColumn1;
        private System.Data.DataColumn dataColumn2;
        private System.Data.DataColumn dataColumn3;
        private System.Windows.Forms.CheckBox ckSleep;
        private System.Data.DataColumn dataColumn4;
        private System.Windows.Forms.DataGridViewTextBoxColumn iDDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn Number;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Sleep;
        private System.Windows.Forms.DataGridViewTextBoxColumn Description;
    }
}