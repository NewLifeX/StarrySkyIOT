using InterconnectionManagementAPP.Models;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Text.Json;
using XingLucifer.IBase.Enums;
using XingLucifer.IBase;
using System.IO;
using static Xamarin.Essentials.AppleSignInAuthenticator;
using InterconnectionManagementAPP.Domains;
using Prism.Commands;
using InterconnectionManagementAPP.Domains.Devices;
using Xamarin.Forms;
using XingLucifer.Devices;
using System.Linq;
using Prism.Events;

namespace InterconnectionManagementAPP.ViewModels
{
    public class TimingInitViewModel : ViewModelBase
    {
        private ObservableCollection<DeviceModel> _devices;
        private TimingManager _manager;

        public TimingManager Manager
        {
            get { return _manager; }
            set { SetProperty(ref _manager, value); }
        }


        public TimingInitViewModel(INavigationService navigationService, TimingManager manager) : base(navigationService)
        {
            Manager = manager;
        }

        #region 参数
        private DeviceBroadcast _broadcast;
        #endregion
        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            if (parameters["devicesource"] is ObservableCollection<DeviceModel> list)
            {
                _devices = list;
            }
            if (parameters["broadcast"] is DeviceBroadcast broadcast)
            {
                _broadcast = broadcast;
            }
        }

        public DelegateCommand<CollectionView> ItemSelection => new DelegateCommand<CollectionView>(async sender =>
        {
            if (sender.SelectedItem == null)
            {
                return;
            }
            TimingModel model = sender.SelectedItem as TimingModel;
            NavigationParameters parameters = new NavigationParameters
            {
                { "timing", model },
                { "devicesource", _devices },
            };
            await NavigationService.NavigateAsync("TimingEntityConfig", parameters);
            sender.SelectedItem = null;
        });

        public DelegateCommand AddModel => new DelegateCommand(async () =>
        {
            NavigationParameters parameters = new NavigationParameters
            {
                { "devicesource", _devices },
                { "broadcast", _broadcast },
            };
            await NavigationService.NavigateAsync("TimingEntityConfig", parameters);
        });
    }
}
