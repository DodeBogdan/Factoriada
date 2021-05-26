using Factoriada.Models;
using Factoriada.Services.Interfaces;
using Factoriada.Views;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace Factoriada.ViewModels
{
    class HomeViewModel : ViewModelBase
    {
        #region Constructor
        public HomeViewModel(INavigationService navigationService, IDialogService dialogService) : base(dialogService, navigationService)
        {
            InitializeCommands();
        }

        private void InitializeCommands()
        {
            RefreshCommand = new Command(Refresh);
        }
        #endregion
        #region Private Fields
        private bool _isRefreshing;
        #endregion

        #region Proprieties
        public bool IsRefreshing
        {
            get => _isRefreshing;
            set
            {
                _isRefreshing = value;
                OnPropertyChanged();
            }
        }

        public ICommand RefreshCommand { get; set; }
        #endregion

        #region Private Methods
        private void Refresh()
        {
            IsRefreshing = false;
        }
        #endregion
    }
}
