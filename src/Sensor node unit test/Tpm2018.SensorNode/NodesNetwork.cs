using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Text;

namespace Tpm2018.SensorNode
{
    public class NodesNetwork
    {
        private SerialPort sp = new SerialPort();

        public string PortName { get; set; }
        public int BaudRate { get; set; }
        public int DataBits { get; set; }
        public Parity Parity { get; set; }
        public StopBits StopBits { get; set; }
        public Handshake Handshake { get; set; }

        private object _Lock = new object();
        private LogFile.Log _Log;
        public string LastBusStatus;

        public NodesNetwork(string portName, int baudRate, int dataBits, Parity parity, StopBits stopBits, Handshake handshake)
        {
            this.PortName = portName;
            this.BaudRate = baudRate;
            this.DataBits = dataBits;
            this.Parity = parity;
            this.StopBits = stopBits;
            this.Handshake = handshake;

            if (_Log == null) _Log = new LogFile.Log(@"C:\TPM2018 Log", @"Sensor nodes network");
        }

        public Response Open()
        {
            try
            {
                /* -------------------------------------------------------------
                 * Port name validation
                 * -------------------------------------------------------------*/
                this.PortName = (this.PortName ?? "").ToUpper();
                if (!IsPortFound(this.PortName))
                {
                    Response res = Responses.PortNotFound(this.PortName);
                    _Log.AppendText(res.Message);
                    return res;
                }
                else // มี Port -> เปิดพอร์ต
                {
                    /* -------------------------------------------------------------
                     * Open port with mobus class
                     * -------------------------------------------------------------*/
                    lock (_Lock)
                    {
                        this.Open(this.PortName, this.BaudRate, this.DataBits, this.Parity, this.StopBits, this.Handshake);
                        Response res = Responses.OpenSuccess(this.PortName);
                        _Log.AppendText(res.Message);
                        return res;
                    }
                }
            }
            catch (Exception ex)
            {
                _Log.AppendText(ex.Message);
                return Responses.OpenFailed;
            }
        }

        #region Open / Close Procedures
        public bool Open(string portName, int baudRate, int databits, Parity parity, StopBits stopBits, Handshake handshake)
        {
            //Ensure port isn't already opened:
            if (!sp.IsOpen)
            {
                //Assign desired settings to the serial port:
                sp.PortName = portName;
                sp.BaudRate = baudRate;
                sp.DataBits = databits;
                sp.Parity = parity;
                sp.StopBits = stopBits;
                sp.Handshake = handshake; // more assign
                //These timeouts are default and cannot be editted through the class at this point:
                sp.ReadTimeout = 300;
                sp.WriteTimeout = 300;

                try
                {
                    sp.Open();
                }
                catch (Exception err)
                {
                    LastBusStatus = "Error opening " + portName + ": " + err.Message;
                    return false;
                }
                LastBusStatus = portName + " opened successfully";
                return true;
            }
            else
            {
                LastBusStatus = portName + " already opened";
                return false;
            }
        }
        public bool Close()
        {
            //Ensure port is opened before attempting to close:
            if (sp.IsOpen)
            {
                try
                {
                    sp.Close();
                }
                catch (Exception err)
                {
                    LastBusStatus = "Error closing " + sp.PortName + ": " + err.Message;
                    return false;
                }
                LastBusStatus = sp.PortName + " closed successfully";
                return true;
            }
            else
            {
                LastBusStatus = sp.PortName + " is not open";
                return false;
            }
        }
        #endregion

        public Response Weakup(string Addr)
        {
            try
            {
                List<StringBuilder> sbList = new List<StringBuilder>();
                int byteIncoming;
                int iByte = 0;
                bool isMessageComming = false;
                bool receiving = true;
                bool isTimeout = false;
                bool isExceed = false;
                Stopwatch sw = new Stopwatch();

                lock (_Lock)
                {
                    // ---------------------------------------------------------------------
                    // 1. Send environment state query command
                    // ---------------------------------------------------------------------
                    sp.DiscardInBuffer();
                    sp.DiscardOutBuffer();
                    sp.Write(string.Format(":13{0}00051000|#", Addr));

                    // ---------------------------------------------------------------------
                    // Read response bytes
                    // ---------------------------------------------------------------------
                    // System.Threading.Thread.Sleep(150);
                    sw.Reset(); sw.Start(); isTimeout = false;
                    receiving = true; isExceed = false;
                    while (receiving)
                    {
                        System.Threading.Thread.Sleep(1);
                        try
                        {
                            byteIncoming = 0;
                            byteIncoming = sp.ReadByte();
                        }
                        catch (Exception exReceiving)
                        {
                            byteIncoming = 0;
                            return Responses.InvalidInput;
                            //if (1 < iByte)
                            //{
                            //    return Responses.InvalidInput;
                            //}
                        } // end try

                        // <STX> = : come
                        if (byteIncoming.Equals((int)(':')))
                        {
                            sbList.Add(new StringBuilder());
                            isMessageComming = true;
                        }

                        // keep ascii
                        if (20 <= byteIncoming && byteIncoming <= 0x7E && isMessageComming) sbList[sbList.Count - 1].Append((char)byteIncoming);

                        // <ETX> = # come
                        if (byteIncoming.Equals((int)('#')))
                        {
                            isMessageComming = false;
                        }

                        // next byte
                        iByte++;
                        if (256 < iByte) // Check exeed
                        {
                            isExceed = true; break;
                        }

                        // Check timeout
                        if (1000 < sw.ElapsedMilliseconds)
                        {
                            sw.Stop(); isTimeout = true; break;
                        }

                    } // end while

                    // ---------------------------------------------------------------------
                    // Pos-processing
                    // ---------------------------------------------------------------------
                    // Timeout
                    if (isTimeout && sbList.Count <= 0) return Responses.PortTimeout;

                    // Exceed
                    if (isExceed && sbList.Count <= 0) return Responses.InvalidInput;

                    // Success received data
                    var response = Responses.Success;
                    if (0 < sbList.Count)
                    {
                        foreach (StringBuilder sb in sbList) _Log.AppendText(sb.ToString());
                        response.Data = sbList;
                    }
                    else
                    {
                        _Log.AppendText("Data lenght equals 0!");
                    }

                    return response;

                }

            }
            catch (TimeoutException tmex)
            {
                _Log.AppendText(tmex.Message);
                _Log.AppendText(tmex.StackTrace);
                return Responses.PortTimeout;
            }
            catch (Exception ex)
            {
                _Log.AppendText(ex.StackTrace);
            }

            return Responses.UnknownError;
        }

        public Response Read(string Addr)
        {
            try
            {
                List<StringBuilder> sbList = new List<StringBuilder>();
                int byteIncoming;
                int iByte = 0;
                bool isMessageComming = false;
                bool receiving = true;
                bool isTimeout = false;
                bool isExceed = false;
                Stopwatch sw = new Stopwatch();

                lock (_Lock)
                {
                    // ---------------------------------------------------------------------
                    // 1. Send environment state query command
                    // ---------------------------------------------------------------------
                    sp.DiscardInBuffer();
                    sp.DiscardOutBuffer();
                    sp.Write(string.Format(":13{0}00152000|#", Addr));

                    // ---------------------------------------------------------------------
                    // Read response bytes
                    // ---------------------------------------------------------------------
                    // System.Threading.Thread.Sleep(150);
                    sw.Reset(); sw.Start(); isTimeout = false;
                    receiving = true; isExceed = false;
                    while (receiving)
                    {
                        System.Threading.Thread.Sleep(1);
                        try
                        {
                            byteIncoming = 0;
                            byteIncoming = sp.ReadByte();
                        }
                        catch (Exception exReceiving)
                        {
                            byteIncoming = 0;
                            if (1 < iByte)
                            {
                                return Responses.InvalidInput;
                            }
                        } // end try

                        // <STX> = : come
                        if (byteIncoming.Equals((int)(':')))
                        {
                            sbList.Add(new StringBuilder());
                            isMessageComming = true;
                        }

                        // keep ascii
                        if (20 <= byteIncoming && byteIncoming <= 0x7E && isMessageComming) sbList[sbList.Count - 1].Append((char)byteIncoming);

                        // <ETX> = # come
                        if (byteIncoming.Equals((int)('#')))
                        {
                            isMessageComming = false;
                        }

                        // next byte
                        iByte++;
                        if (256 < iByte) // Check exeed
                        {
                            isExceed = true; break;
                        }

                        // Check timeout
                        if (1000 < sw.ElapsedMilliseconds)
                        {
                            sw.Stop(); isTimeout = true; break;
                        }

                    } // end while

                    // ---------------------------------------------------------------------
                    // Pos-processing
                    // ---------------------------------------------------------------------
                    // Timeout
                    if (isTimeout && sbList.Count <= 0) return Responses.PortTimeout;

                    // Exceed
                    if (isExceed && sbList.Count <= 0) return Responses.InvalidInput;

                    // Success received data
                    var response = Responses.Success;
                    if (0 < sbList.Count)
                    {
                        foreach (StringBuilder sb in sbList) _Log.AppendText(sb.ToString());
                        response.Data = sbList;
                    }
                    else
                    {
                        _Log.AppendText("Data lenght equals 0!");
                    }

                    return response;

                }

            }
            catch (TimeoutException tmex)
            {
                _Log.AppendText(tmex.Message);
                _Log.AppendText(tmex.StackTrace);
                return Responses.PortTimeout;
            }
            catch (Exception ex)
            {
                _Log.AppendText(ex.StackTrace);
            }

            return Responses.UnknownError;
        }

        protected bool IsPortFound(string portName) { return SerialPort.GetPortNames().Any(n => (n ?? "").ToLower().Equals((portName ?? "").ToLower())); }
    }
}
