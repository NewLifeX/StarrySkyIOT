using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XingLucifer.IBase;
using XingLucifer.IBase.Enums;
using Xamarin.Forms;
using System.Collections.ObjectModel;
using InterconnectionManagementAPP.Models;
using Xamarin.Forms.Internals;
using System.ComponentModel;
using Prism.Navigation.Xaml;
using InterconnectionManagementAPP.Domains.Devices;
using Prism.Events;
using InterconnectionManagementAPP.Enums;
using System.Text.Json;
using System.IO;
using Prism.Ioc;
using InterconnectionManagementAPP.Domains;
using XingLucifer.Devices;
using System.Security.Cryptography;
using InterconnectionManagementAPP.Helpers;

namespace InterconnectionManagementAPP.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        public ObservableCollection<DeviceModel> DevicePool { get; set; }
        private DeviceInteractionManager _interactionManager;
        private IToast _toast;
        public MainPageViewModel(INavigationService navigationService, IToast toast, IEventAggregator aggregator)
            : base(navigationService)
        {
            Title = "星空互联";
            _toast = toast;
            _interactionManager = new DeviceInteractionManager(toast);
            TimingManager timingManager = new TimingManager(str => toast.Show(str, LogLevelEnums.Info), aggregator);
            IContainerRegistry containerRegistry = (IContainerRegistry)ContainerLocator.Container;
            containerRegistry.RegisterInstance(timingManager.GetType(), timingManager);
            if (File.Exists(GlobalStatics.DeviceList))
            {
                try
                {
                    DevicePool = JsonSerializer.Deserialize<ObservableCollection<DeviceModel>>(File.ReadAllText(GlobalStatics.DeviceList, Encoding.UTF8));
                    DevicePool.ForEach(x => x.IsOnline = false);
                }
                catch (Exception)
                {
                    _toast.Show("读取设备列表配置异常", LogLevelEnums.Warning);
                    DevicePool = new ObservableCollection<DeviceModel>();
                }
            }
            else
            {
                DevicePool = new ObservableCollection<DeviceModel>();
            }
            EventInit(aggregator);
            DeviceStatusEventInit();
            Task.Run(() => 
            {
                _interactionManager.Broadcast().GetAwaiter().GetResult();
                Task.Delay(2000).GetAwaiter().GetResult();
                GetDeviceStatus();
            });
        }

        public DelegateCommand TestBoradcast => new DelegateCommand(async () =>
        {
            DevicePool.ForEach(x => x.IsOnline = false);
            await _interactionManager.Broadcast();
            _toast.Show("设备搜索完成", LogLevelEnums.Info);
        });

        /// <summary>
        /// 获取全部设备状态
        /// </summary>
        private void GetDeviceStatus()
        {
            foreach (var item in _interactionManager.GetDevicePool())
            {
                var models = DevicePool.Where(x => x.DeviceSocket.MACString == item.MACString).ToArray();
                if (models.Length == 0)
                {
                    continue;
                }
                switch (models[0].DeviceSocket.DeviceType)
                {
                    case XingLucifer.Devices.Enums.DeviceTypes.单通:
                    case XingLucifer.Devices.Enums.DeviceTypes.单开:
                        OneSwitch oneSwitch = new OneSwitch(_interactionManager.GetBroadcast, models[0], models[0].Name, _snowflakeManager.NextId());
                        oneSwitch.GetStatus();
                        break;
                    case XingLucifer.Devices.Enums.DeviceTypes.四开:
                        break;
                    case XingLucifer.Devices.Enums.DeviceTypes.四通:
                        break;
                    case XingLucifer.Devices.Enums.DeviceTypes.八开:
                        break;
                    case XingLucifer.Devices.Enums.DeviceTypes.八通:
                        break;
                    case XingLucifer.Devices.Enums.DeviceTypes.四开四通:
                        break;
                    case XingLucifer.Devices.Enums.DeviceTypes.单DS18B20:
                        break;
                    case XingLucifer.Devices.Enums.DeviceTypes.八DS18B20:
                        break;
                    case XingLucifer.Devices.Enums.DeviceTypes.单体红外体感:
                        break;
                    case XingLucifer.Devices.Enums.DeviceTypes.八体红外体感:
                        break;
                    case XingLucifer.Devices.Enums.DeviceTypes.红外线遥控:
                        break;
                    default:
                        break;
                }
            }
        }

        public DelegateCommand TimingCmd => new DelegateCommand(async () =>
        {
            Prism.Navigation.NavigationParameters parameters = new Prism.Navigation.NavigationParameters
            {
                { "devicesource", DevicePool },
                { "broadcast", _interactionManager.GetBroadcast },
            };
            await NavigationService.NavigateAsync("TimingInit", parameters);
        });

        public DelegateCommand<CollectionView> ItemSelection => new DelegateCommand<CollectionView>(async sender =>
        {
            if (sender.SelectedItem == null)
            {
                return;
            }
            DeviceModel model = sender.SelectedItem as DeviceModel;
            Prism.Navigation.NavigationParameters parameters = new Prism.Navigation.NavigationParameters
            {
                { "deviceModel", model},
                { "broadcast", _interactionManager.GetBroadcast },
            };
            await NavigationService.NavigateAsync("SwitchConfig", parameters);
            sender.SelectedItem = null;
        });

        #region 发布订阅初始化
        private SnowflakeManager _snowflakeManager;
        private int _timed_task_lock = 0;
        private int _caches_lock = 0;
        private List<IProtocolInteraction> _protocolInteractions = new List<IProtocolInteraction>();
        private Dictionary<long, TimingModel> _protocolInteractionCaches = new Dictionary<long, TimingModel>();
        private void EventInit(IEventAggregator aggregator)
        {
            _snowflakeManager = new SnowflakeManager(12);
            aggregator.GetEvent<MessageEvent>().Subscribe(sender => {
                try
                {
                    DeviceModel deviceModel = sender.Data as DeviceModel;
                    switch (deviceModel.DeviceSocket.DeviceType)
                    {
                        case XingLucifer.Devices.Enums.DeviceTypes.单开:
                        case XingLucifer.Devices.Enums.DeviceTypes.单通:
                        case XingLucifer.Devices.Enums.DeviceTypes.单DS18B20:
                        case XingLucifer.Devices.Enums.DeviceTypes.单体红外体感:
                            DevicePool.Remove(deviceModel);
                            return;
                    }
                    var models = DevicePool.Where(x => x.DeviceSocket.MACString == deviceModel.DeviceSocket.MACString).ToList();
                    foreach (var item in models)
                    {
                        DevicePool.Remove(item);
                    }
                }
                catch (Exception ex)
                {
                    _toast.Show($"删除设备异常：{ex}", LogLevelEnums.Error);
                }
            }, arg => arg.Command == EventTypes.删除);
            aggregator.GetEvent<MessageEvent>().Subscribe(sender => {
                SaveDeviceList();
            }, arg => arg.Command == EventTypes.更新);

            aggregator.GetEvent<MessageEvent>().Subscribe(sender =>
            {
                var timing = sender.Data as TimingModel;
                //!DevicePool.Any(x => x.IsOnline && x.DeviceSocket.MACString == timing.Device.DeviceSocket.MACString)
                if (timing.Device == null)
                {
                    return;
                }
                if (!_protocolInteractions.Any(x => x.TaskName == timing.Name))
                {
                    Caches_Locked();
                    _protocolInteractionCaches.Add(_snowflakeManager.NextId(), timing);
                    Caches_UnLock();
                    if (Interlocked.Exchange(ref _timed_task_lock, 1) == 0)
                    {
                        Task.Run(() =>
                        {
                            while (_protocolInteractionCaches.Count > 0)
                            {
                                Caches_Locked();
                                foreach (var item in _protocolInteractionCaches)
                                {
                                    OneSwitch oneSwitch = new OneSwitch(_interactionManager.GetBroadcast, timing.Device, timing.Name, item.Key);
                                    oneSwitch.EventParameter = item.Value.EventParameter;
                                    _protocolInteractions.Add(oneSwitch);
                                }
                                Caches_UnLock();
                                foreach (var item in _protocolInteractions)
                                {
                                    var model = item as OneSwitch;
                                    switch (item.TaskType)
                                    {
                                        case TaskTimingTypes.定时开关:
                                            object obj = model.EventParameter["switch"];
                                            bool obj_bool = false;
                                            if (obj is bool bl)
                                            {
                                                obj_bool = bl;
                                            }
                                            if (obj is JsonElement valueKind)
                                            {
                                                obj_bool = valueKind.ValueKind == JsonValueKind.True;
                                            }
                                            if (obj_bool)
                                            {
                                                model.ON();
                                            }
                                            else
                                            {
                                                model.OFF();
                                            }
                                            break;
                                    }
                                    model.Interaction();
                                }
                                _protocolInteractions.Clear();
                                Thread.Sleep(600);
                            }
                            Interlocked.Exchange(ref _timed_task_lock, 0);
                        });
                    }
                }
            }, arg => arg.Command == EventTypes.启动定时任务);
        }
        #endregion

        #region Synchronize
        protected void Caches_Locked()
        {
            int lockNumber = 0;
            while (Interlocked.Exchange(ref _caches_lock, 1) != 0)
            {
                _ = 0; _ = 0; _ = 0; _ = 0; _ = 0; _ = 0; _ = 0; _ = 0; _ = 0; _ = 0; _ = 0; _ = 0; _ = 0; _ = 0; _ = 0; _ = 0;
                if (++lockNumber > 50)
                {
                    Thread.Sleep(1);
                    lockNumber = 0;
                }
            }
        }

        protected void Caches_UnLock() => Interlocked.Exchange(ref _caches_lock, 0);
        #endregion

        #region 报文处理订阅
        private void DeviceStatusEventInit()
        {
            _interactionManager.AddEvent(XingLucifer.Devices.Enums.CommandType.广播, entity =>
            {
                var models = DevicePool.Where(x => x.DeviceSocket.MACString == entity.MACString).ToArray();
                if (models.Length == 0)
                {
                    switch (entity.DeviceType)
                    {
                        case XingLucifer.Devices.Enums.DeviceTypes.单开:
                        case XingLucifer.Devices.Enums.DeviceTypes.单通:
                        case XingLucifer.Devices.Enums.DeviceTypes.单DS18B20:
                        case XingLucifer.Devices.Enums.DeviceTypes.单体红外体感:
                        case XingLucifer.Devices.Enums.DeviceTypes.红外线遥控:
                            DevicePool.Add(new DeviceModel()
                            {
                                Index = 1,
                                DeviceSocket = entity,
                                Name = "1 #"
                            });
                            break;
                        case XingLucifer.Devices.Enums.DeviceTypes.四开四通:
                        case XingLucifer.Devices.Enums.DeviceTypes.四开:
                        case XingLucifer.Devices.Enums.DeviceTypes.四通:
                            for (int i = 1; i < 5; i++)
                            {
                                DevicePool.Add(new DeviceModel()
                                {
                                    Index = i,
                                    DeviceSocket = entity,
                                    Name = $"{i} #",
                                });
                            }
                            break;
                        case XingLucifer.Devices.Enums.DeviceTypes.八DS18B20:
                        case XingLucifer.Devices.Enums.DeviceTypes.八开:
                        case XingLucifer.Devices.Enums.DeviceTypes.八通:
                            for (int i = 1; i < 9; i++)
                            {
                                DevicePool.Add(new DeviceModel()
                                {
                                    Index = i,
                                    DeviceSocket = entity,
                                    Name = $"{i} #",
                                });
                            }
                            break;
                    }
                    return;
                }
                models.ForEach(k =>
                {
                    k.IsOnline = true;
                    k.DeviceSocket.IP = entity.IP;
                });
                SaveDeviceList();
            });

            _interactionManager.AddEvent(XingLucifer.Devices.Enums.CommandType.读, entity =>
            {
                var models = DevicePool.Where(x => x.DeviceSocket.MACString == entity.MACString).ToArray();
                if (models.Length == 0)
                {
                    return;
                }
                long id = BitConverter.ToInt64(entity.Buffer, 10);
                if (_protocolInteractionCaches.Count > 0 && _protocolInteractionCaches.ContainsKey(id))
                {
                    Caches_Locked();
                    _protocolInteractionCaches.Remove(id);
                    Caches_UnLock();
                }
                switch (entity.DeviceType)
                {
                    case XingLucifer.Devices.Enums.DeviceTypes.单开:
                    case XingLucifer.Devices.Enums.DeviceTypes.单通:
                        models[0].Status = entity.Buffer[20] == 0x00;
                        break;
                    case XingLucifer.Devices.Enums.DeviceTypes.四开:
                        break;
                    case XingLucifer.Devices.Enums.DeviceTypes.四通:
                        break;
                    case XingLucifer.Devices.Enums.DeviceTypes.八开:
                        break;
                    case XingLucifer.Devices.Enums.DeviceTypes.八通:
                        break;
                    case XingLucifer.Devices.Enums.DeviceTypes.四开四通:
                        break;
                    case XingLucifer.Devices.Enums.DeviceTypes.单DS18B20:
                        break;
                    case XingLucifer.Devices.Enums.DeviceTypes.八DS18B20:
                        break;
                    case XingLucifer.Devices.Enums.DeviceTypes.单体红外体感:
                        break;
                    case XingLucifer.Devices.Enums.DeviceTypes.八体红外体感:
                        break;
                    case XingLucifer.Devices.Enums.DeviceTypes.红外线遥控:
                        break;
                    default:
                        break;
                }
            });
        }
        #endregion

        private void SaveDeviceList() => File.WriteAllText(GlobalStatics.DeviceList, JsonSerializer.Serialize(DevicePool, GlobalStatics.Options), Encoding.UTF8);
    }
}
