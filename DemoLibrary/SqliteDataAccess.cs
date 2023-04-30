using System;
using System.Configuration;
using System.Data;
using System.Data.SQLite;
using Dapper;
using System.Data.SqlTypes;
#pragma warning disable IDE0063 // Use simple 'using' statement

namespace DemoLibrary
{
    public class SqliteDataAccess
    {
        private static string LoadConnectionString(string id = "Default")
        {
            return ConfigurationManager.ConnectionStrings[id].ConnectionString;
        }


        #region Players db
        public static List<Player> LoadAllPlayers()
        {
            using (SQLiteConnection cnn = new(LoadConnectionString()))
            {
                var output = cnn.Query<Player>("select * from players", new DynamicParameters());
                return output.ToList();
            }
        }

        public static Player SavePlayer(Player pName)
        {
            int money = 100000000;
#if !DEBUG
            money = 50000;
#endif
            int new_player_id = 0;

            using (SQLiteConnection cnn = new(LoadConnectionString()))
            {
                cnn.Open();
                cnn.Execute($"INSERT INTO players (Name, Money) VALUES (@Name, {money})", pName);

                new_player_id = (int)cnn.LastInsertRowId;
                cnn.Close();

                return LoadPlayer(new_player_id);
            }
        }


        public static Player LoadPlayer(int Id)
        {
            using (SQLiteConnection cnn = new(LoadConnectionString()))
            {
                Player output = cnn.QuerySingleOrDefault<Player>($"select * from players where Id={Id}", new DynamicParameters());
                return output;
            }
        }

        public static Player UpdatePlayer(Player player)
        {
            using (SQLiteConnection cnn = new(LoadConnectionString()))
            {
                cnn.Open();
                string update_record = ($"UPDATE players SET " +
                    $"Money='{player.Money}', " +
                    $"Loans='{player.Loans}', " +
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
            return LoadPlayer(player.Id);
        }

#endregion


#region Cars db

        public static Car LoadCar(int? Id)
        {
            if (Id != null)
            {
                using (SQLiteConnection cnn = new(LoadConnectionString()))
                {
                    Car output = cnn.QuerySingleOrDefault<Car>($"select * from garage where Id={Id}", new DynamicParameters());
                    return output;
                }
            }
            else
            {
                throw new Exception("No id provided");
            }
        }

        public static List<Car> GetPlayerCars(Player profile)
        {
            using (SQLiteConnection cnn = new(LoadConnectionString()))
            {
                IEnumerable<Car>? output = cnn.Query<Car>($"select * from garage where Owner='{profile.Id}'", new DynamicParameters());

                return output.ToList();
            }
        }

        public static Car InsertCar(Car car)
        {
            using (SQLiteConnection cnn = new(LoadConnectionString()))
            {
                string cmd = "INSERT INTO garage (" +
                    "Name," +
                    "Description," +
                    "Year," +
                    "Class," +
                    "Path," +
                    "Preview," +
                    "TopSpeed," +
                    "Price," +
                    "Kms," +
                    "Owner, " +
                    "ForSale" +
                    ") VALUES (" +
                    "@Name, " +
                    "@Description, " +
                    "@Year, " +
                    "@Class, " +
                    "@Path, " +
                    "@Preview, " +
                    "@TopSpeed, " +
                    "@Price, " +
                    "@Kms, " +
                    "@Owner, " +
                    "@ForSale)";

                cnn.Open();
                cnn.Execute(cmd, car);
                long car_id = cnn.LastInsertRowId;
                return LoadCar((int)car_id);
            }
        }

        /// <summary>
        /// Updates price, mileage and owner. Returns the car updated
        /// </summary>
        public static Car UpdateCar(Car car)
        {
            using (SQLiteConnection cnn = new(LoadConnectionString()))
            {

                cnn.Open();
                string update_record = $"UPDATE garage SET " +
                    $"Price='{car.Price}', " +
                    $"Kms='{car.Kms}', " +
                    $"ForSale='{car.ForSale}', " +
                    $"Owner='{car.Owner}' " +
                    $"WHERE Id='{car.Id}'";

                SQLiteCommand command = new(update_record, cnn);
                command.ExecuteNonQuery();
                cnn.Close();

                return LoadCar(car.Id);
            }
        }

        public static List<Car> LoadForSaleCars()
        {
            using (SQLiteConnection cnn = new(LoadConnectionString()))
            {
                var output = cnn.Query<Car>($"select * from garage where Owner=NULL OR ForSale=1", new DynamicParameters()).ToList();
                return output;
            }
        }

#endregion
    }
}
