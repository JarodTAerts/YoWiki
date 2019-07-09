namespace YoWiki.Models
{
    class Article
    {
        /// <summary>
        /// Title of the article
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// HTML text of the article that can be displayed in a webview
        /// </summary>
        public string Text { get; set; }
    }
}
