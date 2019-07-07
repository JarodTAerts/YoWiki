using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace YoWiki.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DiscoverPage : ContentPage
    {
        public DiscoverPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            resultsList.SelectedItem = null;
            base.OnAppearing();
        }

        protected override void OnDisappearing()
        {
            resultsList.SelectedItem = null;
            base.OnDisappearing();
        }
    }
}