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
    public class BillViewModel : ViewModelBase
    {
        public BillViewModel(IDialogService dialogService, INavigationService navigationService, IApartmentService apartmentService) : base(dialogService, navigationService)
        {
            _apartmentService = apartmentService ?? throw new ArgumentNullException(nameof(apartmentService));

            Initialize();
            InitializeCommands();
        }

        #region Private Fields
        private readonly IApartmentService _apartmentService;

        private Guid _apartmentId = Guid.Empty;

        private List<Reminder> _reminderList;
        private Reminder _currentReminder;

        #endregion

        #region Proprieties
        public Reminder CurrentReminder
        {
            get => _currentReminder;
            set
            {
                _currentReminder = value;
                OnPropertyChanged();
            }
        }
        public List<Reminder> ReminderList
        {
            get => _reminderList;
            set
            {
                _reminderList = value;
                OnPropertyChanged();
            }
        }

        public ICommand AddBillCommand { get; set; }
        public ICommand SeeBillsCommand { get; set; }
        public ICommand SetReminderCommand { get; set; }
        #endregion

        #region Private Methods
        private void InitializeCommands()
        {
            AddBillCommand = new Command(AddBill);
            SeeBillsCommand = new Command(SeeBills);
            SetReminderCommand = new Command(SetReminder);
        }

        private async void Initialize()
        {
            _apartmentId = await _apartmentService.GetApartmentIdByUser(ActiveUser.User.UserId);
            //ReminderList = await _apartmentService.GetRemindersByApartmentId(_apartmentId);
        }

        private async void AddBill()
        {
            await _navigationService.PushAsync(new AddBillView());
        }

        private async void SeeBills()
        {
            await _navigationService.PushAsync(new SeeBillsView());
        }

        private async void SetReminder()
        {
        }
        #endregion
    }
}