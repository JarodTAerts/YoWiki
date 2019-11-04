using System;
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

        private List<string> _visibleArticles;
        public List<string> VisibleArticles
        {
            get => _visibleArticles;
            private set => SetProperty(ref _visibleArticles, value);
        }

        private string _messageText;
        public string MessageText
        {
            get => _messageText;
            set => SetProperty(ref _messageText, value);
        }

        private string _numbersText;
        public string NumbersText
        {
            get => _numbersText;
            set => SetProperty(ref _numbersText, value);
        }

        private string _entryText;
        public string EntryText
        {
            get => _entryText;
            set 
            { 
                SetProperty(ref _entryText, value);
                OnSearchButtonClicked();
            }
        }

        private bool _resultsReturned;
        public bool ResultsReturned
        {
            get => _resultsReturned;
            set => SetProperty(ref _resultsReturned, value);
        }

        // Private Properties
        private string currentArticleTitle;
        private List<string> AllSavedArticles;
        #endregion

        #region Commands
        public Command SearchButtonClickedCommand { get; set; }
        public Command RandomButtonClickedCommand { get; set; }
        #endregion

        #region Constructor
        public BrowseViewModel()
        {
            localArticlesService = DependencyService.Resolve<ILocalArticlesService>();
            hTMLService = DependencyService.Resolve<IHTMLService>();

            SearchButtonClickedCommand = new Command(OnSearchButtonClicked);
            RandomButtonClickedCommand = new Command(OnRandomArticleClicked);
            MessageText = "Search your local library to read articles.";
        }
        #endregion

        #region Command Functions

        /// <summary>
        /// Command function to handle event when an item in the list is selected
        /// </summary>
        private async void OnSelectedItemChanged()
        {
            if (SelectedItem != null)
            {
                IsBusy = true;
                currentArticleTitle = hTMLService.ReplaceColons(SelectedItem);
                SelectedItem = null;

                string articleHtml = localArticlesService.GetHTMLTextFromFile(currentArticleTitle);
                WebViewSource webViewSource = new HtmlWebViewSource { Html = articleHtml };
                await Shell.Current.Navigation.PushModalAsync(new NavigationPage(new ViewArticlePage(webViewSource, "Delete", new Command(DeleteArticle))));
                IsBusy = false;
            }
        }

        private async void OnRandomArticleClicked()
        {
            var rand = new Random();
            SelectedItem = VisibleArticles[rand.Next(VisibleArticles.Count)];
        }

        /// <summary>
        /// Command function to handle when the search button is clicked 
        /// </summary>
        private void OnSearchButtonClicked()
        {
            if(EntryText == null)
            {
                VisibleArticles = AllSavedArticles;
            }
            else
            {
                VisibleArticles = AllSavedArticles.Where(a => a.ToUpper().Contains(EntryText.ToUpper())).ToList();
            }
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

                // Re-search if there is a search in the search box
                if (EntryText != string.Empty && EntryText != null)
                    OnSearchButtonClicked();
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
