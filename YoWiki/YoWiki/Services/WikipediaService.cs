using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;
using YoWiki.Accessors.Interfaces;
using YoWiki.Models;
using YoWiki.Services.Interfaces;

namespace YoWiki.Services
{
    public class WikipediaService : IWikipediaService
    {
        private readonly IWikipediaAccessor wikipediaAccessor;

        public WikipediaService()
        {
            wikipediaAccessor = DependencyService.Resolve<IWikipediaAccessor>();
        }

        public Task<string> DownloadArticleHTML(string title)
        {
            return wikipediaAccessor.DownloadArticleHTML(title);
        }

        public Task<List<string>> GetAllNamesFromSearch(string search, int totalHits)
        {
            return wikipediaAccessor.GetAllNamesFromSearch(search, totalHits);
        }

        public Task<WikipediaSearchResult> SearchTopic(string search, int numExampleArticles)
        {
            return wikipediaAccessor.SearchTopic(search, numExampleArticles);
        }
    }
}
