using ProtoBuf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using Utilities.Views;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Data;
using System.Resources;

namespace Utilities
{
    public static class Utils
    {

        public static ImageSource? RetriveImage(string imagePath)
        {

            Uri myUri = new(imagePath, UriKind.Absolute);

            try
            {
                BitmapDecoder decoder = BitmapDecoder.Create(myUri, BitmapCreateOptions.IgnoreImageCache, BitmapCacheOption.None);
                return decoder.Frames[0];
            }
            catch (System.IO.FileNotFoundException)
            {
                return null;
            }
        }

        public static void Alert(string title, string message)
        {
            Window1 wn1 = new(title, message);
            wn1.ShowDialog();
        }


        /// <summary>
        /// Checks if a bin file exists for the current day. If it does, it returns elements
        /// from there. Else will create a bin file with the objects provided by the second parameter
        /// and return that. Mostly used to save lists of objects like cars and tracks daily
        /// </summary>
        public static T ReadWriteBin<T>(string path, T objectToSave)
        {
            // Get objects from cache if they are from the same day
            if (File.Exists(path) && DateTime.Today == File.GetLastWriteTime(path).Date)
            {
                return (T)ProtoDeserialize<T>(path);
            }

            // Create cache with given list
            ProtoSerialize(objectToSave, path);
            return objectToSave;
        }

        /// <summary>
        /// Protobuf serializer. Cars and tracks are kept as bin files to work easier and faster
        /// </summary>
        public static void ProtoSerialize(object value, string path)
        {
            using (Stream fStream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                ProtoBuf.Serializer.Serialize(fStream, value);
            }
        }


        /// <summary>
        /// Protobuf deserializer
        /// </summary>
        public static object ProtoDeserialize<T>(string path)
        {
            if (!File.Exists(path)) { throw new FileNotFoundException(); }


            using (Stream fStream = File.OpenRead(path))
            {
                return ProtoBuf.Serializer.Deserialize<T>(fStream);
            }
        }


        public static int TodaysSeed()
        {
            int Year = DateTime.Now.Year;
            int Month = DateTime.Now.Month;
            int Day = DateTime.Now.Day;
            int DateAsSeed = Year * Month * Day;

            // DateAsSeed makes sure the the seed is daily
            return DateAsSeed;
        }

        public static double GetRandomNumber(double minimum, double maximum, int Seed)
        {
            Random random = new(Seed);
            return random.NextDouble() * (maximum - minimum) + minimum;
        }

        public static int RoundX(double i, int x)
        {
            return Convert.ToInt32(i / x) * x;
        }

        public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTime = dateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dateTime;
        }


        /// <summary>
        /// Returns te amount of installments to pay
        /// </summary>
        public static int IsPaymentDue(DateTime lastPaid, int paymentInterval)
        {
            DateTime today = DateTime.Today;

            // ie 25th - 13th = 12 days
            // loan interval must be smaller than 12
            int interval = (int)(today - lastPaid).TotalDays;


            if (paymentInterval < interval)
            {
                double p = interval / paymentInterval;

                return Convert.ToInt32(Math.Floor(p));
            }
            return 0;
        }


        public static GridViewColumnHeader _lastHeaderClicked = null;
        public static ListSortDirection _lastDirection = ListSortDirection.Ascending;

        public static void HeaderClickedHandler(object sender, RoutedEventArgs e, ListView lv)
        {
            GridViewColumnHeader? headerClicked = e.OriginalSource as GridViewColumnHeader;

            ListSortDirection direction;

            if (headerClicked != null)
            {
                if (headerClicked.Role != GridViewColumnHeaderRole.Padding)
                {
                    if (headerClicked != _lastHeaderClicked)
                    {
                        direction = ListSortDirection.Ascending;
                    }
                    else
                    {
                        if (_lastDirection == ListSortDirection.Ascending)
                        {
                            direction = ListSortDirection.Descending;
                        }
                        else
                        {
                            direction = ListSortDirection.Ascending;
                        }
                    }

                    var columnBinding = headerClicked.Column.DisplayMemberBinding as Binding;
                    var sortBy = columnBinding?.Path.Path ?? headerClicked.Column.Header as string;

                    Sort(sortBy, direction, lv);

                    if (direction == ListSortDirection.Ascending)
                    {
                        headerClicked.Column.HeaderTemplate =
                          lv.Resources["HeaderTemplateArrowUp"] as DataTemplate;
                    }
                    else
                    {
                        headerClicked.Column.HeaderTemplate =
                          lv.Resources["HeaderTemplateArrowDown"] as DataTemplate;
                    }

                    // Remove arrow from previously sorted header
                    if (_lastHeaderClicked != null && _lastHeaderClicked != headerClicked)
                    {
                        _lastHeaderClicked.Column.HeaderTemplate = null;
                    }

                    _lastHeaderClicked = headerClicked;
                    _lastDirection = direction;
                }
            }
        }

        public static void Sort(string sortBy, ListSortDirection direction, ListView lv)
        {
            ICollectionView dataView =
              CollectionViewSource.GetDefaultView(lv.ItemsSource);

            dataView.SortDescriptions.Clear();
            SortDescription sd = new SortDescription(sortBy, direction);
            dataView.SortDescriptions.Add(sd);
            dataView.Refresh();
        }

    }

}