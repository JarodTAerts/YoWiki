using OfflineWikipedia.Helpers;
using OfflineWikipedia.Services;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace OfflineWikipedia.ViewModels
{
    /// <summary>
    /// Class to control the functionality and bindings of the ViewArticlePage. The page to display articles you have downloaded
    /// </summary>
	public class ViewArticlePageViewModel : ViewModelBase
	{
        #region Properties and Bindings
        private string _articleTitle;
        public string ArticleTitle
        {
            get => _articleTitle;
            set => SetProperty(ref _articleTitle, value);
        }

        private string _articleText;
        public string ArticleText
        {
            get => _articleText;
            set => SetProperty(ref _articleText, value);
        }

        private bool _isSearching;
        public bool IsSearching
        {
            get => _isSearching;
            set => SetProperty(ref _isSearching, value);
        }
        #endregion

        #region Constructor
        public ViewArticlePageViewModel(INavigationService navigationService) : base(navigationService)
        {
            IsSearching = true;
        }
        #endregion

        #region Command Functions

        //Nothing here yet

        #endregion

        #region Overrides
        public async override void OnNavigatedTo(NavigationParameters parameters)
        {
            //If a TITLE was passed in then get the text from the file of that title and set it to the displayed text
            if (parameters.ContainsKey("TITLE"))
            {
                ArticleTitle = (string)parameters.Where(i => i.Key == "TITLE").SingleOrDefault().Value;
                IsSearching = true;
                ArticleText =await StorageService.GetHTMLTextFromFile(ArticleTitle);
                IsSearching = false;
            }
        }
        #endregion
    }
}
