using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Essentials;
using XingLucifer.IBase;

namespace InterconnectionManagementAPP.Droid.Functions
{
    public class Android_CaptureVideo : ICaptureVideo
    {
        private FileResult _fileResult;
        public Android_CaptureVideo()
        {
        }

        public async Task<string> Open()
        {
            if (_fileResult == null)
            {
                try
                {
                    _fileResult = await MediaPicker.CaptureVideoAsync();
                }
                catch (Exception ex)
                {
                    return ex.ToString();
                }
            }
            return "ok";
        }

        public void ReadStream(Action<byte[]> action, CancellationTokenSource tokenSource)
        {
            if (_fileResult == null)
            {
                throw new Exception("未初始化摄像头对象");
            }
            Task.Run(async () =>
            {
                while (!tokenSource.Token.IsCancellationRequested)
                {
                    using (var stream = await _fileResult.OpenReadAsync())
                    {
                        byte[] bytes = new byte[stream.Length];
                        stream.Read(bytes, 0, bytes.Length);
                        action?.Invoke(bytes);
                    }
                    Thread.Sleep(5);
                }
            });
        }
    }
}