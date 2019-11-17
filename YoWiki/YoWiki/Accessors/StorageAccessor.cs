using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using YoWiki.Accessors.Interfaces;

namespace YoWiki.Accessors
{
    public class StorageAccessor : IStorageAccessor
    {
        public void DeleteDirectory(string directoryPath)
        {
            if (!Directory.Exists(directoryPath))
                return;

            Directory.Delete(directoryPath, recursive: true);
        }

        public void DeleteFile(string filePath)
        {
            if (!File.Exists(filePath))
                return;

            File.Delete(filePath);
        }

        public List<string> GetFileNamesInDirectory(string directoryPath)
        {
            if (!Directory.Exists(directoryPath))
                return new List<string>();

            //Get an array of all the file names, including paths
            string[] resultsStrings = Directory.GetFiles(directoryPath);

            return new List<string>(resultsStrings);
        }

        public string ReadTextFromFile(string filePath)
        {
            return File.ReadAllText(filePath);
        }

        public void WriteTextToFile(string filePath, string text)
        {
            File.WriteAllText(filePath, text);
        }

        public bool FileExists(string filePath)
        {
            return File.Exists(filePath);
        }
    }
}
