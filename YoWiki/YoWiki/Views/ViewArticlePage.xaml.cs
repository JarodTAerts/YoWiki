using Xamarin.Forms;

namespace YoWiki.Views
{
    public partial class ViewArticlePage : ContentPage
    {
        public Command GoBackCommand;

        public ViewArticlePage(WebViewSource webViewSource, string actionText, Command actionCommand)
        {
            InitializeComponent();
            webView.Source = webViewSource;
            GoBackCommand = new Command(GoBack);
            backButton.Command = GoBackCommand;

            actionButton.Text = actionText;
            actionButton.Command = actionCommand;
        }

        public ViewArticlePage()
        {
            InitializeComponent();
            GoBackCommand = new Command(GoBack);
        }

        public void GoBack()
        {
            Shell.Current.Navigation.PopModalAsync();
        }
    }
}
