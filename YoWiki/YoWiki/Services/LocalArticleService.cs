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
        private static string storagePath = $"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}/articles";

        public LocalArticleService()
        {
            this.storageAccessor = DependencyService.Resolve<IStorageAccessor>();
        }

        public void ClearSavedArticles()
        {
            storageAccessor.DeleteDirectory(storagePath);
        }

        public void DeleteArticle(string title)
        {
            string fileName = Path.Combine(storagePath, $"{title}.html");
            File.Delete(fileName);
        }

        public string GetHTMLTextFromFile(string title)
        {
            //Get the path of the file and get all the text
            string fileName = Path.Combine(storagePath, $"{title}.html");
            return storageAccessor.ReadTextFromFile(fileName);
        }

        public List<string> GetNamesOfSavedArticles()
        {
            //Get an array of all the file names, including paths
            List<string> resultsStrings = storageAccessor.GetFileNamesInDirectory(storagePath);

            resultsStrings = resultsStrings.Select(r => {
                string[] splitString = r.Split('/');
                string title = splitString[splitString.Length - 1];
                return title.Split('.')[0];
            }).ToList();

            return resultsStrings;
        }

        public void SaveHTMLFileToStorage(string title, string text)
        {
            if (!Directory.Exists(storagePath))
                Directory.CreateDirectory(storagePath);
            //Get the path of the file and get all the text
            string fileName = Path.Combine(storagePath, $"{title}.html");
            File.WriteAllText(fileName, text);
        }
    }
}
