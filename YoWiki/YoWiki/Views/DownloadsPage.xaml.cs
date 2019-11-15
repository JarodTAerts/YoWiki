using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using YoWiki.Models;
using YoWiki.Services;

namespace YoWiki.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DownloadsPage : ContentPage
    {
        #region Properties
        /// <summary>
        /// List that displays all the articles that need to be downloaded yet
        /// </summary>
        ObservableCollection<string> downloads = new ObservableCollection<string>();
        public ObservableCollection<string> DownloadListItems { get { return downloads; } }
        #endregion

        #region Commands
        public Command GoBackCommand;
        #endregion

        public DownloadsPage()
        {
            InitializeComponent();
            GoBackCommand = new Command(GoBack);
            backButton.Command = GoBackCommand;

            DownloadList.ItemsSource = DownloadListItems;

            DownloadsStatusUpdate update = PersistentDownloadService.GetStatus();

            if(update.TotalNumArticlesToDownload == 0)
            {
                StatusText.Text = "There are currently no articles queued for download.";
                DownloadProgress.Progress = 0.0;
            }

            UpdateDownloadStatus(update);

            PersistentDownloadService.AddStatusCallBack(UpdateDownloadStatus);
        }

        /// <summary>
        /// Function to update all properties of this page with new information from the download service
        /// </summary>
        /// <param name="update"></param>
        public async void UpdateDownloadStatus(DownloadsStatusUpdate update)
        {
            StatusText.Text = update.StatusMessage;

            await DownloadProgress.ProgressTo((float)update.NumberOfArticlesDownloaded / (float)update.TotalNumArticlesToDownload, 100, Easing.Linear);

            // Load all the articles left to download in the articles left to download
            // TODO: Make this faster by instead returning the articles that was just downloaded and removing it from the list
            downloads.Clear();
            foreach(string article in update.ArticlesLeftToDownload)
            {
                downloads.Add(article);
            }
        }

        /// <summary>
        /// Function to go pop this page off the navigation stack
        /// </summary>
        public void GoBack()
        {
            Shell.Current.Navigation.PopModalAsync();
        }
    }
}