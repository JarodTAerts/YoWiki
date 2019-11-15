using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using YoWiki.Services;

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

        private async void DownloadPage_Clicked(object sender, EventArgs e)
        {
            await Shell.Current.Navigation.PushModalAsync(new NavigationPage(new DownloadsPage()));
        }

        public void UpdateBadgeNumber(int num)
        {
            BadgedIcon.BadgeNumber = num;
        }
    }
}