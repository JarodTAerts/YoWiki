using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using YoWiki.Accessors;
using YoWiki.Accessors.Interfaces;
using YoWiki.Services;
using YoWiki.Services.Interfaces;
using YoWiki.Views;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;

namespace YoWiki
{
    public partial class App : Application
    {

        public App()
        {
            InitializeComponent();

            // Register dependency injection stuff
            DependencyService.Register<IStorageAccessor, StorageAccessor>();
            DependencyService.Register<IWikipediaAccessor, WikipediaAccessor>();
            DependencyService.Register<IHTMLService, HTMLService>();
            DependencyService.Register<ILocalArticlesService, LocalArticleService>();
            DependencyService.Register<IWikipediaService, WikipediaService>();

            MainPage = new AppShell();
        }

        protected override void OnStart()
        {
            // Handle when your app starts
            AppCenter.Start("android=97b563a9-317d-491a-973b-730e19c4f7c0;" +
                  "uwp={Your UWP App secret here};" +
                  "ios={Your iOS App secret here}",
                  typeof(Analytics), typeof(Crashes));
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
