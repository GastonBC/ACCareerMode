using DemoLibrary;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace AC_Career_Mode
{
    public class Record
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public int UserId { get; set; }
        public string LogType { get; set; }
        public string Description { get; set; }

        // For xml serialization
        internal Record()
        {
            
        }

        internal Record(string logType, int userId, string description)
        {
            Date = DateTime.Now;
            UserId = userId;
            LogType = logType;
            Description = description;
        }

        internal static Record RecordRace(Player player, Race race, RaceResult result)
        {
            string msg = $"{player.Name} raced in {race.Track.Name} with a {race.Car.Name} and came out {result.Position}, winning {result.PrizeAwarded}";
            Record r = new Record("Race", player.Id, msg);

            InsertLog(r, player);
            return r;
        }

        internal static Record RecordError(Exception ex, Player player)
        {
            string msg = $"EXCEPTION -> {ex.Message} at {ex.Source}";
            Record r = new Record("Exception", player.Id, msg);

            InsertLog(r, player);
            return r;
        }

        /// Inserts a record in the log file, if it doesnt exists, creates one
        private static void InsertLog(Record record, Player profile)
        {
            XmlSerializer serializer = new XmlSerializer(record.GetType());



            string log_path = $"./{profile.Name}.xml";

            using (StreamWriter sw = new(log_path, true))
            {
                serializer.Serialize(sw, record);
            }

        }

    }
}

