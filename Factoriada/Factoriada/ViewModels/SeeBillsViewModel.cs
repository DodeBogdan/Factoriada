using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Factoriada.Models;
using Factoriada.Popups;
using Factoriada.Services.Interfaces;
using Factoriada.Utility;
using Factoriada.Views;
using Xamarin.CommunityToolkit.Extensions;
using Xamarin.Forms;

namespace Factoriada.ViewModels
{
    public class SeeBillsViewModel : ViewModelBase
    {
        #region Constructor
        public SeeBillsViewModel(IDialogService dialogService, INavigationService navigationService, IApartmentService apartmentService) : base(dialogService, navigationService)
        {
            _apartmentService = apartmentService;

            Initialize();
            InitializeCommands();
        }
        #endregion

        #region Private Fields

        private readonly IApartmentService _apartmentService;

        private List<Bill> _billList;
        private Guid _apartmentDetail;
        private bool _notStartedToPay = true;
        private Bill _selectedBill;
        private bool _startedToPay;
        private List<BillPaidPersons> _billPaidPersonsList;
        private bool _payButtonIsVisible;
        private List<string> _payButtonTextList = new List<string>(3);
        private List<Bill> _copyBillPaidPersonsList;
        private bool _monthIsDescending = false;
        private bool _priceIsDescending = false;
        #endregion

        #region Proprieties
        private string _payButtonText;
        private int _index;

        public int Index
        {
            get => _index;
            set
            {
                _index = value;
                if (value >= _payButtonTextList.Count)
                    _index = 0;
            }
        }

        public string PayButtonText
        {
            get => _payButtonText;
            set
            {
                _payButtonText = value;
                OnPropertyChanged();
            }
        }

        public bool PayButtonIsVisible
        {
            get => _payButtonIsVisible;
            set
            {
                _payButtonIsVisible = value;
                OnPropertyChanged();
            }
        }

        public List<BillPaidPersons> BillPaidPersonsList
        {
            get => _billPaidPersonsList;
            set
            {
                _billPaidPersonsList = value;
                OnPropertyChanged();
            }
        }

        public bool StartedToPay
        {
            get => _startedToPay;
            set
            {
                _startedToPay = value;
                OnPropertyChanged();
            }
        }

        public bool NotStartedToPay
        {
            get => _notStartedToPay;
            set
            {
                _notStartedToPay = value;
                OnPropertyChanged();
            }
        }

        public Bill SelectedBill
        {
            get => _selectedBill;
            set
            {
                _selectedBill = value;
                if (value != null)
                    BillHasBeenChoose();
                OnPropertyChanged();
            }
        }

        public List<Bill> BillList
        {
            get => _billList;
            set
            {
                _billList = value;
                OnPropertyChanged();
            }
        }

        public ICommand PayBillCommand { get; set; }
        public ICommand CancelCommand { get; set; }
        public ICommand SortByPaidCommand { get; set; }
        public ICommand SortByMonthCommand { get; set; }
        public ICommand SortByPriceCommand { get; set; }

        #endregion

        #region Private Methods

        private async void Initialize()
        {
            _dialogService.ShowLoading();

            _payButtonTextList.Add("Platite");
            _payButtonTextList.Add("Neplatite");
            _payButtonTextList.Add("Toate");

            Index = 0;
            PayButtonText = _payButtonTextList[Index];

            _apartmentDetail = ActiveUser.ApartmentGuid.ApartmentDetailId;
            BillList = await _apartmentService.GetBillsByApartment(_apartmentDetail);
            _copyBillPaidPersonsList = BillList;

            _dialogService.HideLoading();
        }

        private void InitializeCommands()
        {
            PayBillCommand = new Command(PayBill);
            CancelCommand = new Command(Cancel);
            SortByPaidCommand = new Command(SortByPaid);
            SortByMonthCommand = new Command(SortByMonth);
            SortByPriceCommand = new Command(SortByPrice);
        }

        private void SortByPrice()
        {
            if (_priceIsDescending)
            {
                BillList = BillList
                    .OrderByDescending(x => x.BillPrice)
                    .ToList();
            }
            else
            {
                BillList = BillList
                    .OrderBy(x => x.BillPrice)
                    .ToList();
            }

            _priceIsDescending = !_priceIsDescending;
        }

        private void SortByMonth()
        {
            if (_monthIsDescending)
            {
                BillList = BillList
                    .OrderByDescending(x => x.DateOfIssue)
                    .ToList();
            }
            else
            {
                BillList = BillList
                    .OrderBy(x => x.DateOfIssue)
                    .ToList();
            }

            _monthIsDescending = !_monthIsDescending;
        }

        private void SortByPaid()
        {
            Index++;
            PayButtonText = _payButtonTextList[Index];

            switch (Index)
            {
                case 0:
                    _dialogService.ShowLoading();
                    BillList = _copyBillPaidPersonsList;
                    _dialogService.HideLoading();
                    break;

                case 1:
                    _dialogService.ShowLoading();
                    BillList = _copyBillPaidPersonsList
                        .Where(x => x.Paid == true).ToList();
                    _dialogService.HideLoading();
                    break;

                case 2:
                    _dialogService.ShowLoading();
                    BillList = _copyBillPaidPersonsList
                        .Where(x => x.Paid == false).ToList();
                    _dialogService.HideLoading();
                    break;
            }
        }

        private async void PayBill()
        {
            _dialogService.ShowLoading();

            await _apartmentService.PayBill(SelectedBill);
            BillList = await _apartmentService.GetBillsByApartment(_apartmentDetail);
            _copyBillPaidPersonsList = BillList;

            var reminder = new Reminder()
            {
                ApartmentDetail = _apartmentDetail,
                Message = $"A fost platita factura la {SelectedBill.Type} in data de {DateTime.Now:d}",
                ReminderId = Guid.NewGuid()
            };

            await _apartmentService.AddReminder(reminder);

            _dialogService.HideLoading();
            await _dialogService.ShowDialog("Factura a fost platita cu succes.", "Succes");
            Cancel();
        }

        private void Cancel()
        {
            NotStartedToPay = true;
            StartedToPay = false;
            PayButtonIsVisible = false;
        }

        private async void ShowDetailsOfBill()
        {
            _dialogService.ShowLoading();

            NotStartedToPay = false;
            StartedToPay = true;
            BillPaidPersonsList =
                await _apartmentService.GenerateBillPaidPersons(SelectedBill,
                    _apartmentDetail);

            _dialogService.HideLoading();
        }

        private async void BillHasBeenChoose()
        {
            if (SelectedBill.Paid)
            {
                var result = await App.Current.MainPage.Navigation.ShowPopupAsync(new BillPopup("Paid"));

                switch (result)
                {
                    case null:
                    case "Cancel":
                        SelectedBill = null;
                        return;
                    case "See Details":
                        ShowDetailsOfBill();
                        break;
                    case "Delete":
                        await _apartmentService.DeleteBill(SelectedBill);
                        await _dialogService.ShowDialog("Factura a fost stearsa.", "Succes");
                        BillList = await _apartmentService.GetBillsByApartment(_apartmentDetail);
                        _copyBillPaidPersonsList = BillList;
                        SelectedBill = null;
                        break;
                }
            }
            else
            {
                var result = await App.Current.MainPage.Navigation.ShowPopupAsync(new BillPopup(""));

                switch (result)
                {
                    case null:
                    case "Cancel":
                        SelectedBill = null;
                        return;
                    case "Pay":
                        ShowDetailsOfBill();
                        PayButtonIsVisible = true;
                        break;
                    case "Delete":
                        _dialogService.ShowLoading();

                        await _apartmentService.DeleteBill(SelectedBill);
                        await _dialogService.ShowDialog("Factura a fost stearsa.", "Succes");
                        BillList = await _apartmentService.GetBillsByApartment(_apartmentDetail);
                        _copyBillPaidPersonsList = BillList;
                        SelectedBill = null;

                        _dialogService.HideLoading();
                        break;
                }
            }
        }

        #endregion
    }
}