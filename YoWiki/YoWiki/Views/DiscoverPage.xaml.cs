using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using YoWiki.Services;

namespace YoWiki.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DiscoverPage : ContentPage
    {
        public DiscoverPage()
        {
            InitializeComponent();

            PersistentDownloadService.AddBadgeCallback(UpdateBadgeNumber);
            BadgedIcon.AddButtonEventHandler(DownloadPage_Clicked);
        }

        public void UpdateBadgeNumber(int num)
        {
            BadgedIcon.BadgeNumber = num;
        }

        private async void DownloadPage_Clicked(object sender, EventArgs e)
        {
            await Shell.Current.Navigation.PushModalAsync(new NavigationPage(new DownloadsPage()));
        }

        protected override void OnAppearing()
        {
            //resultsList.SelectedItem = null;
            base.OnAppearing();
        }

        protected override void OnDisappearing()
        {
            //resultsList.SelectedItem = null;
            base.OnDisappearing();
        }
    }
}