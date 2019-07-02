using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OfflineWikipedia.ViewModels
{
	public class AboutAppPageViewModel : BindableBase
	{

        private string _aboutText;
        public string AboutText
        {
            get => _aboutText;
            set => SetProperty(ref _aboutText, value);
        }

        public AboutAppPageViewModel()
        {
            AboutText = "YoWiki is an open source application created with Xamarin.Forms and C# developed by me, Jarod Aerts. The idea for this application came to me while flying. " +
                " I had no real good way to look up questions that I had or read about various topics online. Of course I could purchase connection for in flight wifi or buy tickets for flights that have it complementary, but I am cheap." +
                " I decided to create this application which allows you to download wikipedia articles into a local library for later viewing. I wanted something that was simple to use, completely open source," +
                " and 100 % local.I didn't want to force people to make accounts, and I didnt want to store anyone's data. Therefore, this application is completely private. It will only communicate with the" +
                " Wikipedia API.It will never communicate with any server that I have. I will never be able to see any of what you download or read on your device. YoWiki is simply an easy way to view wikipedia" +
                " articles offline. This application is part of an open source git repository of various" +
                " projects and tools I have made. This is the first real addition to that repository. If you have any interest in using any of my other tools, you probably wont find a use for them, or branching and" +
                " developing any of them for yourself find them here: https://github.com/JarodTAerts/ToolsAndPrograms.git. If you are interested in helping out with the development for the production version of this application please contact me" +
                " at the email provided on the play store page. Thank you for downloading YoWiki. I hope you find a use for it. If you have suggestions for how to improve it please leave a review or email me.";
        }
	}
}
