using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using YoWiki.Accessors.Interfaces;
using YoWiki.Models;

namespace YoWiki.Accessors
{
    public class WikipediaAccessor : IWikipediaAccessor
    {
        private const string baseAPIUrl = @"https://en.wikipedia.org/w/api.php?";
        private const string basePageUrl = @"https://en.wikipedia.org/wiki/";

        public WikipediaAccessor()
        {
        }

        public async Task<string> DownloadArticleHTML(string title)
        {
            //Create the client encode the title and get the html response then return it
            var client = new HttpClient();
            Debug.WriteLine("Title: " + title);
            title = Regex.Replace(title, "\\?", "%3F");
            string htmlStr = await client.GetStringAsync(basePageUrl + title);

            return htmlStr;
        }

        public async Task<List<string>> GetAllNamesFromSearch(string search, int totalHits)
        {
            List<string> names = new List<string>();
            //Create the client and URL string and get a string of json text of the result of the search
            var client = new HttpClient();
            string searchEncoded = HttpUtility.HtmlEncode(search);
            for (int i = 0; i < totalHits; i += 500)
            {
                string addon = "action=query&list=search&srsearch=" + searchEncoded + "&utf8=&format=json&srlimit=500&srwhat=text&srprop=size&sroffset=" + i;
                var jsonstr = await client.GetStringAsync(baseAPIUrl + addon);

                //Create a JSON obejct with the string and get the search token and the number of hits token from it
                JObject obj = JObject.Parse(jsonstr);
                var token = (JArray)obj.SelectToken("query.search");

                //Get a list of WikipediaSearchItems from the search token of the response
                var searchItems = JsonConvert.DeserializeObject<List<WikipediaSearchItem>>(token.ToString());

                foreach (WikipediaSearchItem item in searchItems)
                {
                    names.Add(item.Title);
                }
            }

            return names;
        }

        public async Task<WikipediaSearchResult> SearchTopic(string search, int numExampleArticles)
        {
            //Create the client and URL string and get a string of json text of the result of the search
            var client = new HttpClient();
            string searchEncoded = HttpUtility.HtmlEncode(search);
            string addon = "action=query&list=search&srsearch=" + searchEncoded + "&utf8=&format=json&srlimit=" + numExampleArticles + "&srwhat=text";
            var jsonstr = await client.GetStringAsync(baseAPIUrl + addon);

            //Create a JSON obejct with the string and get the search token and the number of hits token from it
            JObject obj = JObject.Parse(jsonstr);
            var token = (JArray)obj.SelectToken("query.search");
            int hits = Convert.ToInt32(obj.SelectToken("query.searchinfo.totalhits"));

            //Get a list of WikipediaSearchItems from the search token of the response
            var searchItems = JsonConvert.DeserializeObject<List<WikipediaSearchItem>>(token.ToString());

            //Create a new searchResult add the information and return it
            WikipediaSearchResult result = new WikipediaSearchResult();
            result.Totalhits = hits;
            result.Items = searchItems;

            return result;
        }
    }
}
