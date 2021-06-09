using Factoriada.Services.Interfaces;
using Factoriada.Utility;
using Factoriada.Views;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Factoriada.Services;
using Xamarin.Forms;

namespace Factoriada.ViewModels
{
    public class LogInViewModel : ViewModelBase
    {
        #region Constructor
        public LogInViewModel(IDialogService dialogService, INavigationService navigationService, IUserService userService) : base(dialogService, navigationService)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));

            InitializeCommands();
        }

        private void InitializeCommands()
        {
            LogInCommand = new Command(LogIn);
            RegisterCommand = new Command(Register);
        }
        #endregion

        #region Private Fields
        private readonly IUserService _userService;

        private string _email;
        private string _password;
        #endregion

        #region Prorieties
        public string Email
        {
            get => _email;
            set
            {
                _email = value;
                OnPropertyChanged();
            }
        }

        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                OnPropertyChanged();
            }
        }

        public ICommand LogInCommand { get; set; }
        public ICommand RegisterCommand { get; set; }
        #endregion

        #region Private Methods
        private async void LogIn()
        {
            try
            {
                _dialogService.ShowLoading();

                var result = await _userService.LogIn(Email, Password);

                ActiveUser.User = result;

                Email = Password = "";

                App.Current.MainPage = new AppShell();

                _dialogService.HideLoading();
                
                await _dialogService.ShowDialog("Ai fost autentificat cu succes.", "Success");
            }
            catch(Exception ex)
            {
                _dialogService.HideLoading();
                await _dialogService.ShowDialog(ex.Message, "Atentie!");
            }
        }

        private async void Register()
        {
            await _navigationService.PushAsync(new RegisterView());
        }
        #endregion
    }
}
