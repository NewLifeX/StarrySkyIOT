using System;
using System.Collections.Generic;
using System.Text;

namespace InterconnectionManagementAPP.Models
{
    public class DeviceModel : Prism.Mvvm.BindableBase
    {
        public int Index { get; set; }
        private string _name;
        /// <summary>
        /// 名字
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }


        private bool _status;
        /// <summary>
        /// 开关状态
        /// </summary>
        public bool Status
        {
            get { return _status; }
            set { SetProperty(ref _status, value); }
        }


        private bool _isOnline = true;
        /// <summary>
        /// 是否在线
        /// </summary>
        public bool IsOnline
        {
            get { return _isOnline; }
            set { SetProperty(ref _isOnline, value); }
        }

        private float _temperature;
        /// <summary>
        /// 温度值
        /// </summary>
        public float Temperature
        {
            get { return _temperature; }
            set { SetProperty(ref _temperature, value); }
        }

        private XingLucifer.Devices.Models.DeviceSocketModel _deviceSocket;
        /// <summary>
        /// 设备Socket
        /// </summary>
        public XingLucifer.Devices.Models.DeviceSocketModel DeviceSocket
        {
            get { return _deviceSocket; }
            set { SetProperty(ref _deviceSocket, value); }
        }

    }
}
