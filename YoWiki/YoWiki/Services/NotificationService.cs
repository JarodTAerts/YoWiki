using Plugin.LocalNotification;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace YoWiki.Services
{
    /// <summary>
    /// Service to send alerts and notifications to the user
    /// </summary>
    public static class NotificationService
    {
        /// <summary>
        /// Function to handle sending notifications and alerts. If the app is in background it will send notification,
        /// if the app is in the foreground it will send an alert
        /// </summary>
        /// <param name="title">Title of alert or notification</param>
        /// <param name="text">Main body text of alert or notification</param>
        /// <param name="buttonText">Button text for alert</param>
        /// <returns>Nothing</returns>
        public static async void SendAlertOrNotification(string title, string text, string buttonText)
        {
            if (App.IsInBackground)
            {
                var notification = new NotificationRequest
                {
                    NotificationId = 100,
                    Title = title,
                    Description = text
                };
                NotificationCenter.Current.Show(notification);
            }

            // Also send alert they can see when they get back into the app
            await Shell.Current.DisplayAlert(title, text, buttonText);
        }
    }
}
