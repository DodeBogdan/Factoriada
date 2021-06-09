using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Factoriada.Models;
using Factoriada.Services.Interfaces;
using Factoriada.Utility;
using Xamarin.Forms;

namespace Factoriada.ViewModels
{
    class TimeAwayViewModel : ViewModelBase
    {
        #region Constructor
        public TimeAwayViewModel(IDialogService dialogService, INavigationService navigationService, IApartmentService apartmentService) : base(dialogService, navigationService)
        {
            _apartmentService = apartmentService;

            Initialize();
            InitializeCommands();
        }

        #endregion

        #region Private Fields

        private readonly IApartmentService _apartmentService;

        private DateTime _minimumDateTime;
        private DateTime _startDateTime;
        private DateTime _maximumDateTime;
        private DateTime _endDateTime;
        private List<TimeAway> _timeAwayList;
        private TimeAway _selectedTimeAway;
        private bool _isSelectedTimeAwayToEdit;

        #endregion

        #region Proprieties
        private DateTime _minimDateForMaximumDateTime;

        public DateTime MinimDateForMaximumDateTime
        {
            get => _minimDateForMaximumDateTime;
            set
            {
                _minimDateForMaximumDateTime = value;
                OnPropertyChanged();
            }
        }


        public bool IsSelectedTimeAwayToEdit
        {
            get => _isSelectedTimeAwayToEdit;
            set
            {
                _isSelectedTimeAwayToEdit = value;
                OnPropertyChanged();
            }
        }
        private bool _isSelectedTimeAwayToDelete;

        public bool IsSelectedTimeAwayToDelete
        {
            get => _isSelectedTimeAwayToDelete;
            set
            {
                _isSelectedTimeAwayToDelete = value;
                OnPropertyChanged();
            }
        }

        public TimeAway SelectedTimeAway
        {
            get => _selectedTimeAway;
            set
            {
                _selectedTimeAway = value;
                if (value == null)
                {
                    IsSelectedTimeAwayToEdit = false;
                    IsSelectedTimeAwayToDelete = false;
                }
                else
                {
                    if (value.LeaveFrom.AddDays(3) < DateTime.Now)
                    {
                        IsSelectedTimeAwayToEdit = false;
                        IsSelectedTimeAwayToDelete = true;
                    }
                    else
                    {
                        IsSelectedTimeAwayToEdit = true;
                        IsSelectedTimeAwayToDelete = true;
                        StartDateTime = value.LeaveFrom;
                        EndDateTime = value.LeaveTo;
                    }
                }
                OnPropertyChanged();
            }
        }

        public List<TimeAway> TimeAwayList
        {
            get => _timeAwayList;
            set
            {
                _timeAwayList = value;
                OnPropertyChanged();
            }
        }

        public DateTime EndDateTime
        {
            get => _endDateTime;
            set
            {
                _endDateTime = value;
                OnPropertyChanged();
            }
        }

        public DateTime MaximumDateTime
        {
            get => _maximumDateTime;
            set
            {
                _maximumDateTime = value;
                OnPropertyChanged();
            }
        }

        public DateTime StartDateTime
        {
            get => _startDateTime;
            set
            {
                _startDateTime = value;
                MinimDateForMaximumDateTime = value.AddDays(2);
                OnPropertyChanged();
            }
        }

        public DateTime MinimumDateTime
        {
            get => _minimumDateTime;
            set
            {
                _minimumDateTime = value;
                OnPropertyChanged();
            }
        }

        public ICommand AddTimeAwayCommand { get; set; }
        public ICommand EditTimeAwayCommand { get; set; }
        public ICommand DeleteTimeAwayCommand { get; set; }
        #endregion

        #region Private Methods
        private void InitializeCommands()
        {
            AddTimeAwayCommand = new Command(AddTimeAway);
            EditTimeAwayCommand = new Command(EditTimeAway);
            DeleteTimeAwayCommand = new Command(DeleteTimeAway);
        }

        private async void AddTimeAway()
        {
            var timeAway = new TimeAway()
            {
                TimeAwayId = Guid.NewGuid(),
                LeaveFrom = StartDateTime,
                LeaveTo = EndDateTime,
                DaysLeft = (EndDateTime - StartDateTime).Days,
                User = ActiveUser.User
            };
            try
            {
                _dialogService.ShowLoading();
                await _apartmentService.AddOrUpdateTimeAway(timeAway);
                TimeAwayList = await _apartmentService.GetTimeAwayByUser(ActiveUser.User.UserId);
                _dialogService.HideLoading();
                await _dialogService.ShowDialog("Plecarea a fost adaugata cu succes.", "Succes");
            }
            catch (Exception ex)
            {
                _dialogService.HideLoading();
                await _dialogService.ShowDialog(ex.Message, "Atentie!");
            }
        }
        private async void EditTimeAway()
        {
            var result = await _dialogService.DisplayAlert("Editeaza plecarea",
                "Pentru a edita plecarea trebuie sa completezi noua data mai sus.", "Am completat");

            if (result == false)
                return;

            var timeAway = SelectedTimeAway;
            timeAway.LeaveFrom = StartDateTime;
            timeAway.LeaveTo = EndDateTime;

            try
            {
                _dialogService.ShowLoading();
                await _apartmentService.AddOrUpdateTimeAway(timeAway);
                TimeAwayList = await _apartmentService.GetTimeAwayByUser(ActiveUser.User.UserId);
                _dialogService.HideLoading();
                await _dialogService.ShowDialog("Plecarea a fost editata cu succes.", "Succes");
            }
            catch (Exception ex)
            {
                _dialogService.HideLoading();
                await _dialogService.ShowDialog(ex.Message, "Atentie!");
            }
        }
        private async void DeleteTimeAway()
        {
            _dialogService.ShowLoading();
            await _apartmentService.DeleteTimeAway(SelectedTimeAway);
            TimeAwayList = await _apartmentService.GetTimeAwayByUser(ActiveUser.User.UserId);
            _dialogService.HideLoading();
            await _dialogService.ShowDialog("Plecarea a fost stearsa cu succes.", "Succes");
        }

        private async void Initialize()
        {
            MinimumDateTime = DateTime.Now.Subtract(new TimeSpan(2, 0, 0, 0));
            MaximumDateTime = DateTime.Now.AddMonths(2);
            StartDateTime = DateTime.Now;
            EndDateTime = MinimDateForMaximumDateTime;
            _dialogService.ShowLoading();
            TimeAwayList = await _apartmentService.GetTimeAwayByUser(ActiveUser.User.UserId);
            _dialogService.HideLoading();
        }
        #endregion

    }
}
