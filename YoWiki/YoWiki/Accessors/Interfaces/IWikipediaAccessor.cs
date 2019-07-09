using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YoWiki.Models;

namespace YoWiki.Accessors.Interfaces
{
    /// <summary>
    /// Interface representing a Wikipedia api accessor
    /// </summary>
    public interface IWikipediaAccessor
    {
        /// <summary>
        /// Function to make a search query to the wikipedia api
        /// </summary>
        /// <param name="searchText">Text to search for</param>
        /// <param name="numExampleArticles">Number of example articles that will be returned with this query</param>
        /// <returns>A Wikipedia Search Result Object <see cref="WikipediaSearchResult"/></returns>
        Task<WikipediaSearchResult> SearchTopic(string searchText, int numExampleArticles);

        /// <summary>
        /// Function to make a query to wikipedia api to get the names of all the articles for a search query
        /// </summary>
        /// <param name="searchText">Text that is being searched</param>
        /// <param name="totalHits">Number of articles to get names for</param>
        /// <returns>List of the names of all the articles the search returned</returns>
        Task<List<string>> GetAllNamesFromSearch(string searchText, int totalHits);

        /// <summary>
        /// Function to download the HTML text for a given article
        /// </summary>
        /// <param name="title">Title of the article that will be downloaded</param>
        /// <returns>String of article text</returns>
        Task<string> DownloadArticleHTML(string title);
    }
}
