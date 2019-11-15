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
        private static List<string> downloadQueue;
        public static int TotalNumArticles
        {
            get; set;
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
            downloadQueue = new List<string>();
            subscribedBadgeCallbacks = new List<Action<int>>();

            List<string> settingsDownloadQueue = Settings.DownloadQueue;

            if(settingsDownloadQueue.Count > 0)
            {
                downloadQueue = settingsDownloadQueue;
                TotalNumArticles = Settings.TotalNumberOfArticlesToDownload;
            }

            // Start Consumer Function
            Task.Run(() => DownloadQueueConsumer());
        }

        public static void AddArticlesToList(List<string> articlesToAdd, Action<string> updateAction)
        {
            lock (queueLock)
            {
                downloadQueue.AddRange(articlesToAdd);
                TotalNumArticles += articlesToAdd.Count;
            }

            Settings.TotalNumberOfArticlesToDownload = TotalNumArticles;
            Settings.DownloadQueue = downloadQueue;
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
                    if (downloadQueue.Count == 0)
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

                    string articleToDownload = downloadQueue[0];

                    DownloadArticle(articleToDownload).Wait();

                    lock (queueLock)
                    {
                        downloadQueue.RemoveAt(0);
                    }

                    Settings.DownloadQueue = downloadQueue;

                    updateAction?.Invoke($"Downloading article {TotalNumArticles - downloadQueue.Count} out of {TotalNumArticles}.");
                    UpdateBadgeCallbacks();

                    justDownloaded = true;
                }
                catch(Exception e)
                {
                    int i = 88;
                }
            }
        }

        private static void UpdateBadgeCallbacks()
        {
            foreach(Action<int> action in subscribedBadgeCallbacks)
            {
                Device.BeginInvokeOnMainThread(()=>action?.Invoke(downloadQueue.Count));
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

    }
}
