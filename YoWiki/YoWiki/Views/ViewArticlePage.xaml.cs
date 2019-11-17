using System.Text.RegularExpressions;
using Xamarin.Forms;
using YoWiki.Services;
using YoWiki.Services.Interfaces;

namespace YoWiki.Views
{
    public partial class ViewArticlePage : ContentPage
    {
        public Command GoBackCommand;
        public Command ActionCommand;

        private ILocalArticlesService localArticlesService;
        private IHTMLService hTMLService;

        public ViewArticlePage(WebViewSource webViewSource, string actionText, Command actionCommand)
        {
            InitializeComponent();
            webView.Source = webViewSource;
            GoBackCommand = new Command(GoBack);
            backButton.Command = GoBackCommand;

            ActionCommand = actionCommand;
            actionButton.Text = actionText;
            actionButton.Command = actionCommand;

            localArticlesService = DependencyService.Resolve<ILocalArticlesService>();
            hTMLService = DependencyService.Resolve<IHTMLService>();
        }

        public ViewArticlePage()
        {
            InitializeComponent();
            GoBackCommand = new Command(GoBack);
            backButton.Command = GoBackCommand;
        }

        public void GoBack()
        {
            Shell.Current.Navigation.PopModalAsync();
        }

        /// <summary>
        /// Function to intercept when a user clicks a link in an article and stop the webview from trying to navigate to it
        /// It also brings up that article so long as it is stored locally
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public async void LinkClicked(object sender, WebNavigatingEventArgs e)
        {
            e.Cancel = true;
            string[] urlSplit = e.Url.Split('/');
            string articleName = hTMLService.ReplaceColons(urlSplit[urlSplit.Length - 1]);

            if (localArticlesService.ArticleExists(articleName))
            {
                string articleHtml = localArticlesService.GetHTMLTextFromFile(articleName);
                WebViewSource webViewSource = new HtmlWebViewSource { Html = articleHtml };
                await Shell.Current.Navigation.PushModalAsync(new NavigationPage(new ViewArticlePage(webViewSource, "Delete", ActionCommand)));
            }
            else
            {
                // Maybe do something here
            }
        }

        protected override bool OnBackButtonPressed()
        {
            Shell.Current.Navigation.PopModalAsync();
            return true;
        }
    }
}
