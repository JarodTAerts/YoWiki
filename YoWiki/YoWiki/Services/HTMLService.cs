using System.Text.RegularExpressions;
using YoWiki.Services.Interfaces;

namespace YoWiki.Services
{
    public class HTMLService : IHTMLService
    {

        public string ReplaceColons(string input)
        {
            input = Regex.Replace(input, "/", "-");
            return Regex.Replace(input, ":", "-");
        }

        public string SimpleHTMLStrip(string input)
        {
            return Regex.Replace(input, "<.*?>", string.Empty);
        }

    }
}
