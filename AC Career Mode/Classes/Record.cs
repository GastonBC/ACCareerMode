using DBLink;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
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
        internal Record() { }

        internal Record(string logType, int userId, string description)
        {
            Date = DateTime.Now;
            UserId = userId;
            LogType = logType;
            Description = description;
        }

        internal static void RecordRace(Player player, RaceResult result)
        {
            string msg = $"{player.Name} raced in {result.Race.Track.Name} with a {result.Race.Car.Name} and came out {result.Position}, winning {result.PrizeAwarded}";
            Record r = new Record("Race", player.Id, msg);

            InsertLog(r, player);
        }

        internal static void RecordBuy(Player player, Car car)
        {
            string msg = $"{player.Name} bought {car.Name} for ${car.Price}";
            Record r = new Record("BuyCar", player.Id, msg);

            InsertLog(r, player);
        }


        internal static void RecordLoanExecute(Player player, Loan loan)
        {
            string msg = $"{player.Name} took a loan of ${loan.AmountLeft}";
            Record r = new Record("ExecuteLoan", player.Id, msg);

            InsertLog(r, player);
        }

        internal static void RecordLoanPaid(Player player, Loan loan)
        {
            string msg = $"{player.Name} paid ${loan.Installment} off a loan";
            Record r = new Record("PaidLoan", player.Id, msg);

            InsertLog(r, player);
        }

        internal static void RecordSell(Player player, Car car)
        {
            string msg = $"{player.Name} sold {car.Name} for ${car.Price}";
            Record r = new Record("SellCar", player.Id, msg);

            InsertLog(r, player);
        }

        internal static void RecordRegister(Player player)
        {
            string msg = $"{player.Name} created their account.";
            Record r = new Record("AccountCreate", player.Id, msg);

            InsertLog(r, player);
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
            List<Record> records = new();
            XmlSerializer serializer = new XmlSerializer(records.GetType());

            string log_path = $"./userlogs/{profile.Id}_{profile.Name}.xml";

            if (File.Exists(log_path))
            {
                records = DeserializeRecords(profile);
            }
            record.Id = records.Count;
            records.Add(record);

            using (StreamWriter sw = new(log_path))
            {
                serializer.Serialize(sw, records);
            }

        }


        internal static List<Record> DeserializeRecords(Player profile)
        {
            string log_path = $"./userlogs/{profile.Id}_{profile.Name}.xml";

            XmlSerializer deserializer = new XmlSerializer(typeof(List<Record>));

            TextReader reader = new StreamReader(log_path);

            object obj = deserializer.Deserialize(reader);
            List<Record> XmlData = (List<Record>)obj;

            reader.Close();

            return XmlData;
        }


    }
}

