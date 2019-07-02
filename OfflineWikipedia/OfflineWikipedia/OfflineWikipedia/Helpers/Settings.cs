﻿using System;
using System.Collections.Generic;
using System.Text;
using Plugin.Settings;
using Plugin.Settings.Abstractions;

namespace OfflineWikipedia.Helpers
{
    /// <summary>
    /// Class that handles all the settings and data that must persist from one opening to another
    /// </summary>
    public class Settings
    {
        protected static ISettings AppSettings=CrossSettings.Current;

        /// <summary>
        /// Settings that controls whether the app will download over cellular connection 
        /// Currently not connected to anything
        /// </summary>
        public static bool DownloadOverCell
        {
            get
            {
                return AppSettings.GetValueOrDefault("DownloadOverCell", false);
            }
            set
            {
                AppSettings.AddOrUpdateValue("DownloadOverCell", value);
            }
        }

        /// <summary>
        /// Setting that controls whether images will be downloaded or not
        /// Currently not connected to anything
        /// </summary>
        public static bool DownloadImages
        {
            get
            {
                return AppSettings.GetValueOrDefault("DownloadImages", false);
            }
            set
            {
                AppSettings.AddOrUpdateValue("DownloadImages", value);
            }
        }

        /// <summary>
        /// Setting that controls the number of example articles that will be returned when searching
        /// </summary>
        public static int NumberOfResults
        {
            get
            {
                return AppSettings.GetValueOrDefault("NumberOfResults", 5);
            }
            set
            {
                AppSettings.AddOrUpdateValue("NumberOfResults", value);
            }
        }

        /// <summary>
        /// Setting that controls whether it is the first time the app is opened
        /// </summary>
        public static bool FirstTimeOpened
        {
            get
            {
                return AppSettings.GetValueOrDefault("FirstTimeOpened",true);
            }
            set
            {
                AppSettings.AddOrUpdateValue("FirstTimeOpened", value);
            }
        }

        /// <summary>
        /// Stored data that holds the average time it takes to download and process a file
        /// Used for estimating the time to finish downloading files
        /// </summary>
        public static double AverageDownloadAndProcessingTimePerFile
        {
            get
            {
                return AppSettings.GetValueOrDefault("AverageDownloadAndProcessingTimePerFile", 500.0);
            }
            set
            {
                AppSettings.AddOrUpdateValue("AverageDownloadAndProcessingTimePerFile", value);
            }
        }

        /// <summary>
        /// Stored data which holds the number of entries in the average. This is used to get a weighted average time for better time estimates
        /// </summary>
        public static int NumberOfEntriesInAverageDownloadTime
        {
            get
            {
                return AppSettings.GetValueOrDefault("NumberOfEntriesInAverageDownloadTime", 0);
            }
            set
            {
                AppSettings.AddOrUpdateValue("NumberOfEntriesInAverageDownloadTime", value);
            }
        }
    }
}
