using System;
using System.Windows.Input;
using Factoriada.Models;
using Factoriada.Services.Interfaces;
using Factoriada.Utility;
using Factoriada.Views;
using Xamarin.Forms;

namespace Factoriada.ViewModels
{
    public class MyApartmentViewModel : ViewModelBase
    {
        #region Constructor
        public MyApartmentViewModel(IDialogService dialogService, INavigationService navigationService, IApartmentService apartmentService) : base(dialogService, navigationService)
        {
            _apartmentService = apartmentService;

            InitializeCommands();
            InitializeAddress();
        }
        #endregion

        #region Private Fields
        private readonly IApartmentService _apartmentService;
        private Guid _currentApartment;
        private string _apartmentAddress;
        private string _token = "";
        private bool _isOwner;
        private bool _isUser;

        #endregion

        #region Proprieties

        public bool IsUser
        {
            get => _isUser;
            set
            {
                _isUser = value;
                OnPropertyChanged();
            }
        }

        public bool IsOwner
        {
            get => _isOwner;
            set
            {
                _isOwner = value;
                OnPropertyChanged();
            }
        }

        public string Token
        {
            get => _token;
            set
            {
                _token = value;
                OnPropertyChanged();
            }
        }

        public string ApartmentAddress
        {
            get => _apartmentAddress;
            set
            {
                _apartmentAddress = value;
                OnPropertyChanged();
            }
        }

        public ICommand GoToRulesCommand { get; set; }
        public ICommand GoToAnnouncementsCommand { get; set; }
        public ICommand GoToBillsCommand { get; set; }
        public ICommand GoToBuyListCommand { get; set; }
        public ICommand GoToCleanCommand { get; set; }
        public ICommand GoToBudgetCommand { get; set; }
        public ICommand GoToChatCommand { get; set; }
        public ICommand GoToTimeAwayCommand { get; set; }
        public ICommand GoToSeePersonsFromApartmentCommand { get; set; }
        public ICommand GoToDeleteApartmentCommand { get; set; }
        public ICommand GoToExitApartmentCommand { get; set; }
        #endregion

        #region Private Methods

        private void InitializeCommands()
        {
            GoToRulesCommand = new Command(async () => await _navigationService.PushAsync(new RulesView()));
            GoToAnnouncementsCommand = new Command(async () => await _navigationService.PushAsync(new AnnounceView()));
            GoToBillsCommand = new Command(async () => await _navigationService.PushAsync(new BillView()));
            GoToBuyListCommand = new Command(async () => await _navigationService.PushAsync(new BuyListView()));
            GoToCleanCommand = new Command(async () => await _navigationService.PushAsync(new ApartmentChoresView()));
            GoToBudgetCommand = new Command(async () => await _navigationService.PushAsync(new BudgetView()));
            GoToChatCommand = new Command(async () => await _navigationService.PushAsync(new ChatView()));
            GoToTimeAwayCommand = new Command(async () => await _navigationService.PushAsync(new TimeAwayView()));
            GoToSeePersonsFromApartmentCommand = new Command(async () => await _navigationService.PushAsync(new SeePersonsFromApartmentView()));
            GoToExitApartmentCommand = new Command(ExitApartment);
            GoToDeleteApartmentCommand = new Command(DeleteApartment);
        }
        private async void InitializeAddress()
        {
            _dialogService.ShowLoading();

            if (ActiveUser.User.Role.RoleTypeName == "Owner")
            {
                IsOwner = true;
            }
            else
            {
                IsUser = true;
            }

            _currentApartment = await _apartmentService.GetApartmentIdByUser(ActiveUser.User.UserId);
            ApartmentAddress = await _apartmentService.GetApartmentAddressByUser(ActiveUser.User.UserId);
            Token = (await _apartmentService.GetApartmentByUser(ActiveUser.User.UserId)).Token;

            _dialogService.HideLoading();
        }

        private async void ExitApartment()
        {
            var result =
                await _dialogService.DisplayAlert("Paraseste apartamentul", "Doresti sa parasesti apartamentul?");

            if (result == false)
                return;

            _dialogService.ShowLoading();
            await _apartmentService.RemoveUserFromApartment(ActiveUser.User);

            var announce = new Announce
                { AnnounceId = Guid.NewGuid(), User = ActiveUser.User, AnnounceMessage = $"{ActiveUser.User.FullName} a parasit apartamentul." , InsertedDateTime = DateTime.Now };

            await _apartmentService.AddAnnounceToApartment(announce, _currentApartment);

            _dialogService.HideLoading();
            App.Current.MainPage = new AppShell();
        }

        private async void DeleteApartment()
        {
            var result =
                await _dialogService.DisplayAlert("Sterge apartamentul", "Doresti sa stergi apartamentul?");

            if (result == false)
                return;

            _dialogService.ShowLoading();
            var apartment = await _apartmentService.GetApartmentByUser(ActiveUser.User.UserId);
            await _apartmentService.DeleteApartment(apartment);
            _dialogService.HideLoading();
            App.Current.MainPage = new AppShell();
        }
        #endregion
    }
}