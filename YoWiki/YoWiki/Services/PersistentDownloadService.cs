using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using YoWiki.Accessors.Interfaces;
using YoWiki.Services.Interfaces;

namespace YoWiki.Services
{
    public static class PersistentDownloadService
    {
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

        private static Action<string> updateAction;
        private static List<Action<int>> subscribedBadgeCallbacks;
        public static void Start()
        {
            wikipediaAccessor = DependencyService.Resolve<IWikipediaAccessor>();
            hTMLService = DependencyService.Resolve<IHTMLService>();
            localArticlesService = DependencyService.Resolve<ILocalArticlesService>();
            DownloadQueue = new List<string>();
            subscribedBadgeCallbacks = new List<Action<int>>();

            List<string> settingsDownloadQueue = Settings.DownloadQueue;

            if (settingsDownloadQueue.Count > 0)
            {
                DownloadQueue = settingsDownloadQueue;
                TotalNumArticles = Settings.TotalNumberOfArticlesToDownload;
            }

            // Start Consumer Function
            Task.Run(() => DownloadQueueConsumer());
        }

        public static void AddArticlesToList(List<string> articlesToAdd)
        {
            lock (queueLock)
            {
                DownloadQueue.AddRange(articlesToAdd);
                TotalNumArticles += articlesToAdd.Count;
            }

            Settings.TotalNumberOfArticlesToDownload = TotalNumArticles;
            Settings.DownloadQueue = DownloadQueue;
        }

        public static void SetUpdateMethod(Action<string> action)
        {
            updateAction = action;
        }

        public static void AddBadgeCallback(Action<int> action)
        {
            subscribedBadgeCallbacks.Add(action);
        }

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
                            updateAction?.Invoke($"Downloaded {TotalNumArticles} articles.");
                            UpdateBadgeCallbacks();
                            int articlesDownloaded = TotalNumArticles;
                            Device.BeginInvokeOnMainThread(() => NotificationService.SendAlertOrNotification("Articles Added", $"{articlesDownloaded} articles have been downloaded and added to your library.", "Okay"));
                            TotalNumArticles = 0;
                            Settings.TotalNumberOfArticlesToDownload = TotalNumArticles;
                            justDownloaded = false;
                        }

                        // Only delay if there are not articles to download
                        Task.Delay(1000).Wait();
                        continue;
                    }

                    string articleToDownload = DownloadQueue[0];

                    DownloadArticle(articleToDownload).Wait();

                    lock (queueLock)
                    {
                        DownloadQueue.RemoveAt(0);
                    }

                    Settings.DownloadQueue = DownloadQueue;

                    updateAction?.Invoke($"Downloading article {TotalNumArticles - DownloadQueue.Count} out of {TotalNumArticles}.");
                    UpdateBadgeCallbacks();

                    justDownloaded = true;
                }
                catch (Exception e)
                {
                    int i = 88;
                }
            }
        }

        private static void UpdateBadgeCallbacks()
        {
            foreach (Action<int> action in subscribedBadgeCallbacks)
            {
                Device.BeginInvokeOnMainThread(() => action?.Invoke(DownloadQueue.Count));
            }
        }

        private static async Task DownloadArticle(string title)
        {
            try
            {
                string articleText = hTMLService.InjectCSS(await wikipediaAccessor.DownloadArticleHTML(title));

                localArticlesService.SaveHTMLFileToStorage(hTMLService.ReplaceColons(title), articleText);
            }
            catch
            {
                //MessageText = "Failed";
            }
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
    }

}

