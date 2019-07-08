using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Xamarin.Forms;
using YoWiki.Services;
using YoWiki.Services.Interfaces;
using YoWiki.Views;

namespace YoWiki.ViewModels
{
    class BrowseViewModel : BaseViewModel
    {
        #region Properties and Bindings
        private ILocalArticlesService localArticlesService;
        private IHTMLService hTMLService;

        private List<string> _savedArticles;
        public List<string> SavedArticles
        {
            get => _savedArticles;
            set => SetProperty(ref _savedArticles, value);
        }

        private List<string> _allSavedArticles;
        public List<string> AllSavedArticles
        {
            get => _allSavedArticles;
            set => SetProperty(ref _allSavedArticles, value);
        }

        private string _selectedItem;
        public string SelectedItem
        {
            get => _selectedItem;
            set { SetProperty(ref _selectedItem, value); OnSelectedItemChanged(); }
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
            set => SetProperty(ref _entryText, value);
        }

        private bool _resultsReturned;
        public bool ResultsReturned
        {
            get => _resultsReturned;
            set => SetProperty(ref _resultsReturned, value);
        }

        private string currentArticleTitle;
        #endregion

        #region Commands
        public Command SearchButtonClickedCommand { get; set; }
        public Command DeleteArticleCommand { get; set; }
        #endregion

        #region Constructor
        public BrowseViewModel()
        {
            this.localArticlesService = DependencyService.Resolve<ILocalArticlesService>();
            hTMLService = DependencyService.Resolve<IHTMLService>();

            Title = "Browse";
            //Set values to what they should be when the page opens
            SearchButtonClickedCommand = new Command(OnSearchButtonClicked);
            DeleteArticleCommand = new Command(DeleteArticle);
            SavedArticles = new List<string>();
            ResultsReturned = false;
            MessageText = "Search your local library to read articles.";
            EntryText = "";
            IsBusy = false;
        }
        #endregion

        #region Command Functions
        /// <summary>
        /// Function to handle event when an item in the list is selected
        /// </summary>
        private void OnSelectedItemChanged()
        {
            //If the selected item is not null then open the view article page and send the title you selected as a parameter
            if (SelectedItem != null)
            {
                IsBusy = true;
                currentArticleTitle = hTMLService.ReplaceColons(SelectedItem);
                SelectedItem = null;
                // Download HTML for this article before saving it to storage
                string articleHtml = localArticlesService.GetHTMLTextFromFile(currentArticleTitle);
                WebViewSource webViewSource = new HtmlWebViewSource { Html = articleHtml };
                Shell.Current.Navigation.PushModalAsync(new NavigationPage(new ViewArticlePage(webViewSource, "Delete", DeleteArticleCommand)));
                IsBusy = false;
            }
        }

        /// <summary>
        /// Function to handle when the search button is clicked 
        /// </summary>
        private void OnSearchButtonClicked()
        {
            //Filter the articles based on the search that you entered
            SavedArticles = AllSavedArticles.Where(a => a.ToUpper().Contains(EntryText.ToUpper())).ToList();
            NumbersText = "Number of Articles: " + SavedArticles.Count;
        }

        private void DeleteArticle()
        {
            localArticlesService.DeleteArticle(currentArticleTitle);
            Shell.Current.DisplayAlert("Article Deleted", $"Article {currentArticleTitle} was deleted from local library.", "Ight");
        }
        #endregion

        #region Helpers
        public void LoadLocalArticles()
        {
            //When navigated to make sure no item is selected and set is searching so the activity monitor shows up
            SelectedItem = null;

                IsBusy = true;
                //Get all the names of the articles and put them into the all articles list
                //Then set Saved articles to all articles so they are all displayed to start
                AllSavedArticles = localArticlesService.GetNamesOfSavedArticles();
                SavedArticles = AllSavedArticles;
                NumbersText = "Number of Articles: " + SavedArticles.Count;
                //Once that is all done then make the activity monitor go away
                IsBusy = false;
            
            if (SavedArticles != null && SavedArticles.Count > 0)
            {
                //If there were names of articles returned then set results returned to show the list
                ResultsReturned = true;
            }
            else
            {
                //Otherwise let them know they dont have anything saved and dont show the list
                MessageText = "It doesn't seem that you have any articles saved. Go and add download some articles in the Add to Library page.";
                ResultsReturned = false;
            }
        }
        #endregion

    }
}
