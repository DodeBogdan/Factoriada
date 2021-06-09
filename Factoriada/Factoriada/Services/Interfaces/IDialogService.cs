using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Factoriada.Services.Interfaces
{
    public interface IDialogService
    {
        Task ShowDialog(string message, string title, string buttonLabel = "ok");
        void ShowToast(string message);
        void ShowLoading();
        void HideLoading();
        Task<string> DisplayPromptAsync(string title, string message, string accept = "OK",
            string cancel = "Anuleaza", string placeholder = null, int maxLength = -1,
            Keyboard keyboard = default, string initialValue = "");
        Task<bool> DisplayAlert(string title, string message, string accept = "Ok", string cancel = "Anuleaza");
    }
}
