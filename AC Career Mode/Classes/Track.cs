using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;

namespace AC_Career_Mode
{
    [Serializable]
    public class Track
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Length { get; set; }
        public double LengthKm { get; set; }
        public string Year { get; set; }
        public List<string> Tags { get; set; }
        public string Path { get; set; }
        public string OutlinePath { get; set; }
        public string PreviewPath { get; set; }
        public string Author { get; set; }



        public static Track LoadTrackJson(string json_path)
        {
            Track track = new();

            // Json file fills most of the parameters
            using (StreamReader r = new(json_path))
            {
                string json = r.ReadToEnd();
                track = JsonConvert.DeserializeObject<Track>(json);
                JsonSerializerSettings settings = new();
            }

            // Link outline and race preview images
            DirectoryInfo ui_folder = Directory.GetParent(json_path);
            string map_outline = ui_folder.ToString() + @"\" + "outline.png";
            track.OutlinePath = map_outline;

            string preview_outline = ui_folder.ToString() + @"\" + "preview.png";
            track.PreviewPath = preview_outline;


            // Try to get it's length for race distance calculations
            // A lot of tracks are badly formatted
            string lengthM = new(track.Length.Where(char.IsDigit).ToArray());

            double length_m;

            if (!Double.TryParse(lengthM, out length_m))
            {
                length_m = 1;
            }

            track.LengthKm = length_m / 1000;

            return track;
        }

    }
}

