using System;

namespace YoWiki.Models
{
    /// <summary>
    /// Class to represent a returned wikipedia search item. This is a single Article from a search result that contains various information about the article
    /// </summary>
    public class WikipediaSearchItem
    {
        /// <summary>
        /// Title of the article
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Number of words in the article
        /// </summary>
        public int Wordcount { get; set; }

        /// <summary>
        /// Size of the article in bytes
        /// </summary>
        public int Size { get; set; }

        /// <summary>
        /// Snippet of text from the article 
        /// </summary>
        public string Snippet { get; set; }

        /// <summary>
        /// Date of the article
        /// </summary>
        public DateTime Timestamp { get; set; }
    }
}
