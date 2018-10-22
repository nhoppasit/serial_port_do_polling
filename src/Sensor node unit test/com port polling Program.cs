using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Threading;

namespace LoadCell.ConsoleMeter
{
    class Program
    {
        static LogFile.Log _Log = new LogFile.Log(@"C:\RVM Log", @"Load Cell Console Meter");
        static bool IsHideResult = false;
        static LoadCell.Service.LoadCellService service = new Service.LoadCellService();
        static DB_Management.ResultModelType res;
        static string logText;

        static string portName = string.Empty;
        static int baudRate = 9600;
        static int dataBits = 8;
        static Parity parity = Parity.None;
        static StopBits stopBits = StopBits.One;
        static Handshake handshake = Handshake.None;

        static LoadCell.E_Power.EP3101_K eP3101_K;
        static private volatile bool StopFlag = false;
        static private volatile bool Connecting, Running;
        static Thread tRecon;

        static void ReconnectionTimer_Tick(object sender, EventArgs e)
        {
            if (sender.GetType() == typeof(System.Windows.Forms.Timer))
            {
                System.Windows.Forms.Timer t = (System.Windows.Forms.Timer)sender;
                t.Enabled = false;
            }
        }

        static void Connect()
        {
            Connecting = true;

            System.Windows.Forms.Timer ReconnectionTimer = new System.Windows.Forms.Timer();
            ReconnectionTimer.Tick += ReconnectionTimer_Tick;
            ReconnectionTimer.Interval = 300;
            int Attemps = 10;

            for (int i = 0; i < Attemps; i++)
            {
                LoadCell.Utilities.Response meter_res = eP3101_K.Open();
                logText = meter_res.Message; _Log.AppendText(logText); if (!IsHideResult) Console.WriteLine(logText);
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

        static void Reconnection()
        {
            while (!Running)
            {
                Connect();
                Thread.Sleep(100);
                if (StopFlag) return;
            }
        }

        static void Main(string[] args)
        {
            try
            {
                // Cancel by Ctrl-C
                Console.CancelKeyPress += delegate (object sender, ConsoleCancelEventArgs e)
                {
                    logText = "Cancel by keyboard."; _Log.AppendText(logText); if (!IsHideResult) Console.WriteLine(logText);
                    e.Cancel = true;
                    StopFlag = true;
                };

                /* ----------------------------------------------------------------------------
                 * Start
                 * ----------------------------------------------------------------------------*/
                logText = "Start load cell console meter."; _Log.AppendText(logText); Console.WriteLine(logText);
                logText = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"); _Log.AppendText(logText); Console.WriteLine(logText);

                /* ----------------------------------------------------------------------------
                 * Is hide result on console
                 * ----------------------------------------------------------------------------*/
                res = service.Config_GetHideConsoleResult();
                IsHideResult = res.Flag;

                /* ----------------------------------------------------------------------------
                 * Verify port name
                 * ----------------------------------------------------------------------------*/
                if (!VerifyPortName() || !VerifyBaudRate() || !VerifyDataBits() || !VerifyParity() || !VerifyStopBits() || !VerifyHandshake())
                {
                    // Exit
                    logText = "Load cell console meter stopped."; _Log.AppendText(logText); if (!IsHideResult) Console.WriteLine(logText);
                    Console.WriteLine(Environment.NewLine);
                    Console.Write("Press any key to exit.");
                    Console.ReadKey();
                    return;
                }

                /* ----------------------------------------------------------------------------
                 * Treat load cell class with the port
                 * ----------------------------------------------------------------------------*/
                eP3101_K = new E_Power.EP3101_K(portName, baudRate, dataBits, parity, stopBits, handshake);

                //LoadCell.Utilities.Response meter_res = eP3101_K.Open();
                //logText = meter_res.Message; _Log.AppendText(logText); if (!IsHideResult) Console.WriteLine(logText);
                //if (!meter_res.Success)
                //{
                //    // Exit
                //    logText = "Load cell console meter stopped."; _Log.AppendText(logText); if (!IsHideResult) Console.WriteLine(logText);
                //    Console.WriteLine(Environment.NewLine);
                //    Console.Write("Press any key to exit.");
                //    Console.ReadKey();
                //    return;
                //}


                /* ----------------------------------------------------------------------------
                 * Connect
                 * ----------------------------------------------------------------------------*/
                logText = "Connecting..."; _Log.AppendText(logText); if (!IsHideResult) Console.WriteLine(logText);
                StopFlag = false;
                Running = false;

                Connect();
                if (!Running || Connecting)
                {
                    logText = "Cannot connect to load cell! The process going to end."; _Log.AppendText(logText); if (!IsHideResult) Console.WriteLine(logText);
                    Console.WriteLine(Environment.NewLine);
                    Console.Write("Press any key to exit.");
                    Console.ReadKey();
                    return;
                }

                /* ----------------------------------------------------------------------------
                 * LOOP
                 * ----------------------------------------------------------------------------*/
                res = service.Weight_DeleteAll();
                logText = res.Message; _Log.AppendText(logText); if (!IsHideResult) Console.WriteLine(logText);
                short weight = 0;
                while (!StopFlag)
                {
                    if (Running && !Connecting)
                    {
                        // logText = "Do polling..."; _Log.AppendText(logText); if (!IsHideResult) Console.WriteLine(logText);
                        LoadCell.Utilities.Response meter_res = eP3101_K.Read(); // DO PULLING
                        if (!meter_res.Success) // for failed
                        {
                            if (!StopFlag)
                            {
                                Running = false;
                                tRecon = new Thread(Reconnection);
                                tRecon.Start();
                            }
                        }
                        else // next step is save it into database
                        {
                            weight = (short)meter_res.Data;
                            res = service.Weight_AddNew(weight);
                            if (!res.Flag)
                            {
                                logText = string.Format("{0}. Weight = {1} kg.", DateTime.Now.ToString("hh:mm:ss fff"), weight); _Log.AppendText(logText); if (!IsHideResult) Console.WriteLine(logText);
                                logText = res.Message; _Log.AppendText(logText); if (!IsHideResult) Console.WriteLine(logText);
                            }
                            else
                            {
                                res = service.Weight_Avg();
                                double? avg, std, var;
                                try { avg = (double?)res.DataObjects[0]; } catch { avg = null; }
                                try { std = (double?)res.DataObjects[1]; } catch { std = null; }
                                try { var = (double?)res.DataObjects[2]; } catch { var = null; }
                                logText = string.Format("{0}. Weight = {1} kg. Avg = {2:F1}, Std = {3:F1}, Var = {4:F1}", DateTime.Now.ToString("hh:mm:ss fff"), weight, avg, std, var);
                                _Log.AppendText(logText);
                                if (!IsHideResult) Console.WriteLine(logText);
                            }
                        }

                        //Thread.Sleep(1);
                    }
                }

                /* ----------------------------------------------------------------------------
                 * Exit
                 * ----------------------------------------------------------------------------*/
                logText = "End the process."; _Log.AppendText(logText); if (!IsHideResult) Console.WriteLine(logText);
                logText = "Load cell console meter stopped."; _Log.AppendText(logText); if (!IsHideResult) Console.WriteLine(logText);
                Console.WriteLine(Environment.NewLine);
                Console.Write("Press any key to exit.");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                /* ----------------------------------------------------------------------------
                 * Log
                 * ----------------------------------------------------------------------------*/
                logText = "Load cell console meter stopped."; _Log.AppendText(logText); if (!IsHideResult) Console.WriteLine(logText);
                logText = ex.Message; _Log.AppendText(logText); if (!IsHideResult) Console.WriteLine(logText);

                /* ----------------------------------------------------------------------------
                 * Exit
                 * ----------------------------------------------------------------------------*/
                Console.WriteLine("");
                Console.Write("Press any key to exit.");
                Console.ReadKey();
            }
        }

        static bool VerifyPortName()
        {
            logText = "Verify port name..."; _Log.AppendText(logText); if (!IsHideResult) Console.WriteLine(logText);
            res = service.Config_PortName();
            if (res.Flag)
            {
                if (res.DataTexts != null) portName = res.DataTexts[0];
                logText = String.Format("Baud rate is {0}.", portName.Equals("") ? "EMPTY" : portName); _Log.AppendText(logText); if (!IsHideResult) Console.WriteLine(logText);
                if (portName.Equals(""))
                {
                    logText = "Invalid baudrate!"; _Log.AppendText(logText); if (!IsHideResult) Console.WriteLine(logText);
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                logText = res.Message; _Log.AppendText(logText); if (!IsHideResult) Console.WriteLine(logText);
                return false;
            }
        }

        static bool VerifyBaudRate()
        {
            logText = "Verify baud rate..."; _Log.AppendText(logText); if (!IsHideResult) Console.WriteLine(logText);
            res = service.Config_BaudRate();
            if (res.Flag)
            {
                if (res.DataObjects != null) baudRate = (int)res.DataObjects[0];
                logText = String.Format("Baud rate is {0}.", baudRate); _Log.AppendText(logText); if (!IsHideResult) Console.WriteLine(logText);
                if (portName.Equals(""))
                {
                    logText = "Invalid baud rate!"; _Log.AppendText(logText); if (!IsHideResult) Console.WriteLine(logText);
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                logText = res.Message; _Log.AppendText(logText); if (!IsHideResult) Console.WriteLine(logText);
                return false;
            }
        }

        static bool VerifyDataBits()
        {
            logText = "Verify data bits..."; _Log.AppendText(logText); if (!IsHideResult) Console.WriteLine(logText);
            res = service.Config_DataBits();
            if (res.Flag)
            {
                if (res.DataObjects != null) dataBits = (int)res.DataObjects[0];
                logText = String.Format("Data bits is {0}.", dataBits); _Log.AppendText(logText); if (!IsHideResult) Console.WriteLine(logText);
                if (portName.Equals(""))
                {
                    logText = "Invalid data bits!"; _Log.AppendText(logText); if (!IsHideResult) Console.WriteLine(logText);
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                logText = res.Message; _Log.AppendText(logText); if (!IsHideResult) Console.WriteLine(logText);
                return false;
            }
        }

        static bool VerifyParity()
        {
            logText = "Verify parity..."; _Log.AppendText(logText); if (!IsHideResult) Console.WriteLine(logText);
            res = service.Config_Parity();
            if (res.Flag)
            {
                if (res.DataObjects != null) parity = (Parity)res.DataObjects[1];
                logText = String.Format("Parity is {0}.", parity); _Log.AppendText(logText); if (!IsHideResult) Console.WriteLine(logText);
                if (portName.Equals(""))
                {
                    logText = "Invalid parity!"; _Log.AppendText(logText); if (!IsHideResult) Console.WriteLine(logText);
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                logText = res.Message; _Log.AppendText(logText); if (!IsHideResult) Console.WriteLine(logText);
                return false;
            }
        }

        static bool VerifyStopBits()
        {
            logText = "Verify stop bits..."; _Log.AppendText(logText); if (!IsHideResult) Console.WriteLine(logText);
            res = service.Config_StopBits();
            if (res.Flag)
            {
                if (res.DataObjects != null) stopBits = (StopBits)res.DataObjects[1];
                logText = String.Format("Stop bits is {0}.", stopBits); _Log.AppendText(logText); if (!IsHideResult) Console.WriteLine(logText);
                if (portName.Equals(""))
                {
                    logText = "Invalid stop bits!"; _Log.AppendText(logText); if (!IsHideResult) Console.WriteLine(logText);
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                logText = res.Message; _Log.AppendText(logText); if (!IsHideResult) Console.WriteLine(logText);
                return false;
            }
        }

        static bool VerifyHandshake()
        {
            logText = "Verify handshake..."; _Log.AppendText(logText); if (!IsHideResult) Console.WriteLine(logText);
            res = service.Config_Handshake();
            if (res.Flag)
            {
                if (res.DataObjects != null) handshake = (Handshake)res.DataObjects[1];
                logText = String.Format("Handshake is {0}.", handshake); _Log.AppendText(logText); if (!IsHideResult) Console.WriteLine(logText);
                if (portName.Equals(""))
                {
                    logText = "Invalid handshake!"; _Log.AppendText(logText); if (!IsHideResult) Console.WriteLine(logText);
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                logText = res.Message; _Log.AppendText(logText); if (!IsHideResult) Console.WriteLine(logText);
                return false;
            }
        }

    }
}
