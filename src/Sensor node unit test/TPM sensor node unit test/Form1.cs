using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Tpm2018.SensorNode;

namespace TPM_sensor_node_unit_test
{
    public partial class Form1 : Form
    {
        private delegate void VoidDelegate();

        public void PostResponse(string text)
        {
            if (rtbResponse.InvokeRequired)
            {
                rtbResponse.Invoke(new VoidDelegate(delegate ()
                {
                    rtbResponse.Text = text + Environment.NewLine + rtbResponse.Text;
                }));
            }
            else
            {
                rtbResponse.Text = text + Environment.NewLine + rtbResponse.Text;
            }
        }

        public void ClearResponse()
        {
            if (rtbResponse.InvokeRequired)
            {
                rtbResponse.Invoke(new VoidDelegate(delegate ()
                {
                    rtbResponse.Text = string.Empty;
                }));
            }
            else
            {
                rtbResponse.Text = string.Empty;
            }
        }

        public void ChangeDelayInteval()
        {
            if (cboPollingRate.InvokeRequired)
            {
                cboPollingRate.Invoke(new VoidDelegate(delegate ()
                {
                    try { DelayInteval = int.Parse(cboPollingRate.Text); } catch { DelayInteval = 1000; }
                }));
            }
            else
            {
                try { DelayInteval = int.Parse(cboPollingRate.Text); } catch { DelayInteval = 1000; }
            }
        }

        public void StopPolling()
        {
            if (cboPollingRate.InvokeRequired)
            {
                cboPollingRate.Invoke(new VoidDelegate(delegate ()
                {
                    StopFlag = true; chkPolling.Text = "Start Polling"; chkPolling.BackColor = Color.Lime; chkPolling.Checked = false;
                }));
            }
            else
            {
                StopFlag = true; chkPolling.Text = "Start Polling"; chkPolling.BackColor = Color.Lime; chkPolling.Checked = false;
            }
        }


        LogFile.Log _Log = new LogFile.Log(@"C:\TPM2018 Log", @"Unit tester");

        public Form1()
        {
            InitializeComponent();

            this.Text = this.Text + " Version " + Assembly.GetExecutingAssembly().GetName().Version.ToString();

            SensorNodesNetwork = new NodesNetwork(portName, baudRate, dataBits, parity, stopBits, handshake);

            try
            {
                cboPort.Items.Clear();
                cboPort.DataSource = SerialPort.GetPortNames();
                cboPort.Text = SerialPort.GetPortNames()[0];
                this.portName = cboPort.Text;
            }
            catch
            {
                cboPort.Text = "-";
                this.portName = string.Empty;
            }
        }

        #region Polling thread

        int DelayInteval = 1000;
        volatile bool StopFlag = true;
        string logText;

        Thread tPolling;
        void StartPolling()
        {
            tPolling = new Thread(PollingLoop);
            tPolling.Start();
        }

        /// <summary>
        /// DO POLLING
        /// </summary>
        void PollingLoop()
        {
            try
            {
                /* ----------------------------------------------------------------------------
                 * Start
                 * ----------------------------------------------------------------------------*/
                logText = "Start nodes network unit test."; _Log.AppendText(logText); PostResponse(logText);
                logText = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"); _Log.AppendText(logText); PostResponse(logText);


                /* ----------------------------------------------------------------------------
                 * Connect
                 * ----------------------------------------------------------------------------*/
                SensorNodesNetwork.PortName = this.portName;
                SensorNodesNetwork.BaudRate = this.baudRate;
                SensorNodesNetwork.DataBits = this.dataBits;
                SensorNodesNetwork.Parity = this.parity;
                SensorNodesNetwork.StopBits = this.stopBits;
                SensorNodesNetwork.Handshake = this.handshake;

                // 
                StopFlag = false;
                Running = false;
                Connect();
                if (!Running || Connecting)
                {
                    logText = "Cannot connect to load cell! The process going to end."; _Log.AppendText(logText); PostResponse(logText);
                    return;
                }

                // 
                while (!StopFlag)
                {
                    if (Running && !Connecting)
                    {
                        logText = string.Format("{0} << Do polling...", DateTime.Now.ToString("hh:mm:ss fff")); _Log.AppendText(logText); PostResponse(logText);
                        for (int i = 0; i < chklAddress.CheckedItems.Count; i++)
                        {
                            string addr = chklAddress.CheckedItems[i].ToString();
                            PostResponse(string.Format("{0} >> Wakeup {1}...", DateTime.Now.ToString("hh:mm:ss fff"), addr));
                            Response weakup_res = SensorNodesNetwork.Weakup(addr);
                            if (!weakup_res.Success)
                            {
                                PostResponse(string.Format("{0} << {1} do not wake!", DateTime.Now.ToString("hh:mm:ss fff"), addr));
                            }
                            else
                            {
                                Response query_res = SensorNodesNetwork.Read(addr);
                                if (!query_res.Success)
                                {
                                    // do nothing
                                }
                                else
                                {
                                    List<StringBuilder> sbList = (List<StringBuilder>)query_res.Data;
                                    foreach (StringBuilder sb in sbList)
                                    {
                                        PostResponse(sb.ToString());
                                    }

                                }
                            }
                        }

                        //
                        PostResponse(string.Format("{0} << Running...", DateTime.Now.ToString("hh:mm:ss fff")));

                        Thread.Sleep(DelayInteval);
                    }
                } // End while of polling loop

                SensorNodesNetwork.Close();
                StopPolling();
            }
            catch (Exception ex)
            {
                /* ----------------------------------------------------------------------------
                 * Log
                 * ----------------------------------------------------------------------------*/
                logText = "Load cell console meter stopped."; _Log.AppendText(logText);
                logText = ex.Message; _Log.AppendText(logText);
                PostResponse(logText);
                StopPolling();
            }
        } 

        string portName = string.Empty;
        int baudRate = 9600;
        int dataBits = 8;
        Parity parity = Parity.None;
        StopBits stopBits = StopBits.One;
        Handshake handshake = Handshake.None;

        NodesNetwork SensorNodesNetwork;
        private volatile bool Connecting, Running;
        Thread tRecon;

        void ReconnectionTimer_Tick(object sender, EventArgs e)
        {
            if (sender.GetType() == typeof(System.Windows.Forms.Timer))
            {
                System.Windows.Forms.Timer t = (System.Windows.Forms.Timer)sender;
                t.Enabled = false;
            }
        }

        void Connect()
        {
            Connecting = true;

            System.Windows.Forms.Timer ReconnectionTimer = new System.Windows.Forms.Timer();
            ReconnectionTimer.Tick += ReconnectionTimer_Tick;
            ReconnectionTimer.Interval = 300;
            int Attemps = 10;

            for (int i = 0; i < Attemps; i++)
            {
                Response meter_res = SensorNodesNetwork.Open();
                logText = meter_res.Message; _Log.AppendText(logText);
                if (meter_res.Success)
                {
                    Connecting = false;
                    Running = true;
                    return;
                }

                ReconnectionTimer.Start();
                while (ReconnectionTimer.Enabled)
                {
                    if (StopFlag)
                    {
                        Connecting = false;
                        return;
                    }
                    Thread.Sleep(1);
                }
            }
            Connecting = false;
        }

        void Reconnection()
        {
            while (!Running)
            {
                Connect();
                Thread.Sleep(100);
                if (StopFlag) return;
            }
        }

        #endregion

        #region Events

        private void chkPolling_CheckedChanged(object sender, EventArgs e)
        {
            if (chkPolling.Checked) { if (StopFlag) StartPolling(); chkPolling.Text = "Stop Polling"; chkPolling.BackColor = Color.Red; }
            else { StopFlag = true; chkPolling.Text = "Start Polling"; chkPolling.BackColor = Color.Lime; }
        }

        private void cboPollingRate_SelectedIndexChanged(object sender, EventArgs e)
        {
            ChangeDelayInteval();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearResponse();
        }

        private void cboPort_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                StopPolling();
                this.portName = cboPort.Text;
            }
            catch (Exception ex)
            {

            }
        }

        private void btnGetPortName_Click(object sender, EventArgs e)
        {
            try
            {
                StopPolling();
                cboPort.Items.Clear();
                cboPort.DataSource = SerialPort.GetPortNames();
                cboPort.Text = SerialPort.GetPortNames()[0];
                this.portName = cboPort.Text;
            }
            catch
            {
                cboPort.Text = "-";
                this.portName = string.Empty;
            }
        }
        
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            StopPolling();
        }

        #endregion
    }
}
