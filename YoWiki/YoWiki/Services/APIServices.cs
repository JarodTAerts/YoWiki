using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using YoWiki.Models;

namespace YoWiki.Services
{
    /// <summary>
    /// This class is for all the necessary functions to connect and carry out Wikipedia API related work
    /// </summary>
    class APIServices
    {
        //Connection strings
        private const string baseAPIUrl = @"https://en.wikipedia.org/w/api.php?";
        private const string basePageUrl = @"https://en.wikipedia.org/wiki/";

        /// <summary>
        /// This function is used to get search results for a string of text. It will search through both titles and text of wikipedia articles to find all articles related to the searched topic
        /// </summary>
        /// <param name="search"> The things to be searched</param>
        /// <param name="numberOfResults"> Number of example articles to be returned</param>
        /// <returns></returns>
        public static async Task<WikipediaSearchResult> SearchTopic(string search,int numberOfResults)
        {         
            //Create the client and URL string and get a string of json text of the result of the search
            var client = new HttpClient();
            string searchEncoded = HttpUtility.HtmlEncode(search);
            string addon = "action=query&list=search&srsearch=" + searchEncoded + "&utf8=&format=json&srlimit=" + numberOfResults + "&srwhat=text";
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

        /// <summary>
        /// Function that is used to get all the article titles from the Wikipedia search so that they can be downloaded
        /// This function is very inefficent right now and can only get 10,000 articles. This limit seems to be part of the Wikipedia API and needs to be investigated
        /// There could also be lots of general improvements
        /// </summary>
        /// <param name="search">Text entered in the search box</param>
        /// <param name="totalHits">Number of hits that were returned from that search</param>
        /// <returns></returns>
        public static async Task<List<string>> GetAllNamesFromSearch(string search, int totalHits)
        {
            List<string> names = new List<string>();
            //Create the client and URL string and get a string of json text of the result of the search
            var client = new HttpClient();
            string searchEncoded = HttpUtility.HtmlEncode(search);
            for (int i = 0; i < totalHits; i += 500)
            {
                string addon = "action=query&list=search&srsearch=" + searchEncoded + "&utf8=&format=json&srlimit=500&srwhat=text&srprop=size&sroffset="+i;
                var jsonstr = await client.GetStringAsync(baseAPIUrl + addon);

                //Create a JSON obejct with the string and get the search token and the number of hits token from it
                JObject obj = JObject.Parse(jsonstr);
                var token = (JArray)obj.SelectToken("query.search");

                //Get a list of WikipediaSearchItems from the search token of the response
                var searchItems = JsonConvert.DeserializeObject<List<WikipediaSearchItem>>(token.ToString());

                foreach(WikipediaSearchItem item in searchItems)
                {
                    names.Add(item.Title);
                }
            }

            return names;
        }

        /// <summary>
        /// Function to get the HTML from a wikipedia article based on its title
        /// </summary>
        /// <param name="title">Title of the article that you want to get the HTML from</param>
        /// <returns></returns>
        public static async Task<string> GetAllHTMLFromWikipediaArticle(string title)
        {
            //Create the client encode the title and get the html response then return it
            var client = new HttpClient();
            Debug.WriteLine("Title: " + title);
            title = Regex.Replace(title,"\\?", "%3F");
            string htmlStr = (string)await client.GetStringAsync(basePageUrl + title);

            return htmlStr;
        }
    }
}
