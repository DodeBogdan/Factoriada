using Factoriada.Models;
using Factoriada.Services.Interfaces;
using Factoriada.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace Factoriada.ViewModels
{
    internal class UserAccountViewModel : ViewModelBase
    {
        #region Constructor
        public UserAccountViewModel(IDialogService dialogService, INavigationService navigationService, IUserService userService) : base(dialogService, navigationService)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));

            InitializeCommands();
        }
        #endregion

        #region Private Fields 
        private readonly IUserService _userService;

        private bool _isRefreshing;
        private ImageSource _userImage;
        private bool _isProfileEditable = true;
        private User _currentUser = new User();
        private bool _startEditProfile;
        private bool _endEditProfile;
        private bool _editAddressVisible;
        private Address _userAddress = new Address();
        private bool _isUserAddressEditable = true;
        private bool _passwordChangeVisible;
        private string _oldPassword = "";
        private string _newPassword = "";
        private string _repeatNewPassword = "";
        private bool _editAddressIsVisible = true;
        private bool _saveAddressIsVisible;

        #endregion

        #region Proprieties

        public bool SaveAddressIsVisible
        {
            get => _saveAddressIsVisible;
            set
            {
                _saveAddressIsVisible = value;
                OnPropertyChanged();
            }
        }

        public bool EditAddressIsVisible
        {
            get => _editAddressIsVisible;
            set
            {
                _editAddressIsVisible = value;
                OnPropertyChanged();
            }
        }

        public string RepeatNewPassword
        {
            get => _repeatNewPassword;
            set
            {
                _repeatNewPassword = value;
                OnPropertyChanged();
            }
        }
        public string NewPassword
        {
            get => _newPassword;
            set
            {
                _newPassword = value; 
                OnPropertyChanged();
            }
        }
        public string OldPassword
        {
            get => _oldPassword;
            set
            {
                _oldPassword = value;
                OnPropertyChanged();
            }
        }
        public bool PasswordChangeVisible
        {
            get => _passwordChangeVisible;
            set
            {
                _passwordChangeVisible = value;
                OnPropertyChanged();
            }
        }

        public bool IsUserAddressEditable
        {
            get => _isUserAddressEditable;
            set
            {
                _isUserAddressEditable = value;
                OnPropertyChanged();
            }
        }
        public Address UserAddress
        {
            get => _userAddress;
            set
            {
                _userAddress = value;
                OnPropertyChanged();
            }
        }
        public bool EditAddressVisible
        {
            get => _editAddressVisible;
            set
            {
                _editAddressVisible = value;
                OnPropertyChanged();
            }
        }
        public bool EndEditProfile
        {
            get => _endEditProfile;
            set
            {
                _endEditProfile = value;
                OnPropertyChanged();
            }
        }

        public bool StartEditProfile
        {
            get => _startEditProfile;
            set
            {
                _startEditProfile = value;
                EndEditProfile = !EndEditProfile;
                OnPropertyChanged();
            }
        }

        public User CurrentUser
        {
            get => _currentUser;
            set
            {
                _currentUser = value;
                OnPropertyChanged();
            }
        }
        public bool IsProfileEditable
        {
            get => _isProfileEditable;
            set
            {
                _isProfileEditable = value;
                OnPropertyChanged();
            }
        }
        public ImageSource UserImage
        {
            get => _userImage;
            set
            {
                _userImage = value;
                OnPropertyChanged();
            }
        }
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
        public ICommand ChangePasswordCommand { get; set; }
        public ICommand StartEditAddressCommand { get; set; }
        public ICommand EditProfileCommand { get; set; }
        public ICommand SaveProfileCommand { get; set; }
        public ICommand CancelEditAddressCommand { get; set; }
        public ICommand EditAddressCommand { get; set; }
        public ICommand SaveAddressCommand { get; set; }
        public ICommand SavePasswordChangeCommand { get; set; }
        public ICommand CancelPasswordChangeCommand { get; set; }
        #endregion

        #region Private Methods
        private void InitializeCommands()
        {
            Refresh();

            RefreshCommand = new Command(Refresh);
            EditProfileCommand = new Command(EditProfile);
            SaveProfileCommand = new Command(SaveProfile);
            StartEditAddressCommand = new Command(StartEditAddress);
            CancelEditAddressCommand = new Command(CancelEditAddress);
            EditAddressCommand = new Command(EditAddress);
            SaveAddressCommand = new Command(SaveAddress);
            ChangePasswordCommand = new Command(ChangePassword);
            SavePasswordChangeCommand = new Command(SavePasswordChange);
            CancelPasswordChangeCommand = new Command(CancelPasswordChange);
        }

        private void Refresh()
        {
            _isProfileEditable = true;
            StartEditProfile = true;
            EndEditProfile = false;
            EditAddressVisible = false;
            IsRefreshing = false;

            CurrentUser = GetActiveUser();
            UserAddress = GetActiveUserAddress();
            SetUserImage();
        }

        private bool CheckPassword()
        {
            if (NewPassword != RepeatNewPassword)
            {
                _dialogService.ShowDialog("Parolele nu corespund.", "Atentie!");
                return false;
            }

            if (CurrentUser.Password != OldPassword)
            {
                _dialogService.ShowDialog("Vechea parola nu este buna.", "Atentie!");
                return false;
            }

            return true;
        }
        private async void SavePasswordChange()
        {
            if (CheckPassword() == false)
                return;
            try
            {
                await _userService.ChangePassword(CurrentUser, NewPassword);

                CancelPasswordChange();
            }
            catch (Exception ex)
            {
               await _dialogService.ShowDialog(ex.Message, "Atentie!");
            }
        }
        private void CancelPasswordChange()
        {
            OldPassword = NewPassword = RepeatNewPassword = "";
            PasswordChangeVisible = false;
            StartEditProfile = true;
        }
        private void ChangePassword()
        {
            PasswordChangeVisible = true;
            StartEditProfile = false;
        }
        private void EditAddress()
        {
            IsUserAddressEditable = false;
            EditAddressIsVisible = false;
            SaveAddressIsVisible = true;
        }
        private void SaveAddress()
        {
            IsUserAddressEditable = true;
            EditAddressIsVisible = true;
            SaveAddressIsVisible = false;
        }
        private void CancelEditAddress()
        {
            Refresh();
        }
        private void StartEditAddress()
        {
            StartEditProfile = false;
            EditAddressVisible = true;
        }

        private async void SaveProfile()
        {
            try
            {
               await _userService.ChangeProfile(CurrentUser);

               await _dialogService.ShowDialog("Modificarile au fost facute cu succes.", "Succes");
            }
            catch (Exception ex)
            {
               await _dialogService.ShowDialog(ex.Message, "Atentie!");
            }

            Refresh();
        }
        private void EditProfile()
        {
            IsProfileEditable = false;
            StartEditProfile = false;
            EndEditProfile = true;
        }
        private static User GetActiveUser()
        {
            return ActiveUser.User ?? new User();
        }

        private static Address GetActiveUserAddress()
        {
            if (ActiveUser.User == null)
                return new Address();

            return ActiveUser.User.Address ?? new Address();
        }

        private void SetUserImage()
        {
            if (ActiveUser.User == null)
                return;

            if (ActiveUser.User.ImagesByte == null)
                return;

            var stream = new MemoryStream(ActiveUser.User.ImagesByte);
            UserImage = ImageSource.FromStream(() => stream);
        }
        #endregion

    }
}
