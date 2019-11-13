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
        private static IWikipediaAccessor wikipediaAccessor;
        private static IHTMLService hTMLService;
        private static ILocalArticlesService localArticlesService;

        public static void Start()
        {
            wikipediaAccessor = DependencyService.Resolve<IWikipediaAccessor>();
            hTMLService = DependencyService.Resolve<IHTMLService>();
            localArticlesService = DependencyService.Resolve<ILocalArticlesService>();
            downloadQueue = new List<string>();

            List<string> settingsDownloadQueue = Settings.DownloadQueue;

            if(settingsDownloadQueue.Count > 0)
            {
                downloadQueue = settingsDownloadQueue;
            }

            // Start Consumer Function
            Task.Run(() => DownloadQueueConsumer());
        }

        public static void AddArticlesToList(List<string> articlesToAdd)
        {
            lock (queueLock)
            {
                downloadQueue.AddRange(articlesToAdd);
            }

            Settings.DownloadQueue = downloadQueue;
        }

        private static void DownloadQueueConsumer()
        {
            while (true)
            {
                if (downloadQueue.Count == 0)
                {
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
