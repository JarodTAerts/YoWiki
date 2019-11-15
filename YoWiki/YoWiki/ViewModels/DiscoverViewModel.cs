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
                NotificationService.SendAlertOrNotification("Error Loading Results", MessageText, "Okay");
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
            NotificationService.SendAlertOrNotification("Article Downloaded", $"Article {currentArticleTitle} has been downloaded and added to your library!", "Cool!");
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
                    IsBusy = true;

                    // Figure out what articles have not been downloaded yet and aren't currently downloading
                    List<string> names = await wikipediaService.GetAllNamesFromSearch(EntryText, SearchResult.Totalhits);
                    List<string> savedNames = localArticlesService.GetNamesOfSavedArticles();
                    List<string> downloadingNames = PersistentDownloadService.GetStatus().ArticlesLeftToDownload;
                    List<string> namesToDownload = names.Where(n => !savedNames.Contains(hTMLService.ReplaceColons(n)) && !downloadingNames.Contains(hTMLService.ReplaceColons(n))).ToList();

                    IsBusy = false;

                    if (namesToDownload.Count <= 0)
                    {
                        NotificationService.SendAlertOrNotification("No New Articles", "It looks like all these articles are downloaded or are currently downloading. No new articles will be added to the queue.", "Alright");
                        return;
                    }

                    PersistentDownloadService.AddArticlesToList(namesToDownload);

                    NotificationService.SendAlertOrNotification("Downloads Queued:", "The articles you selected have been added to the download queue. You can see the progress by entering the download center in the top right corner.", "Cool");
                }
                else
                {
                    NotificationService.SendAlertOrNotification("To Many Articles", "Due to limitations with the Wikipedia API you cannot download more than 10,000 articles at once. Try adding another word to your search to narrow it. I am working on it.", "Okay");
                }
            }
        }
        #endregion
    }
}
