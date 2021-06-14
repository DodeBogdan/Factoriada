using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Factoriada.Models;
using Xamarin.CommunityToolkit.UI.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Factoriada.Popups
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BillPopup : Popup<string>
    {
        public BillPopup(string result)
        {
            InitializeComponent();

            this._result = result;
            if (_result == "Paid")
            {
                PayBillButton.IsVisible = false;
                SeeDetailsButton.IsVisible = true;
            }
            else
            {
                PayBillButton.IsVisible = true;
                SeeDetailsButton.IsVisible = false;
            }
        }

        private  string _result;

        private void DeleteBill(object sender, EventArgs e)
        {
            _result = "Delete";
            Dismiss(_result);
        }

        private void PayBill(object sender, EventArgs e)
        {
            _result = "Pay";
            Dismiss(_result);
        }

        private void CancelBill(object sender, EventArgs e)
        {
            _result = "Cancel";
            Dismiss(_result);
        }

        private void SeeDetails(object sender, EventArgs e)
        {
            _result = "See Details";
            Dismiss(_result);
        }
    }
}