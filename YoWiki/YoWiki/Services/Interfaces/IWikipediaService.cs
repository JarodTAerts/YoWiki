using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YoWiki.Models;

namespace YoWiki.Services.Interfaces
{
    public interface IWikipediaService
    {
        Task<WikipediaSearchResult> SearchTopic(string search, int numExampleArticles);

        Task<List<string>> GetAllNamesFromSearch(string search, int totalHits);

        Task<string> DownloadArticleHTML(string title);
    }
}
