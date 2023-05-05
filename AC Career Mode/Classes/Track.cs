using Newtonsoft.Json;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;

namespace AC_Career_Mode
{
    [ProtoContract]
    public class Track
    {
        [ProtoMember(1)]
        public string Name { get; set; }
        [ProtoMember(2)]
        public string Length { get; set; }
        [ProtoMember(3)]
        public double LengthKm { get; set; }
        [ProtoMember(4)]
        public string Path { get; set; }
        [ProtoMember(5)]
        public string OutlinePath { get; set; }
        [ProtoMember(6)]
        public string PreviewPath { get; set; }
        [ProtoMember(7)]
        public string Author { get; set; }
        [ProtoMember(8)]
        public int Revenue { get; set; }
        [ProtoMember(9)]
        public int Tier { get; set; }
        [ProtoMember(10)]
        public int LastPaid { get; set; }
        [ProtoMember(11)]
        public int RevenueInterval { get; set; }



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

            track.Revenue = 0;


            return track;
        }

    }
}

