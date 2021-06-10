using System;
using System.Collections.Generic;
using System.Windows.Input;
using Factoriada.Models;
using Factoriada.Services.Interfaces;
using Factoriada.Utility;
using Xamarin.Forms;

namespace Factoriada.ViewModels
{
    public class BudgetViewModel : ViewModelBase
    {
        public BudgetViewModel(IDialogService dialogService, INavigationService navigationService, IApartmentService apartmentService) : base(dialogService, navigationService)
        {
            _apartmentService = apartmentService;

            Initialize();
            InitializeCommands();
        }

        #region Private Fields
        private readonly IApartmentService _apartmentService;
        private ApartmentDetail _currentApartment;
        private List<BudgetHistory> _budgetHistoryList;
        #endregion

        #region Proprieties
        public List<BudgetHistory> BudgetHistoryList
        {
            get => _budgetHistoryList;
            set
            {
                _budgetHistoryList = value;
                OnPropertyChanged();
            }
        }
        public ApartmentDetail CurrentApartment
        {
            get => _currentApartment;
            set
            {
                _currentApartment = value;
                OnPropertyChanged();
            }
        }
        public ICommand AddMoneyCommand { get; set; }
        #endregion

        #region Private Methods
        private void InitializeCommands()
        {
            AddMoneyCommand = new Command(AddMoney);
        }

        private async void Initialize()
        {
            _dialogService.ShowLoading();
            CurrentApartment = await _apartmentService.GetApartmentByUser(ActiveUser.User.UserId);
            BudgetHistoryList = await _apartmentService.GetBudgetHistoryByApartmentId(CurrentApartment.ApartmentDetailId);
            _dialogService.HideLoading();
        }

        private async void AddMoney()
        {
            var money = new BudgetHistory()
            {
                BudgetHistoryId = Guid.NewGuid(),
                User = ActiveUser.User,
                ApartmentDetail = CurrentApartment
                
            };

            var result = await _dialogService.DisplayPromptAsync("Buget", "Introdu cati bani doresti sa adaugi.", keyboard: Keyboard.Numeric);

            _dialogService.ShowLoading();

            if (result == null)
                return;

            money.Amount = float.Parse(result);

            await _apartmentService.AddMoneyToApartment(money);

            CurrentApartment.UnspentMoney += money.Amount;
            await _apartmentService.UpdateApartment(CurrentApartment);

            BudgetHistoryList = await _apartmentService.GetBudgetHistoryByApartmentId(CurrentApartment.ApartmentDetailId);
            CurrentApartment = CurrentApartment;

            _dialogService.HideLoading();
        }
        #endregion
    }
}