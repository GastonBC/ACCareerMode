﻿using Dapper;
using Newtonsoft.Json;
using ProtoBuf;
using System.Data.SQLite;
using Utilities;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

namespace DBLink
{
    [ProtoContract]
    public class Car
    {
        [ProtoMember(1)]
        public int Id { get; set; }
        [ProtoMember(2)]
        public string Name { get; set; }
        [ProtoMember(3)]
        public int ForSale { get; set; }
        [ProtoMember(4)]
        public string Description { get; set; }
        [ProtoMember(5)]
        public string Year { get; set; }
        [ProtoMember(6)]
        public string Class { get; set; }
        [ProtoMember(7)]
        public List<string> Tags { get; set; }
        [ProtoMember(8)]
        public string Path { get; set; }
        [ProtoMember(9)]
        public string Preview { get; set; }
        [ProtoMember(10)]
        public CarGroup Group { get; set; }
        [ProtoMember(11)]
        public int TopSpeed { get; set; }
        [ProtoMember(12)]
        public Dictionary<string, string> Specs { get; set; }
        [ProtoMember(13)]
        public int Kms { get; set; }
        [ProtoMember(14)]
        public int Price { get; set; }
        [ProtoMember(15)]
        public int? Owner { get; set; }

        public static Car LoadCarJson(string json_path)
        {
            Car car = new();

            // Json fills most of the parameters
            using (StreamReader r = new(json_path))
            {
                string json = r.ReadToEnd();
                car = JsonConvert.DeserializeObject<Car>(json);
                JsonSerializerSettings settings = new();
            }

            // Link any car preview
            DirectoryInfo car_folder = Directory.GetParent(json_path).Parent;
            car.Preview = Directory.GetFiles(car_folder.ToString(), "preview.jpg", SearchOption.AllDirectories).First().ToString();

            car.Path = car_folder.ToString();

            // Try to get it's top speed for race time calculations
            // Most cars dont have it or it's badly formatted

            if (!Int32.TryParse(new string(car.Specs["topspeed"].Where(char.IsDigit).ToArray()), out int top_speed))
            {
                top_speed = 200;
            }
            car.TopSpeed = top_speed;



            ManualCarData manual_values = ManualCarData.LoadCarValues().Find(c => c.Name == car.Name);


                
            
            if (manual_values == null)
            {
                throw new Exception($"Car not manually inputted. {car.Name}");
            }


            double PriceFactor = Utils.GetRandomNumber(0.9, 1.2, car.Path.Length);

            car.Price = Utils.RoundX(manual_values.Price * PriceFactor, 100);

            car.Group = manual_values.Group;
            car.Kms = 0;
            car.Owner = null;
            car.ForSale = 1;
            car.Id = 0;

            /// Price cars according their type
            /// Maybe a json file with middle prices

            return car;
        }

        public Car LoadCar(int? Id)
        {
            if (Id != null)
            {
                using (SQLiteConnection cnn = new(SqliteDataAccess.LoadConnectionString()))
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

        public static List<Car> GetPlayerCars(Player player)
        {
            using (SQLiteConnection cnn = new(SqliteDataAccess.LoadConnectionString()))
            {
                IEnumerable<Car>? output = cnn.Query<Car>($"select * from garage where Owner='{player.Id}'", new DynamicParameters());

                return output.ToList();
            }
        }

        public static Car InsertCar(Car car)
        {
            using (SQLiteConnection cnn = new(SqliteDataAccess.LoadConnectionString()))
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
                cnn.Close();

                return LoadCar((int)car_id);
            }
        }



        /// <summary>
        /// Updates price, mileage and owner. Returns the car updated
        /// </summary>
        public static Car UpdateCar(Car car)
        {
            using (SQLiteConnection cnn = new(SqliteDataAccess.LoadConnectionString()))
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
            using (SQLiteConnection cnn = new(SqliteDataAccess.LoadConnectionString()))
            {
                var output = cnn.Query<Car>($"select * from garage where Owner=NULL OR ForSale=1", new DynamicParameters()).ToList();
                return output;
            }
        }
    }
}
