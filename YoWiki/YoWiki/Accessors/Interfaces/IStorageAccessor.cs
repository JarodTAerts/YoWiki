using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace YoWiki.Accessors.Interfaces
{
    public interface IStorageAccessor
    {
        string ReadTextFromFile(string fileName);

        void WriteTextToFile(string fileName, string text);

        List<string> GetFileNamesInDirectory(string directoryPath);

        void DeleteDirectory(string directoryPath);
    }
}
