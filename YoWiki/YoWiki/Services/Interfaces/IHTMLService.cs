namespace YoWiki.Services.Interfaces
{
    /// <summary>
    /// Interface for HTML Service that will deal with messing with HTML strings and stuff
    /// </summary>
    public interface IHTMLService
    {
        /// <summary>
        /// Function to replace the colons in a string
        /// </summary>
        /// <param name="input">String to replace colons in</param>
        /// <returns>String with colons replaced</returns>
        string ReplaceColons(string input);

        /// <summary>
        /// Function to strip out most HTML tags from HTML text so it is more readable
        /// </summary>
        /// <param name="input">String to strip HTML tags from</param>
        /// <returns>String without HTML tags</returns>
        string SimpleHTMLStrip(string input);

        /// <summary>
        /// Function to inject the common Wikipedia CSS into the HTML document
        /// </summary>
        /// <param name="htmlString">HTML string of the wikipedia article</param>
        /// <returns>String with injected CSS</returns>
        string InjectCSS(string htmlString);
    }
}
