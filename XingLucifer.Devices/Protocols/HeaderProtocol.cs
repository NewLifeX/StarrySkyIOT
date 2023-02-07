using System;
using System.Collections.Generic;
using System.Text;

namespace XingLucifer.Devices.Protocols
{
    /// <summary>
    /// 基础协议----【统一】
    /// </summary>
    public class HeaderProtocol
    {
        /// <summary>
        /// 设备MAC
        /// </summary>
        public string MAC { get; set; }
        /// <summary>
        /// IP地址
        /// </summary>
        public string IPAddress { get; set; }
        /// <summary>
        /// 雪花ID----用于标识唯一报文
        /// </summary>
        public long SnowflakeID { get; set; }
        /// <summary>
        /// 设备类型
        /// </summary>
        public Enums.DeviceTypes DeviceType { get; set; }
        /// <summary>
        /// 命令类型
        /// </summary>
        public Enums.CommandType Command { get; set; }
    }
}
