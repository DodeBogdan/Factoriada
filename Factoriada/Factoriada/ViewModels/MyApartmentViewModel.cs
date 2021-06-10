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

            ApartmentAddress = await _apartmentService.GetApartmentAddressByUser(ActiveUser.User.UserId);
            Token = (await _apartmentService.GetApartmentByUser(ActiveUser.User.UserId)).Token;
            if (ActiveUser.User.Role.RoleTypeName == "Owner")
            {
                IsOwner = true;
            }
            else
            {
                IsUser = true;
            }

            _dialogService.HideLoading();
        }

        private async void ExitApartment()
        {
            _dialogService.ShowLoading();
            await _apartmentService.RemoveUserFromApartment(ActiveUser.User);
            _dialogService.HideLoading();
            App.Current.MainPage = new AppShell();
        }

        private async void DeleteApartment()
        {
            _dialogService.ShowLoading();
            var apartment = await _apartmentService.GetApartmentByUser(ActiveUser.User.UserId);
            await _apartmentService.DeleteApartment(apartment);
            _dialogService.HideLoading();
            App.Current.MainPage = new AppShell();
        }
        #endregion
    }
}