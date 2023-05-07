using Dapper;
using Newtonsoft.Json;
using ProtoBuf;
using System.Data.SQLite;
using System.Numerics;
using Utilities;

namespace DBLink
{
    [ProtoContract]
    public class Track
    {
        [ProtoMember(1)]
        public int Id { get; set; }
        [ProtoMember(2)]
        public string Name { get; set; }
        [ProtoMember(3)]
        public int OwnerId { get; set; }
        [ProtoMember(4)]

        public int Revenue { get; set; }
        [ProtoMember(5)]
        public int Tier { get; set; }
        [ProtoMember(6)]
        private long _LastPaid { get; set; }
        [ProtoMember(7)]
        public int RevenueInterval { get; set; }
        [ProtoMember(8)]
        public int Price { get; set; }
        [ProtoMember(9)]
        public string Length { get; set; }
        [ProtoMember(10)]
        public double LengthKm { get; set; }
        [ProtoMember(11)]

        public string OutlinePath { get; set; }
        [ProtoMember(12)]
        public string PreviewPath { get; set; }
        // Dates in DB are stored as UNIX (int) and converted to string YYYY-MM-DD
        public DateTime LastPaid
        {
            get { return Utils.UnixTimeStampToDateTime(_LastPaid); }
            set { _LastPaid = ((DateTimeOffset)value).ToUnixTimeSeconds(); }
        }



        public static Track LoadTrackJson(string json_path)
        {
            Track track = new();
            track.Id = 0;

            // Json file fills most of the parameters
            using (StreamReader r = new(json_path))
            {
                string json = r.ReadToEnd();
                track = JsonConvert.DeserializeObject<Track>(json);
                JsonSerializerSettings settings = new();
            }

            // Link outline and race preview images
            DirectoryInfo ui_folder = Directory.GetParent(json_path);
            track.OutlinePath = ui_folder.ToString() + @"\" + "outline.png";
            track.PreviewPath = ui_folder.ToString() + @"\" + "preview.png";

            track.RevenueInterval = 5;
            track.LastPaid = DateTime.Today;
            track.Tier = 1;

            Random rd = new Random(Utils.TodaysSeed() + track.Name.Length);

            track.Revenue = Utils.RoundX(rd.Next(5000, 15000), 1000);
            track.Price = Utils.RoundX(rd.Next(100000, 350000), 10000);
            track.OwnerId = 0;



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


        public void PayRevenue(Player player, int multiplier)
        {
            int amount = Revenue * multiplier;

            player.Money += amount;
            LastPaid = DateTime.Today;

            player.UpdateInDB();

            UpdateInDB();
        }

        public void UpdateInDB()
        {
            string cmd = $"UPDATE tracks SET _LastPaid={_LastPaid} WHERE Id={Id}";
            SqliteDataAccess.ExecCmd(cmd);
        }

        public void InsertInDB()
        {
            string cmd = $"INSERT INTO tracks (Name, OwnerId, Revenue, Tier, _LastPaid, RevenueInterval, Price, LengthKm, OutlinePath, PreviewPath) " +
                $"VALUES ('{Name}', {OwnerId}, {Revenue}, {Tier}, {_LastPaid}, {RevenueInterval}, {Price}, {LengthKm}, '{OutlinePath}', '{PreviewPath}')";

            SqliteDataAccess.ExecCmd(cmd);

        }
    }
}

