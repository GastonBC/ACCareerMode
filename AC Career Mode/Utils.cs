using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace AC_Career_Mode
{
    internal static class Utils
    {

        internal static double GetRandomNumber(double minimum, double maximum, int Seed)
        {
            Random random = new Random(Seed);
            return random.NextDouble() * (maximum - minimum) + minimum;
        }


        internal static ImageSource? RetriveImage(string imagePath)
        {

            Uri myUri = new Uri(imagePath, UriKind.Absolute);
            string str = imagePath.Substring(imagePath.Length - 3);
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

        //Serialize: pass your object to this method to serialize it
        public static void Serialize(object value, string path)
        {
            BinaryFormatter formatter = new BinaryFormatter();

            using (Stream fStream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                formatter.Serialize(fStream, value);
            }
        }

        //Deserialize: Here is what you are looking for
        public static object Deserialize(string path)
        {
            if (!File.Exists(path)) { throw new FileNotFoundException(); }

            BinaryFormatter formatter = new BinaryFormatter();

            using (Stream fStream = File.OpenRead(path))
            {
                return formatter.Deserialize(fStream);
            }
        }


        public static bool IsFileBelowThreshold(string filename, int hours)
        {
            var threshold = DateTime.Now.AddHours(-hours);
            return File.GetCreationTime(filename) >= threshold;
        }

    }
}
