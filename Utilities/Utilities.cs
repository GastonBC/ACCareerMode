﻿using ProtoBuf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using Utilities.Views;

namespace Utilities
{
    public static class Utils
    {
        public static void Alert(string title, string message)
        {
            Window1 wn1 = new(title, message);
            wn1.ShowDialog();
        }


        /// <summary>
        /// Checks if a bin file exists for the current day. If it does, it returns elements
        /// from there. Else will create a bin file with the objects provided by the second parameter
        /// and return that
        /// </summary>
        public static T ReadCreateBin<T>(string path, T objectToSave)
        {
            // Get objects from cache
            if (File.Exists(path) && DateTime.Today == File.GetLastWriteTime(path).Date)
            {
                return (T)Deserialize<T>(path);
            }

            // Create cache with given list
            else
            {
                Serialize(objectToSave, path);
                return objectToSave;
            }
        }

        /// <summary>
        /// Protobuf serializer. Cars and tracks are kept as bin files to work easier and faster
        /// </summary>
        public static void Serialize(object value, string path)
        {
            using (Stream fStream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                Serializer.Serialize(fStream, value);
            }
        }


        /// <summary>
        /// Protobuf deserializer
        /// </summary>
        public static object Deserialize<T>(string path)
        {
            if (!File.Exists(path)) { throw new FileNotFoundException(); }


            using (Stream fStream = File.OpenRead(path))
            {
                return Serializer.Deserialize<T>(fStream);
            }
        }


        public static int TodaysSeed()
        {
            string date = DateTime.Today.ToString();

            int Year = DateTime.Now.Year;
            int Month = DateTime.Now.Month;
            int Day = DateTime.Now.Day;
            int DateAsSeed = Year * Month * Day;

            // DateAsSeed makes sure the race is changed daily
            // RaceType and group flavour the seed according to the type of race
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
    }
}