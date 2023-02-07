using System;
using System.Collections.Generic;
using System.Text;

namespace XingLucifer.Devices.Protocols.Devices
{
    public class OneSwitchProtocol
    {
        /// <summary>
        /// 头部
        /// </summary>
        public HeaderProtocol Header { get; set; }
        /// <summary>
        /// 开关状态
        /// </summary>
        public bool Status { get; set; }
    }
}
