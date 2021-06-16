using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Factoriada.Models;
using Factoriada.Services.Interfaces;
using Factoriada.Utility;
using Xamarin.Forms;

namespace Factoriada.ViewModels
{
    public class ApartmentChoresViewModel : ViewModelBase
    {
        #region Constructor
        public ApartmentChoresViewModel(IDialogService dialogService, INavigationService navigationService, IApartmentService apartmentService) : base(dialogService, navigationService)
        {
            _apartmentService = apartmentService;

            Initialize();
            InitializeCommands();
        }
        #endregion

        #region Private Fields

        private readonly IApartmentService _apartmentService;
        private Guid _apartmentDetail;
        private List<Job> _jobList;
        private List<string> _userNameList;
        private Job _selectedJob;

        #endregion

        #region Proprieties
        private bool _isItemSelected;

        public bool IsItemSelected
        {
            get => _isItemSelected;
            set
            {
                _isItemSelected = value;
                OnPropertyChanged();
            }
        }

        public Job SelectedJob
        {
            get => _selectedJob;
            set
            {
                _selectedJob = value;
                IsItemSelected = value != null;
                OnPropertyChanged();
            }
        }

        public List<string> UserNameList
        {
            get => _userNameList;
            set
            {
                _userNameList = value;
                OnPropertyChanged();
            }
        }

        public List<Job> JobList
        {
            get => _jobList;
            set
            {
                _jobList = value;
                OnPropertyChanged();
            }
        }

        public ICommand AddJobCommand { get; set; }
        public ICommand EditJobCommand { get; set; }
        public ICommand DeleteJobCommand { get; set; }
        #endregion

        #region Private Methods

        private async void Initialize()
        {
            _dialogService.ShowLoading();
            _apartmentDetail = ActiveUser.ApartmentGuid.ApartmentDetailId;
            JobList = await _apartmentService.GetJobsByApartment(_apartmentDetail);
            UserNameList = (await _apartmentService.GetUsersByApartment(_apartmentDetail))
                .Select(x => x.FullName)
                .ToList();
            _dialogService.HideLoading();
        }

        private void InitializeCommands()
        {
            AddJobCommand = new Command(AddJob);
            EditJobCommand = new Command(EditJob);
            DeleteJobCommand = new Command(DeleteJob);
        }

        private async void AddJob()
        {
            var jobName = await _dialogService.DisplayPromptAsync("Adaugati job", "Numele job-ului:");

            if (jobName == null)
                return;

            var names = new StringBuilder();
            for (var index = 0; index < UserNameList.Count; index++)
            {
                names.Append("\n");
                names.Append($"{index+1}.{UserNameList[index]}");
            }

            var number = await _dialogService.DisplayPromptAsync("Adauga job",
                $"Alege persoana care sa faca job-ul {jobName} din lista: {names}", keyboard: Keyboard.Numeric);

            if (string.IsNullOrEmpty(number))
                return;

            var count = int.Parse(number);

            if (count < 0 || count > UserNameList.Count)
            {
                await _dialogService.ShowDialog("Ai ales un user inexistent.", "Atentie!");
                return;
            }

            var job = new Job()
            {
                JobId = Guid.NewGuid(),
                ApartmentDetail = _apartmentDetail,
                JobType = jobName,
                User = UserNameList[count - 1],
            };

            _dialogService.ShowLoading();
            await _apartmentService.AddOrUpdateJob(job);
            JobList.Add(job);
            JobList = new List<Job>(JobList);
            _dialogService.HideLoading();
            await _dialogService.ShowDialog("Job-ul a fost adaugat cu succes.", "Succes");

        }
        private async void EditJob()
        {
            if (SelectedJob == null)
                return;

            var result =
                await _dialogService.DisplayAlert("Editare job", "Ce doresti sa modifici?", "Job-ul", "Persoana");

            if (result)
            {
                var jobName = await _dialogService.DisplayPromptAsync("Editare job", "Numele job-ului:");

                if (jobName == null)
                    return;

                SelectedJob.JobType = jobName;
            }
            else
            {
                var names = new StringBuilder();
                for (var index = 0; index < UserNameList.Count; index++)
                {
                    names.Append("\n");
                    names.Append($"{index + 1}.{UserNameList[index]}");
                }

                var number = await _dialogService.DisplayPromptAsync("Editare job",
                    $"Alege persoana care sa faca job-ul {SelectedJob.JobType} din lista: {names}", keyboard: Keyboard.Numeric);

                if (number == null)
                    return;

                var count = int.Parse(number);

                if (count < 0 || count > UserNameList.Count)
                {
                    await _dialogService.ShowDialog("Ai ales un user inexistent.", "Atentie!");
                    return;
                }

                SelectedJob.User = UserNameList[count - 1];

            }

            _dialogService.ShowLoading();
            await _apartmentService.AddOrUpdateJob(SelectedJob);
            JobList
                .Select(x =>
                {
                    if (x.JobId == SelectedJob.JobId)
                        x = SelectedJob;

                    return x;
                });

            JobList = new List<Job>(JobList);

            _dialogService.HideLoading();
            await _dialogService.ShowDialog("Job-ul a fost editat cu succes.", "Succes");
        }
        private async void DeleteJob()
        {
            if (SelectedJob == null)
                return;

            _dialogService.ShowLoading();
            await _apartmentService.DeleteJob(SelectedJob);
            JobList.Remove(SelectedJob);
            JobList = new List<Job>(JobList);
            _dialogService.HideLoading();
            await _dialogService.ShowDialog("Job-ul a fost sters cu succes.", "Succes");
        }

        #endregion

    }
}