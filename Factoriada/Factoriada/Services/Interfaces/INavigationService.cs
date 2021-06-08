using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Factoriada.Views;
using Xamarin.CommunityToolkit.UI.Views;
using Xamarin.Forms;

namespace Factoriada.Services.Interfaces
{
    public interface INavigationService
    {
        Task PushAsync(Page page);
        Task PushModalAsync(Page page);
        Task<Page> PopAsync();
    }
}
