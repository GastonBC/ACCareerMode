using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DBLink.Classes
{
    public class Driver
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int XP { get; set; }
        public int KmsDriven { get; set; }
        public int Races { get; set; }
        public int RaceWins { get; set; }
        public int RacePodiums { get; set; }
        public int EquippedCarId { get; set; }
        protected int _IsAI { get; set; }
        public bool IsAI
        {
            get { return Convert.ToBoolean(_IsAI); }
            set { Convert.ToInt32(value); }
        }


        // Load player from DB given an Id
        public static Driver LoadDriver(int Id)
        {
            using (SQLiteConnection cnn = new(SqliteDataAccess.LoadConnectionString()))
            {
                Player output = cnn.QuerySingleOrDefault<Player>($"SELECT * FROM drivers WHERE Id={Id}", new DynamicParameters());
                return output;
            }
        }


        protected void UpdateInDB()
        {
            using (SQLiteConnection cnn = new(SqliteDataAccess.LoadConnectionString()))
            {
                cnn.Open();
                string update_record = $"UPDATE drivers SET " +
                    $"Races='{Races}', " +
                    $"RaceWins='{RaceWins}', " +
                    $"RacePodiums='{RacePodiums}', " +
                    $"KmsDriven='{KmsDriven}', " +
                    $"XP='{XP}', " +
                    $"EquippedCarId='{EquippedCarId}' " +
                    $"WHERE Id='{Id}'";

                SQLiteCommand command = new(update_record, cnn);
                command.ExecuteNonQuery();
                cnn.Close();
            }
            return;
        }



    }


}
