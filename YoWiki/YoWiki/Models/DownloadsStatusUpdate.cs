using System;
using System.Collections.Generic;
using System.Text;

namespace YoWiki.Models
{
    /// <summary>
    /// Model to represent an update about the status of the Download Service
    /// </summary>
    public class DownloadsStatusUpdate
    {
        /// <summary>
        /// Message that can be displayed to show how many articles have been downloaded out of how many
        /// </summary>
        public string StatusMessage { get; set; }

        /// <summary>
        /// Total number of articles that was added to the download queue
        /// </summary>
        public int TotalNumArticlesToDownload { get; set; }

        /// <summary>
        /// List of all articles left to download
        /// </summary>
        public List<string> ArticlesLeftToDownload { get; set; }

        /// <summary>
        /// Getter for the number of articles that have already been downloaded
        /// </summary>
        public int NumberOfArticlesDownloaded { get => TotalNumArticlesToDownload - ArticlesLeftToDownload.Count; }
    }
}
