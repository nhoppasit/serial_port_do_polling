using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tpm2018.SensorNode
{
    public class Response
    {
        public bool Success { get; set; }
        public int Code { get; set; }
        public string Message { get; set; }
        public object Data { get; set; }
        public object Analytic { get; set; }
        public override string ToString()
        {
            return string.Format("CODE-{0}, {1}", Code, Message);
        }
    }

    public struct AnalyticStructure
    {
        public string NodeSn;
        public decimal Battery;
        public decimal Moisture0;
        public decimal Moisture30;
        public decimal Dendrometer;
        public decimal Humidity;
        public decimal Temperature;
    }

    public static class Responses
    {
        public static Response Success = new Response()
        {
            Success = true,
            Code = 0,
            Message = "Success"
        };

        public static Response OpenSuccess(string portName)
        {
            Response res = new Response()
            {
                Success = true,
                Code = 0,
            };
            res.Message = string.Format("Open {0} success.", portName);
            return res;
        }

        public static Response UnknownError = new Response()
        {
            Success = false,
            Code = 1,
            Message = "Unknown error!"
        };

        public static Response InvalidInput = new Response()
        {
            Success = false,
            Code = 2,
            Message = "Invalid input!"
        };

        public static Response PortNotFound(string portName)
        {
            Response res = new Response()
            {
                Success = true,
                Code = 0,
            };
            res.Message = string.Format("{0} not found!", portName);
            return res;
        }

        public static Response PortTimeout = new Response()
        {
            Success = false,
            Code = 4,
            Message = "Port time-out!"
        };

        public static Response OpenFailed = new Response()
        {
            Success = false,
            Code = 5,
            Message = "Open port failed!"
        };
    }
}
