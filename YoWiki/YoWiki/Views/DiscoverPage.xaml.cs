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

        /// <summary>
        /// Callback function to update the badge number on the download center icon button
        /// </summary>
        /// <param name="num">Number to put in the badge</param>
        public void UpdateBadgeNumber(int num)
        {
            BadgedIcon.BadgeNumber = num;
        }

        /// <summary>
        /// Handler to navigate to download center when the download center button is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void DownloadPage_Clicked(object sender, EventArgs e)
        {
            await Shell.Current.Navigation.PushModalAsync(new NavigationPage(new DownloadsPage()));
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
        }
    }
}