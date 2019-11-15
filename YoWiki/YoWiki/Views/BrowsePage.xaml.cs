using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using YoWiki.Services;
using YoWiki.ViewModels;

namespace YoWiki.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BrowsePage : ContentPage
    {
        public BrowsePage()
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

        protected override void OnAppearing()
        {
            var viewModel = (BrowseViewModel)BindingContext;
            Task.Run(() => viewModel.LoadLocalArticles());
            base.OnAppearing();
        }
    }
}