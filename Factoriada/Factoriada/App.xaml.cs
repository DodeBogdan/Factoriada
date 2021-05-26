using Factoriada.Bootstrap;
using Factoriada.Services;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Factoriada
{
    public partial class App : Application
    {

        public App()
        {
            Initialize();

            InitializeComponent();

            MainPage = new AppShell();
        }

        private void Initialize()
        {
            AppContainer.RegisterDependencies();
            _ = ApiService.ServiceClientInstance;
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
