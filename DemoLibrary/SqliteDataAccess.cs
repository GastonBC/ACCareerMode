using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Data;
using System.Data.SQLite;
using Dapper;
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
                var output = cnn.Query<Player>("select * from Player", new DynamicParameters());
                return output.ToList();
            }
        }

        public static void SavePlayer(Player pName)
        {
            using (SQLiteConnection cnn = new(LoadConnectionString()))
            {
#if RELEASE
                cnn.Execute("insert into Player (Name, Money) values (@Name, 50000)", pName);
#endif
#if !RELEASE
cnn.Execute("insert into Player (Name, Money) values (@Name, 100000000)", pName);
#endif
            }
        }


        public static Player LoadPlayer(int Id)
        {
            using (SQLiteConnection cnn = new(LoadConnectionString()))
            {
                Player output = cnn.QuerySingleOrDefault<Player>($"select * from Player where Id={Id}", new DynamicParameters());
                return output;
            }
        }

        public static Player UpdatePlayer(Player player)
        {
            using (SQLiteConnection cnn = new(LoadConnectionString()))
            {
                cnn.Open();
                string update_record = ($"UPDATE Player SET " +
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
                    Car output = cnn.QuerySingleOrDefault<Car>($"select * from Cars where Id={Id}", new DynamicParameters());
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
                IEnumerable<Car>? output = cnn.Query<Car>($"select * from Cars where Owner='{profile.Id}'", new DynamicParameters());

                return output.ToList();
            }
        }

        public static void InsertCar(Car car)
        {
            using (SQLiteConnection cnn = new(LoadConnectionString()))
            {
                string cmd = "INSERT INTO Cars (" +
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
                    "@Mileage, " +
                    "@Owner, " +
                    "@ForSale)";

                cnn.Execute(cmd, car);
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
                string update_record = $"UPDATE Cars SET " +
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
                var output = cnn.Query<Car>($"select * from Cars where Owner=NULL OR ForSale=1", new DynamicParameters()).ToList();
                return output;
            }
        }

#endregion
    }
}
