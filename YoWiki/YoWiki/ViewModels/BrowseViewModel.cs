using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;
using YoWiki.Services.Interfaces;
using YoWiki.Views;

namespace YoWiki.ViewModels
{
    class BrowseViewModel : BaseViewModel
    {
        #region Properties
        // Services
        private ILocalArticlesService localArticlesService;
        private IHTMLService hTMLService;

        // Public Properties
        private string _selectedItem;
        public string SelectedItem
        {
            get => _selectedItem;
            set { SetProperty(ref _selectedItem, value); OnSelectedItemChanged(); }
        }

        public List<string> VisibleArticles { get; set; } = new List<string>();
        public List<string> AllSavedArticles { get; set; } = new List<string>();
        public string MessageText { get; set; } = "Search your local library to read articles.";
        public string NumbersText { get; set; }
        public string EntryText { get; set; }
        public bool ResultsReturned { get; set; } = false;

        // Private Properties
        private string currentArticleTitle;
        #endregion

        #region Commands
        public Command SearchButtonClickedCommand { get; set; }
        #endregion

        #region Constructor
        public BrowseViewModel()
        {
            localArticlesService = DependencyService.Resolve<ILocalArticlesService>();
            hTMLService = DependencyService.Resolve<IHTMLService>();

            SearchButtonClickedCommand = new Command(OnSearchButtonClicked);
        }
        #endregion

        #region Command Functions

        /// <summary>
        /// Command function to handle event when an item in the list is selected
        /// </summary>
        private void OnSelectedItemChanged()
        {
            if (SelectedItem != null)
            {
                IsBusy = true;
                currentArticleTitle = hTMLService.ReplaceColons(SelectedItem);
                SelectedItem = null;

                string articleHtml = localArticlesService.GetHTMLTextFromFile(currentArticleTitle);
                WebViewSource webViewSource = new HtmlWebViewSource { Html = articleHtml };
                Shell.Current.Navigation.PushModalAsync(new NavigationPage(new ViewArticlePage(webViewSource, "Delete", new Command(DeleteArticle))));
                IsBusy = false;
            }
        }

        /// <summary>
        /// Command function to handle when the search button is clicked 
        /// </summary>
        private void OnSearchButtonClicked()
        {
            VisibleArticles = AllSavedArticles.Where(a => a.ToUpper().Contains(EntryText.ToUpper())).ToList();
            NumbersText = "Number of Articles: " + VisibleArticles.Count;
        }

        /// <summary>
        /// Command function to delete the article that the user is currently viewing
        /// </summary>
        private void DeleteArticle()
        {
            localArticlesService.DeleteArticle(currentArticleTitle);
            Shell.Current.DisplayAlert("Article Deleted", $"Article {currentArticleTitle} was deleted from local library.", "Ight");
        }
        #endregion

        #region Helpers
        /// <summary>
        /// Function to load/reload all the local articles from storage
        /// </summary>
        public void LoadLocalArticles()
        {
            SelectedItem = null;
            IsBusy = true;

            AllSavedArticles = localArticlesService.GetNamesOfSavedArticles();
            VisibleArticles = AllSavedArticles;
            NumbersText = "Number of Articles: " + VisibleArticles.Count;

            IsBusy = false;

            if (VisibleArticles != null && VisibleArticles.Count > 0)
            {
                MessageText = "Search your local library to read articles.";
                ResultsReturned = true;
            }
            else
            {
                MessageText = "It doesn't seem that you have any articles saved. Go and add download some articles in the Add to Library page.";
                ResultsReturned = false;
            }
        }
        #endregion

    }
}
