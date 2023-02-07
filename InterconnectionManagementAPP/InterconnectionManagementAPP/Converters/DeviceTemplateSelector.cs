using InterconnectionManagementAPP.Models;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using XingLucifer.Devices.Models;

namespace InterconnectionManagementAPP.Converters
{
    public class DeviceTemplateSelector : DataTemplateSelector
    {
        public DataTemplate SwitchTemplate { get; set; }
        public DataTemplate TemperatureTemplate { get; set; }
        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            DeviceSocketModel deviceSocket = (item as DeviceModel).DeviceSocket;
            switch (deviceSocket.DeviceType)
            {
                case XingLucifer.Devices.Enums.DeviceTypes.单开:
                case XingLucifer.Devices.Enums.DeviceTypes.四开:
                case XingLucifer.Devices.Enums.DeviceTypes.八开:
                case XingLucifer.Devices.Enums.DeviceTypes.单通:
                case XingLucifer.Devices.Enums.DeviceTypes.四通:
                case XingLucifer.Devices.Enums.DeviceTypes.八通:
                case XingLucifer.Devices.Enums.DeviceTypes.四开四通:
                    return SwitchTemplate;
                case XingLucifer.Devices.Enums.DeviceTypes.单DS18B20:
                case XingLucifer.Devices.Enums.DeviceTypes.八DS18B20:
                    return TemperatureTemplate;
                case XingLucifer.Devices.Enums.DeviceTypes.单体红外体感:
                    break;
                case XingLucifer.Devices.Enums.DeviceTypes.八体红外体感:
                    break;
                case XingLucifer.Devices.Enums.DeviceTypes.红外线遥控:
                    break;
                default:
                    break;
            }
            return SwitchTemplate;
        }
    }
}
