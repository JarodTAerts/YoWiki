using System;
using System.Collections.Generic;
using System.IO;
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

        public string GetHTMLTextFromFile(string title)
        {
            //Get the path of the file and get all the text
            string fileName = Path.Combine(storagePath, $"{title}.wik");
            return storageAccessor.ReadTextFromFile(fileName);
        }

        public List<string> GetNamesOfSavedArticles()
        {
            //Get an array of all the file names, including paths
            List<string> resultsStrings = storageAccessor.GetFileNamesInDirectory(storagePath);

            //resultsStrings = resultsStrings;

            return resultsStrings;
        }

        public void SaveHTMLFileToStorage(string title, string text)
        {
            //Get the path of the file and get all the text
            string fileName = Path.Combine(storagePath, $"{title}.wik");
            File.WriteAllText(fileName, text);
        }
    }
}
