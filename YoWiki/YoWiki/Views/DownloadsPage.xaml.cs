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
        ObservableCollection<string> downloads = new ObservableCollection<string>();
        public ObservableCollection<string> DownloadListItems { get { return downloads; } }

        public Command GoBackCommand;

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

            UpdateDownloadStatusMessage(update);

            PersistentDownloadService.AddStatusCallBack(UpdateDownloadStatusMessage);
        }

        public async void UpdateDownloadStatusMessage(DownloadsStatusUpdate update)
        {
            StatusText.Text = update.StatusMessage;

            await DownloadProgress.ProgressTo((float)update.NumberOfArticlesDownloaded / (float)update.TotalNumArticlesToDownload, 100, Easing.Linear);

            downloads.Clear();

            foreach(string article in update.ArticlesLeftToDownload)
            {
                downloads.Add(article);
            }
        }

        public void GoBack()
        {
            Shell.Current.Navigation.PopModalAsync();
        }
    }
}