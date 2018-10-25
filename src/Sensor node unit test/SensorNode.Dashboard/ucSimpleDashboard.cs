using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SensorNode.Dashboard
{
    public partial class ucSimpleDashboard : UserControl
    {
        private delegate void VoidDelegate();

        public void PostResponse(string sn, decimal bat, decimal moisture0, decimal moisture30, decimal dendrometer, decimal humidity, decimal temperature)
        {
            if (pgBat.InvokeRequired)
            {
                pgBat.Invoke(new VoidDelegate(delegate ()
                {
                    if (sn.Equals(this.NodeSn))
                    {
                        txtNodeSn.Text = sn;
                        pgBat.Value = (int)(bat / 931m * 100m);
                        lblBat.Text = (bat / 931m * 3.3m).ToString("F1");
                        pgMoisture0.Value = (int)((1m - (moisture0 / 1023)) * 100m);
                        lblMoisture0.Text = (moisture0 / 1023m * 3.3m).ToString("F1");
                        pgMoisture30.Value = (int)((1m - (moisture30 / 1023)) * 100m);
                        lblMoisture30.Text = (moisture30 / 1023m * 3.3m).ToString("F1");
                        pgDendrometer.Value = (int)(dendrometer / 530m * 100m);
                        lblDendrometer.Text = (dendrometer / 530m * 6m).ToString("F1");
                        pgHumidity.Value = (int)humidity;
                        lblHumidity.Text = humidity.ToString("F1");
                        pgTemperature.Value = (int)((temperature) / (55m) * 100m);
                        lblTemperature.Text = temperature.ToString("F1");
                    }
                }));
            }
            else
            {
                if (sn.Equals(this.NodeSn))
                {
                    txtNodeSn.Text = sn;
                    pgBat.Value = (int)(bat / 931m * 100m);
                    lblBat.Text = (bat / 931m * 3.3m).ToString("F1");
                    pgMoisture0.Value = (int)((1m - (moisture0 / 1023)) * 100m);
                    lblMoisture0.Text = (moisture0 / 1023m * 3.3m).ToString("F1");
                    pgMoisture30.Value = (int)((1m - (moisture30 / 1023)) * 100m);
                    lblMoisture30.Text = (moisture30 / 1023m * 3.3m).ToString("F1");
                    pgDendrometer.Value = (int)(dendrometer / 530m * 100m);
                    lblDendrometer.Text = (dendrometer / 530m * 6m).ToString("F1");
                    pgHumidity.Value = (int)humidity;
                    lblHumidity.Text = humidity.ToString("F1");
                    pgTemperature.Value = (int)((temperature) / (55m) * 100m);
                    lblTemperature.Text = temperature.ToString("F1");
                }
            }
        }

        public string NodeSn { get; set; }

        public ucSimpleDashboard()
        {
            InitializeComponent();
        }
    }
}
