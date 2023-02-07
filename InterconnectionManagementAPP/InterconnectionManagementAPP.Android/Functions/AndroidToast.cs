using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Org.Apache.Http.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XingLucifer.IBase;
using XingLucifer.IBase.Enums;

namespace InterconnectionManagementAPP.Droid.Functions
{
    public class AndroidToast : IToast
    {
        private Context _context;
        public AndroidToast(Context context)
        {
            _context = context;
        }
        public void Show(string str, LogLevelEnums level)
        {
            Toast.MakeText(_context, $"{EnumsToString(level)} {str}", ToastLength.Long).Show();
        }

        private string EnumsToString(LogLevelEnums level) =>  level switch
        {
            LogLevelEnums.Info => "信息",
            LogLevelEnums.Success => "成功",
            LogLevelEnums.Warning => "警告",
            LogLevelEnums.Error => "错误",
            _ => "",
        };
    }
}