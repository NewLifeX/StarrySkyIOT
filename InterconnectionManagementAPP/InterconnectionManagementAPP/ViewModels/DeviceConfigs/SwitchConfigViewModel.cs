using InterconnectionManagementAPP.Domains.Devices;
using InterconnectionManagementAPP.Enums;
using InterconnectionManagementAPP.Models;
using Prism.Commands;
using Prism.Events;
using Prism.Navigation;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using XingLucifer.Devices;

namespace InterconnectionManagementAPP.ViewModels.DeviceConfigs
{
    public class SwitchConfigViewModel : ViewModelBase
    {
        private DeviceModel _device;
        private OneSwitch _oneSwitch;
        private readonly IEventAggregator _aggregator;
        public DeviceModel Device
        {
            get { return _device; }
            set { SetProperty(ref _device, value); }
        }

        public SwitchConfigViewModel(INavigationService navigationService, IEventAggregator aggregator) : base(navigationService)
        {
            _aggregator = aggregator;
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            Device = parameters["deviceModel"] as DeviceModel;
            _oneSwitch = new OneSwitch(parameters["broadcast"] as DeviceBroadcast, Device, "", 12156);
        }

        public DelegateCommand ONSwitch => new DelegateCommand(() =>
        {
            _oneSwitch.ON();
            _oneSwitch.Interaction();
        });

        public DelegateCommand OFFSwitch => new DelegateCommand(() =>
        {
            _oneSwitch.OFF();
            _oneSwitch.Interaction();
        });

        public DelegateCommand Delete => new DelegateCommand(async () =>
        {
            MessageModel message = new MessageModel
            {
                Command = EventTypes.删除,
                Data = _device,
            };
            _aggregator.GetEvent<MessageEvent>().Publish(message);
            await NavigationService.GoBackToRootAsync();
        });

        public DelegateCommand Confirm => new DelegateCommand(async () =>
        {
            MessageModel message = new MessageModel
            {
                Command = EventTypes.更新,
                Data = _device,
            };
            _aggregator.GetEvent<MessageEvent>().Publish(message);
            await NavigationService.GoBackToRootAsync();
        });
    }
}
