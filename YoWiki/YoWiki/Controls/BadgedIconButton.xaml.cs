using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using YoWiki.Services;
using YoWiki.Views;

namespace YoWiki.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BadgedIconButton : AbsoluteLayout
    {
        public string IconSource
        {
            get
            {
                return IconButton.ImageSource.ToString();
            }
            set
            {
                IconButton.ImageSource = value;
            }
        }

        public int BadgeNumber
        {
            get
            {
                return int.Parse(ValueLabel.Text);
            }
            set 
            {
                string valueString = String.Format("{0:n0}", value);
                ValueLabel.Text = valueString;
                int frameWidth = 25 + (valueString.Length>3 ? valueString.Length-3 : 0)*5;
                SetLayoutBounds(Frame, new Rectangle(0.75, 0.25, frameWidth, 25));
                SetLayoutBounds(LabelLayout, new Rectangle(0.75, 0.25, frameWidth, 25));
                if(value <= 0)
                {
                    Frame.IsVisible = false;
                    LabelLayout.IsVisible = false;
                }
                else
                {
                    Frame.IsVisible = true;
                    LabelLayout.IsVisible = true;
                }
            }
        }

        public Color BadgeColor
        {
            get
            {
                return Frame.BackgroundColor;
            }
            set
            {
                Frame.BackgroundColor = value;
            }
        }

        public BadgedIconButton()
        {
            InitializeComponent();
        }

        public void AddButtonEventHandler(EventHandler eventHandler)
        {
            TransparentIconButton.Clicked += eventHandler;
        }
    }
}