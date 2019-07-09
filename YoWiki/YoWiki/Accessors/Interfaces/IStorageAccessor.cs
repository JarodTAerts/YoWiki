using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace YoWiki.Accessors.Interfaces
{
    /// <summary>
    /// Interface for accessor to local storage
    /// </summary>
    public interface IStorageAccessor
    {
        /// <summary>
        /// Function to read all the text from a local file
        /// </summary>
        /// <param name="filePath">Path to file to read</param>
        /// <returns>String of all the text in that file</returns>
        string ReadTextFromFile(string filePath);

        /// <summary>
        /// Function to write a string to a file. Overwrites file if it exists
        /// </summary>
        /// <param name="filePath">Path of file to write to</param>
        /// <param name="text">Text to write to file</param>
        void WriteTextToFile(string filePath, string text);

        /// <summary>
        /// Function to get a list of all files in a directory
        /// </summary>
        /// <param name="directoryPath">Path to directory to get file names from</param>
        /// <returns>List of file names</returns>
        List<string> GetFileNamesInDirectory(string directoryPath);

        /// <summary>
        /// Function to delete a directory from storage
        /// </summary>
        /// <param name="directoryPath">Path to directory to delete</param>
        void DeleteDirectory(string directoryPath);
    }
}
