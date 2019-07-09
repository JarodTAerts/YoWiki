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
        /// Function to strip out most html tags from html text so it is more readable
        /// </summary>
        /// <param name="input">String to strip html tags from</param>
        /// <returns>String without html tags</returns>
        string SimpleHTMLStrip(string input);
    }
}
