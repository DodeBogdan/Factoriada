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
            CurrentApartment = ActiveUser.ApartmentGuid;
            BudgetHistoryList = await _apartmentService.GetBudgetHistoryByApartmentId(CurrentApartment.ApartmentDetailId);
            _dialogService.HideLoading();
        }

        private async void AddMoney()
        {
            var result = await _dialogService.DisplayPromptAsync("Buget", "Introdu cati bani doresti sa adaugi.", keyboard: Keyboard.Numeric);

            if (string.IsNullOrEmpty(result))
            {
                await _dialogService.ShowDialog("Nu a fost introdus nimic.", "Atentie!");
                return;
            }

            if (float.Parse(result) > 500)
            {
                await _dialogService.ShowDialog("Nu poti adauga mai mult de 500 Ron intr-o tranzactie.", "Atentie!");
                return;
            }

            if (float.Parse(result) + CurrentApartment.UnspentMoney < 0)
            {
                await _dialogService.ShowDialog($"Nu poti extrage mai mult de {CurrentApartment.UnspentMoney} Ron", "Atentie!");
                return;
            }

            _dialogService.ShowLoading();

            var money = new BudgetHistory
            {
                BudgetHistoryId = Guid.NewGuid(),
                User = ActiveUser.User,
                ApartmentDetail = CurrentApartment.ApartmentDetailId,
                Amount = float.Parse(result),
                InsertedDateTime = DateTime.Now
            };

            await _apartmentService.AddMoneyToApartment(money);

            CurrentApartment.UnspentMoney += money.Amount;
            await _apartmentService.UpdateApartment(CurrentApartment);

            BudgetHistoryList.Add(money);
            BudgetHistoryList = new List<BudgetHistory>(BudgetHistoryList);
            CurrentApartment = CurrentApartment;

            _dialogService.HideLoading();
        }
        #endregion
    }
}