using Factoriada.Bootstrap;
using Factoriada.Services.Interfaces;
using Factoriada.ViewModels;
using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace Factoriada
{
    public partial class AppShell : Xamarin.Forms.Shell
    {
        public AppShell()
        {
            BindingContext = new AppShellViewModel(AppContainer.Resolve<IDialogService>(), AppContainer.Resolve<INavigationService>(), AppContainer.Resolve<IApartmentService>());

            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
        }
    }
}
