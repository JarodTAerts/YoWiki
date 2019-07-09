using System.Collections.Generic;
using System.Threading.Tasks;
using YoWiki.Models;

namespace YoWiki.Services.Interfaces
{
    /// <summary>
    /// Interface for service that will deal with data from Wikipedia
    /// </summary>
    public interface IWikipediaService
    {
        /// <summary>
        /// Function to search wikipedia
        /// </summary>
        /// <param name="searchText">Text to search for</param>
        /// <param name="numExampleArticles">Number of example articles that will be returned with this query</param>
        /// <returns>A Wikipedia Search Result Object <see cref="WikipediaSearchResult"/></returns>
        Task<WikipediaSearchResult> SearchTopic(string searchText, int numExampleArticles);

        /// <summary>
        /// Function to get the names of all the results from a search
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
