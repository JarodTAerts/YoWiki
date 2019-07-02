using Prism;
using Prism.Ioc;
using OfflineWikipedia.ViewModels;
using OfflineWikipedia.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using OfflineWikipedia.Helpers;
using Prism.Unity;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace OfflineWikipedia
{
    /// <summary>
    /// Main class of overarching application
    /// </summary>
    public partial class App : PrismApplication
    {
        /* 
         * The Xamarin Forms XAML Previewer in Visual Studio uses System.Activator.CreateInstance.
         * This imposes a limitation in which the App class must have a default constructor. 
         * App(IPlatformInitializer initializer = null) cannot be handled by the Activator.
         */
        public App() : this(null) { }

        public static bool IsInBackgrounded { get; private set; }

        public App(IPlatformInitializer initializer) : base(initializer) { }

        protected override async void OnInitialized()
        {
            InitializeComponent();
            IsInBackgrounded = false;
            await NavigationService.NavigateAsync("NavigationPage/StartPage");
        }
        
        /// <summary>
        /// Function to check to see if it is the first time you opened the app. If it is, maybe do something. After doing that set it to not the first time.
        /// </summary>
        protected void InitializeSettings()
        {
            if (Settings.FirstTimeOpened)
            {
                //Maybe do something for the first time opened?
                Settings.FirstTimeOpened = false;
            }
        }

        /// <summary>
        /// Function to set the navagation pages so that they can be used
        /// </summary>
        /// <param name="containerRegistry"></param>
        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<NavigationPage>();
            containerRegistry.RegisterForNavigation<MainPage>();
            containerRegistry.RegisterForNavigation<StartPage>();
            containerRegistry.RegisterForNavigation<SettingsPage>();
            containerRegistry.RegisterForNavigation<BrowseLibraryPage>();
            containerRegistry.RegisterForNavigation<ViewArticlePage>();
            containerRegistry.RegisterForNavigation<AboutAppPage>();
        }

        protected override void OnResume()
        {
            base.OnResume();
            IsInBackgrounded = false;
        }

        protected override void OnSleep()
        {
            base.OnSleep();
            IsInBackgrounded = true;
        }
    }
}
