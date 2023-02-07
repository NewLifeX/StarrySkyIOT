using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace XingLucifer.Devices.Models
{
    public class FromDataModel
    {
        public byte[] Buffer { get; set; }
        public int Port { get; set; }
        public EndPoint Point { get; set; }
    }
}
