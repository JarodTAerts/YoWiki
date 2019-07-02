﻿using LearningAPIs;
using OfflineWikipedia.Services;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;

namespace OfflineWikipedia.ViewModels
{
    /// <summary>
    /// Class that controls the functionality of the BrowseLibraryPage
    /// </summary>
	public class BrowseLibraryPageViewModel : ViewModelBase
	{

        #region Properties and Bindings
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

        #region Delagate Commands
        public DelegateCommand SearchButtonClickedCommand { get; set; }
        #endregion

        #region Constructor
        public BrowseLibraryPageViewModel(INavigationService navigationService) : base(navigationService)
        {
            //Set values to what they should be when the page opens
            SearchButtonClickedCommand = new DelegateCommand(OnSearchButtonClicked);
            SavedArticles = new List<string>();
            ResultsReturned = false;
            ReturnedText = "Search your local library to read articles.";
            EntryText = "";
            IsSearching = false;
        }
        #endregion

        #region Command Functions
        /// <summary>
        /// Function to handle event when an item in the list is selected
        /// </summary>
        private async void OnSelectedItemChanged()
        {
            //If the selected item is not null then open the view article page and send the title you selected as a paramater
            if (SelectedItem != null)
            {
                Debug.WriteLine("Selected thing changed" + SelectedItem);
                var navparams = new NavigationParameters();
                navparams.Add("TITLE", SelectedItem);
                await NavigationService.NavigateAsync("ViewArticlePage",navparams);
            }
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
        public async override void OnNavigatedTo(NavigationParameters parameters)
        {
            //When navigated to make sure no item is selected and set is searching so the activity monitor shows up
            SelectedItem = null;
            if (SavedArticles == null || SavedArticles.Count == 0)
            {
                IsSearching = true;
                //Get all the names of the articles and put them into the all articles list
                //Then set Saved articles to all articles so they are all displayed to start
                AllSavedArticles = await StorageService.GetNamesOfSavedArticles();
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
                //Otehrwise let them know they dont have anything saved and dont show the list
                ReturnedText = "It doesn't seem that you have any articles saved. Go and add download some articles in the Add to Library page.";
                ResultsReturned = false;
            }
        }
        #endregion

    }
}
