using Android.Util;
using System;
using System.Collections.Generic;
using System.Text;
using XingLucifer.IBase.Enums;

namespace XingLucifer.Devices
{
    public interface IProtocolInteraction
    {
        TaskTimingTypes TaskType { get; }
        int FailureCount { get; set; }
        long SnowflakeID { get; }
        string TaskName { get; }
        bool Interaction();
        void GetStatus();
    }
}
