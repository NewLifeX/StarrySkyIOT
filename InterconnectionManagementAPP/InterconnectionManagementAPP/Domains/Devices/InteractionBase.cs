using InterconnectionManagementAPP.Helpers;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Xml.Linq;
using Xamarin.Forms;
using XingLucifer.Devices;
using XingLucifer.Devices.Enums;
using XingLucifer.Devices.Protocols;
using XingLucifer.IBase.Enums;

namespace InterconnectionManagementAPP.Domains.Devices
{
    public abstract class InteractionBase : IProtocolInteraction
    {
        protected byte[] _protocol = null;
        protected DeviceBroadcast Broadcast { get; set; }
        protected long _snowflakeID;
        protected HeaderProtocol _hp;
        public long SnowflakeID => _snowflakeID;
        private string _name;
        public string TaskName { get => _name; }
        public int FailureCount { get; set; }
        public TaskTimingTypes TaskType { get; }

        public InteractionBase(DeviceBroadcast broadcast, Models.DeviceModel model, TaskTimingTypes taskTiming, string name, long id)
        {
            _name = name;
            TaskType = taskTiming;
            _snowflakeID = id;
            Broadcast = broadcast;
            _hp = new HeaderProtocol();
            _hp.IPAddress = model.DeviceSocket.IP;
            _hp.MAC = model.DeviceSocket.MACString;
            _hp.SnowflakeID = _snowflakeID;
            _hp.DeviceType = DeviceTypes.单开;
        }

        public virtual bool Interaction()
        {
            try
            {
                Broadcast.SendTo(_protocol, new IPEndPoint(IPAddress.Parse(_hp.IPAddress), 23456));
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        protected void GetBytes()
        {
            string[] mac = _hp.MAC.Split('-');
            _protocol[0] = Convert.ToByte(mac[0], 16);
            _protocol[1] = Convert.ToByte(mac[1], 16);
            _protocol[2] = Convert.ToByte(mac[2], 16);
            _protocol[3] = Convert.ToByte(mac[3], 16);
            _protocol[4] = Convert.ToByte(mac[4], 16);
            _protocol[5] = Convert.ToByte(mac[5], 16);
            string[] ip = _hp.IPAddress.Split('.');
            _protocol[6] = Convert.ToByte(ip[0]);
            _protocol[7] = Convert.ToByte(ip[1]);
            _protocol[8] = Convert.ToByte(ip[2]);
            _protocol[9] = Convert.ToByte(ip[3]);
            Array.Copy(BitConverter.GetBytes(_snowflakeID), 0, _protocol, 10, 8);
            _protocol[18] = (byte)_hp.DeviceType;
            _protocol[19] = (byte)_hp.Command;
        }

        public virtual void GetStatus()
        {
        }
    }
}
