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
    public enum ListTypesEnum
    {
        None = 0,
        Public,
        Private
    }

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

        private List<BuyList> _publicBuyList = new List<BuyList>();
        private List<BuyList> _privateBuyList = new List<BuyList>();
        private BuyList _publicSelectedBuy;
        private BuyList _privateSelectedBuy;
        private Guid _apartmentDetail;
        private bool _isItemSelected;
        private bool _publicListIsVisible = true;
        private bool _privateListIsVisible;
        private List<ListTypesEnum> _listTypes = new List<ListTypesEnum>();
        private ListTypesEnum _selectedList = ListTypesEnum.Public;
        #endregion

        #region Proprieties

        public ListTypesEnum SelectedList
        {
            get => _selectedList;
            set
            {
                _selectedList = value;

                PublicSelectedBuy = PrivateSelectedBuy = new BuyList();
                IsItemSelected = false;
                if (value == ListTypesEnum.Public)
                {
                    PrivateListIsVisible = false;
                    PublicListIsVisible = true;
                    PublicBuyList = new List<BuyList>(PublicBuyList);
                }
                else
                {
                    PrivateListIsVisible = true;
                    PublicListIsVisible = false;
                    PrivateBuyList = new List<BuyList>(PrivateBuyList);
                }
                OnPropertyChanged();
            }
        }

        public List<ListTypesEnum> ListTypes
        {
            get => _listTypes;
            set
            {
                _listTypes = value;
                OnPropertyChanged();
            }
        }


        public bool PrivateListIsVisible
        {
            get => _privateListIsVisible;
            set
            {
                _privateListIsVisible = value;
                OnPropertyChanged();
            }
        }

        public bool PublicListIsVisible
        {
            get => _publicListIsVisible;
            set
            {
                _publicListIsVisible = value;
                OnPropertyChanged();
            }
        }

        public bool IsItemSelected
        {
            get => _isItemSelected;
            set
            {
                _isItemSelected = value;
                OnPropertyChanged();
            }
        }
        public BuyList PrivateSelectedBuy
        {
            get => _privateSelectedBuy;
            set
            {
                _privateSelectedBuy = value;
                if (value != null)
                    IsItemSelected = true;
                else
                {
                    if (PublicSelectedBuy == null)
                        IsItemSelected = false;
                }
                OnPropertyChanged();
            }
        }
        public BuyList PublicSelectedBuy
        {
            get => _publicSelectedBuy;
            set
            {
                _publicSelectedBuy = value;
                if (value != null)
                    IsItemSelected = true;
                else
                {
                    if(PrivateSelectedBuy == null)
                        IsItemSelected = false;
                }
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

            ListTypes.Add(ListTypesEnum.Public);
            ListTypes.Add(ListTypesEnum.Private);

            _apartmentDetail = ActiveUser.ApartmentGuid.ApartmentDetailId;

            PublicBuyList = (await _apartmentService.GetBuyListFromApartment(_apartmentDetail))
                .Where(x => x.Hidden == false).ToList();
            PrivateBuyList = (await _apartmentService.GetBuyListFromApartment(_apartmentDetail))
                .Where(x => x.Owner.UserId == ActiveUser.User.UserId && x.Hidden == true).ToList();

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
            var productName = await _dialogService.DisplayPromptAsync("Adaugare produs", "Denumirea produsului: ",
                placeholder: "Ex: Lapte");

            if (string.IsNullOrEmpty(productName))
            {
                await _dialogService.ShowDialog("Nu a fost introdus nimic.", "Atentie!");
                return;
            }

            var productCount = await _dialogService.DisplayPromptAsync("Adaugare produs", "Numarul de produse: ",
                placeholder: "Ex: 4", keyboard: Keyboard.Numeric);

            if (string.IsNullOrEmpty(productCount))
            {
                await _dialogService.ShowDialog("Nu a fost introdus nimic.", "Atentie!");
                return;
            }

            var toBuy = new BuyList()
            {
                BuyListId = Guid.NewGuid(),
                ApartmentDetail = _apartmentDetail,
                Count = int.Parse(productCount),
                Owner = ActiveUser.User,
                Product = productName
            };

            if (SelectedList == ListTypesEnum.Public)
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
            PublicBuyList.Add(toBuy);
            PublicBuyList = new List<BuyList>(PublicBuyList);

            _dialogService.HideLoading();
            await _dialogService.ShowDialog("Produsul a fost adaugat cu succes.", "Succes");
        }

        private async void AddProductToPrivate(BuyList toBuy)
        {
            toBuy.Hidden = true;
            _dialogService.ShowLoading();
            await _apartmentService.AddOrUpdateProductToBuy(toBuy);

            PrivateBuyList.Add(toBuy);
            PrivateBuyList = new List<BuyList>(PrivateBuyList);

            _dialogService.HideLoading();
            await _dialogService.ShowDialog("Produsul a fost adaugat cu succes.", "Succes");
        }

        private async void EditProduct()
        {
            var selectedProduct = SelectProduct();

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

                if (string.IsNullOrEmpty(productName))
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

                if (string.IsNullOrEmpty(productCount))
                {
                    await _dialogService.ShowDialog("Nu a fost introdus nimic.", "Atentie!");
                    return;
                }

                selectedProduct.Count = int.Parse(productCount);
            }

            _dialogService.ShowLoading();
            await _apartmentService.AddOrUpdateProductToBuy(selectedProduct);
            if (selectedProduct.Hidden)
            {
                PrivateSelectedBuy = null;
                PrivateBuyList
                    .Select(x =>
                    {
                        if (x.ApartmentDetail == selectedProduct.ApartmentDetail)
                            x = selectedProduct;

                        return x;
                    });

                PrivateBuyList = new List<BuyList>(PrivateBuyList);

            }
            else
            {
                PublicSelectedBuy = null;
                PublicBuyList
                    .Select(x =>
                    {
                        if (x.ApartmentDetail == selectedProduct.ApartmentDetail)
                            x = selectedProduct;

                        return x;
                    });
                
                PublicBuyList = new List<BuyList>(PublicBuyList);
            }

            _dialogService.HideLoading();
            await _dialogService.ShowDialog("Produsul a fost editat cu succes.", "Succes");
        }

        private async void DeleteProduct()
        {
            var selectedProduct = SelectProduct();

            if (selectedProduct == null)
            {
                await _dialogService.ShowDialog("Nu a fost selectat nici un produs.", "Atentie!");
                return;
            }

            _dialogService.ShowLoading();
            await _apartmentService.DeleteToBuy(selectedProduct);
            if (selectedProduct.Hidden)
            {
                PrivateBuyList.Remove(selectedProduct);
                PrivateSelectedBuy = null;
                PrivateBuyList = new List<BuyList>(PrivateBuyList);
            }
            else
            {
                PublicBuyList.Remove(selectedProduct);
                PublicSelectedBuy = null;
                PublicBuyList = new List<BuyList>(PublicBuyList);
            }

            _dialogService.HideLoading();
            await _dialogService.ShowDialog("Produsul a fost sters cu succes.", "Succes");
        }

        private BuyList SelectProduct()
        {
            if (PublicSelectedBuy != null && PrivateSelectedBuy != null)
            {
                return SelectedList == ListTypesEnum.Public ? PublicSelectedBuy : PrivateSelectedBuy;
            }

            return PublicSelectedBuy ?? (PrivateSelectedBuy ?? null);
        }
        #endregion
    }
}