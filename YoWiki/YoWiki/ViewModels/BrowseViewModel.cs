using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Xamarin.Forms;
using YoWiki.Services;
using YoWiki.Services.Interfaces;

namespace YoWiki.ViewModels
{
    class BrowseViewModel : BaseViewModel
    {
        #region Properties and Bindings
        private ILocalArticlesService localArticlesService;

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

        private string _returnedText;
        public string ReturnedText
        {
            get => _returnedText;
            set => SetProperty(ref _returnedText, value);
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

        private bool _isSearching;
        public bool IsSearching
        {
            get => _isSearching;
            set => SetProperty(ref _isSearching, value);
        }
        #endregion

        #region Commands
        public Command SearchButtonClickedCommand { get; set; }
        #endregion

        #region Constructor
        public BrowseViewModel()
        {
            this.localArticlesService = DependencyService.Resolve<ILocalArticlesService>();

            Title = "Browse";
            //Set values to what they should be when the page opens
            SearchButtonClickedCommand = new Command(OnSearchButtonClicked);
            SavedArticles = new List<string>();
            ResultsReturned = false;
            ReturnedText = "Search your local library to read articles.";
            EntryText = "";
            IsSearching = false;

            OnNavigatedTo();
        }
        #endregion

        #region Command Functions
        /// <summary>
        /// Function to handle event when an item in the list is selected
        /// </summary>
        private void OnSelectedItemChanged()
        {
            ////If the selected item is not null then open the view article page and send the title you selected as a parameter
            //if (SelectedItem != null)
            //{
            //    Debug.WriteLine("Selected thing changed" + SelectedItem);
            //    var navparams = new NavigationParameters();
            //    navparams.Add("TITLE", SelectedItem);
            //    await NavigationService.NavigateAsync("ViewArticlePage", navparams);
            //}
        }

        /// <summary>
        /// Function to handle when the search button is clicked 
        /// </summary>
        private void OnSearchButtonClicked()
        {
            Debug.WriteLine("Searched something");
            //Filter the articles based on the search that you entered
            SavedArticles = AllSavedArticles.Where(a => a.ToUpper().Contains(EntryText.ToUpper())).ToList();
            NumbersText = "Number of Articles: " + SavedArticles.Count;
        }
        #endregion

        #region Overrides
        public void OnNavigatedTo()
        {
            //When navigated to make sure no item is selected and set is searching so the activity monitor shows up
            SelectedItem = null;
            if (SavedArticles == null || SavedArticles.Count == 0)
            {
                IsSearching = true;
                //Get all the names of the articles and put them into the all articles list
                //Then set Saved articles to all articles so they are all displayed to start
                AllSavedArticles = localArticlesService.GetNamesOfSavedArticles();
                SavedArticles = AllSavedArticles;
                NumbersText = "Number of Articles: " + SavedArticles.Count;
                //Once that is all done then make the activity monitor go away
                IsSearching = false;
            }
            if (SavedArticles != null && SavedArticles.Count > 0)
            {
                //If there were names of articles returned then set results returned to show the list
                ResultsReturned = true;
            }
            else
            {
                //Otherwise let them know they dont have anything saved and dont show the list
                ReturnedText = "It doesn't seem that you have any articles saved. Go and add download some articles in the Add to Library page.";
                ResultsReturned = false;
            }
        }
        #endregion

    }
}
