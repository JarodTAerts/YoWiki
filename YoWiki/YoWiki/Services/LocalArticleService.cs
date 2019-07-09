using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xamarin.Forms;
using YoWiki.Accessors.Interfaces;
using YoWiki.Services.Interfaces;

namespace YoWiki.Services
{
    public class LocalArticleService : ILocalArticlesService
    {
        private readonly IStorageAccessor storageAccessor;
        // String that gets a path to the local storage of the device being used
        private static readonly string storagePath = $"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}/articles";

        public LocalArticleService()
        {
            storageAccessor = DependencyService.Resolve<IStorageAccessor>();
        }

        public void ClearSavedArticles()
        {
            storageAccessor.DeleteDirectory(storagePath);
        }

        public void DeleteArticle(string title)
        {
            string filePath = Path.Combine(storagePath, $"{title}.html");
            storageAccessor.DeleteFile(filePath);
        }

        public string GetHTMLTextFromFile(string title)
        {
            string fileName = Path.Combine(storagePath, $"{title}.html");
            return storageAccessor.ReadTextFromFile(fileName);
        }

        public List<string> GetNamesOfSavedArticles()
        {
            List<string> resultsStrings = storageAccessor.GetFileNamesInDirectory(storagePath);

            // Take the paths of all the files and get only the actual article names from them
            resultsStrings = resultsStrings.Select(r => {
                string[] splitString = r.Split('/');
                string title = splitString[splitString.Length - 1];
                return title.Split(new string[] { ".html" }, StringSplitOptions.None)[0];
            }).ToList();

            return resultsStrings;
        }

        public void SaveHTMLFileToStorage(string title, string text)
        {
            // Make sure the directory to save the articles in exists before trying to write there
            if (!Directory.Exists(storagePath))
                Directory.CreateDirectory(storagePath);

            string filePath = Path.Combine(storagePath, $"{title}.html");

            storageAccessor.WriteTextToFile(filePath, text);
        }
    }
}
