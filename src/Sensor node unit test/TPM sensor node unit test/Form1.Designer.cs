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
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(18, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(77, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Node Address:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(18, 159);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(58, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Response:";
            // 
            // chklAddress
            // 
            this.chklAddress.FormattingEnabled = true;
            this.chklAddress.Items.AddRange(new object[] {
            "31",
            "32",
            "41",
            "42"});
            this.chklAddress.Location = new System.Drawing.Point(101, 12);
            this.chklAddress.Name = "chklAddress";
            this.chklAddress.Size = new System.Drawing.Size(74, 139);
            this.chklAddress.TabIndex = 5;
            // 
            // chkPolling
            // 
            this.chkPolling.Appearance = System.Windows.Forms.Appearance.Button;
            this.chkPolling.AutoSize = true;
            this.chkPolling.BackColor = System.Drawing.Color.Lime;
            this.chkPolling.Location = new System.Drawing.Point(181, 16);
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
            this.label4.Location = new System.Drawing.Point(269, 17);
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
            this.cboPollingRate.Location = new System.Drawing.Point(337, 14);
            this.cboPollingRate.Name = "cboPollingRate";
            this.cboPollingRate.Size = new System.Drawing.Size(76, 21);
            this.cboPollingRate.TabIndex = 9;
            this.cboPollingRate.Text = "1000";
            this.cboPollingRate.SelectedIndexChanged += new System.EventHandler(this.cboPollingRate_SelectedIndexChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(419, 17);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(20, 13);
            this.label5.TabIndex = 10;
            this.label5.Text = "ms";
            // 
            // btnClear
            // 
            this.btnClear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClear.Location = new System.Drawing.Point(590, 154);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(75, 23);
            this.btnClear.TabIndex = 11;
            this.btnClear.Text = "Clear";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // rtbResponse
            // 
            this.rtbResponse.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.rtbResponse.Location = new System.Drawing.Point(12, 183);
            this.rtbResponse.Name = "rtbResponse";
            this.rtbResponse.Size = new System.Drawing.Size(653, 242);
            this.rtbResponse.TabIndex = 12;
            this.rtbResponse.Text = "";
            // 
            // cboPort
            // 
            this.cboPort.FormattingEnabled = true;
            this.cboPort.Location = new System.Drawing.Point(337, 41);
            this.cboPort.Name = "cboPort";
            this.cboPort.Size = new System.Drawing.Size(76, 21);
            this.cboPort.TabIndex = 14;
            this.cboPort.SelectedIndexChanged += new System.EventHandler(this.cboPort_SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(302, 44);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(29, 13);
            this.label3.TabIndex = 13;
            this.label3.Text = "Port:";
            // 
            // btnGetPortName
            // 
            this.btnGetPortName.Location = new System.Drawing.Point(419, 39);
            this.btnGetPortName.Name = "btnGetPortName";
            this.btnGetPortName.Size = new System.Drawing.Size(88, 23);
            this.btnGetPortName.TabIndex = 15;
            this.btnGetPortName.Text = "Get port name";
            this.btnGetPortName.UseVisualStyleBackColor = true;
            this.btnGetPortName.Click += new System.EventHandler(this.btnGetPortName_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(677, 437);
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
    }
}

