using System;
using System.Collections.Generic;
using System.Text;

namespace XingLucifer.Devices.Protocols
{
    /// <summary>
    /// WIFI配置协议
    /// </summary>
    public class ConfigurationProtocol
    {
        public HeaderProtocol Header { get; set; }

        public string SSID { get; set; }

        public string PASSWORD { get; set; }

        public string ADDRESS { get; set; }
    }
}
