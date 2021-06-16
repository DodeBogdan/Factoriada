using Factoriada.Models;
using Factoriada.Services.Interfaces;
using Factoriada.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Input;
using Factoriada.Services;
using Factoriada.Views;
using Xamarin.Forms;

namespace Factoriada.ViewModels
{
    internal class AppShellViewModel : ViewModelBase
    {
        #region Constructor
        public AppShellViewModel(IDialogService dialogService, INavigationService navigationService, IApartmentService apartmentService) : base(dialogService, navigationService)
        {
            _apartmentService = apartmentService;

            Initialize();
            InitializeCommands();
        }

        #endregion

        #region Private Fields

        private readonly IApartmentService _apartmentService;

        private User _connectedUser = new User();
        private bool _userNotHavingApartment;
        private ImageSource _currentUserImage;
        private bool _userHaveApartment;

        #endregion

        #region Proprieties

        public bool UserHaveApartment
        {
            get => _userHaveApartment;
            set
            {
                _userHaveApartment = value;
                OnPropertyChanged();
            }
        }

        public ImageSource CurrentUserImage
        {
            get => _currentUserImage;
            set
            {
                _currentUserImage = value;
                OnPropertyChanged();
            }
        }

        public bool UserNotHavingApartment
        {
            get => _userNotHavingApartment;
            set
            {
                _userNotHavingApartment = value;
                OnPropertyChanged();
            }
        }

        public User ConnectedUser
        {
            get => _connectedUser;
            set
            {
                _connectedUser = value;
                if (value.ImagesByte != null)
                {
                    var stream = new MemoryStream(value.ImagesByte);
                    if(stream.CanRead)
                        CurrentUserImage = ImageSource.FromStream(() => stream);
                }
                
                else CurrentUserImage = ImageSource.FromFile("userimage.png");
                OnPropertyChanged();
            }
        }

        public ICommand LogOutCommand { get; set; }
        public ICommand JoinApartmentCommand { get; set; }
        #endregion

        #region Private Methods

        private async void Initialize()
        {
            _dialogService.ShowLoading();

            ConnectedUser = ActiveUser.User;

            if (ConnectedUser.Role.RoleTypeName == "Default")
            {
                UserHaveApartment = false;
                UserNotHavingApartment = true;
            }
            else
            {
                UserNotHavingApartment = false;
                UserHaveApartment = true;
                ActiveUser.ApartmentGuid = await _apartmentService.GetApartmentByUser(ConnectedUser.UserId);
            }

            _dialogService.HideLoading();
        }
        private void InitializeCommands()
        {
            LogOutCommand = new Command(LogOut);
            JoinApartmentCommand = new Command(JoinApartment);
        }

        private async void JoinApartment()
        {
            if (ActiveUser.User.Role.RoleTypeName != "Default")
            {
                await _dialogService.ShowDialog("Nu te poti conecta la mai multe apartamente.", "Atentie!");
                return;
            }

            var result = await _dialogService.DisplayPromptAsync("Apartament", "Introdu codul apartamentului.");

            try
            {
                await _apartmentService.JoinApartment(ConnectedUser, result);

                await _dialogService.ShowDialog("Ai fost conectat cu succes.", "Succes");

                App.Current.MainPage = new AppShell();
            }
            catch (Exception ex)
            {
                await _dialogService.ShowDialog(ex.Message, "Atentie!");
            }
        }

        private void LogOut()
        {
            ActiveUser.User = new User();
            ActiveUser.ApartmentGuid = null;
            App.Current.MainPage = new NavigationPage(new LogInView());
        }

        #endregion
    }
}
