using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Factoriada.Models;
using Factoriada.Services.Interfaces;
using Factoriada.Utility;
using Xamarin.Forms;

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
        private bool _deleteUserIsVisible;
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

        public bool DeleteUserIsVisible
        {
            get => _deleteUserIsVisible;
            set
            {
                _deleteUserIsVisible = value;
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
                        DeleteUserIsVisible = true;
                    }
                    else
                    {
                        DeleteUserIsVisible = false;
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
        #endregion

        #region Private Methods

        private async void Initialize()
        {
             _dialogService.ShowLoading();
            _apartmentId = await _apartmentService.GetApartmentIdByUser(ActiveUser.User.UserId);
            UserList = await _apartmentService.GetUsersByApartment(_apartmentId);
            if (ActiveUser.User.Role.RoleTypeName == "Owner")
                IsOwner = true;
            _dialogService.HideLoading();
        }

        private void InitializeCommands()
        {
            DeleteUserCommand = new Command(DeleteUser);
            AddPersonCommand = new Command(AddUser);
        }

        private async void DeleteUser()
        {
            _dialogService.ShowLoading();
            await _apartmentService.RemoveUserFromApartment(SelectedUser);
            UserList = await _apartmentService.GetUsersByApartment(_apartmentId);
            _dialogService.HideLoading();
        }

        private async void AddUser()
        {
            var result = await _dialogService.DisplayPromptAsync("Adauga user", "Care este email-ul user-ului?",
                placeholder: "exemplu@yahoo.com");

            _dialogService.ShowLoading();

            try
            {
                var user = await _apartmentService.GetUserByEmail(result);
                var token = (await _apartmentService.GetApartmentByUser(ActiveUser.User.UserId))
                    .Token;
                await _apartmentService.JoinApartment(user, token);
                UserList = await _apartmentService.GetUsersByApartment(_apartmentId);
                _dialogService.HideLoading();
            }
            catch (Exception ex)
            {
                _dialogService.HideLoading();
                await _dialogService.ShowDialog(ex.Message, "Atentie!");
            }
        }

        #endregion
    }
}
