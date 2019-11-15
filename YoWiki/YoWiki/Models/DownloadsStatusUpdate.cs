using System;
using System.Collections.Generic;
using System.Text;

namespace YoWiki.Models
{
    public class DownloadsStatusUpdate
    {
        public string StatusMessage { get; set; }

        public int TotalNumArticlesToDownload { get; set; }

        public List<string> ArticlesLeftToDownload { get; set; }

        public int NumberOfArticlesDownloaded { get => TotalNumArticlesToDownload - ArticlesLeftToDownload.Count; }
    }
}
