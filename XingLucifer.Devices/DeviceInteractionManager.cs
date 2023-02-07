using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XingLucifer.Devices.Models;
using XingLucifer.IBase;
using XingLucifer.Devices.Helpers;
using Android.OS;
using Android.Views.Accessibility;
using XingLucifer.Devices.Enums;
using Javax.Crypto;

namespace XingLucifer.Devices
{
    public class DeviceInteractionManager
    {
        private DeviceBroadcast _broadcast;
        public DeviceBroadcast GetBroadcast { get => _broadcast; }
        private bool _beingProbed = false;
        /// <summary>
        /// 是否正在探测
        /// </summary>
        public bool BeingProbed { get => _beingProbed; }
        private List<DeviceSocketModel> _deviceSocketPool { get; set; }
        private IToast _toast;
        private IPEndPoint _broadcastIPAddress;
        private byte[] _broadcastProtocol = new byte[] { 0xFF, 0x31, 0x32, 0x35, 0x36, 0x30, 0x36, 0x39, 0x35, 0x38, 0x30, 0xFF };
        private Dictionary<CommandType, Action<DeviceSocketModel>> TaskEvent;

        public DeviceInteractionManager(IToast toast)
        {
            TaskEvent = new Dictionary<CommandType, Action<DeviceSocketModel>>();
            _deviceSocketPool = new List<DeviceSocketModel>();
            _toast = toast;
            IPHostEntry iPHostEntry = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var item in iPHostEntry.AddressList)
            {
                if (item.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    toast.Show($"获取到的IP为：{item}", IBase.Enums.LogLevelEnums.Info);
                    _broadcast = new DeviceBroadcast(item.ToString());
                    string[] ip = item.ToString().Split('.');
                    _broadcastIPAddress = new IPEndPoint(IPAddress.Parse($"{ip[0]}.{ip[1]}.{ip[2]}.255"), 23456);
                    break;
                }
            }
            BroadcastPool();
        }

        /// <summary>
        /// 添加事件
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="action"></param>
        public void AddEvent(CommandType eventType, Action<DeviceSocketModel> action)
        {
            if (!TaskEvent.ContainsKey(eventType))
            {
                TaskEvent.Add(eventType, action);
            }
        }

        /// <summary>
        /// 移除事件
        /// </summary>
        /// <param name="eventType"></param>
        public void RemoveEvent(CommandType eventType)
        {
            if (TaskEvent.ContainsKey(eventType))
            {
                TaskEvent.Remove(eventType);
            }
        }

        public List<DeviceSocketModel> GetDevicePool() => _deviceSocketPool;

        /// <summary>
        /// 广播探测
        /// </summary>
        /// <returns></returns>
        public async Task<bool> Broadcast()
        {
            return await Task.Run(() => {
                _beingProbed = true;
                foreach (var item in _deviceSocketPool)
                {
                    item.IsUpdate = false;
                }
                for (int i = 0; i < 3; i++)
                {
                    _broadcast.SendTo(_broadcastProtocol, _broadcastIPAddress);
                    Thread.Sleep(100);
                }
                _beingProbed = false;
                return true; 
            });
        }

        private void UpdateDevicePool(FromDataModel dataModel)
        {
            IPEndPoint iP = dataModel.Point as IPEndPoint;
            if (iP == null) return;
            string mac = BitConverter.ToString(dataModel.Buffer.Take(6).ToArray());
            var model = _deviceSocketPool.FirstOrDefault(x => x.Port == iP.Port && x.MACString == mac);
            if (model == null)
            {
                Locked();
                _deviceSocketPool.Add(new DeviceSocketModel() 
                { 
                    IP = iP.Address.ToString(), 
                    Port = iP.Port,
                    MACString = mac,
                    DeviceType = (Enums.DeviceTypes)dataModel.Buffer[11],
                    IsUpdate = true,
                    Buffer = dataModel.Buffer,
                });
                UnLock();
            }
            else
            {
                model.IsUpdate = true;
                model.IP = iP.Address.ToString();
                model.Buffer = dataModel.Buffer;
            }
            TaskEvent[CommandType.广播].Invoke(model);
        }

        public void BroadcastPool()
        {
            Task.Run(() =>
            {
                try
                {
                    while (true)
                    {
                        var model = _broadcast.ReceiveFrom();
                        if (model == null) break;
                        switch (model.Buffer[19])
                        {
                            //读
                            case 0: 
                                Task.Run(() =>
                                {
                                    string mac = BitConverter.ToString(model.Buffer.Take(6).ToArray());
                                    var entity = _deviceSocketPool.FirstOrDefault(x => x.MACString == mac);
                                    entity.Buffer = model.Buffer;
                                    TaskEvent[CommandType.读].Invoke(entity);
                                });
                                break;
                            //广播
                            case 0x03: Task.Run(() => UpdateDevicePool(model)); break;
                        }
                    }
                }
                catch (Exception)
                {
                }
            });
        }

        #region Synchronize
        private volatile int _lock = 0;
        protected void Locked()
        {
            int lockNumber = 0;
            while (Interlocked.Exchange(ref _lock, 1) != 0)
            {
                _ = 0; _ = 0; _ = 0; _ = 0; _ = 0; _ = 0; _ = 0; _ = 0; _ = 0; _ = 0; _ = 0; _ = 0; _ = 0; _ = 0; _ = 0; _ = 0;
                if (++lockNumber > 50)
                {
                    Thread.Sleep(1);
                    lockNumber = 0;
                }
            }
        }

        protected void UnLock() => Interlocked.Exchange(ref _lock, 0);
        #endregion
    }
}
