using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Factoriada.Services.Interfaces
{
    public interface IDialogService
    {
        Task ShowDialog(string message, string title, string buttonLabel);
        void ShowToast(string message);
        Task<string> DisplayPromptAsync(string title, string message, string accept = "OK",
            string cancel = "Anuleaza", string placeholder = null, int maxLength = -1,
            Keyboard keyboard = default(Keyboard), string initialValue = "");
        Task<bool> DisplayAlert(string title, string message, string accept, string cancel);
    }
}
