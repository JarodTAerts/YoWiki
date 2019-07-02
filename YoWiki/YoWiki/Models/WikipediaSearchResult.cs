using System;
using System.Collections.Generic;
using System.Text;

namespace YoWiki.Models
{
    /// <summary>
    /// This is a class to contain a whole wikipedia search results. Containing a whole bunch of wikipedia search items
    /// </summary>
    public class WikipediaSearchResult
    {
        //Total number of articles found
        public int Totalhits { get; set; }

        //List of example articles returned
        public List<WikipediaSearchItem> Items { get; set; }

        public override string ToString()
        {
            StringBuilder str = new StringBuilder();
            str.Append("\nTotal Hits: " + Totalhits);
            str.Append("\n\n" + Items.Count + " Example Results: ");
            foreach(WikipediaSearchItem item in Items)
            {
                str.Append("\n"+item.ToString());
            }
            return str.ToString();
        }
    }
}
