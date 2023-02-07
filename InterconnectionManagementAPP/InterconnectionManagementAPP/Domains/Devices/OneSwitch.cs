using System;
using System.Collections.Generic;
using XingLucifer.Devices;
using XingLucifer.Devices.Enums;
using XingLucifer.Devices.Protocols;
using XingLucifer.Devices.Protocols.Devices;

namespace InterconnectionManagementAPP.Domains.Devices
{
    public class OneSwitch : InteractionBase
    {
        public Dictionary<string,object> EventParameter { get; set; }
        public OneSwitch(DeviceBroadcast broadcast, Models.DeviceModel model, string name, long id) : base(broadcast, model, XingLucifer.IBase.Enums.TaskTimingTypes.定时开关, name, id)
        {
            _protocol = new byte[21];
        }

        public void OFF()
        {
            _hp.Command = CommandType.写;
            GetBytes();
            _protocol[20] = 0x01;
        }

        public void ON() 
        {
            _hp.Command = CommandType.写;
            GetBytes();
            _protocol[20] = 0x00;
        }

        public override void GetStatus()
        {
            _hp.Command = CommandType.读;
            GetBytes();
            base.Interaction();
        }

    }
}
