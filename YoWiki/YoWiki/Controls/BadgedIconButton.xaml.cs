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
    /// <summary>
    /// Custom control for an icon button that has a numbered badge
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BadgedIconButton : AbsoluteLayout
    {
        /// <summary>
        /// Property for the source of the icon to be used for the button
        /// </summary>
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

        /// <summary>
        /// Property for the number that will be shown on the badge
        /// TODO: Make this bindable
        /// </summary>
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

                // Change the width of the badge to fit the number
                int frameWidth = 25 + (valueString.Length>3 ? valueString.Length-3 : 0)*5;
                SetLayoutBounds(Frame, new Rectangle(0.75, 0.25, frameWidth, 25));
                SetLayoutBounds(LabelLayout, new Rectangle(0.75, 0.25, frameWidth, 25));
                
                // Do not show badge if there is no number
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

        /// <summary>
        /// Property for the color of the badge
        /// </summary>
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

        /// <summary>
        /// Function to add an event-handler to be called when the button is clicked
        /// </summary>
        /// <param name="eventHandler"></param>
        public void AddButtonEventHandler(EventHandler eventHandler)
        {
            TransparentIconButton.Clicked += eventHandler;
        }
    }
}