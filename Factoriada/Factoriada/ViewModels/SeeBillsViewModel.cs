using Factoriada.Services.Interfaces;

namespace Factoriada.ViewModels
{
    public class SeeBillsViewModel : ViewModelBase
    {
        public SeeBillsViewModel(IDialogService dialogService, INavigationService navigationService) : base(dialogService, navigationService)
        {
        }
    }
}