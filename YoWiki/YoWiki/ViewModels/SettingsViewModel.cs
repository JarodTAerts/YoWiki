using System;
using Xamarin.Forms;
using YoWiki.Models;
using YoWiki.Services;
using YoWiki.Services.Interfaces;

namespace YoWiki.ViewModels
{
    class SettingsViewModel : BaseViewModel
    {
        #region Properties
        private readonly ILocalArticlesService localArticlesService;

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

        private string _storageUsedString;
        public string StorageUsedString
        {
            get => _storageUsedString;
            set => SetProperty(ref _storageUsedString, value);
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
            localArticlesService = DependencyService.Resolve<ILocalArticlesService>();

            //Set the UI elements to the values stored in settings
            PickedItemNumber = Settings.NumberOfResults.ToString();
            DownloadOverCeullular = Settings.DownloadOverCell;
            DownloadImages = Settings.DownloadImages;
            ClearArticlesCommand = new Command(OnClearArticles);
            AboutYoWikiCommand = new Command(OnAboutYoWiki);

            StorageUsedString = $"Storage Used: {localArticlesService.GetStorageUsed()}";

            PersistentDownloadService.AddStatusCallBack(UpdateViewElements);
        }
        #endregion

        public void UpdateViewElements()
        {
            StorageUsedString = $"Storage Used: {localArticlesService.GetStorageUsed()}";
            PickedItemNumber = Settings.NumberOfResults.ToString();
            DownloadOverCeullular = Settings.DownloadOverCell;
            DownloadImages = Settings.DownloadImages;
        }

        public void UpdateViewElements(DownloadsStatusUpdate update)
        {
            StorageUsedString = $"Storage Used: {localArticlesService.GetStorageUsed()}";
            PickedItemNumber = Settings.NumberOfResults.ToString();
            DownloadOverCeullular = Settings.DownloadOverCell;
            DownloadImages = Settings.DownloadImages;
        }

        #region Command Functions
        private void OnAboutYoWiki()
        {
            // TODO: Create about page and navigate there
            Shell.Current.DisplayAlert("About YoWiki","YoWiki is an open-source, cross-platform mobile app that allows users to download Wikipedia articles and read them offline. " +
                "Users can maintain a personal Wikipedia library of any size with articles and topics of their choosing. It comes packed with various features to facilitate " +
                "learning everywhere and at every time whether there is a connection to the Internet or not. \n\n Website Coming Soon.", "Cool");
        }

        /// <summary>
        /// Function to handle when user sets the number of search results examples to return
        /// </summary>
        private void OnItemNumberPickerChanged()
        {
            Settings.NumberOfResults = Convert.ToInt32(PickedItemNumber);
        }

        /// <summary>
        /// Function to handle download images switch
        /// </summary>
        private void OnDownloadImagesChanged()
        {
            Settings.DownloadImages = (DownloadImages);
        }

        /// <summary>
        /// Function to handle download on cellular switch
        /// </summary>
        private void OnDownloadOverCellularChanged()
        {
            Settings.DownloadOverCell = (DownloadOverCeullular);
        }

        /// <summary>
        /// Function to handle when a user press the clear all saved articles button
        /// </summary>
        private async void OnClearArticles()
        {
            string choice = await Shell.Current.DisplayActionSheet("Are you sure you want to delete all your local articles?", "Nah", "DELETE 'UM");

            if (choice == "DELETE 'UM")
            {
                localArticlesService.ClearSavedArticles();
                _ = Shell.Current.DisplayAlert("Articles Cleared", "All articles from your local library.", "Ok");
                StorageUsedString = $"Storage Used: {localArticlesService.GetStorageUsed()}";
            }
        }
        #endregion

    }
}
