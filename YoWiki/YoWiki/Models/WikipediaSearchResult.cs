using System.Collections.Generic;

namespace YoWiki.Models
{
    /// <summary>
    /// Class to represent returned wikipedia search result that contains many example articles
    /// </summary>
    public class WikipediaSearchResult
    {
        /// <summary>
        /// Total number of articles found in this search
        /// </summary>
        public int Totalhits { get; set; }

        /// <summary>
        /// List of example articles that can be shown in the results
        /// </summary>
        public List<WikipediaSearchItem> Items { get; set; } = new List<WikipediaSearchItem>();
    }
}
