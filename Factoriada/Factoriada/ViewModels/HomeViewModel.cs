using Factoriada.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace Factoriada.ViewModels
{
    class HomeViewModel : ViewModelBase
    {
        #region Constructor
        public HomeViewModel(INavigationService navigationService, IDialogService dialogService) : base(dialogService, navigationService)
        {
            InitializeCommands();
        }

        private void InitializeCommands()
        {
            RefreshCommand = new Command(Refresh);
            //SingInCommand = new Command(SingIn);
            //SingUpCommand = new Command(SignUp);
            //LogOutCommand = new Command(LogOut);
            //MyAccountCommand = new Command(MyAccount);
        }
        #endregion

        #region Private Fields
        private bool _isRefreshing;
        #endregion

        #region Proprieties
        public bool IsRefreshing
        {
            get => _isRefreshing;
            set
            {
                _isRefreshing = value;
                OnPropertyChanged();
            }
        }

        public ICommand RefreshCommand { get; set; }
        public ICommand SingInCommand { get; set; }
        public ICommand SingUpCommand { get; set; }
        public ICommand LogOutCommand { get; set; }
        public ICommand MyAccountCommand { get; set; }
        #endregion

        #region Private Methods
        private void Refresh()
        {
            IsRefreshing = false;
        }

        //private void SingIn()
        //{
        //    _navigationService.PushAsync(new LogInView());
        //}

        //private void SignUp()
        //{
        //    _navigationService.PushAsync(new RegisterView());
        //}

        //private void LogOut()
        //{
        //    ((Application.Current.MainPage as AppShell).BindingContext as AppShellViewModel).IsVisible = false;
        //    ((Application.Current.MainPage as AppShell).BindingContext as AppShellViewModel).AddProductIsVisible = false;
        //    ((Application.Current.MainPage as AppShell).BindingContext as AppShellViewModel).VisitorAndBuyerIsVisible = true;
        //    ((Application.Current.MainPage as AppShell).BindingContext as AppShellViewModel).SellerProductsIsVisible = false;
        //    ((Application.Current.MainPage as AppShell).BindingContext as AppShellViewModel).CategoryIsVisible = false;

        //    ConnectedUser.ResetUser();
        //    ActualUser = "";
        //    UserLoggedIn = true;
        //    UserLoggedOut = false;
        //}

        //private void MyAccount()
        //{
        //    _navigationService.PushAsync(new UserAccountView());
        //}
        #endregion
    }
}
