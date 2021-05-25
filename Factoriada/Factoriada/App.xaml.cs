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
            InitializeComponent();

            _ = ApiService.ServiceClientInstance;
            MainPage = new AppShell();
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
