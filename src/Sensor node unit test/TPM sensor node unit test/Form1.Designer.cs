namespace TPM_sensor_node_unit_test
{
    partial class Form1
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.chklAddress = new System.Windows.Forms.CheckedListBox();
            this.chkPolling = new System.Windows.Forms.CheckBox();
            this.label4 = new System.Windows.Forms.Label();
            this.cboPollingRate = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.btnClear = new System.Windows.Forms.Button();
            this.rtbResponse = new System.Windows.Forms.RichTextBox();
            this.cboPort = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.btnGetPortName = new System.Windows.Forms.Button();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.ucSimpleDashboard1 = new SensorNode.Dashboard.ucSimpleDashboard();
            this.ucSimpleDashboard2 = new SensorNode.Dashboard.ucSimpleDashboard();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(77, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Node Address:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 200);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(58, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Response:";
            // 
            // chklAddress
            // 
            this.chklAddress.FormattingEnabled = true;
            this.chklAddress.Items.AddRange(new object[] {
            "01",
            "02",
            "03",
            "11",
            "12",
            "13",
            "21",
            "22",
            "23",
            "31",
            "32",
            "33"});
            this.chklAddress.Location = new System.Drawing.Point(12, 33);
            this.chklAddress.Name = "chklAddress";
            this.chklAddress.Size = new System.Drawing.Size(74, 154);
            this.chklAddress.TabIndex = 5;
            this.chklAddress.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.chklAddress_ItemCheck);
            this.chklAddress.Enter += new System.EventHandler(this.chklAddress_Enter);
            // 
            // chkPolling
            // 
            this.chkPolling.Appearance = System.Windows.Forms.Appearance.Button;
            this.chkPolling.AutoSize = true;
            this.chkPolling.BackColor = System.Drawing.Color.Lime;
            this.chkPolling.Location = new System.Drawing.Point(92, 33);
            this.chkPolling.Name = "chkPolling";
            this.chkPolling.Size = new System.Drawing.Size(73, 23);
            this.chkPolling.TabIndex = 6;
            this.chkPolling.Text = "Start Polling";
            this.chkPolling.UseVisualStyleBackColor = false;
            this.chkPolling.CheckedChanged += new System.EventHandler(this.chkPolling_CheckedChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(92, 70);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(62, 13);
            this.label4.TabIndex = 8;
            this.label4.Text = "Polling rate:";
            // 
            // cboPollingRate
            // 
            this.cboPollingRate.FormattingEnabled = true;
            this.cboPollingRate.Items.AddRange(new object[] {
            "500",
            "1000",
            "1500",
            "2000",
            "2500",
            "3000",
            "3500",
            "4000",
            "4500",
            "5000",
            "5500",
            "6000"});
            this.cboPollingRate.Location = new System.Drawing.Point(95, 86);
            this.cboPollingRate.Name = "cboPollingRate";
            this.cboPollingRate.Size = new System.Drawing.Size(85, 21);
            this.cboPollingRate.TabIndex = 9;
            this.cboPollingRate.Text = "1000";
            this.cboPollingRate.SelectedIndexChanged += new System.EventHandler(this.cboPollingRate_SelectedIndexChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(186, 89);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(20, 13);
            this.label5.TabIndex = 10;
            this.label5.Text = "ms";
            // 
            // btnClear
            // 
            this.btnClear.Location = new System.Drawing.Point(170, 195);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(75, 23);
            this.btnClear.TabIndex = 11;
            this.btnClear.Text = "Clear";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // rtbResponse
            // 
            this.rtbResponse.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.rtbResponse.Location = new System.Drawing.Point(12, 224);
            this.rtbResponse.Name = "rtbResponse";
            this.rtbResponse.Size = new System.Drawing.Size(233, 399);
            this.rtbResponse.TabIndex = 12;
            this.rtbResponse.Text = "";
            // 
            // cboPort
            // 
            this.cboPort.FormattingEnabled = true;
            this.cboPort.Location = new System.Drawing.Point(95, 126);
            this.cboPort.Name = "cboPort";
            this.cboPort.Size = new System.Drawing.Size(85, 21);
            this.cboPort.TabIndex = 14;
            this.cboPort.SelectedIndexChanged += new System.EventHandler(this.cboPort_SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(92, 110);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(29, 13);
            this.label3.TabIndex = 13;
            this.label3.Text = "Port:";
            // 
            // btnGetPortName
            // 
            this.btnGetPortName.Location = new System.Drawing.Point(95, 153);
            this.btnGetPortName.Name = "btnGetPortName";
            this.btnGetPortName.Size = new System.Drawing.Size(85, 23);
            this.btnGetPortName.TabIndex = 15;
            this.btnGetPortName.Text = "Get port name";
            this.btnGetPortName.UseVisualStyleBackColor = true;
            this.btnGetPortName.Click += new System.EventHandler(this.btnGetPortName_Click);
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.flowLayoutPanel1.BackColor = System.Drawing.SystemColors.AppWorkspace;
            this.flowLayoutPanel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.flowLayoutPanel1.Controls.Add(this.ucSimpleDashboard1);
            this.flowLayoutPanel1.Controls.Add(this.ucSimpleDashboard2);
            this.flowLayoutPanel1.Location = new System.Drawing.Point(251, 17);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(754, 606);
            this.flowLayoutPanel1.TabIndex = 16;
            // 
            // ucSimpleDashboard1
            // 
            this.ucSimpleDashboard1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ucSimpleDashboard1.Location = new System.Drawing.Point(3, 3);
            this.ucSimpleDashboard1.Name = "ucSimpleDashboard1";
            this.ucSimpleDashboard1.Size = new System.Drawing.Size(366, 237);
            this.ucSimpleDashboard1.TabIndex = 0;
            // 
            // ucSimpleDashboard2
            // 
            this.ucSimpleDashboard2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ucSimpleDashboard2.Location = new System.Drawing.Point(375, 3);
            this.ucSimpleDashboard2.Name = "ucSimpleDashboard2";
            this.ucSimpleDashboard2.Size = new System.Drawing.Size(366, 237);
            this.ucSimpleDashboard2.TabIndex = 1;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1017, 635);
            this.Controls.Add(this.flowLayoutPanel1);
            this.Controls.Add(this.btnGetPortName);
            this.Controls.Add(this.cboPort);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.rtbResponse);
            this.Controls.Add(this.btnClear);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.cboPollingRate);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.chkPolling);
            this.Controls.Add(this.chklAddress);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "Form1";
            this.Text = "TPM2018 Nodes Network Unit Tester";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckedListBox chklAddress;
        private System.Windows.Forms.CheckBox chkPolling;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox cboPollingRate;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.RichTextBox rtbResponse;
        private System.Windows.Forms.ComboBox cboPort;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnGetPortName;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private SensorNode.Dashboard.ucSimpleDashboard ucSimpleDashboard1;
        private SensorNode.Dashboard.ucSimpleDashboard ucSimpleDashboard2;
    }
}

