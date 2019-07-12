using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using YoWiki.ViewModels;

namespace YoWiki.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BrowsePage : ContentPage
    {
        public BrowsePage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            var viewModel = (BrowseViewModel)BindingContext;
            Task.Run(()=>viewModel.LoadLocalArticles());
            base.OnAppearing();
        }
    }
}