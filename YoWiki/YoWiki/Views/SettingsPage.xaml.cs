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
        }

        public void UpdateBadgeNumber(int num)
        {
            BadgedIcon.BadgeNumber = num;
        }
    }
}