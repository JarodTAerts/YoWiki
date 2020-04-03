using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using YoWiki.Accessors.Interfaces;
using YoWiki.Models;
using YoWiki.Services.Interfaces;

namespace YoWiki.Services
{
    /// <summary>
    /// This is a static service class that deals with downloading articles. It is built so that it maintains state in case the application is killed, and should be relatively thread safe
    /// </summary>
    public static class PersistentDownloadService
    {
        #region Properties and Class Variables
        private static object queueLock = new object();
        public static List<string> DownloadQueue
        {
            get; private set;
        }
        public static int TotalNumArticles
        {
            get; private set;
        } = 0;

        private static IWikipediaAccessor wikipediaAccessor;
        private static IHTMLService hTMLService;
        private static ILocalArticlesService localArticlesService;

        private static List<Action<DownloadsStatusUpdate>> subscribedStatusCallbacks;
        private static List<Action<int>> subscribedBadgeCallbacks;

        #endregion

        /// <summary>
        /// Function to start the service. This will prime all the used services and start the download queue consumer function.
        /// It will also load any cached un-downloaded articles.
        /// </summary>
        public static void Start()
        {
            wikipediaAccessor = DependencyService.Resolve<IWikipediaAccessor>();
            hTMLService = DependencyService.Resolve<IHTMLService>();
            localArticlesService = DependencyService.Resolve<ILocalArticlesService>();

            DownloadQueue = new List<string>();
            subscribedBadgeCallbacks = new List<Action<int>>();
            subscribedStatusCallbacks = new List<Action<DownloadsStatusUpdate>>();

            // Load any articles that failed to download last time the app was open and add them back to the queue
            List<string> settingsDownloadQueue = Settings.DownloadQueue;
            if (settingsDownloadQueue.Count > 0)
            {
                DownloadQueue = settingsDownloadQueue;
                TotalNumArticles = Settings.TotalNumberOfArticlesToDownload;
            }

            // Start Consumer Function
            Task.Run(() => DownloadQueueConsumer());
        }

        #region Public Facing Methods
        /// <summary>
        /// Function to add a list of articles to the download queue
        /// </summary>
        /// <param name="articlesToAdd">List of article titles to add</param>
        public static void AddArticlesToList(List<string> articlesToAdd)
        {
            lock (queueLock)
            {
                DownloadQueue.AddRange(articlesToAdd);
                TotalNumArticles += articlesToAdd.Count;
                Settings.TotalNumberOfArticlesToDownload = TotalNumArticles;
                Settings.DownloadQueue = DownloadQueue;
            }

            UpdateBadgeCallbacks();
            UpdateStatusCallbacks($"Downloading article {TotalNumArticles - DownloadQueue.Count} out of {TotalNumArticles}.");
        }

        /// <summary>
        /// Function to add a callback function to be called with the status of the downloads when something updates
        /// </summary>
        /// <param name="action">Action Delegate that will be called</param>
        public static void AddStatusCallBack(Action<DownloadsStatusUpdate> action)
        {
            subscribedStatusCallbacks.Add(action);
        }

        /// <summary>
        /// Function to add a callback function to receive the number of articles left to download when that number changes
        /// </summary>
        /// <param name="action">Action Delegate to be called</param>
        public static void AddBadgeCallback(Action<int> action)
        {
            subscribedBadgeCallbacks.Add(action);
        }

        /// <summary>
        /// Function for other classes to poll to get the status of this service
        /// </summary>
        /// <returns></returns>
        public static DownloadsStatusUpdate GetStatus()
        {
            string message = $"Downloading article {TotalNumArticles - DownloadQueue.Count} out of {TotalNumArticles}.";
            if (DownloadQueue.Count == 0)
                message = "There are currently no articles queued for download.";

            DownloadsStatusUpdate update;
            // We need to lock here so that the queue consumer does not remove from the list while we are trying to make a copy of it to send away.
            // We have to make a copy because there are places where we iterate though the list of articles left to download to copy them into an observable,
            // and without a copy this service might make a change while it is copying
            lock (queueLock)
            {
                update = new DownloadsStatusUpdate
                {
                    StatusMessage = message,
                    TotalNumArticlesToDownload = TotalNumArticles,
                    ArticlesLeftToDownload = DownloadQueue.ToList()
                };
            }

            return update;
        }

        #endregion

        /// <summary>
        /// Function that acts as a continuous consumer of the download queue and actually does the downloading of the articles
        /// </summary>
        private static void DownloadQueueConsumer()
        {
            bool justDownloaded = false;
            while (true)
            {
                try
                {
                    if (DownloadQueue.Count == 0)
                    {
                        if (justDownloaded)
                        {
                            // If we just finished download all articles in the list we must return the service to state and give a final update to all the subscribed customers
                            UpdateStatusCallbacks($"Downloading article {TotalNumArticles - DownloadQueue.Count} out of {TotalNumArticles}.");
                            UpdateBadgeCallbacks();
                            int articlesDownloaded = TotalNumArticles;
                            Device.BeginInvokeOnMainThread(() => NotificationService.SendAlertOrNotification("Articles Added", $"{articlesDownloaded} articles have been downloaded and added to your library.", "Okay"));
                            TotalNumArticles = 0;
                            Settings.TotalNumberOfArticlesToDownload = TotalNumArticles;
                            justDownloaded = false;
                        }

                        // Only delay if there are not articles to download
                        Task.Delay(2000).Wait();
                        continue;
                    }

                    if (!CheckInternetConnection())
                    {
                        UpdateStatusCallbacks($"Waiting for Internet connection to continue downloading.");
                        Task.Delay(2000).Wait();
                        continue;
                    }

                    if(!CheckWifiConnection() && !Settings.DownloadOverCell)
                    {
                        UpdateStatusCallbacks($"Waiting for wifi connection to continue downloading.");
                        Task.Delay(2000).Wait();
                        continue;
                    }

                    DateTime startTime = DateTime.Now;

                    string articleToDownload = DownloadQueue[0];
                    DownloadArticle(articleToDownload).Wait();

                    lock (queueLock)
                    {
                        DownloadQueue.RemoveAt(0);
                    }

                    // Update the cached download queue so we can be stateful
                    Settings.DownloadQueue = DownloadQueue;

                    UpdateAverageDownloadTime((DateTime.Now - startTime).Milliseconds, 1);

                    UpdateStatusCallbacks($"Downloading article {TotalNumArticles - DownloadQueue.Count} out of {TotalNumArticles}.");
                    UpdateBadgeCallbacks();

                    justDownloaded = true;
                }
                catch (Exception e)
                {
                    // TODO: Handle this in some way
                    int i = 88;
                    UpdateStatusCallbacks($"Error downloading. Waiting for Internet connection to continue downloading. If the downloads do not continue after connection is reestablished try restarting the application and downloads will continue.");
                    Task.Delay(2000).Wait();
                }
            }
        }


        #region Private Helper Functions
        /// <summary>
        /// Function to check if there is an active connection to the internet
        /// Uses the Xamarin.Essentials API
        /// </summary>
        /// <returns>Bool representing if we are connected to internet or not</returns>
        private static bool CheckInternetConnection()
        {
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
                return true;

            return false;
        }

        /// <summary>
        /// Function to check if there is an active connection to WIFI
        /// Using the Xamarin.Essentials API
        /// </summary>
        /// <returns>Bool of if we are connected to wifi</returns>
        private static bool CheckWifiConnection()
        {
            if (Connectivity.ConnectionProfiles.Contains(ConnectionProfile.WiFi))
                return true;

            return false;
        }

        /// <summary>
        /// Function to update all subscribed callbacks with the number of articles remaining
        /// This is used for the badges on the download center icon
        /// </summary>
        private static void UpdateBadgeCallbacks()
        {
            foreach (Action<int> action in subscribedBadgeCallbacks)
            {
                Device.BeginInvokeOnMainThread(() => action?.Invoke(DownloadQueue.Count));
            }
        }

        /// <summary>
        /// Update all subscribed callbacks with the status of the service
        /// </summary>
        private static void UpdateStatusCallbacks(string message)
        {
            if (DownloadQueue.Count == 0)
                message = "There are currently no articles queued for download.";

            DownloadsStatusUpdate update;
            lock (queueLock)
            {
                update = new DownloadsStatusUpdate
                {
                    StatusMessage = message,
                    TotalNumArticlesToDownload = TotalNumArticles,
                    ArticlesLeftToDownload = DownloadQueue.ToList()
                };
            }

            foreach (Action<DownloadsStatusUpdate> action in subscribedStatusCallbacks)
            {
                Device.BeginInvokeOnMainThread(() => action?.Invoke(update));
            }
        }

        /// <summary>
        /// Function to download a single article to the disk
        /// </summary>
        /// <param name="title">Title of the article to download</param>
        /// <returns></returns>
        private static async Task DownloadArticle(string title)
        {
            string articleText = hTMLService.InjectCSS(await wikipediaAccessor.DownloadArticleHTML(title));

            localArticlesService.SaveHTMLFileToStorage(hTMLService.ReplaceColons(title), articleText);
        }


        /// <summary>
        /// Function to update the average download times
        /// </summary>
        /// <param name="totalTime">Total time it took to download the articles</param>
        /// <param name="numberDownloaded">Number of articles that were downloaded</param>
        private static void UpdateAverageDownloadTime(double totalTime, int numberDownloaded)
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

        /// <summary>
        /// Function to get time formatted in a user friendly way
        /// </summary>
        /// <param name="milliseconds">Millisecond time to format</param>
        /// <returns>String with formatted time</returns>
        private static string FormatTime(double milliseconds)
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
        private static double GetMilliSecondsSinceStart(DateTime start)
        {
            return (DateTime.Now - start).TotalMilliseconds;
        }

        #endregion
    }

}

