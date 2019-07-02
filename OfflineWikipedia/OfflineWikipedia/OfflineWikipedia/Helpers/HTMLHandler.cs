using OfflineWikipedia.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace OfflineWikipedia.Helpers
{
    /// <summary>
    /// Class to handle the parsing and processing of HTML Text
    /// </summary>
    class HTMLHandler
    {
        /// <summary>
        /// Function that takes the title of an article that has been store locally and takes the text from that file and strips it of 
        /// any HTML and writes the stripped and processed text back to the file
        /// </summary>
        /// <param name="title">Name of the article that needs to be processed</param>
        /// <returns>Nothing</returns>
        public async static Task CleanHTMLFile(string title)
        {
            string fileText = await StorageService.GetHTMLTextFromFile(title);
            fileText = StripHTML(fileText);
            await StorageService.WriteTextToFile(title,fileText);
        }

        /// <summary>
        /// Function that takes in text and strips it of any HTML messiness through a number of Regexs and custom functions.
        /// What comes out is relativlely readable text
        /// </summary>
        /// <param name="input">Text to be processed</param>
        /// <returns>String of processed text</returns>
        public static string StripHTML(string input)
        {
            //Remove any of the scripting that might be in the wikipedia HTML file
            input = Regex.Replace(input, "<script>.*</script>", String.Empty);
            input = Regex.Replace(input, "<script>.*\n.*</script>", String.Empty);
            //Remove all other HTML tags
            input = Regex.Replace(input, "<.*?>", String.Empty);
            //Remove HTML comments and do an html decoding of the text
            input = HttpUtility.HtmlDecode(Regex.Replace(input, "<.*?->", String.Empty));
            //Cut out all random header text before the 'Jump to Search' which is in every article
            input = CutOutBeforeString(input,"Jump to search");
            //Do a variety of cuts to remove text after the main body of the article. There has to be so many different ones because
            //articles get very different near the end 
            input = CutOutAfterStringReference(input, "References[edit]");
            input = CutOutAfterStringReference(input, "References");
            input = CutOutAfterString(input, "<!-- \nNewPP limit report");
            input = CutOutAfterString(input, "Notes[edit]");
            input = CutOutAfterString(input, "See also[edit]");
            input = CutOutAfterString(input, "References:");
            //Cut out any other text that might look werid or such
            input = Regex.Replace(input, "\nv\nt\ne\n", String.Empty);
            input = Regex.Replace(input, "See also:.*?\n", String.Empty);
            input = Regex.Replace(input, "\\[edit\\]", ":\n");
            input = Regex.Replace(input, "\n{3,}", "\n\n");
            //TODO: Figure out a way to process the text so that the tables at the beginning look good
            //Return the text trimmed 
            return input.Trim();
        }

        /// <summary>
        /// Function to replace the senstive characters in title names so they do not mess up the file paths
        /// </summary>
        /// <param name="input">Title to be processed</param>
        /// <returns>String of processed filename</returns>
        public static string ReplaceColons(string input)
        {
            input = Regex.Replace(input, "/", "-");
            return Regex.Replace(input, ":", "-");
        }

        /// <summary>
        /// Function to simplpy strip out any HTML tags from text. This is used on the short descriptions of articles in search
        /// The more in depth stripping messed up their text
        /// </summary>
        /// <param name="input">Text from details of artivle</param>
        /// <returns>Processed text</returns>
        public static string SimpleHTMLStrip(string input)
        {
            return Regex.Replace(input, "<.*?>", String.Empty);
        }

        /// <summary>
        /// Function that cuts out all the text before a specified string. Used to help clean out unwanted text inn the wikipedia article
        /// </summary>
        /// <param name="input">Whole text of the wikipedia article</param>
        /// <param name="cutString">String that will be the cut off point. Al text before this string, and this string, will be removed</param>
        /// <returns>String with processed text</returns>
        public static string CutOutBeforeString(string input, string cutString)
        {
            int indexOfSub = input.IndexOf(cutString);
            return input.Substring(indexOfSub+cutString.Length);
        }

        /// <summary>
        /// Function that cuts out all text after and including the reference section of the article
        /// This has to be a seperete function since there are reference headings before the main reference body and such
        /// Basically this function finds the seconed instance of the cut string and removes all text after that
        /// </summary>
        /// <param name="input">Whole text of the article</param>
        /// <param name="cutString">String that will be removed with all text after that</param>
        /// <returns>String with processed text</returns>
        public static string CutOutAfterStringReference(string input, string cutString)
        {
            int indexOfSub = input.IndexOf(cutString);
            if (indexOfSub > 0)
            {
                indexOfSub = input.IndexOf(cutString, indexOfSub + cutString.Length);
                //Debug.WriteLine("Index of: " + indexOfSub);
                if (indexOfSub > 0) {
                    return input.Substring(0, indexOfSub);
                }
            }
                return input;
            
        }

        /// <summary>
        /// Function that cuts out all text of the input after the specified cut string
        /// </summary>
        /// <param name="input">Whole text of article</param>
        /// <param name="cutString">String that will be cut out with all text after it</param>
        /// <returns>String of processed text</returns>
        public static string CutOutAfterString(string input, string cutString)
        {
            int indexOfSub = input.IndexOf(cutString);
                if (indexOfSub > 0)
                {
                    return input.Substring(0, indexOfSub);
                }

            return input;

        }
    }
}
