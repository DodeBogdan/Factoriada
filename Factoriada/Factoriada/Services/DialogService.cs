using Acr.UserDialogs;
using Factoriada.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Factoriada.Services
{
    public class DialogService : IDialogService
    {
        public Task ShowDialog(string message, string title, string buttonLabel = "OK")
        {
            return UserDialogs.Instance.AlertAsync(message, title, buttonLabel);
        }

        public void ShowToast(string message)
        {
            UserDialogs.Instance.Toast(message);
        }

        public void ShowLoading()
        {
            UserDialogs.Instance.ShowLoading("Se incarca.");
        }

        public void HideLoading()
        {
            UserDialogs.Instance.HideLoading();
        }

        private Page MainPage
        {
            get { return Application.Current.MainPage; }
        }

        public Task<string> DisplayPromptAsync(string title, string message, string accept = "OK",
            string cancel = "Cancel", string placeholder = null, int maxLength = -1,
            Keyboard keyboard = default, string initialValue = "")
        {
            return MainPage.DisplayPromptAsync(title, message, accept, cancel, placeholder, maxLength, keyboard,
                initialValue);
        }

        public Task<bool> DisplayAlert(string title, string message, string accept, string cancel)
        {
            return MainPage.DisplayAlert(title, message, accept, cancel);
        }
    }
}
