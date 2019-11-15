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
        }

        protected override void OnAppearing()
        {
            var viewModel = (BrowseViewModel)BindingContext;
            Task.Run(() => viewModel.LoadLocalArticles());
            base.OnAppearing();
        }

        public void UpdateBadgeNumber(int num)
        {
            BadgedIcon.BadgeNumber = num;
        }
    }
}