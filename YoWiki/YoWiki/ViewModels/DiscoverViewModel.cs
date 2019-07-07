using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
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

        private readonly ILocalArticlesService localArticlesService;
        private readonly IHTMLService hTMLService;
        private readonly IWikipediaService wikipediaService;

        private WikipediaSearchResult _searchResult;
        public WikipediaSearchResult SearchResult
        {
            get => _searchResult;
            set => SetProperty(ref _searchResult, value);
        }

        private WikipediaSearchItem _selectedItem;
        public WikipediaSearchItem SelectedItem
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

        private double _barProgress;
        public double BarProgress
        {
            get => _barProgress;
            set => SetProperty(ref _barProgress, value);
        }
        #endregion

        #region Commands
        public Command SearchButtonClickedCommand { get; set; }
        public Command DownloadAllArticlesCommand { get; set; }
        #endregion

        #region Constructor
        public DiscoverViewModel()
        {
            this.localArticlesService = DependencyService.Resolve<ILocalArticlesService>();
            this.hTMLService = DependencyService.Resolve<IHTMLService>();
            this.wikipediaService = DependencyService.Resolve<IWikipediaService>();

            Title = "Discover";
            EntryText = "Nebraska"; // Just for testing

            SearchButtonClickedCommand = new Command(OnSearchButtonClicked);
            DownloadAllArticlesCommand = new Command(OnDownloadAllClicked);
            ResultsReturned = false;
            ReturnedText = "Search any topic you are interested in to get some results.";
            IsBusy = false;
        }
        #endregion

        #region Command Functions
        /// <summary>
        /// Function to handle when the search button is clicked
        /// </summary>
        private async void OnSearchButtonClicked()
        {
            try
            {
                ResultsReturned = false;
                ReturnedText = "Searching...";
                //Set is searching to true, get the number of articles to get as examples and call function to search from APIService
                IsBusy = true;
                int numberOfArticlesReturned = Settings.NumberOfResults;
                SearchResult = await wikipediaService.SearchTopic(EntryText, numberOfArticlesReturned);
                //Set is searching to false and set the returned text and show the list
                IsBusy = false;
                ReturnedText = "Total Articles: " + SearchResult.Totalhits + "\n\n" + SearchResult.Items.Count + " Example Articles:";
                ResultsReturned = true;
            }
            catch (Exception)
            {
                //If there is an exception, probably due to Internet connectivity then let them know and stop searching
                ReturnedText = "Results could not be loaded. Internet connection is required for this functionality, please check your connection.";
                IsBusy = false;
                ResultsReturned = false;
            }
        }

        /// <summary>
        /// Function to handle when an item is selected from the list. If one item is selected then it will be downloaded to the library
        /// This should probably be changed to displaying the article and offering a download button. But who knows?
        /// </summary>
        private async Task OnSelectedItemChanged()
        {
            //If the item you selected is not null then use the storage service to save that article to storage
            if (SelectedItem != null)
            {
                IsBusy = true;
                string articleTitle = SelectedItem.Title;
                SelectedItem = null;
                // Download HTML for this article before saving it to storage
                string articleHtml = await wikipediaService.DownloadArticleHTML(articleTitle);
                WebViewSource webViewSource= new HtmlWebViewSource { Html=articleHtml };
                //await Shell.Current.Navigation.PushModalAsync(new NavigationPage(new ViewArticlePage(webViewSource)));

                localArticlesService.SaveHTMLFileToStorage(articleTitle, articleHtml);
                IsBusy = false;
                //await Shell.Current.DisplayAlert("Article Added", $"Article {SelectedItem.Title} has been downloaded and added to your library!", "Cool!");
            }
        }

        /// <summary>
        /// Function to handle when the download all articles button is clicked
        /// </summary>
        private async void OnDownloadAllClicked()
        {
            if (SearchResult.Items != null)
            {
                //Because of limit in Wikipedia's API no more than 10,000 articles can be downloaded from one search query
                if (SearchResult.Totalhits < 10000)
                {
                    await SendAlertOrNotification("Downloading your articles:", "The articles will now be downloaded. You can leave the app. A notification will be sent when downloading is finished." +
                        "\n Only articles that you have not already saved will be downloaded to save time.", "Okay");
                    //Start activity indicator and Let them know what is happening
                    IsBusy = true;
                    ReturnedText = "Fetching the names of all articles from Wikipedia.";

                    //Set the start time and get all the names of the articles to be downloaded, then log
                    DateTime startTime = DateTime.Now;
                    List<string> names = await wikipediaService.GetAllNamesFromSearch(EntryText, SearchResult.Totalhits);
                    List<string> savedNames = localArticlesService.GetNamesOfSavedArticles();
                    List<string> namesToDownload = names.Where(n => !savedNames.Contains(hTMLService.ReplaceColons(n))).ToList();
                    Debug.WriteLine("Time to get Names (Seconds): " + GetMilliSecondsSinceStart(startTime));

                    //Actually download all the articles
                    await DownloadAllArticlesFromList(namesToDownload);

                    //Stop the activity indicator, set the returned text with some info and send an alert
                    IsBusy = false;
                    ReturnedText = "Downloaded " + names.Count + " Articles. In " + GetTimeAddOnForEstimate(GetMilliSecondsSinceStart(startTime)) + ".";
                    await SendAlertOrNotification("Articles Added", names.Count + " articles have been downloaded and added to your library.", "Okay");
                }
                else
                {
                    //If there search has more than 10,000 articles, warn them they need to make their search more specific and don't let them proceed
                    await SendAlertOrNotification("To Many Articles", "Due to limitations with the Wikipedia API you cannot download more than 10,000 articles at once. Try adding another word to your search to narrow it. I am working on it.", "Okay");
                }
            }
        }
        #endregion

        #region Helper Functions
        /// <summary>
        /// Function that will send a notification if the app is in the background and an alert if in the foreground
        /// </summary>
        /// <param name="title">Title of the alert or notification</param>
        /// <param name="text">Main text of the alert or notification</param>
        /// <param name="buttonText">Text for the dismiss button</param>
        /// <returns></returns>
        private async Task SendAlertOrNotification(string title, string text, string buttonText)
        {
            // TODO: Send notification if app is in background
            await Shell.Current.DisplayAlert(title, text, buttonText);
        }

        /// <summary>
        /// Function to get the text to be displayed at the end of the estimated time text on the download screen
        /// </summary>
        /// <param name="milliseconds">Estimated time in milliseconds</param>
        /// <returns></returns>
        private string GetTimeAddOnForEstimate(double milliseconds)
        {
            //Set the initial string and get the amount of time in seconds, minutes, hours, and days
            string addOn = String.Format("{0:f2} Milliseconds", milliseconds);
            double seconds = milliseconds / 1000;
            double minutes = seconds / 60;
            double hours = minutes / 60;
            double days = hours / 24;

            //Set the string to be the one that will appear the best based on the numbering
            if (seconds > 1)
            {
                addOn = string.Format("{0:f2} Seconds", seconds);
            }
            if (minutes > 1)
            {
                addOn = string.Format("{0:f2} Minutes", minutes);
            }
            if (hours > 1)
            {
                addOn = string.Format("{0:f2} Hours", hours);
            }
            if (days > 1)
            {
                addOn = string.Format("{0:f2} Days", days);
            }

            return addOn;
        }

        /// <summary>
        /// Function that gets the amount of time between the start time and now in milliseconds
        /// </summary>
        /// <param name="start">DateTime of the start time</param>
        /// <returns></returns>
        private double GetMilliSecondsSinceStart(DateTime start)
        {
            return (DateTime.Now - start).TotalMilliseconds;
        }

        /// <summary>
        /// Function that actually downloads the articles in a list to local storage
        /// </summary>
        /// <param name="names">String list containing the names of all the articles to be downloaded</param>
        /// <returns></returns>
        private async Task DownloadAllArticlesFromList(List<string> names)
        {
            for (int i = 0; i < names.Count; i++)
            {
                //Estimate the amount of time remaining and show that information and the progress to the user
                int itemsLeft = names.Count - i;
                double estTime = itemsLeft * Settings.AverageDownloadAndProcessingTimePerFile;
                string estAddOn = GetTimeAddOnForEstimate(estTime);
                ReturnedText = "Downloading " + i + " out of " + names.Count + " Articles...\nEstimated time: " + estAddOn;

                //Set the start time for downloading and download the article with name names[i]
                DateTime timeStartDownload = DateTime.Now;
                // Download text first
                localArticlesService.SaveHTMLFileToStorage(names[i], "");

                //Set the start time and process the HTML text from article
                DateTime timeStartClean = DateTime.Now;
                await hTMLService.CleanHTMLFile(names[i]);

                //Get the time spent for each of the processes, and to total. And then log it
                double timeSpentDownloading = (timeStartClean - timeStartDownload).TotalMilliseconds;
                double timeSpentClean = (DateTime.Now - timeStartClean).TotalMilliseconds;
                double totalTime = timeSpentClean + timeSpentDownloading;
                Debug.WriteLine("Time to Download: " + timeSpentDownloading);
                Debug.WriteLine("Time to Process: " + timeSpentClean);
                //TODO: Some better type of logging than just writing to console. Maybe to other file or something easier to read?

                //Update the averages for more accurate predictions in the future
                SetDownloadAndProcessingAverages(totalTime);
            }
        }

        /// <summary>
        /// Function to update or set the average time it takes to download and process a file
        /// </summary>
        /// <param name="totalTime">Time in milliseconds it took to download and process the last file</param>
        private void SetDownloadAndProcessingAverages(double totalTime)
        {
            //If there have been no entries into this yet, set the average equal to the time spent and the number of entries to 1
            if (Math.Abs(Settings.AverageDownloadAndProcessingTimePerFile - 1) < 0.001)
            {
                Settings.AverageDownloadAndProcessingTimePerFile = totalTime;
                Settings.NumberOfEntriesInAverageDownloadTime = 1;
            }
            else
            {
                //otherwise increase the number of entries in the average and get a new average. Maybe this should be the other way. But it works the same
                Settings.NumberOfEntriesInAverageDownloadTime++;
                Settings.AverageDownloadAndProcessingTimePerFile = (Settings.AverageDownloadAndProcessingTimePerFile * (Settings.NumberOfEntriesInAverageDownloadTime - 1)
                    + totalTime) / Settings.NumberOfEntriesInAverageDownloadTime;
            }
        }
        #endregion

    }
}
