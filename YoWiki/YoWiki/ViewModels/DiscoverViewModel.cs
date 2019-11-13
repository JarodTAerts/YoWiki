using Plugin.LocalNotification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using YoWiki.Models;
using YoWiki.Services;
using YoWiki.Services.Interfaces;
using YoWiki.Views;

namespace YoWiki.ViewModels
{
    class DiscoverViewModel : BaseViewModel
    {
        #region Properties and Bindings
        // Services
        private readonly ILocalArticlesService localArticlesService;
        private readonly IHTMLService hTMLService;
        private readonly IWikipediaService wikipediaService;

        // Public properties
        private WikipediaSearchItem _selectedItem;
        public WikipediaSearchItem SelectedItem
        {
            get => _selectedItem;
            // Do this weird set function so that we can call function whenever the property changes
            set { SetProperty(ref _selectedItem, value); _ = OnSelectedItemChanged(); }
        }

        private WikipediaSearchResult _searchResult;
        public WikipediaSearchResult SearchResult
        {
            get => _searchResult;
            set => SetProperty(ref _searchResult, value);
        }

        private string _messageText;
        public string MessageText
        {
            get => _messageText;
            set => SetProperty(ref _messageText, value);
        }

        private double _barProgress;
        public double BarProgress
        {
            get => _barProgress;
            set => SetProperty(ref _barProgress, value);
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

        // Private Properties
        private string currentArticleTitle;
        private string currentArticleText;
        #endregion

        #region Commands
        public Command SearchButtonClickedCommand { get; set; }
        public Command DownloadAllArticlesCommand { get; set; }
        #endregion

        #region Constructor
        public DiscoverViewModel()
        {
            localArticlesService = DependencyService.Resolve<ILocalArticlesService>();
            hTMLService = DependencyService.Resolve<IHTMLService>();
            wikipediaService = DependencyService.Resolve<IWikipediaService>();

            MessageText = "Search anything you are interested in to get started.";

            SearchButtonClickedCommand = new Command(OnSearchButtonClicked);
            DownloadAllArticlesCommand = new Command(OnDownloadAllClicked);
        }
        #endregion

        #region Command Functions
        /// <summary>
        /// Command function to handle functionality when the search button is pressed
        /// </summary>
        private async void OnSearchButtonClicked()
        {
            ResultsReturned = false;
            MessageText = "Searching...";
            IsBusy = true;
            try
            {
                int numberOfArticlesReturned = Settings.NumberOfResults;
                SearchResult = await wikipediaService.SearchTopic(EntryText, numberOfArticlesReturned);

                // Clean the article snippets so they don't look all htmly
                foreach (WikipediaSearchItem w in SearchResult.Items)
                {
                    w.Snippet = hTMLService.SimpleHTMLStrip(w.Snippet);
                }

                IsBusy = false;
                MessageText = $"Total Articles: {SearchResult.Totalhits} \n {SearchResult.Items.Count} Example Articles:";
                ResultsReturned = true;
            }
            catch (Exception)
            {
                // TODO: Catch different types of errors and probably give a pop up instead of just a text notification
                MessageText = "Results could not be loaded. Internet connection is required for this functionality, please check your connection.";
                IsBusy = false;
                ResultsReturned = false;
            }
        }

        /// <summary>
        /// Command function to handle when the selected item from the list is changed
        /// </summary>
        /// <returns></returns>
        private async Task OnSelectedItemChanged()
        {
            if (SelectedItem != null)
            {
                IsBusy = true;
                currentArticleTitle = SelectedItem.Title;
                SelectedItem = null; // Set to null so the item isn't highlighted in the list

                currentArticleText = hTMLService.InjectCSS(await wikipediaService.DownloadArticleHTML(currentArticleTitle));

                WebViewSource webViewSource = new HtmlWebViewSource { Html = currentArticleText };
                await Shell.Current.Navigation.PushModalAsync(new NavigationPage(new ViewArticlePage(webViewSource, "Save", new Command(DownloadCurrentArticle))));

                IsBusy = false;
            }
        }

        /// <summary>
        /// Command function to download the article the user is currently viewing
        /// </summary>
        private void DownloadCurrentArticle()
        {
            localArticlesService.SaveHTMLFileToStorage(hTMLService.ReplaceColons(currentArticleTitle), currentArticleText);
            _ = SendAlertOrNotification("Article Downloaded", $"Article {currentArticleTitle} has been downloaded and added to your library!", "Cool!");
        }

        /// <summary>
        /// Command function to handle when the user selects to download all the articles
        /// </summary>
        private async void OnDownloadAllClicked()
        {
            if (SearchResult.Items != null)
            {
                // Make sure there are less than 10000 articles since the way we are calling the wikipedia api only allows for 10000 downloads
                if (SearchResult.Totalhits < 10000)
                {
                    _ = SendAlertOrNotification("Downloading your articles:", "The articles will now be downloaded. You can leave the app. A notification will be sent when downloading is finished." +
                        "\n Only articles that you have not already saved will be downloaded to save time.", "Okay");

                    //IsBusy = true;
                    MessageText = "Fetching the names of all articles from Wikipedia.";
                    //DateTime startTime = DateTime.Now;

                    // Figure out what articles have not been downloaded yet
                    List<string> names = await wikipediaService.GetAllNamesFromSearch(EntryText, SearchResult.Totalhits);
                    List<string> savedNames = localArticlesService.GetNamesOfSavedArticles();
                    List<string> namesToDownload = names.Where(n => !savedNames.Contains(hTMLService.ReplaceColons(n))).ToList();

                    PersistentDownloadService.AddArticlesToList(namesToDownload);
                    
                    //await DownloadAllArticlesFromList(namesToDownload);

                    //IsBusy = false;

                    //UpdateAverageDownloadTime(GetMilliSecondsSinceStart(startTime), namesToDownload.Count);
                    MessageText = $"Downloaded {names.Count} Articles."; //In {FormatTime(GetMilliSecondsSinceStart(startTime))}.";
                    _ = SendAlertOrNotification("Articles Added", $"{names.Count} articles have been downloaded and added to your library.", "Okay");
                }
                else
                {
                    _ = SendAlertOrNotification("To Many Articles", "Due to limitations with the Wikipedia API you cannot download more than 10,000 articles at once. Try adding another word to your search to narrow it. I am working on it.", "Okay");
                }
            }
        }
        #endregion

        #region Helper Functions
        /// <summary>
        /// Function to handle sending notifications and alerts. If the app is in background it will send notification,
        /// if the app is in the foreground it will send an alert
        /// </summary>
        /// <param name="title">Title of alert or notification</param>
        /// <param name="text">Main body text of alert or notification</param>
        /// <param name="buttonText">Button text for alert</param>
        /// <returns>Nothing</returns>
        private async Task SendAlertOrNotification(string title, string text, string buttonText)
        {
            if (App.IsInBackground)
            {
                var notification = new NotificationRequest
                {
                    NotificationId = 100,
                    Title = title,
                    Description = text
                };
                NotificationCenter.Current.Show(notification);
            }

            // Also send alert they can see when they get back into the app
            await Shell.Current.DisplayAlert(title, text, buttonText);
        }

        /// <summary>
        /// Function to get time formatted in a user friendly way
        /// </summary>
        /// <param name="milliseconds">Millisecond time to format</param>
        /// <returns>String with formatted time</returns>
        private string FormatTime(double milliseconds)
        {
            string formattedTime = string.Format("{0:f2} Milliseconds", milliseconds);
            double seconds = milliseconds / 1000;
            double minutes = seconds / 60;
            double hours = minutes / 60;
            double days = hours / 24;

            if (seconds > 1)
            {
                formattedTime = string.Format("{0:f2} Seconds", seconds);
            }
            if (minutes > 1)
            {
                formattedTime = string.Format("{0:f2} Minutes", minutes);
            }
            if (hours > 1)
            {
                formattedTime = string.Format("{0:f2} Hours", hours);
            }
            if (days > 1)
            {
                formattedTime = string.Format("{0:f2} Days", days);
            }

            return formattedTime;
        }

        /// <summary>
        /// Function to get the number of milliseconds that have passed since a certain time
        /// </summary>
        /// <param name="start">Start time to measure from</param>
        /// <returns>Double representing the milliseconds</returns>
        private double GetMilliSecondsSinceStart(DateTime start)
        {
            return (DateTime.Now - start).TotalMilliseconds;
        }

        /// <summary>
        /// Function to download all the articles in a list of article names
        /// </summary>
        /// <param name="articleNames">List with names of articles to download</param>
        /// <returns>Nothing</returns>
        private async Task DownloadAllArticlesFromList(List<string> articleNames)
        {
            for (int i = 0; i < articleNames.Count; i++)
            {
                int itemsLeft = articleNames.Count - i;
                double estTime = itemsLeft * Settings.AverageDownloadTime;
                string formattedEstTime = FormatTime(estTime);
                MessageText = $"Downloading {i} out of {articleNames.Count} Articles...\nEstimated time: {formattedEstTime}";

                await DownloadArticle(articleNames[i]);
            }
        }

        private async Task DownloadArticle(string title)
        {
            try
            {
                string articleText = hTMLService.InjectCSS(await wikipediaService.DownloadArticleHTML(title));

                localArticlesService.SaveHTMLFileToStorage(hTMLService.ReplaceColons(title), articleText);
            }
            catch
            {
                MessageText = "Failed";
            }
        }

        /// <summary>
        /// Function to update the average download times
        /// </summary>
        /// <param name="totalTime">Total time it took to download the articles</param>
        /// <param name="numberDownloaded">Number of articles that were downloaded</param>
        private void UpdateAverageDownloadTime(double totalTime, int numberDownloaded)
        {
            // Check if any data is in the average download time, would be 0 if there was no data
            if (Math.Abs(Settings.AverageDownloadTime - 1) < 0.001)
            {
                Settings.AverageDownloadTime = totalTime / numberDownloaded;
                Settings.NumberOfEntriesInAverageDownloadTime = numberDownloaded;
            }
            else
            {
                Settings.NumberOfEntriesInAverageDownloadTime += numberDownloaded;
                Settings.AverageDownloadTime = (Settings.AverageDownloadTime * (Settings.NumberOfEntriesInAverageDownloadTime - numberDownloaded)
                    + totalTime) / Settings.NumberOfEntriesInAverageDownloadTime;
            }
        }
        #endregion

    }
}
