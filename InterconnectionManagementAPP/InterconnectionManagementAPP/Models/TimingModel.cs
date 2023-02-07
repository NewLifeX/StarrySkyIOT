using InterconnectionManagementAPP.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using XingLucifer.Devices.Models;
using XingLucifer.IBase;
using XingLucifer.IBase.Enums;

namespace InterconnectionManagementAPP.Models
{
    public class TimingModel : Prism.Mvvm.BindableBase
    {
        private string _name;
        /// <summary>
        /// 任务名称
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }

        private int _runCount;
        /// <summary>
        /// 运行次数
        /// </summary>
        public int RunCount
        {
            get { return _runCount; }
            set { SetProperty(ref _runCount, value); }
        }

        private DeviceModel _device;
        /// <summary>
        /// 联动设备
        /// </summary>
        public DeviceModel Device
        {
            get { return _device; }
            set { SetProperty(ref _device, value); }
        }


        private TaskTimingTypes _taskType;
        /// <summary>
        /// 任务类型
        /// </summary>
        public TaskTimingTypes TaskType
        {
            get { return _taskType; }
            set { SetProperty(ref _taskType, value); }
        }

        /// <summary>
        /// 运行次数到达后是否删除
        /// </summary>
        public bool IsDelete { get; set; } = true;

        private bool _isEnabled = true;
        /// <summary>
        /// 是否启用该条任务
        /// </summary>
        public bool IsEnabled
        {
            get { return _isEnabled; }
            set { SetProperty(ref _isEnabled, value); }
        }


        /// <summary>
        /// 时段类型
        /// </summary>
        public PeriodTypes PeriodType { get; set; }
        /// <summary>
        /// 指定时段（与时段类型对应，[周 0,1,2,3,4,5,6] [年 2-6 ps 指定每年的2月6号] [月 每个月的第几号]）
        /// </summary>
        public string Weeks { get; set; }

        public int Hour { get; set; }
        public int Minute { get; set; }
        public int Seconds { get; set; }
        public Dictionary<string, object> EventParameter { get; set; }
    }
}
