using ProtoBuf;
using System;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
#pragma warning disable IDE0063 // Use simple 'using' statement

namespace AC_Career_Mode
{
    internal static class Utils
    {



        internal static double GetRandomNumber(double minimum, double maximum, int Seed)
        {
            Random random = new(Seed);
            return random.NextDouble() * (maximum - minimum) + minimum;
        }

        internal static int RoundX(double i, int x)
        {
            return Convert.ToInt32(i / x) * x;
        }

        internal static int TodaysSeed()
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

        internal static ImageSource? RetriveImage(string imagePath)
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


        public static bool IsFileBelowThreshold(string filename, int hours)
        {
            var threshold = DateTime.Now.AddHours(-hours);
            return File.GetCreationTime(filename) >= threshold;
        }

    }
}
