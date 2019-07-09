using System.Collections.Generic;

namespace YoWiki.Services.Interfaces
{
    /// <summary>
    /// Interface for service to deal with handling Local Wikipedia Articles
    /// </summary>
    public interface ILocalArticlesService
    {
        /// <summary>
        /// Function to save the HTML text of a wikipedia article to storage
        /// </summary>
        /// <param name="title">Title of article to save</param>
        /// <param name="text">HTML text of that article</param>
        void SaveHTMLFileToStorage(string title, string text);

        /// <summary>
        /// Function to get the HTML text of an article stored locally
        /// </summary>
        /// <param name="title">Title of article to get HTML text from</param>
        /// <returns>String of HTML text for the article</returns>
        string GetHTMLTextFromFile(string title);

        /// <summary>
        /// Function to get the names of all the articles saved locally
        /// </summary>
        /// <returns>List of all article names saved locally</returns>
        List<string> GetNamesOfSavedArticles();

        /// <summary>
        /// Function to clear all of the saved articles from storage
        /// </summary>
        void ClearSavedArticles();

        /// <summary>
        /// Function to delete a specific article from storage
        /// </summary>
        /// <param name="title">Title of article to delete</param>
        void DeleteArticle(string title);
    }
}
