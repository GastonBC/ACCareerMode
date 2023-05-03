using Dapper;
using Newtonsoft.Json;
using ProtoBuf;
using System.Data.SQLite;
using System.Security.Policy;
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
        public string Path { get; set; }
        [ProtoMember(5)]
        public string Preview { get; set; }
        [ProtoMember(6)]
        public CarGroup Group { get; set; }
        [ProtoMember(7)]
        public int TopSpeed { get; set; }
        [ProtoMember(8)]
        public Dictionary<string, string> Specs { get; set; }
        [ProtoMember(9)]
        public int Kms { get; set; }
        [ProtoMember(10)]
        public int Price { get; set; }
        [ProtoMember(11)]
        public int Owner { get; set; }

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
            car.Owner = 0;
            car.ForSale = 1;
            car.Id = 0;

            /// Price cars according their type
            /// Maybe a json file with middle prices

            return car;
        }

        public static Car LoadCar(int? Id)
        {
            if (Id == 0)
            {
                throw new Exception("No id provided");
            }

            using (SQLiteConnection cnn = new(SqliteDataAccess.LoadConnectionString()))
            {
                Car output = cnn.QuerySingleOrDefault<Car>($"select * from garage where Id={Id}", new DynamicParameters());
                return output;
            }
        }



        public void InsertInDB()
        {
            // Id must be 0 to guarantee it's a new car in the db
            if (Id != 0)
            {
                throw new InvalidOperationException("Car Id is not 0");
            }

            try
            {
                using (SQLiteConnection cnn = new(SqliteDataAccess.LoadConnectionString()))
                {
                    cnn.Open();
                    string cmd = $"INSERT INTO garage (Name, Path, Preview, TopSpeed, Price, Kms, Owner, ForSale) " +
                        $"VALUES ('{Name}', '{Path}', '{Preview}', {TopSpeed}, {Price}, {Kms}, {Owner}, {ForSale})";

                    SQLiteCommand command = new(cmd, cnn);
                    cnn.Execute(cmd);
                    Id = (int)cnn.LastInsertRowId;
                    cnn.Close();
                }
            }
            catch (Exception ex) { System.Diagnostics.Trace.WriteLine(ex.Message); }
        }



        /// <summary>
        /// Updates price, mileage and owner. Returns the car updated
        /// </summary>
        public void UpdateInDB()
        {
            using (SQLiteConnection cnn = new(SqliteDataAccess.LoadConnectionString()))
            {

                cnn.Open();
                string update_record = $"UPDATE garage SET " +
                    $"Price='{Price}', " +
                    $"Kms='{Kms}', " +
                    $"ForSale='{ForSale}', " +
                    $"Owner='{Owner}' " +
                    $"WHERE Id='{Id}'";

                cnn.Execute(update_record);
                cnn.Close();
            }
        }

        public static List<Car> LoadForSaleCars()
        {
            using (SQLiteConnection cnn = new(SqliteDataAccess.LoadConnectionString()))
            {
                var output = cnn.Query<Car>($"SELECT * FROM garage WHERE Owner=0 OR ForSale=1", new DynamicParameters()).ToList();
                return output;
            }
        }
    }
}
