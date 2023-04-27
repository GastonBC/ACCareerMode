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
        public static List<Player> LoadPlayers()
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
                cnn.Execute("insert into Player (Name) values (@Name)", pName);
            }
        }

        public static Player LoadPlayer(int Id)
        {
            using (SQLiteConnection cnn = new(LoadConnectionString()))
            {
                Player output = cnn.QueryFirst<Player>($"select * from Player where Id={Id}", new DynamicParameters());
                return output;
            }
        }
        public static Car LoadCar(int Id)
        {
            using (SQLiteConnection cnn = new(LoadConnectionString()))
            {
                Car output = cnn.QueryFirst<Car>($"select * from Car where Id={Id}", new DynamicParameters());
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
                    
               

                string cmd = "INSERT INTO Cars (Name,Description,Year,Class,Path,Preview,TopSpeed,Price,Mileage,Owner) VALUES (@Name, @Description, @Year, @Class, @Path, @Preview, @TopSpeed, @Price, @Mileage, @Owner)";

                cnn.Execute(cmd, car);

                //string insert_record = $"INSERT INTO Cars " +
                //    $"Name='{car.Name}', " +
                //    $"Description='{car.Description}', " +
                //    $"Year='{car.Year}', " +
                //    $"Class='{car.Class}', " +
                //    $"Path='{car.Path}', " +
                //    $"Preview='{car.Preview}', " +
                //    $"TopSpeed='{car.TopSpeed}', " +
                //    $"Price='10000', " +
                //    $"Mileage='{car.Mileage}', +" +
                //    $"Owner='1'";

                //SQLiteCommand command = new(insert_record, cnn);
                //command.ExecuteNonQuery();
                //cnn.Close();
            }
        }

        public static Car UpdateCar(Car car)
        {
            using (SQLiteConnection cnn = new(LoadConnectionString()))
            {
                cnn.Open();
                string update_record = ($"UPDATE Player SET " +
                    $"Mileage='{car.Mileage}', " +
                    $"WHERE Id='{car.Id}'");

                SQLiteCommand command = new(update_record, cnn);
                command.ExecuteNonQuery();
                cnn.Close();
            }
            return LoadCar(car.Id);
        }
    }
}
