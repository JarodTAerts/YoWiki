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
        }

        public void UpdateBadgeNumber(int num)
        {
            BadgedIcon.BadgeNumber = num;
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