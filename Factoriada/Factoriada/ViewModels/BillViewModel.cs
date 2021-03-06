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
        private Guid _apartmentId;
        private List<Reminder> _reminderList;
        private Reminder _currentReminder;
        private bool _isReminderSelected;
        #endregion

        #region Proprieties

        public bool IsReminderSelected
        {
            get => _isReminderSelected;
            set
            {
                _isReminderSelected = value;
                OnPropertyChanged();
            }
        }

        public Reminder CurrentReminder
        {
            get => _currentReminder;
            set
            {
                _currentReminder = value;
                if (value != null)
                {
                    if (ActiveUser.User.Role.RoleTypeName == "Owner")
                    {
                        IsReminderSelected = true;
                    }
                }
                else
                {
                    IsReminderSelected = false;
                }
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
        public ICommand DeleteReminderCommand { get; set; }
        #endregion

        #region Private Methods
        private void InitializeCommands()
        {
            AddBillCommand = new Command(AddBill);
            SeeBillsCommand = new Command(SeeBills);
            DeleteReminderCommand = new Command(DeleteReminder);
        }

        public async void Initialize()
        {
            _dialogService.ShowLoading();
            _apartmentId = ActiveUser.ApartmentGuid.ApartmentDetailId;
            ReminderList = await _apartmentService.GetRemindersByApartmentId(_apartmentId);
            _dialogService.HideLoading();
        }

        private async void AddBill()
        {
            await _navigationService.PushAsync(new AddBillView());
        }

        private async void SeeBills()
        {
            await _navigationService.PushAsync(new SeeBillsView());
        }

        private async void DeleteReminder()
        {
            _dialogService.ShowLoading();
            await _apartmentService.DeleteReminder(CurrentReminder);
            ReminderList.Remove(CurrentReminder);
            ReminderList = new List<Reminder>(ReminderList);
            CurrentReminder = null;
            _dialogService.HideLoading();

            await _dialogService.ShowDialog("Reminder-ul a fost sters cu succes.", "Succes");
        }
        #endregion
    }
}