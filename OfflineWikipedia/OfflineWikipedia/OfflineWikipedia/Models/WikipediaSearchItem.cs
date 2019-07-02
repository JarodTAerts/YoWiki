using OfflineWikipedia.Helpers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace LearningAPIs
{
    /// <summary>
    /// Class to represent a returned wikipedia search item. This is a single Article from a search result that contains various information about the article
    /// </summary>
    public class WikipediaSearchItem
    {
        public string Title { get; set; }
        public int Wordcount { get; set; }
        public int Size { get; set; }
        private string _snippet;
        public string Snippet {
            get => _snippet;
            set => _snippet=HTMLHandler.SimpleHTMLStrip(value);
        }
        public DateTime Timestamp { get; set; }

        public override string ToString()
        {
            return "\nTitle: " + Title + "\nWordCount: " + Wordcount + "\nSize: " + Size + "\nSnippet: " + HTMLHandler.StripHTML(Snippet) + "\nTime: " + Timestamp ;
        }

    }
}
