using System;
using System.Threading.Tasks;

namespace YoWiki.Services.Interfaces
{
    public interface IHTMLService
    {
        Task CleanHTMLFile(string title);

        string StripHTML(string input);

        string ReplaceColons(string input);

        string SimpleHTMLStrip(string input);

        string CutOutBeforeString(string input, string cutString);

        string CutOutAfterStringReference(string input, string cutString);

        string CutOutAfterString(string input, string cutString);
    }
}
