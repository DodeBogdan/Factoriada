using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Factoriada.Models;
using Factoriada.Services.Interfaces;
using Factoriada.Utility;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Factoriada.ViewModels
{
    internal class SeePersonsFromApartmentViewModel : ViewModelBase
    {
        #region Constructor
        public SeePersonsFromApartmentViewModel(IDialogService dialogService, INavigationService navigationService, IApartmentService apartmentService) : base(dialogService, navigationService)
        {
            _apartmentService = apartmentService ?? throw new ArgumentNullException(nameof(apartmentService));
            Initialize();
            InitializeCommands();
        }
        #endregion

        #region Private Fields

        private readonly IApartmentService _apartmentService;
        private Guid _apartmentId;
        private List<User> _userList;
        private User _selectedUser;
        private bool _userIsSelected;
        #endregion

        #region Proprieties
        private bool _isOwner;

        public bool IsOwner
        {
            get => _isOwner;
            set
            {
                _isOwner = value;
                OnPropertyChanged();
            }
        }

        public bool UserIsSelected
        {
            get => _userIsSelected;
            set
            {
                _userIsSelected = value;
                OnPropertyChanged();
            }
        }

        public User SelectedUser
        {
            get => _selectedUser;
            set
            {
                _selectedUser = value;
                if (value != null)
                {
                    if (ActiveUser.User.Role.RoleTypeName == "Owner" && value.Role.RoleTypeName != "Owner")
                    {
                        UserIsSelected = true;
                    }
                    else
                    {
                        UserIsSelected = false;
                    }
                }
                OnPropertyChanged();
            }
        }

        public List<User> UserList
        {
            get => _userList;
            set
            {
                _userList = value;
                OnPropertyChanged();
            }
        }

        public ICommand AddPersonCommand { get; set; }
        public ICommand DeleteUserCommand { get; set; }
        public ICommand ChangeOwnerCommand { get; set; }
        #endregion

        #region Private Methods

        private async void Initialize()
        {
             _dialogService.ShowLoading();
            _apartmentId = ActiveUser.ApartmentGuid.ApartmentDetailId;
            UserList = await _apartmentService.GetUsersByApartment(_apartmentId);
            if (ActiveUser.User.Role.RoleTypeName == "Owner")
                IsOwner = true;
            _dialogService.HideLoading();
        }

        private void InitializeCommands()
        {
            DeleteUserCommand = new Command(DeleteUser);
            AddPersonCommand = new Command(AddUser);
            ChangeOwnerCommand = new Command(ChangeOwner);
        }

        private async void DeleteUser()
        {
            _dialogService.ShowLoading();
            await _apartmentService.RemoveUserFromApartment(SelectedUser);
            UserList = await _apartmentService.GetUsersByApartment(_apartmentId);
            _dialogService.HideLoading();

            await _dialogService.ShowDialog("User-ul a fost sters cu succes.", "Succes");
        }

        private async void AddUser()
        {
            var result = await _dialogService.DisplayPromptAsync("Adauga user", "Care este email-ul user-ului?",
                placeholder: "exemplu@yahoo.com");

            if (string.IsNullOrEmpty(result))
            {
                await _dialogService.ShowDialog("Email-ul nu a fost introdus.", "Atentie!");
                return;
            }

            _dialogService.ShowLoading();

            try
            {
                var user = await _apartmentService.GetUserByEmail(result);
                var token = (await _apartmentService.GetApartmentByUser(ActiveUser.User.UserId))
                    .Token;
                await _apartmentService.JoinApartment(user, token);
                UserList = await _apartmentService.GetUsersByApartment(_apartmentId);
                _dialogService.HideLoading();

                await _dialogService.ShowDialog("Persoana a fost introdusa cu succes..", "Succes!");
            }
            catch (Exception ex)
            {
                _dialogService.HideLoading();
                await _dialogService.ShowDialog(ex.Message, "Atentie!");
            }
        }

        private async void ChangeOwner()
        {
            var result =
                await _dialogService.DisplayAlert("Schimbare proprietar", "Esti sigur ca vrei sa schimbi owner-ul");

            if (result == false)
                return;

            _dialogService.ShowLoading();
            await _apartmentService.ChangeOwner(_apartmentId, SelectedUser);
            UserList = await _apartmentService.GetUsersByApartment(_apartmentId);
            ActiveUser.User = await _apartmentService.GetUserByEmail(ActiveUser.User.Email);
            SelectedUser = null;
            IsOwner = UserIsSelected = false;
            _dialogService.HideLoading();

            await _dialogService.ShowDialog("Owner-ul a fost schimbat cu succes.", "Succes");
        }

        #endregion
    }
}
