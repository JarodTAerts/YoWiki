using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using YoWiki.Services.Interfaces;

namespace YoWiki.Services
{
    public class HTMLService : IHTMLService
    {
        public HTMLService()
        {
        }

        public Task CleanHTMLFile(string title)
        {
            throw new NotImplementedException();
        }

        public string CutOutAfterString(string input, string cutString)
        {
            int indexOfSub = input.IndexOf(cutString);
            if (indexOfSub > 0)
            {
                return input.Substring(0, indexOfSub);
            }

            return input;
        }

        public string CutOutAfterStringReference(string input, string cutString)
        {
            int indexOfSub = input.IndexOf(cutString);
            if (indexOfSub > 0)
            {
                indexOfSub = input.IndexOf(cutString, indexOfSub + cutString.Length);
                //Debug.WriteLine("Index of: " + indexOfSub);
                if (indexOfSub > 0)
                {
                    return input.Substring(0, indexOfSub);
                }
            }
            return input;
        }

        public string CutOutBeforeString(string input, string cutString)
        {
            int indexOfSub = input.IndexOf(cutString);
            return input.Substring(indexOfSub + cutString.Length);
        }

        public string ReplaceColons(string input)
        {
            input = Regex.Replace(input, "/", "-");
            return Regex.Replace(input, ":", "-");
        }

        public string SimpleHTMLStrip(string input)
        {
            return Regex.Replace(input, "<.*?>", String.Empty);
        }

        public string StripHTML(string input)
        {
            //Remove any of the scripting that might be in the wikipedia HTML file
            input = Regex.Replace(input, "<script>.*</script>", String.Empty);
            input = Regex.Replace(input, "<script>.*\n.*</script>", String.Empty);
            //Remove all other HTML tags
            input = Regex.Replace(input, "<.*?>", String.Empty);
            //Remove HTML comments and do an HTML decoding of the text
            input = HttpUtility.HtmlDecode(Regex.Replace(input, "<.*?->", String.Empty));
            //Cut out all random header text before the 'Jump to Search' which is in every article
            input = CutOutBeforeString(input, "Jump to search");
            //Do a variety of cuts to remove text after the main body of the article. There has to be so many different ones because
            //articles get very different near the end 
            input = CutOutAfterStringReference(input, "References[edit]");
            input = CutOutAfterStringReference(input, "References");
            input = CutOutAfterString(input, "<!-- \nNewPP limit report");
            input = CutOutAfterString(input, "Notes[edit]");
            input = CutOutAfterString(input, "See also[edit]");
            input = CutOutAfterString(input, "References:");
            //Cut out any other text that might look weird or such
            input = Regex.Replace(input, "\nv\nt\ne\n", String.Empty);
            input = Regex.Replace(input, "See also:.*?\n", String.Empty);
            input = Regex.Replace(input, "\\[edit\\]", ":\n");
            input = Regex.Replace(input, "\n{3,}", "\n\n");
            //TODO: Figure out a way to process the text so that the tables at the beginning look good
            //Return the text trimmed 
            return input.Trim();
        }
    }
}
