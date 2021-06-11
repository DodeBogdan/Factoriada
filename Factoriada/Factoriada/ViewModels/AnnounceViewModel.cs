using System;
using System.Collections.Generic;
using System.Windows.Input;
using Factoriada.Models;
using Factoriada.Services.Interfaces;
using Factoriada.Utility;
using Xamarin.Forms;

namespace Factoriada.ViewModels
{
    public class AnnounceViewModel : ViewModelBase
    {
        #region Constructor
        public AnnounceViewModel(IDialogService dialogService, INavigationService navigationService, IApartmentService apartmentService) : base(dialogService, navigationService)
        {
            _apartmentService = apartmentService;

            Initialize();
            InitializeCommands();
        }
        #endregion

        #region Private Fields
        private readonly IApartmentService _apartmentService;
        private Guid _apartmentId = Guid.Empty;
        private List<Announce> _announceList;
        private Announce _currentAnnounce;
        private bool _userIsOwnerOrCreator;
        #endregion

        #region Proprieties
        public bool UserIsOwnerOrCreator
        {
            get => _userIsOwnerOrCreator;
            set
            {
                _userIsOwnerOrCreator = value;
                OnPropertyChanged();
            }
        }
        public Announce CurrentAnnounce
        {
            get => _currentAnnounce;
            set
            {
                _currentAnnounce = value;
                if (CurrentAnnounce != null)
                    UserIsOwnerOrCreator = ActiveUser.User.Role.RoleTypeName == "Owner" ||
                                           CurrentAnnounce.User.Email == ActiveUser.User.Email;
                OnPropertyChanged();
            }
        }
        public List<Announce> AnnounceList
        {
            get => _announceList;
            set
            {
                _announceList = value;
                OnPropertyChanged();
            }
        }
        public ICommand AddAnnounceCommand { get; set; }
        public ICommand EditAnnounceCommand { get; set; }
        public ICommand DeleteAnnounceCommand { get; set; }
        #endregion

        #region Private Methods
        private void InitializeCommands()
        {
            AddAnnounceCommand = new Command(AddAnnounce);
            EditAnnounceCommand = new Command(EditAnnounce);
            DeleteAnnounceCommand = new Command(DeleteAnnounce);
        }
        private async void Initialize()
        {
            _dialogService.ShowLoading();

            _apartmentId = await _apartmentService.GetApartmentIdByUser(ActiveUser.User.UserId);
            AnnounceList = await _apartmentService.GetAnnouncesByApartmentId(_apartmentId);

            _dialogService.HideLoading();
        }
        private async void AddAnnounce()
        {
            var result = await _dialogService.DisplayPromptAsync("Anunt", "Introdu noul anunt.");

            if (result == null)
                return;

            _dialogService.ShowLoading();

            var announce = new Announce
            { AnnounceId = Guid.NewGuid(), User = ActiveUser.User, AnnounceMessage = result, InsertedDateTime = DateTime.Now};

            await _apartmentService.AddAnnounceToApartment(announce, _apartmentId);

            AnnounceList = await _apartmentService.GetAnnouncesByApartmentId(_apartmentId);
            UserIsOwnerOrCreator = false;
            _dialogService.HideLoading();
        }

        private async void EditAnnounce()
        {
            if (CurrentAnnounce == null)
                return;

            var result = await _dialogService.DisplayPromptAsync("Anunt", "Editeaza anuntul.", placeholder: CurrentAnnounce.AnnounceMessage);

            if (result == null)
                return;

            _dialogService.ShowLoading();

            CurrentAnnounce.AnnounceMessage = result;

            await _apartmentService.EditAnnounceFromApartment(CurrentAnnounce);

            AnnounceList = await _apartmentService.GetAnnouncesByApartmentId(_apartmentId);
            UserIsOwnerOrCreator = false;
            _dialogService.HideLoading();
        }
        private async void DeleteAnnounce()
        {
            if (CurrentAnnounce == null)
                return;

            _dialogService.ShowLoading();

            await _apartmentService.DeleteAnnounce(CurrentAnnounce);

            AnnounceList = await _apartmentService.GetAnnouncesByApartmentId(_apartmentId);
            UserIsOwnerOrCreator = false;
            _dialogService.HideLoading();
        }
        #endregion
    }
}