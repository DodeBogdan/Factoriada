using Factoriada.Models;
using Factoriada.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace Factoriada.ViewModels
{
    class RegisterViewModel : ViewModelBase
    {
        #region Constructor
        public RegisterViewModel(IUserService userService, IDialogService dialogService, INavigationService navigationService) : base(dialogService, navigationService)
        {
            _userService = userService;

            InitializeCommands();
        }

        private void InitializeCommands()
        {
            RegisterCommand = new Command(Register);
            CancelCommand = new Command(() => _navigationService.PopAsync());
        }
        #endregion

        #region Private Fields
        private readonly IUserService _userService;

        private string _confirmPassword;
        private User _newUser = new User();
        #endregion

        #region Prorieties
        public User NewUser
        {
            get => _newUser;
            set
            {
                _newUser = value;
                OnPropertyChanged();
            }
        }

        public string ConfirmPassword
        {
            get => _confirmPassword;
            set
            {
                _confirmPassword = value;
                OnPropertyChanged();
            }
        }
       
        public ICommand RegisterCommand { get; set; }
        public ICommand CancelCommand { get; set; }

        #endregion

        #region Private Methods
        private async void Register()
        {
            if (ConfirmPassword == null)
            {
                await _dialogService.ShowDialog("Parola de confirmare trebuie sa fie completata!", "Atentie!");
                return;
            }

            if (NewUser.Password != ConfirmPassword)
            {
                await _dialogService.ShowDialog("Parolele nu se potrivesc!", "Atentie!");
                return;
            }

            try
            {
                await _userService.Register(NewUser);

                NewUser = new User
                {
                    Address = new Address()
                    {
                        AddressId = Guid.NewGuid()
                    }
                };
                ConfirmPassword = "";

                await _dialogService.ShowDialog("Te-ai inregistrat cu succes.", "Succes");

                await _navigationService.PopAsync();
            }
            catch (Exception ex)
            {
                await _dialogService.ShowDialog(ex.Message, "Atentie!");
            }

        }
        #endregion
    }
}
