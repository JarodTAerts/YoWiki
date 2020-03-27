using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using YoWiki.Services;
using YoWiki.ViewModels;

namespace YoWiki.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SettingsPage : ContentPage
    {
        public SettingsPage()
        {
            InitializeComponent();

            PersistentDownloadService.AddBadgeCallback(UpdateBadgeNumber);
            UpdateBadgeNumber(PersistentDownloadService.DownloadQueue.Count);
            BadgedIcon.AddButtonEventHandler(DownloadPage_Clicked);
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

        /// <summary>
        /// Callback function to update the badge number on the download center icon button
        /// </summary>
        /// <param name="num">Number to put in the badge</param>
        public void UpdateBadgeNumber(int num)
        {
            BadgedIcon.BadgeNumber = num;
        }

        protected override void OnAppearing()
        {
            // Update UI elements in case things have changed since the settings page was last changed
            ((SettingsViewModel)BindingContext).UpdateViewElements();
            base.OnAppearing();
        }
    }
}