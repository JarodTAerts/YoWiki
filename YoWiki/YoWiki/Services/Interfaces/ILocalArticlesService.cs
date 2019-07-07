using System;
using System.Collections.Generic;

namespace YoWiki.Services.Interfaces
{
    public interface ILocalArticlesService
    {
        void SaveHTMLFileToStorage(string title, string text);

        string GetHTMLTextFromFile(string title);

        List<string> GetNamesOfSavedArticles();

        void ClearSavedArticles();

        void DeleteArticle(string title);
    }
}
