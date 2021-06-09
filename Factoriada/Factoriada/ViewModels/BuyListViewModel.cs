using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Factoriada.Models;
using Factoriada.Services.Interfaces;
using Factoriada.Utility;
using Xamarin.Forms;

namespace Factoriada.ViewModels
{
    public class BuyListViewModel : ViewModelBase
    {
        #region Constructor
        public BuyListViewModel(IDialogService dialogService, INavigationService navigationService, IApartmentService apartmentService) : base(dialogService, navigationService)
        {
            _apartmentService = apartmentService;

            Initialize();
            InitializeCommands();
        }
        #endregion


        #region Private Fields

        private readonly IApartmentService _apartmentService;

        private List<BuyList> _publicBuyList;
        private List<BuyList> _privateBuyList;
        private BuyList _publicSelectedBuy;
        private BuyList _privateSelectedBuy;
        private ApartmentDetail _apartmentDetail;
        #endregion

        #region Proprieties
        public BuyList PrivateSelectedBuy
        {
            get => _privateSelectedBuy;
            set
            {
                _privateSelectedBuy = value;
                OnPropertyChanged();
            }
        }

        public BuyList PublicSelectedBuy
        {
            get => _publicSelectedBuy;
            set
            {
                _publicSelectedBuy = value;
                OnPropertyChanged();
            }
        }

        public List<BuyList> PrivateBuyList
        {
            get => _privateBuyList;
            set
            {
                _privateBuyList = value;
                OnPropertyChanged();
            }
        }

        public List<BuyList> PublicBuyList
        {
            get => _publicBuyList;
            set
            {
                _publicBuyList = value;
                OnPropertyChanged();
            }
        }

        public ICommand AddProductToBuyCommand { get; set; }
        public ICommand DeleteProductCommand { get; set; }
        public ICommand EditProductCommand { get; set; }
        #endregion

        #region Private Methods

        private async void Initialize()
        {
            _dialogService.ShowLoading();

            _apartmentDetail = await _apartmentService.GetApartmentByUser(ActiveUser.User.UserId);
            PublicBuyList = (await _apartmentService.GetBuyListFromApartment(_apartmentDetail.ApartmentDetailId))
                .Where(x => x.Hidden == false).ToList();
            PrivateBuyList = (await _apartmentService.GetBuyListFromApartment(_apartmentDetail.ApartmentDetailId))
                .Where(x => x.Hidden == true).ToList();

            _dialogService.HideLoading();
        }

        private void InitializeCommands()
        {
            AddProductToBuyCommand = new Command(AddProductToBuy);
            EditProductCommand = new Command(EditProduct);
            DeleteProductCommand = new Command(DeleteProduct);
        }

        private async void AddProductToBuy()
        {
            var whereToAddProduct = await _dialogService.DisplayAlert("Adaugare produs", "Unde doresti adaugarea produsului:",
                "Cos public", "Cos personal");

            var productName = await _dialogService.DisplayPromptAsync("Adaugare produs", "Denumirea produsului: ",
                placeholder: "Ex: Lapte");

            if (productName == null)
                return;

            var productCount = await _dialogService.DisplayPromptAsync("Adaugare produs", "Numarul de produse: ",
                placeholder: "Ex: 4", keyboard: Keyboard.Numeric);

            if (productCount == null)
                return;

            var toBuy = new BuyList()
            {
                BuyListId = Guid.NewGuid(),
                ApartmentDetail = _apartmentDetail,
                Count = int.Parse(productCount),
                Owner = ActiveUser.User,
                Product = productName
            };

            if (whereToAddProduct)
                AddProductToPublic(toBuy);
            else
            {
                AddProductToPrivate(toBuy);
            }
        }

        private async void AddProductToPublic(BuyList toBuy)
        {
            toBuy.Hidden = false;
            _dialogService.ShowLoading();
            await _apartmentService.AddOrUpdateProductToBuy(toBuy);
            PublicBuyList = (await _apartmentService.GetBuyListFromApartment(_apartmentDetail.ApartmentDetailId))
                .Where(x => x.Hidden == false).ToList();
            _dialogService.HideLoading();
            await _dialogService.ShowDialog("Produsul a fost adaugat cu succes.", "Succes");
        }

        private async void AddProductToPrivate(BuyList toBuy)
        {
            toBuy.Hidden = true;
            _dialogService.ShowLoading();
            await _apartmentService.AddOrUpdateProductToBuy(toBuy);
            PrivateBuyList = (await _apartmentService.GetBuyListFromApartment(_apartmentDetail.ApartmentDetailId))
                .Where(x => x.Hidden == true).ToList();
            _dialogService.HideLoading();
            await _dialogService.ShowDialog("Produsul a fost adaugat cu succes.", "Succes");
        }

        private async void EditProduct()
        {
            var selectedProduct = await SelectProduct();

            if (selectedProduct == null)
            {
                await _dialogService.ShowDialog("Nu a fost selectat nici un produs.", "Atentie!");
                return;
            }

            var result = await _dialogService.DisplayAlert("Editeaza produs", "Ce doresti sa editezi?", "Denumirea",
                "Cantitatea");

            if (result)
            {
                var productName = await _dialogService.DisplayPromptAsync("Editare produs", "Denumirea produsului: ",
                    placeholder: "Ex: Lapte");

                if (productName == null)
                {
                    await _dialogService.ShowDialog("Nu a fost introdus nimic.", "Atentie!");
                    return;
                }

                selectedProduct.Product = productName;
            }
            else
            {
                var productCount = await _dialogService.DisplayPromptAsync("Adaugare produs", "Numarul de produse: ",
                    placeholder: "Ex: 4", keyboard: Keyboard.Numeric);

                if (productCount == null)
                {
                    await _dialogService.ShowDialog("Nu a fost introdus nimic.", "Atentie!");
                    return;
                }

                selectedProduct.Count = int.Parse(productCount);
            }

            _dialogService.ShowLoading();
            await _apartmentService.AddOrUpdateProductToBuy(selectedProduct);
            if (selectedProduct.Hidden)
                PrivateBuyList = (await _apartmentService.GetBuyListFromApartment(_apartmentDetail.ApartmentDetailId))
                    .Where(x => x.Hidden == true).ToList();
            else
                PublicBuyList = (await _apartmentService.GetBuyListFromApartment(_apartmentDetail.ApartmentDetailId))
                    .Where(x => x.Hidden == false).ToList();
            _dialogService.HideLoading();
            await _dialogService.ShowDialog("Produsul a fost editat cu succes.", "Succes");
        }

        private async void DeleteProduct()
        {
            var selectedProduct = await SelectProduct();

            if (selectedProduct == null)
            {
                await _dialogService.ShowDialog("Nu a fost selectat nici un produs.", "Atentie!");
                return;
            }

            _dialogService.ShowLoading();
            await _apartmentService.DeleteToBuy(selectedProduct);
            if (selectedProduct.Hidden)
                PrivateBuyList = (await _apartmentService.GetBuyListFromApartment(_apartmentDetail.ApartmentDetailId))
                    .Where(x => x.Hidden == true).ToList();
            else
                PublicBuyList = (await _apartmentService.GetBuyListFromApartment(_apartmentDetail.ApartmentDetailId))
                    .Where(x => x.Hidden == false).ToList();
            _dialogService.HideLoading();
            await _dialogService.ShowDialog("Produsul a fost sters cu succes.", "Succes");
        }

        private async Task<BuyList> SelectProduct()
        {
            if (PublicSelectedBuy != null && PrivateSelectedBuy != null)
            {
                var result = await _dialogService.DisplayAlert("Alege produs",
                    "Aveti selectate 2 produse. Pe care doresti sa il editezi/stergi", "Public", "Privat");

                if (result)
                    return PublicSelectedBuy;
                else
                    return PrivateSelectedBuy;
            }

            if (PublicSelectedBuy != null)
                return PublicSelectedBuy;

            if (PrivateSelectedBuy != null)
                return PrivateSelectedBuy;

            return null;
        }
        #endregion
    }
}