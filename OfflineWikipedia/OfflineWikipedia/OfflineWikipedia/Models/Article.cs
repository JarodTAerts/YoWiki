using System;
using System.Collections.Generic;
using System.Text;

namespace OfflineWikipedia.Models
{
    /// <summary>
    /// This class is used to represent a wikipedia article. It will be used to take the parsed text from the Wikipedia html and store it in a easier to display format
    /// </summary>
    class Article
    {
        public string Title { get; set; }
        //Dictonary to hold the titles of headings of different sections of the article and then text contained within it
        //This will probably need to be changed to hold a bunch of heading objects, which could also contain heading objects to account for subheadings
        public Dictionary<string,string> HeadingContentDictonary { get; set; }
        //List to hold referenced articles
        public List<string> References { get; set; }
    }
}
