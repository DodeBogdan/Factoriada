using System;
using System.Windows.Input;
using Factoriada.Models;
using Factoriada.Services.Interfaces;
using Factoriada.Utility;
using Xamarin.Forms;

namespace Factoriada.ViewModels
{
    public class CreateApartmentViewModel : ViewModelBase
    {
        #region Constructor
        public CreateApartmentViewModel(IDialogService dialogService, INavigationService navigationService, IApartmentService apartmentService) : base(dialogService, navigationService)
        {
            _apartmentService = apartmentService ?? throw new ArgumentNullException(nameof(apartmentService));
            CurrentApartment = new ApartmentDetail()
            {
                ApartmentAddress = new Address()
            };

            InitializeCommands();
        }

        #endregion

        #region Private Fields
        private readonly IApartmentService _apartmentService;
        private ApartmentDetail _currentApartment;
        private bool _editAddressIsVisible;
        private bool _startEditAddressIsVisible;
        #endregion

        #region Proprieties

        public bool StartEditAddressIsVisible
        {
            get => _startEditAddressIsVisible;
            set
            {
                _startEditAddressIsVisible = value; 
                OnPropertyChanged();
            }
        }
        public bool EditAddressIsVisible
        {
            get => _editAddressIsVisible;
            set
            {
                _editAddressIsVisible = value;
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

        public ICommand RefreshCommand { get; set; }
        public ICommand StartEditAddressCommand { get; set; }
        public ICommand SaveApartmentCommand { get; set; }
        public ICommand SaveAddressCommand { get; set; }
        
        #endregion

        #region Private Methods
        private void InitializeCommands()
        {
            RefreshCommand = new Command(Refresh);
            StartEditAddressCommand = new Command(StartEditAddress);
            SaveAddressCommand = new Command(SaveAddress);
            SaveApartmentCommand = new Command(SaveApartment);
        }

        private async void StartEditAddress()
        {
            if (CurrentApartment.ApartmentName == null)
            {
                await _dialogService.ShowDialog("Numele apartamentului trebuie completat.", "Atentie!");
                return;
            }
            
            if (CurrentApartment.ApartmentName.Length < 3)
            {
                await _dialogService.ShowDialog("Numele apartamentului trebuie sa aibe minim 3 caractere.", "Atentie!");
                return;
            }

            var result = await _dialogService.DisplayAlert("Mod de adaugare a adresei",
                "Cum doresti adaugarea/editarea adresei?", "Adresa noua", "Adresa curenta");

            if (result)
            { 
                CurrentApartment.ApartmentAddress = new Address()
                {
                    AddressId = Guid.NewGuid()
                };
                CurrentApartment = CurrentApartment;
            }
            else
            {
                CurrentApartment.ApartmentAddress = ActiveUser.User.Address;
                CurrentApartment = CurrentApartment;
            }

            EditAddressIsVisible = true;
        }
        private async void SaveApartment()
        {
            try
            {
                SaveAddress();

                _dialogService.ShowLoading();
                
                await _apartmentService.SaveApartment(CurrentApartment);

                _dialogService.HideLoading();

                await _dialogService.ShowDialog("Apartamentul a fost salvat cu succes.", "Succes");

                App.Current.MainPage = new AppShell();
            }
            catch (Exception ex)
            {
                _dialogService.HideLoading();
                await _dialogService.ShowDialog(ex.Message, "Atentie!");
            }
        }

        private void SaveAddress()
        {
            try
            {
                _dialogService.ShowLoading();

                _apartmentService.TestAddress(CurrentApartment.ApartmentAddress);

                _dialogService.HideLoading();
            }
            catch (Exception)
            {
                _dialogService.HideLoading();
                throw;
            }
        }

        private void Refresh()
        {
            StartEditAddressIsVisible = true;
            EditAddressIsVisible = false;
        }

        #endregion

    }
}