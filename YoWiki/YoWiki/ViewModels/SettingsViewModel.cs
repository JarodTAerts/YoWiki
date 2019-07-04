using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Xamarin.Forms;
using YoWiki.Services;
using YoWiki.Services.Interfaces;

namespace YoWiki.ViewModels
{
    class SettingsViewModel : BaseViewModel
    {
        #region Properties and Bindings
        private readonly ILocalArticlesService localArticlesService;

        //private IPageDialogService _dialogService;

        private string _pickedItemNumber;
        public string PickedItemNumber
        {
            get => _pickedItemNumber;
            set { SetProperty(ref _pickedItemNumber, value); OnItemNumberPickerChanged(); }
        }

        private bool _downloadOverCellular;
        public bool DownloadOverCeullular
        {
            get => _downloadOverCellular;
            set { SetProperty(ref _downloadOverCellular, value); OnDownloadOverCellularChanged(); }
        }

        private bool _downloadImages;
        public bool DownloadImages
        {
            get => _downloadImages;
            set { SetProperty(ref _downloadImages, value); OnDownloadImagesChanged(); }
        }
        #endregion

        #region Delegate Commands
        public Command BackButtonCommand { get; set; }
        public Command ClearArticlesCommand { get; set; }
        public Command AboutYoWikiCommand { get; set; }
        #endregion

        #region Constructor
        public SettingsViewModel()
        {
            this.localArticlesService = DependencyService.Resolve<ILocalArticlesService>();

            //Set the UI elements to the values stored in settings
            PickedItemNumber = Settings.NumberOfResults.ToString();
            DownloadOverCeullular = Settings.DownloadOverCell;
            DownloadImages = Settings.DownloadImages;
            BackButtonCommand = new Command(OnBackButton);
            ClearArticlesCommand = new Command(OnClearArticles);
            AboutYoWikiCommand = new Command(OnAboutYoWiki);
            //_dialogService = dialog;
        }
        #endregion

        #region Command Functions
        /// <summary>
        /// Function that sends you back to the start page when you press the back button at the bottom
        /// </summary>
        private void OnBackButton()
        {
            //await NavigationService.GoBackAsync();
        }

        private void OnAboutYoWiki()
        {
            //await NavigationService.NavigateAsync("AboutAppPage");
        }

        /// <summary>
        /// Function that handles when you change the number in the picker to select how many example articles to return
        /// Changes the stored setting to what you entered
        /// </summary>
        private void OnItemNumberPickerChanged()
        {
            Settings.NumberOfResults = Convert.ToInt32(PickedItemNumber);
        }

        /// <summary>
        /// Function to handle when you change the switch to control if you want to download images
        /// Changes the stored setting to what you entered
        /// </summary>
        private void OnDownloadImagesChanged()
        {
            Settings.DownloadImages = (DownloadImages);
        }

        /// <summary>
        /// Function to control when you change the switch to control if you want to download over cell connection
        /// Changes the stored setting to what you entered
        /// </summary>
        private void OnDownloadOverCellularChanged()
        {
            Debug.WriteLine("Cellular: " + DownloadOverCeullular);
            Settings.DownloadOverCell = (DownloadOverCeullular);
        }

        /// <summary>
        /// Function to handle when you press the clear all saved articles button
        /// </summary>
        private void OnClearArticles()
        {
            //TODO: Make a dialog that makes sure they user wants to clear all the articles before we clear them
            localArticlesService.ClearSavedArticles();
            //await _dialogService.DisplayAlertAsync("Articles Cleared", "All articles from your local library.", "Ok");
        }
        #endregion

    }
}
