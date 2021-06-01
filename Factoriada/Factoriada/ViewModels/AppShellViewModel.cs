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
            ConnectedUser = ActiveUser.User;

            InitializeCommands();
        }

        #endregion

        #region Private Fields

        private readonly IApartmentService _apartmentService;

        private User _connectedUser = new User();
        private bool _userType = true;

        #endregion

        #region Proprieties
        private ImageSource _currentUserImage;

        public ImageSource CurrentUserImage
        {
            get => _currentUserImage;
            set
            {
                _currentUserImage = value;
                OnPropertyChanged();
            }
        }

        public bool UserType
        {
            get => _userType;
            set
            {
                _userType = value;
                OnPropertyChanged();
            }
        }

        public User ConnectedUser
        {
            get => _connectedUser;
            set
            {
                _connectedUser = value;
                UserType = value.Role.RoleTypeName == "Default";
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

        private void InitializeCommands()
        {
            LogOutCommand = new Command(LogOut);
            JoinApartmentCommand = new Command(JoinApartment);
        }

        private async void JoinApartment()
        {
            var result = await _dialogService.DisplayPromptAsync("Apartament", "Introdu codul apartamentului.");

            try
            {
                await _apartmentService.JoinApartment(ConnectedUser, result);

                await _dialogService.ShowDialog("Ai fost conectat cu succes.", "Succes");
            }
            catch (Exception ex)
            {
                await _dialogService.ShowDialog(ex.Message, "Atentie!");
            }
        }

        private void LogOut()
        {
            ActiveUser.User = new User();
            App.Current.MainPage = new NavigationPage(new LogInView());
        }


        #endregion



    }
}
