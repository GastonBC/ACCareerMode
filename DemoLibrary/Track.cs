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

        public void Upgrade(Player player)
        {
            // Max tier
            if (Tier == 5)
            {
                throw new InvalidOperationException("Max tier reached");
            }

            if (player.Money >= GetUpgradeCost())
            {
                player.Money -= GetUpgradeCost();
                Tier++;

                switch (Tier)
                {
                    case 2:
                        Price = Utils.RoundX(Price * 1.15, 10000);
                        Revenue = Utils.RoundX(Revenue * 1.20, 1000); 
                        break;

                    case 3:
                        Price = Utils.RoundX(Price * 1.18, 10000);
                        Revenue = Utils.RoundX(Revenue * 1.25, 1000);
                        break;

                    case 4:
                        Price = Utils.RoundX(Price * 1.25, 10000);
                        Revenue = Utils.RoundX(Revenue * 1.30, 1000);
                        break;

                    case 5:
                        Price = Utils.RoundX(Price * 1.30, 10000);
                        Revenue = Utils.RoundX(Revenue * 1.40, 1000);
                        break;
                }

                player.UpdateInDB();
                UpdateInDB();
            }


        }

        public int GetUpgradeCost()
        {
            double TierCost = 0;
            switch (Tier)
            {
                case 1:
                    TierCost = Price * 0.25;
                    break;
                case 2:
                    TierCost = Price * 0.55;
                    break;
                case 3:
                    TierCost = Price * 1.20;
                    break;
                case 4:
                    TierCost = Price * 1.50;
                    break;
            }
            return Utils.RoundX(TierCost, 1000);
        }


        public void UpdateInDB()
        {
            string cmd = $"UPDATE tracks SET _LastPaid={_LastPaid}, Revenue={Revenue}, Tier={Tier}, Price={Price} WHERE Id={Id}";
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

