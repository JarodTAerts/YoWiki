using Xamarin.Forms;
using YoWiki.Accessors;
using YoWiki.Accessors.Interfaces;
using YoWiki.Services;
using YoWiki.Services.Interfaces;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using System.IO;
using System.Reflection;
using System.Linq;
using Plugin.LocalNotification;
using System;

namespace YoWiki
{
    public partial class App : Application
    {
        public static bool IsInBackground;

        public App()
        {
            InitializeComponent();

            // Register dependency injection stuff
            DependencyService.Register<IStorageAccessor, StorageAccessor>();
            DependencyService.Register<IWikipediaAccessor, WikipediaAccessor>();
            DependencyService.Register<IHTMLService, HTMLService>();
            DependencyService.Register<ILocalArticlesService, LocalArticleService>();
            DependencyService.Register<IWikipediaService, WikipediaService>();

            PersistentDownloadService.Start();

            MainPage = new AppShell();
        }

        protected override void OnStart()
        {
            // Handle when your app starts
            AppCenter.Start("android=97b563a9-317d-491a-973b-730e19c4f7c0;" +
                  "uwp={Your UWP App secret here};" +
                  "ios=ae1bb759-7608-42f7-93b6-aaa4b83e311d;",
                  typeof(Analytics), typeof(Crashes));

            IsInBackground = false;
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
            IsInBackground = true;
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
            IsInBackground = false;
        }
    }
}
