namespace TempMonitor.Forms
{
    partial class frmSensors
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmSensors));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle31 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle40 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle32 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle33 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle34 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle35 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle36 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle37 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle38 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle39 = new System.Windows.Forms.DataGridViewCellStyle();
            this.tbEvents = new System.Windows.Forms.TextBox();
            this.btCheck = new System.Windows.Forms.Button();
            this.btWrite = new System.Windows.Forms.Button();
            this.btReload = new System.Windows.Forms.Button();
            this.btPrint = new System.Windows.Forms.Button();
            this.DGV = new System.Windows.Forms.DataGridView();
            this.ID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Bin = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.BinNum = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Cable = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Sensor = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SensorEnabled = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.Offset = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.LastTemp = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.LastTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Address = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataSet1 = new System.Data.DataSet();
            this.dataTable1 = new System.Data.DataTable();
            this.dataColumn1 = new System.Data.DataColumn();
            this.dataColumn2 = new System.Data.DataColumn();
            this.dataColumn3 = new System.Data.DataColumn();
            this.dataColumn4 = new System.Data.DataColumn();
            this.dataColumn5 = new System.Data.DataColumn();
            this.dataColumn6 = new System.Data.DataColumn();
            this.dataColumn7 = new System.Data.DataColumn();
            this.dataColumn8 = new System.Data.DataColumn();
            this.dataColumn9 = new System.Data.DataColumn();
            this.dataColumn12 = new System.Data.DataColumn();
            this.label1 = new System.Windows.Forms.Label();
            this.lbSelected = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.tbCable = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.tbSensor = new System.Windows.Forms.TextBox();
            this.ckEnabled = new System.Windows.Forms.CheckBox();
            this.label6 = new System.Windows.Forms.Label();
            this.tbOffset = new System.Windows.Forms.TextBox();
            this.butCancelEdit = new System.Windows.Forms.Button();
            this.butSaveEdit = new System.Windows.Forms.Button();
            this.butDelete = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.tbBin = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.DGV)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataSet1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataTable1)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tbEvents
            // 
            this.tbEvents.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.tbEvents.Location = new System.Drawing.Point(663, 116);
            this.tbEvents.Multiline = true;
            this.tbEvents.Name = "tbEvents";
            this.tbEvents.ReadOnly = true;
            this.tbEvents.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tbEvents.Size = new System.Drawing.Size(476, 458);
            this.tbEvents.TabIndex = 20;
            // 
            // btCheck
            // 
            this.btCheck.Image = ((System.Drawing.Image)(resources.GetObject("btCheck.Image")));
            this.btCheck.Location = new System.Drawing.Point(1152, 96);
            this.btCheck.Name = "btCheck";
            this.btCheck.Size = new System.Drawing.Size(88, 61);
            this.btCheck.TabIndex = 21;
            this.btCheck.Text = "Check";
            this.btCheck.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.btCheck.UseVisualStyleBackColor = true;
            this.btCheck.Click += new System.EventHandler(this.btCheck_Click);
            // 
            // btWrite
            // 
            this.btWrite.Image = ((System.Drawing.Image)(resources.GetObject("btWrite.Image")));
            this.btWrite.Location = new System.Drawing.Point(1152, 179);
            this.btWrite.Name = "btWrite";
            this.btWrite.Size = new System.Drawing.Size(88, 61);
            this.btWrite.TabIndex = 22;
            this.btWrite.Text = "Write";
            this.btWrite.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.btWrite.UseVisualStyleBackColor = true;
            this.btWrite.Click += new System.EventHandler(this.btWrite_Click);
            // 
            // btReload
            // 
            this.btReload.Image = ((System.Drawing.Image)(resources.GetObject("btReload.Image")));
            this.btReload.Location = new System.Drawing.Point(1152, 13);
            this.btReload.Name = "btReload";
            this.btReload.Size = new System.Drawing.Size(88, 61);
            this.btReload.TabIndex = 23;
            this.btReload.Text = "Reload";
            this.btReload.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.btReload.UseVisualStyleBackColor = true;
            this.btReload.Click += new System.EventHandler(this.btReload_Click);
            // 
            // btPrint
            // 
            this.btPrint.Image = ((System.Drawing.Image)(resources.GetObject("btPrint.Image")));
            this.btPrint.Location = new System.Drawing.Point(1152, 345);
            this.btPrint.Name = "btPrint";
            this.btPrint.Size = new System.Drawing.Size(88, 61);
            this.btPrint.TabIndex = 24;
            this.btPrint.Text = "Print";
            this.btPrint.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.btPrint.UseVisualStyleBackColor = true;
            this.btPrint.Click += new System.EventHandler(this.btPrint_Click);
            // 
            // DGV
            // 
            this.DGV.AllowUserToAddRows = false;
            this.DGV.AllowUserToDeleteRows = false;
            this.DGV.AllowUserToResizeColumns = false;
            this.DGV.AllowUserToResizeRows = false;
            this.DGV.AutoGenerateColumns = false;
            dataGridViewCellStyle31.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle31.BackColor = System.Drawing.SystemColors.ActiveCaption;
            dataGridViewCellStyle31.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle31.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle31.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle31.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle31.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.DGV.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle31;
            this.DGV.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.DGV.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ID,
            this.Bin,
            this.BinNum,
            this.Cable,
            this.Sensor,
            this.SensorEnabled,
            this.Offset,
            this.LastTemp,
            this.LastTime,
            this.Address});
            this.DGV.DataMember = "Table1";
            this.DGV.DataSource = this.dataSet1;
            this.DGV.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.DGV.Location = new System.Drawing.Point(13, 12);
            this.DGV.MultiSelect = false;
            this.DGV.Name = "DGV";
            dataGridViewCellStyle40.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle40.BackColor = System.Drawing.SystemColors.ControlLight;
            dataGridViewCellStyle40.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle40.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle40.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle40.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle40.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.DGV.RowHeadersDefaultCellStyle = dataGridViewCellStyle40;
            this.DGV.RowHeadersVisible = false;
            this.DGV.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.DGV.Size = new System.Drawing.Size(641, 561);
            this.DGV.TabIndex = 1;
            this.DGV.CellEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.DGV_CellEnter);
            // 
            // ID
            // 
            this.ID.DataPropertyName = "ID";
            this.ID.HeaderText = "ID";
            this.ID.Name = "ID";
            this.ID.ReadOnly = true;
            this.ID.Visible = false;
            // 
            // Bin
            // 
            this.Bin.DataPropertyName = "Bin #";
            dataGridViewCellStyle32.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.Bin.DefaultCellStyle = dataGridViewCellStyle32;
            this.Bin.HeaderText = "Bin ";
            this.Bin.Name = "Bin";
            this.Bin.Visible = false;
            this.Bin.Width = 120;
            // 
            // BinNum
            // 
            this.BinNum.DataPropertyName = "BinNum";
            dataGridViewCellStyle33.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.BinNum.DefaultCellStyle = dataGridViewCellStyle33;
            this.BinNum.HeaderText = "BinNum";
            this.BinNum.Name = "BinNum";
            this.BinNum.Width = 80;
            // 
            // Cable
            // 
            this.Cable.DataPropertyName = "Cable #";
            dataGridViewCellStyle34.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.Cable.DefaultCellStyle = dataGridViewCellStyle34;
            this.Cable.HeaderText = "Cable #";
            this.Cable.Name = "Cable";
            this.Cable.Width = 50;
            // 
            // Sensor
            // 
            this.Sensor.DataPropertyName = "Sensor #";
            dataGridViewCellStyle35.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.Sensor.DefaultCellStyle = dataGridViewCellStyle35;
            this.Sensor.HeaderText = "Sensor #";
            this.Sensor.Name = "Sensor";
            this.Sensor.Width = 50;
            // 
            // SensorEnabled
            // 
            this.SensorEnabled.DataPropertyName = "Enabled";
            this.SensorEnabled.FalseValue = "False";
            this.SensorEnabled.HeaderText = "Enabled";
            this.SensorEnabled.Name = "SensorEnabled";
            this.SensorEnabled.TrueValue = "True";
            this.SensorEnabled.Width = 50;
            // 
            // Offset
            // 
            this.Offset.DataPropertyName = "Offset";
            dataGridViewCellStyle36.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle36.Format = "N1";
            dataGridViewCellStyle36.NullValue = null;
            this.Offset.DefaultCellStyle = dataGridViewCellStyle36;
            this.Offset.HeaderText = "Offset";
            this.Offset.Name = "Offset";
            this.Offset.Width = 50;
            // 
            // LastTemp
            // 
            this.LastTemp.DataPropertyName = "Last Temp";
            dataGridViewCellStyle37.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle37.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle37.Format = "N1";
            dataGridViewCellStyle37.NullValue = null;
            this.LastTemp.DefaultCellStyle = dataGridViewCellStyle37;
            this.LastTemp.HeaderText = "Last Temp";
            this.LastTemp.Name = "LastTemp";
            this.LastTemp.ReadOnly = true;
            this.LastTemp.Width = 50;
            // 
            // LastTime
            // 
            this.LastTime.DataPropertyName = "Last Time";
            dataGridViewCellStyle38.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle38.BackColor = System.Drawing.Color.White;
            this.LastTime.DefaultCellStyle = dataGridViewCellStyle38;
            this.LastTime.HeaderText = "Last Time";
            this.LastTime.Name = "LastTime";
            this.LastTime.ReadOnly = true;
            this.LastTime.Width = 120;
            // 
            // Address
            // 
            this.Address.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Address.DataPropertyName = "Address";
            dataGridViewCellStyle39.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle39.BackColor = System.Drawing.Color.White;
            this.Address.DefaultCellStyle = dataGridViewCellStyle39;
            this.Address.HeaderText = "Address";
            this.Address.Name = "Address";
            this.Address.ReadOnly = true;
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
            this.dataColumn4,
            this.dataColumn5,
            this.dataColumn6,
            this.dataColumn7,
            this.dataColumn8,
            this.dataColumn9,
            this.dataColumn12});
            this.dataTable1.TableName = "Table1";
            // 
            // dataColumn1
            // 
            this.dataColumn1.ColumnName = "ID";
            this.dataColumn1.DataType = typeof(short);
            this.dataColumn1.ReadOnly = true;
            // 
            // dataColumn2
            // 
            this.dataColumn2.ColumnName = "Bin #";
            this.dataColumn2.DataType = typeof(short);
            // 
            // dataColumn3
            // 
            this.dataColumn3.ColumnName = "Cable #";
            this.dataColumn3.DataType = typeof(short);
            // 
            // dataColumn4
            // 
            this.dataColumn4.ColumnName = "Sensor #";
            this.dataColumn4.DataType = typeof(short);
            // 
            // dataColumn5
            // 
            this.dataColumn5.ColumnName = "Enabled";
            this.dataColumn5.DataType = typeof(bool);
            // 
            // dataColumn6
            // 
            this.dataColumn6.ColumnName = "Offset";
            this.dataColumn6.DataType = typeof(double);
            // 
            // dataColumn7
            // 
            this.dataColumn7.ColumnName = "Last Temp";
            this.dataColumn7.DataType = typeof(double);
            // 
            // dataColumn8
            // 
            this.dataColumn8.ColumnName = "Last Time";
            this.dataColumn8.DataType = typeof(System.DateTime);
            // 
            // dataColumn9
            // 
            this.dataColumn9.ColumnName = "Address";
            // 
            // dataColumn12
            // 
            this.dataColumn12.Caption = "Bin";
            this.dataColumn12.ColumnName = "BinNum";
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(663, 90);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(476, 23);
            this.label1.TabIndex = 26;
            this.label1.Text = "Read/Write Sensors";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbSelected
            // 
            this.lbSelected.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbSelected.Location = new System.Drawing.Point(24, 16);
            this.lbSelected.Name = "lbSelected";
            this.lbSelected.Size = new System.Drawing.Size(439, 23);
            this.lbSelected.TabIndex = 27;
            this.lbSelected.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(17, 51);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(22, 13);
            this.label3.TabIndex = 29;
            this.label3.Text = "Bin";
            this.label3.Click += new System.EventHandler(this.label3_Click);
            // 
            // tbCable
            // 
            this.tbCable.Location = new System.Drawing.Point(145, 44);
            this.tbCable.Name = "tbCable";
            this.tbCable.Size = new System.Drawing.Size(44, 20);
            this.tbCable.TabIndex = 1;
            this.tbCable.Text = "99";
            this.tbCable.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tbCable.TextChanged += new System.EventHandler(this.tbCable_TextChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(107, 51);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(34, 13);
            this.label4.TabIndex = 31;
            this.label4.Text = "Cable";
            this.label4.Click += new System.EventHandler(this.label4_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(205, 51);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(40, 13);
            this.label5.TabIndex = 33;
            this.label5.Text = "Sensor";
            // 
            // tbSensor
            // 
            this.tbSensor.BackColor = System.Drawing.Color.White;
            this.tbSensor.Location = new System.Drawing.Point(249, 44);
            this.tbSensor.Name = "tbSensor";
            this.tbSensor.Size = new System.Drawing.Size(44, 20);
            this.tbSensor.TabIndex = 2;
            this.tbSensor.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tbSensor.TextChanged += new System.EventHandler(this.tbSensor_TextChanged);
            // 
            // ckEnabled
            // 
            this.ckEnabled.AutoSize = true;
            this.ckEnabled.Location = new System.Drawing.Point(398, 47);
            this.ckEnabled.Name = "ckEnabled";
            this.ckEnabled.Size = new System.Drawing.Size(65, 17);
            this.ckEnabled.TabIndex = 34;
            this.ckEnabled.Text = "Enabled";
            this.ckEnabled.UseVisualStyleBackColor = true;
            this.ckEnabled.CheckedChanged += new System.EventHandler(this.ckEnabled_CheckedChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(305, 51);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(35, 13);
            this.label6.TabIndex = 36;
            this.label6.Text = "Offset";
            // 
            // tbOffset
            // 
            this.tbOffset.Location = new System.Drawing.Point(344, 44);
            this.tbOffset.Name = "tbOffset";
            this.tbOffset.Size = new System.Drawing.Size(44, 20);
            this.tbOffset.TabIndex = 3;
            this.tbOffset.Text = "100.0";
            this.tbOffset.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tbOffset.TextChanged += new System.EventHandler(this.tbOffset_TextChanged);
            // 
            // butCancelEdit
            // 
            this.butCancelEdit.Enabled = false;
            this.butCancelEdit.Image = ((System.Drawing.Image)(resources.GetObject("butCancelEdit.Image")));
            this.butCancelEdit.Location = new System.Drawing.Point(1152, 428);
            this.butCancelEdit.Name = "butCancelEdit";
            this.butCancelEdit.Size = new System.Drawing.Size(88, 61);
            this.butCancelEdit.TabIndex = 37;
            this.butCancelEdit.Text = "Cancel";
            this.butCancelEdit.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.butCancelEdit.UseVisualStyleBackColor = true;
            this.butCancelEdit.Click += new System.EventHandler(this.butCancelEdit_Click);
            // 
            // butSaveEdit
            // 
            this.butSaveEdit.Image = ((System.Drawing.Image)(resources.GetObject("butSaveEdit.Image")));
            this.butSaveEdit.Location = new System.Drawing.Point(1152, 513);
            this.butSaveEdit.Name = "butSaveEdit";
            this.butSaveEdit.Size = new System.Drawing.Size(88, 61);
            this.butSaveEdit.TabIndex = 0;
            this.butSaveEdit.Text = "Close";
            this.butSaveEdit.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.butSaveEdit.UseVisualStyleBackColor = true;
            this.butSaveEdit.Click += new System.EventHandler(this.butSaveEdit_Click);
            // 
            // butDelete
            // 
            this.butDelete.Image = ((System.Drawing.Image)(resources.GetObject("butDelete.Image")));
            this.butDelete.Location = new System.Drawing.Point(1152, 262);
            this.butDelete.Name = "butDelete";
            this.butDelete.Size = new System.Drawing.Size(88, 61);
            this.butDelete.TabIndex = 39;
            this.butDelete.Text = "Delete";
            this.butDelete.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.butDelete.UseVisualStyleBackColor = true;
            this.butDelete.Click += new System.EventHandler(this.butDelete_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.tbBin);
            this.groupBox1.Controls.Add(this.lbSelected);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.tbCable);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.tbSensor);
            this.groupBox1.Controls.Add(this.tbOffset);
            this.groupBox1.Controls.Add(this.ckEnabled);
            this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(663, 13);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(476, 74);
            this.groupBox1.TabIndex = 41;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Edit";
            // 
            // timer1
            // 
            this.timer1.Interval = 5000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // tbBin
            // 
            this.tbBin.Location = new System.Drawing.Point(45, 44);
            this.tbBin.Name = "tbBin";
            this.tbBin.Size = new System.Drawing.Size(44, 20);
            this.tbBin.TabIndex = 0;
            this.tbBin.Text = "99";
            this.tbBin.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tbBin.TextChanged += new System.EventHandler(this.tbBin_TextChanged);
            // 
            // frmSensors
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1250, 585);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.butDelete);
            this.Controls.Add(this.butSaveEdit);
            this.Controls.Add(this.butCancelEdit);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.DGV);
            this.Controls.Add(this.btPrint);
            this.Controls.Add(this.btReload);
            this.Controls.Add(this.btWrite);
            this.Controls.Add(this.btCheck);
            this.Controls.Add(this.tbEvents);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmSensors";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Sensors";
            this.Load += new System.EventHandler(this.frmSensors_Load);
            ((System.ComponentModel.ISupportInitialize)(this.DGV)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataSet1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataTable1)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TextBox tbEvents;
        private System.Windows.Forms.Button btCheck;
        private System.Windows.Forms.Button btWrite;
        private System.Windows.Forms.Button btReload;
        private System.Windows.Forms.Button btPrint;
        private System.Windows.Forms.DataGridView DGV;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lbSelected;
        private System.Data.DataSet dataSet1;
        private System.Data.DataTable dataTable1;
        private System.Data.DataColumn dataColumn1;
        private System.Data.DataColumn dataColumn2;
        private System.Data.DataColumn dataColumn3;
        private System.Data.DataColumn dataColumn4;
        private System.Data.DataColumn dataColumn5;
        private System.Data.DataColumn dataColumn6;
        private System.Data.DataColumn dataColumn7;
        private System.Data.DataColumn dataColumn8;
        private System.Data.DataColumn dataColumn9;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tbCable;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox tbSensor;
        private System.Windows.Forms.CheckBox ckEnabled;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox tbOffset;
        private System.Windows.Forms.Button butCancelEdit;
        private System.Windows.Forms.Button butSaveEdit;
        private System.Windows.Forms.Button butDelete;
        private System.Data.DataColumn dataColumn12;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.DataGridViewTextBoxColumn ID;
        private System.Windows.Forms.DataGridViewTextBoxColumn Bin;
        private System.Windows.Forms.DataGridViewTextBoxColumn BinNum;
        private System.Windows.Forms.DataGridViewTextBoxColumn Cable;
        private System.Windows.Forms.DataGridViewTextBoxColumn Sensor;
        private System.Windows.Forms.DataGridViewCheckBoxColumn SensorEnabled;
        private System.Windows.Forms.DataGridViewTextBoxColumn Offset;
        private System.Windows.Forms.DataGridViewTextBoxColumn LastTemp;
        private System.Windows.Forms.DataGridViewTextBoxColumn LastTime;
        private System.Windows.Forms.DataGridViewTextBoxColumn Address;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.TextBox tbBin;
    }
}