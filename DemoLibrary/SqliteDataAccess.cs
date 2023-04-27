﻿using System;
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



        public static Car LoadCar(int? Id)
        {
            if (Id != null)
            {
                using (SQLiteConnection cnn = new(LoadConnectionString()))
                {
                    Car output = cnn.QueryFirst<Car>($"select * from Car where Id={Id}", new DynamicParameters());
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
                    "Owner" +
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
                    "@Owner)";

                cnn.Execute(cmd, car);
            }
        }

        /// <summary>
        /// Updates price, mileage and owner
        /// </summary>
        public static Car UpdateCar(Car car)
        {
            using (SQLiteConnection cnn = new(LoadConnectionString()))
            {
                string cmd = "INSERT INTO Cars (" +
                    
                    "Price," +
                    "Kms," +
                    "Owner" +

                    ") VALUES (" +
                    
                    "@Price, " +
                    "@Mileage, " +
                    "@Owner)";

                cnn.Execute(cmd, car);
            }

            return LoadCar(car.Id);
        }
    }
}
