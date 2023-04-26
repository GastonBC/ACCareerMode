using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Data;
using System.Data.SQLite;
using Dapper;

namespace DemoLibrary
{
    public class SqliteDataAccess
    {
        public static List<Player> LoadPlayers()
        {
            using (SQLiteConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<Player>("select * from Player", new DynamicParameters());
                return output.ToList();
            }
        }

        public static void SavePlayer(Player pName)
        {
            using (SQLiteConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                cnn.Execute("insert into Player (Name) values (@Name)", pName);
            }
        }

        public static Player LoadPlayer(int Id)
        {

            using (SQLiteConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                Player output = cnn.QueryFirst<Player>($"select * from Player where Id={Id}", new DynamicParameters());
                return output;
            }
        }

        public static Player UpdatePlayer(Player player)
        {
            using (SQLiteConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                cnn.Open();
                string update_record = ($"UPDATE Player SET " +
                    $"Money='{player.Money}', " +
                    $"Loans='{player.Loans}', " +
                    $"Races='{player.Races}', " +
                    $"RaceWins='{player.RaceWins}', " +
                    $"RacePodiums='{player.RacePodiums}', " +
                    $"KmsDriven='{player.KmsDriven}' " +
                    $"WHERE Id='{player.Id}'");

                SQLiteCommand command = new(update_record, cnn);
                command.ExecuteNonQuery();
                cnn.Close();
            }
            return LoadPlayer(player.Id);
        }

        private static string LoadConnectionString(string id = "Default")
        {
            return ConfigurationManager.ConnectionStrings[id].ConnectionString;
        }
    }
}
