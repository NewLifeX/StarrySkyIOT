using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace InterconnectionManagementAPP
{
    public static class GlobalStatics
    {
        private static string _path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        public static string TimingList { get => Path.Combine(_path, "timinglist.txt"); }
        public static string DeviceList { get => Path.Combine(_path, "devicelist.txt"); }
        public static System.Text.Json.JsonSerializerOptions Options
        {
            get => new System.Text.Json.JsonSerializerOptions
            {
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };
        }
    }
}
