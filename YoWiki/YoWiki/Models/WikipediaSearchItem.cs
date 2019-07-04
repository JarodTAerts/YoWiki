using YoWiki.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace YoWiki.Models
{
    /// <summary>
    /// Class to represent a returned wikipedia search item. This is a single Article from a search result that contains various information about the article
    /// </summary>
    public class WikipediaSearchItem
    {
        public string Title { get; set; }
        public int Wordcount { get; set; }
        public int Size { get; set; }
        public string Snippet { get; set; }
        public DateTime Timestamp { get; set; }

        public override string ToString()
        {
            return "\nTitle: " + Title + "\nWordCount: " + Wordcount + "\nSize: " + Size + "\nSnippet: " + Snippet + "\nTime: " + Timestamp ;
        }

    }
}
