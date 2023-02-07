using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using XingLucifer.Devices.Enums;

namespace XingLucifer.Devices.Models
{
    public class DeviceSocketModel : Prism.Mvvm.BindableBase 
    {

        private string _macString;
        /// <summary>
        /// 界面绑定用MAC地址
        /// </summary>
        public string MACString
        {
            get { return _macString; }
            set { SetProperty(ref _macString, value); }
        }

        /// <summary>
        /// 设备类型
        /// </summary>
        public DeviceTypes DeviceType { get; set; }
        /// <summary>
        /// 设备IP
        /// </summary>
        public string IP { get; set; }
        /// <summary>
        /// UDP端口
        /// </summary>
        public int Port { get; set; }
        /// <summary>
        /// 设备地址
        /// </summary>
        public string DeviceAddress { get; set; }

        /// <summary>
        /// 是否被更新
        /// </summary>
        public bool IsUpdate { get; set; }
        /// <summary>
        /// 响应报文
        /// </summary>
        public byte[] Buffer { get; set; }
    }
}
