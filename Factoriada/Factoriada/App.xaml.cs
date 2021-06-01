using Factoriada.Bootstrap;
using Factoriada.Services;
using Factoriada.Views;
using System;
using Factoriada.Utility;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Factoriada
{
    public partial class App : Application
    {

        public App()
        {
            InitializeComponent();
            Initialize();
        }

        private void Initialize()
        {
            AppContainer.RegisterDependencies();
            _ = ApiService.ServiceClientInstance;

            MainPage = new NavigationPage(new LogInView());
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
