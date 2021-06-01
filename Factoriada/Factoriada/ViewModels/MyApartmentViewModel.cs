using System.Windows.Input;
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
        #endregion

        #region Proprieties
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
        #endregion

        #region Private Methods

        private void InitializeCommands()
        {
            GoToRulesCommand = new Command(async () => await _navigationService.PushAsync(new RulesView()));
            GoToAnnouncementsCommand = new Command(async () => await _navigationService.PushAsync(new AnnounceView()));
            GoToBillsCommand = new Command(async () => await _navigationService.PushAsync(new BillView()));
            GoToBuyListCommand = new Command(async () => await _navigationService.PushAsync(new BuyListView()));
            GoToCleanCommand = new Command(async () => await _dialogService.ShowDialog("Nu este implementat.", ""));
            GoToBudgetCommand = new Command(async () => await _navigationService.PushAsync(new BudgetView()));
            GoToChatCommand = new Command(async () => await _navigationService.PushAsync(new ChatView()));
        }
        private async void InitializeAddress()
        {
            ApartmentAddress = await _apartmentService.GetApartmentAddressByUser(ActiveUser.User.UserId);
        }


        #endregion
    }
}