using Factoriada.Models;
using Factoriada.Services.Interfaces;
using Factoriada.Utility;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Factoriada.Services;

namespace Factoriada.ViewModels
{
    class AppShellViewModel : ViewModelBase
    {
        public AppShellViewModel(IDialogService dialogService, INavigationService navigationService) : base(dialogService, navigationService)
        {
            ConnectedUser = ActiveUser.User;
        }

        private User _connectedUser = new User();

        public User ConnectedUser
        {
            get { return _connectedUser; }
            set { _connectedUser = value;
                OnPropertyChanged();
            }
        }
        public ICommand LogOutCommand { get; set; }
    }
}
