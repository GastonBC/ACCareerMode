using Dapper;
using System.Data.SQLite;

namespace DBLink
{
    public class Player
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Money { get; set; }
        public int Races { get; set; }
        public int RaceWins { get; set; }
        public int RacePodiums { get; set; }
        public int KmsDriven { get; set; }
        public int EquippedCarId { get; set; }



        // Load player from DB given an Id
        public static Player LoadPlayer(int Id)
        {
            using (SQLiteConnection cnn = new(SqliteDataAccess.LoadConnectionString()))
            {
                Player output = cnn.QuerySingleOrDefault<Player>($"select * from players where Id={Id}", new DynamicParameters());
                return output;
            }
        }

        public static List<Player> LoadAllPlayers()
        {
            using (SQLiteConnection cnn = new(SqliteDataAccess.LoadConnectionString()))
            {
                var output = cnn.Query<Player>("SELECT * FROM players", new DynamicParameters());
                return output.ToList();
            }
        }


        public static Player Insert(Player pName)
        {
            int money = 100000000;
#if !DEBUG
            money = 50000;
#endif
            int new_player_id = 0;

            using (SQLiteConnection cnn = new(SqliteDataAccess.LoadConnectionString()))
            {
                cnn.Open();
                cnn.Execute($"INSERT INTO players (Name, Money) VALUES (@Name, {money})", pName);

                new_player_id = (int)cnn.LastInsertRowId;
                cnn.Close();

                return LoadPlayer(new_player_id);
            }
        }


        /// <summary>
        /// Loads all player loans from DB
        /// </summary>
        public List<Loan> GetPlayerLoans()
        {
            List<Loan> loans = new();

            using (SQLiteConnection cnn = new(SqliteDataAccess.LoadConnectionString()))
            {
                IEnumerable<Loan> output = cnn.Query<Loan>($"SELECT * FROM loans where OwnerId={this.Id}", new DynamicParameters());

                return output.ToList();
            }
        }


        public static void UpdatePlayer(Player player)
        {
            using (SQLiteConnection cnn = new(SqliteDataAccess.LoadConnectionString()))
            {
                cnn.Open();
                string update_record = ($"UPDATE players SET " +
                    $"Money='{player.Money}', " +
                    $"Races='{player.Races}', " +
                    $"RaceWins='{player.RaceWins}', " +
                    $"RacePodiums='{player.RacePodiums}', " +
                    $"KmsDriven='{player.KmsDriven}', " +
                    $"EquippedCarId='{player.EquippedCarId}' " +
                    $"WHERE Id='{player.Id}'");

                SQLiteCommand command = new(update_record, cnn);
                command.ExecuteNonQuery();
                cnn.Close();
            }
            return;
        }


    }
}