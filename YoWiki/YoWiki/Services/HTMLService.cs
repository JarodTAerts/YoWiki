using HtmlAgilityPack;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using YoWiki.Services.Interfaces;

namespace YoWiki.Services
{
    public class HTMLService : IHTMLService
    {
        private string commonCss;

        public HTMLService()
        {
            var assembly = Assembly.GetExecutingAssembly();
            string resourceName = assembly.GetManifestResourceNames()
                .Single(str => str.EndsWith("common.css"));

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                commonCss = reader.ReadToEnd();
            }
        }

        public string ReplaceColons(string input)
        {
            input = Regex.Replace(input, "/", "-");
            input = Regex.Replace(input, " ", "_");
            return Regex.Replace(input, ":", "-");
        }

        public string SimpleHTMLStrip(string input)
        {
            return Regex.Replace(input, "<.*?>", string.Empty);
        }

        public string InjectCSS(string htmlString)
        {
            HtmlDocument htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(htmlString);

            htmlDocument.DocumentNode.ChildNodes["html"].ChildNodes["head"].AppendChild(HtmlNode.CreateNode($"<style>{commonCss}</style>"));

            string returnString = htmlDocument.DocumentNode.OuterHtml;

            return returnString;
        }

    }
}
