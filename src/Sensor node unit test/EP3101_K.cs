using LoadCell.Utilities;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LoadCell.E_Power
{
    public class EP3101_K
    {
        public string PortName { get; private set; }
        public int BaudRate { get; private set; }
        public int DataBits { get; private set; }
        public Parity Parity { get; private set; }
        public StopBits StopBits { get; private set; }
        public Handshake Handshake { get; private set; }

        private object _Lock = new object();
        private Modbus _Modbus;
        private LogFile.Log _Log;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="portName"></param>
        /// <param name="baudRate"></param>
        /// <param name="dataBits"></param>
        /// <param name="parity"></param>
        /// <param name="stopBits"></param>
        public EP3101_K(string portName, int baudRate, int dataBits, Parity parity, StopBits stopBits, Handshake handshake)
        {
            this.PortName = portName;
            this.BaudRate = baudRate;
            this.DataBits = dataBits;
            this.Parity = parity;
            this.StopBits = stopBits;
            this.Handshake = handshake;

            if (_Log == null) _Log = new LogFile.Log(@"C:\RVM Log", @"EP3101K");
            if (_Modbus == null) _Modbus = new Modbus();
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
                        _Modbus.Open(this.PortName, this.BaudRate, this.DataBits, this.Parity, this.StopBits, this.Handshake);
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

        public Response Read()
        {
            try
            {
                var values = new short[1];
                try
                {
                    var timeout = 3000;
                    const int roundDelay = 50;

                    while (!_Modbus.SendFc3(Convert.ToByte(2), 0, (ushort)values.Length, ref values))
                    {
                        Thread.Sleep(roundDelay);
                        timeout -= roundDelay;
                        if (timeout > 0) continue;
                        try
                        {
                            _Modbus.Close();
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.StackTrace);
                        }

                        return Responses.PortTimeout;
                    }
                }
                catch (Exception err)
                {
                    try
                    {
                        _Modbus.Close();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.StackTrace);
                    }

                    return Responses.UnknownError;
                }

                var response = Responses.Success;
                if (values.Length > 0)
                {
                    _Log.AppendText(">>weight<< " + values.FirstOrDefault());
                    response.Data = values[0];
                }
                else
                {
                    _Log.AppendText("Data lenght equals 0!");
                }

                return response;

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
