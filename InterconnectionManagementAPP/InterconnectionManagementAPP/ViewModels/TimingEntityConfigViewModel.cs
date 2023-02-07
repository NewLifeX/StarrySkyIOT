using InterconnectionManagementAPP.Enums;
using InterconnectionManagementAPP.Models;
using Prism.Commands;
using Prism.Events;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using XingLucifer.Devices;
using XingLucifer.IBase;
using XingLucifer.IBase.Enums;

namespace InterconnectionManagementAPP.ViewModels
{
    public class TimingEntityConfigViewModel : ViewModelBase
    {
        #region 字段
        private ObservableCollection<DeviceModel> _devices;
        private readonly IEventAggregator _aggregator;
        #endregion
        #region 属性
        public TaskEventParameterModel TPParameter { get; set; }
        private TimingModel _timing;

        public TimingModel Timing
        {
            get { return _timing; }
            set { SetProperty(ref _timing, value); }
        }

        public ObservableCollection<TaskTimingTypes> TaskSource { get;set; }
        public ObservableCollection<int> HourSource { get; set; }
        public ObservableCollection<int> MinuteSource { get; set; }

        public ObservableCollection<PeriodTypes> PeriodTypeSource { set; get; }

        private ObservableCollection<string> _deviceNameSource;

        public ObservableCollection<string> DeviceNameSource
        {
            get { return _deviceNameSource; }
            set { SetProperty(ref _deviceNameSource, value); }
        }

        private string _deviceName;

        public string DeviceName
        {
            get { return _deviceName; }
            set
            {
                SetProperty(ref _deviceName, value);
                Timing.Device = _devices.FirstOrDefault(d => d.Name == value);
            }
        }

        #endregion
        private IToast _toast;
        public TimingEntityConfigViewModel(INavigationService navigationService, IEventAggregator aggregator, IToast toast) : base(navigationService)
        {
            _toast = toast;
            HourSource = new ObservableCollection<int>();
            MinuteSource = new ObservableCollection<int>();
            for (int i = 0; i < 24; i++)
            {
                HourSource.Add(i);
            }
            for (int i = 0; i < 60; i++)
            {
                MinuteSource.Add(i);
            }
            PeriodTypeSource = new ObservableCollection<PeriodTypes>();
            TaskSource = new ObservableCollection<TaskTimingTypes>();
            foreach (PeriodTypes c in (PeriodTypes[])Enum.GetValues(typeof(PeriodTypes)))
            {
                PeriodTypeSource.Add(c);
            }
            foreach (TaskTimingTypes c in (TaskTimingTypes[])Enum.GetValues(typeof(TaskTimingTypes)))
            {
                TaskSource.Add(c);
            }
            _aggregator = aggregator;
            TPParameter = new TaskEventParameterModel();
        }
        #region 参数
        private DeviceBroadcast _broadcast;
        #endregion
        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            if (parameters["timing"] is TimingModel timing)
            {
                Timing = timing;
            }
            else
            {
                Timing = new TimingModel();
            }
            if (parameters["broadcast"] is DeviceBroadcast broadcast)
            {
                _broadcast = broadcast;
            }
            if (parameters["devicesource"] is ObservableCollection<DeviceModel> list)
            {
                _devices = list;
                DeviceNameSource = new ObservableCollection<string>(list.Select(x => x.Name));
            }
            base.OnNavigatedTo(parameters);
        }

        public DelegateCommand Save => new DelegateCommand(async () =>
        {
            Dictionary<string, object> eventParameter = default;
            switch (Timing.TaskType)
            {
                case TaskTimingTypes.定时开关:
                    eventParameter = new Dictionary<string, object>()
                    {
                        { "switch", TPParameter.SwitchStatus },
                    };
                    break;
            }
            MessageModel message = new MessageModel
            {
                Command = EventTypes.添加定时任务,
                Data = new TimingModel() { 
                    Seconds = Timing.Seconds,
                    Device = Timing.Device,
                    IsDelete= Timing.IsDelete,
                    EventParameter = eventParameter,
                    Hour= Timing.Hour,
                    IsEnabled= Timing.IsEnabled,
                    Minute= Timing.Minute,
                    Name= Timing.Name,
                    PeriodType= Timing.PeriodType,
                    RunCount= Timing.RunCount,
                    TaskType= Timing.TaskType,
                    Weeks= Timing.Weeks,
                },
            };
            _aggregator.GetEvent<MessageEvent>().Publish(message);
            await Task.Delay(500);
            await NavigationService.GoBackAsync();
        });

        public DelegateCommand Delete => new DelegateCommand(async () =>
        {
            MessageModel message = new MessageModel
            {
                Command = EventTypes.删除定时任务,
                Data = Timing,
            };
            _aggregator.GetEvent<MessageEvent>().Publish(message);
            await Task.Delay(500);
            await NavigationService.GoBackAsync();
        });
    }
}
