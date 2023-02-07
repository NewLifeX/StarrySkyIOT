using InterconnectionManagementAPP.Domains;
using InterconnectionManagementAPP.ViewModels;
using InterconnectionManagementAPP.Views;
using Prism;
using Prism.Ioc;
using System.ComponentModel;
using Xamarin.Essentials.Implementation;
using Xamarin.Essentials.Interfaces;
using Xamarin.Forms;
using XingLucifer.IBase.Enums;

namespace InterconnectionManagementAPP
{
    public partial class App
    {
        public App(IPlatformInitializer initializer)
            : base(initializer)
        {
        }

        protected override async void OnInitialized()
        {
            InitializeComponent();

            await NavigationService.NavigateAsync("NavigationPage/MainPage");

        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<IAppInfo, AppInfoImplementation>();

            containerRegistry.RegisterForNavigation<NavigationPage>();
            containerRegistry.RegisterForNavigation<MainPage, MainPageViewModel>();

            containerRegistry.RegisterForNavigation<SwitchConfig, ViewModels.DeviceConfigs.SwitchConfigViewModel>();
            containerRegistry.RegisterForNavigation<TimingInit, ViewModels.TimingInitViewModel>();
            containerRegistry.RegisterForNavigation<TimingEntityConfig, TimingEntityConfigViewModel>();
        }
    }
}
