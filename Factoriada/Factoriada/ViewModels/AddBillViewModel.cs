﻿using System;
using System.Collections.Generic;
using System.Windows.Input;
using Factoriada.Models;
using Factoriada.Services.Interfaces;
using Factoriada.Utility;
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
            var bill = new Bill()
            {
                BillId = Guid.NewGuid(),
                ApartmentDetail = await _apartmentService.GetApartmentByUser(ActiveUser.User.UserId),
                BillPrice = BillPrice,
                DateOfIssue = BillDateOfIssue,
                StartDate = BillStartDate,
                DueDate = BillDueDate,
                Paid = BillIsPaid,
                Type = SelectedBillType
            };
            try
            {
                await _apartmentService.AddBill(bill);

                await _dialogService.ShowDialog("Factura a fost adaugata cu succes.", "Succes");
                Reset();
            }
            catch (Exception ex)
            {
                await _dialogService.ShowDialog(ex.Message, "Atentie!");
            }
        }

        private void Reset()
        {
            BillDateOfIssue = BillDueDate = BillStartDate = DateTime.Parse("01.01.2000");
            BillIsPaid = false;
            BillPrice = 0.0f;
            SelectedBillType = BillType.None;
        }

        private void Initialize()
        {
            BillTypes = Bill.GetBillTypes();
            SelectedBillType = BillType.Curent;
            BillDateOfIssue = BillDueDate = BillStartDate = DateTime.Parse("01.01.2000");
        }
        #endregion
    }
}