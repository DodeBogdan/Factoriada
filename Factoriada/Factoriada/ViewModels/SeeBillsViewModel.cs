using System.Collections.Generic;
using Factoriada.Models;
using Factoriada.Popups;
using Factoriada.Services.Interfaces;
using Factoriada.Utility;
using Factoriada.Views;
using Xamarin.CommunityToolkit.Extensions;

namespace Factoriada.ViewModels
{
    public class SeeBillsViewModel : ViewModelBase
    {
        #region Constructor
        public SeeBillsViewModel(IDialogService dialogService, INavigationService navigationService, IApartmentService apartmentService) : base(dialogService, navigationService)
        {
            _apartmentService = apartmentService;

            Initialize();
        }
        #endregion

        #region Private Fields

        private readonly IApartmentService _apartmentService;

        private List<Bill> _billList;
        private ApartmentDetail _apartmentDetail;
        private bool _notStartedToPay = true;
        private Bill _selectedBill;
        private bool _startedToPay;

        #endregion

        #region Proprieties
        private List<BillPaidPersons> _billPaidPersonsList;

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

        #endregion

        #region Private Methods

        private async void Initialize()
        {
            _apartmentDetail = await _apartmentService.GetApartmentByUser(ActiveUser.User.UserId);
            BillList = await _apartmentService.GetBillsByApartment(_apartmentDetail.ApartmentDetailId);
        }

        private async void BillHasBeenChoose()
        {
            if (SelectedBill.Paid)
            {
                var result = await App.Current.MainPage.Navigation.ShowPopupAsync(new BillPopup("Paid"));

                if (result == null || result == "Cancel")
                {
                    SelectedBill = null;
                    return;
                }

                if (result == "Delete")
                {
                    await _apartmentService.DeleteBill(SelectedBill);
                    await _dialogService.ShowDialog("Factura a fost stearsa.", "Succes");
                    BillList = await _apartmentService.GetBillsByApartment(_apartmentDetail.ApartmentDetailId);
                    SelectedBill = null;
                }
            }
            else
            {
                var result = await App.Current.MainPage.Navigation.ShowPopupAsync(new BillPopup(""));

                if (result == null || result == "Cancel")
                {
                    SelectedBill = null;
                    return;
                }

                if (result == "Pay")
                {
                    NotStartedToPay = false;
                    StartedToPay = true;
                    BillPaidPersonsList =
                        await _apartmentService.GenerateBillPaidPersons(SelectedBill,
                            _apartmentDetail.ApartmentDetailId);
                }

                if (result == "Delete")
                {
                    await _apartmentService.DeleteBill(SelectedBill);
                    await _dialogService.ShowDialog("Factura a fost stearsa.", "Succes");
                    BillList = await _apartmentService.GetBillsByApartment(_apartmentDetail.ApartmentDetailId);
                    SelectedBill = null;
                }
            }
        }

        #endregion
    }
}