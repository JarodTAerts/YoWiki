using System;
using System.Collections.Generic;
using System.Diagnostics;
using Xamarin.Forms;

namespace YoWiki.Views
{
    public partial class ViewArticlePage : ContentPage
    {
        public Command GoBackCommand;

        public ViewArticlePage(WebViewSource webViewSource)
        {
            InitializeComponent();
            webView.Source = webViewSource;
            GoBackCommand = new Command(GoBack);
            backButton.Command = GoBackCommand;
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
