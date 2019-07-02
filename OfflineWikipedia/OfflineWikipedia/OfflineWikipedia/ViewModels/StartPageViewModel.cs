using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace OfflineWikipedia.ViewModels
{
    /// <summary>
    /// Class to control the functionality and bindings of the start page of the app
    /// </summary>
	public class StartPageViewModel : ViewModelBase
    {
        #region Properties and Bindings
        private string _MessageText;
        public string MessageText
        {
            get => _MessageText;
            set => SetProperty(ref _MessageText, value);
        }
        #endregion

        #region Delegate Commands
        public DelegateCommand AddToLibraryButtonClickedCommand { get; set; }
        public DelegateCommand BrowseLibraryButtonClickedCommand { get; set; }
        public DelegateCommand SettingsButtonClickedCommand { get; set; }
        #endregion

        #region Constructor
        public StartPageViewModel(INavigationService navigationService) : base(navigationService)
        {
            //Set delegate commands and welcome text
            AddToLibraryButtonClickedCommand = new DelegateCommand(OnAddToLibrary);
            BrowseLibraryButtonClickedCommand = new DelegateCommand(OnBrowseLibrary);
            SettingsButtonClickedCommand = new DelegateCommand(OnSettings);
            MessageText = "Welcome to YoWiki, your one stop shot for offline wikipedia. With this app you are able" +
                "to download and store locally wikipedia articles onnvarious topics of your choosing. To get started" +
                " just click the 'Add to Library' button and search anything you are interested in. You will be guided " +
                "through the process and soon you will be able to browse your local wikipedia library!";
        }
        #endregion

        #region Command Functions
        /// <summary>
        /// Function to control when you press the Settings button. Sends you to settings page
        /// </summary>
        private async void OnSettings()
        {
            Debug.WriteLine("Settings");
            await NavigationService.NavigateAsync("SettingsPage");
        }

        /// <summary>
        /// Function to control when you press the Browse Library page. Sends you to that page
        /// </summary>
        private async void OnBrowseLibrary()
        {
            Debug.WriteLine("Browse Library");
            await NavigationService.NavigateAsync("BrowseLibraryPage");
        }

        /// <summary>
        /// Function to control when you press the Add to Library page. Sends you there
        /// </summary>
        private async void OnAddToLibrary()
        {
            Debug.WriteLine("Add to library");
            await NavigationService.NavigateAsync("MainPage");
        }
        #endregion
    }
}
