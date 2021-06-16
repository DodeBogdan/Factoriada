using System;
using System.Collections.Generic;
using System.Windows.Input;
using Factoriada.Models;
using Factoriada.Services.Interfaces;
using Factoriada.Utility;
using Factoriada.Views;
using Xamarin.Forms;

namespace Factoriada.ViewModels
{
    public class AddBillViewModel: ViewModelBase
    {
        #region Constructor
        public AddBillViewModel(IDialogService dialogService, INavigationService navigationService, IApartmentService apartmentService) : base(dialogService, navigationService)
        {
            _apartmentService = apartmentService ?? throw new ArgumentNullException(nameof(apartmentService));
            Initialize();
            InitializeCommands();
        }
        #endregion

        #region Private Fileds
        private readonly IApartmentService _apartmentService;
        private Guid _apartmentDetail;
        private List<BillType> _billTypes;
        private BillType _selectedBillType;
        private float _billPrice;
        private DateTime _billDateOfIssue;
        private DateTime _billStartDate;
        private DateTime _billDueDate;
        private bool _billIsPaid;

        #endregion

        #region Proprieties

        public bool BillIsPaid
        {
            get => _billIsPaid;
            set
            {
                _billIsPaid = value;
                OnPropertyChanged();
            }
        }
        public DateTime BillDueDate
        {
            get => _billDueDate;
            set
            {
                _billDueDate = value;
                OnPropertyChanged();
            }
        }
        public DateTime BillStartDate
        {
            get => _billStartDate;
            set
            {
                _billStartDate = value;
                OnPropertyChanged();
            }
        }
        public DateTime BillDateOfIssue
        {
            get => _billDateOfIssue;
            set
            {
                _billDateOfIssue = value;
                OnPropertyChanged();
            }
        }
        public float BillPrice
        {
            get => _billPrice;
            set
            {
                _billPrice = value;
                OnPropertyChanged();
            }
        }
        public BillType SelectedBillType
        {
            get => _selectedBillType;
            set
            {
                _selectedBillType = value;
                OnPropertyChanged();
            }
        }
        public List<BillType> BillTypes
        {
            get => _billTypes;
            set
            {
                _billTypes = value;
                OnPropertyChanged();
            }
        }

        public ICommand AddBillCommand { get; set; }
        #endregion

        #region Private Methods

        private void InitializeCommands()
        {
            AddBillCommand = new Command(AddBill);
        }

        private async void AddBill()
        {
            if (SelectedBillType == BillType.Tip)
            {
                await _dialogService.ShowDialog("Alege un tip.", "Atentie!");
                return;
            }

            if (BillPrice == 0)
            {
                var result =
                    await _dialogService.DisplayAlert("Atentie!", "Pretul facturii este 0. Doresti sa continui?");

                if (result == false)
                    return;
            }

            var bill = new Bill()
            {
                BillId = Guid.NewGuid(),
                ApartmentDetail = _apartmentDetail,
                BillPrice = BillPrice,
                DateOfIssue = BillDateOfIssue,
                StartDate = BillStartDate,
                DueDate = BillDueDate,
                Paid = BillIsPaid,
                Type = SelectedBillType
            };

            if (bill.BillPrice <= 0)
                bill.Paid = true;

            try
            {
                _dialogService.ShowLoading();

                await _apartmentService.AddBill(bill);

                var reminder = new Reminder()
                {
                    ReminderId = Guid.NewGuid(),
                    ApartmentDetail = _apartmentDetail,
                    Message =
                        $"A fost adaugata factura la {SelectedBillType} cu data scadenta la data de: {BillDateOfIssue:d}."
                };

                await _apartmentService.AddReminder(reminder);

                _dialogService.HideLoading();

                await _dialogService.ShowDialog("Factura a fost adaugata cu succes.", "Succes");
                Reset();

                await _navigationService.PopAsync();
            }
            catch (Exception ex)
            {
                _dialogService.HideLoading();
                await _dialogService.ShowDialog(ex.Message, "Atentie!");
            }
        }

        private void Reset()
        {
            BillDateOfIssue = BillDueDate = BillStartDate = DateTime.Parse("01.01.2021");
            BillIsPaid = false;
            BillPrice = 0.0f;
            SelectedBillType = BillType.Tip;
        }

        private void Initialize()
        {
            _dialogService.ShowLoading();
            BillTypes = Bill.GetBillTypes();
            SelectedBillType = BillType.Tip;
            BillDateOfIssue = BillDueDate = BillStartDate = DateTime.Parse("01.01.2021");
            _apartmentDetail = ActiveUser.ApartmentGuid.ApartmentDetailId;
            _dialogService.HideLoading();
        }
        #endregion
    }
}