using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Factoriada.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Factoriada.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BillView : ContentPage
    {
        public BillView()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            (this.BindingContext as BillViewModel).Initialize();

            base.OnAppearing();
        }
    }
}