namespace YoWiki.Models
{
    class Article
    {
        /// <summary>
        /// Title of the article
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// HTML text of the article that can be displayed in a web view
        /// </summary>
        public string Text { get; set; }
    }
}
