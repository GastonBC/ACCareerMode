using Dapper;
using DBLink.Services;
using Newtonsoft.Json;
using ProtoBuf;
using System.Data.SQLite;
using System.Security.Policy;
using Utilities;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

namespace DBLink.Classes
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
        public int OwnerId { get; set; }

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

            if (!int.TryParse(new string(car.Specs["topspeed"].Where(char.IsDigit).ToArray()), out int top_speed))
            {
                top_speed = 200;
            }
            car.TopSpeed = top_speed;
            double PriceFactor = Utils.GetRandomNumber(0.9, 1.2, car.Path.Length);

            ManualCarData manual_values = ManualCarData.LoadCarValues().Find(c => c.Name == car.Name);





            if (manual_values != null)
            {
                car.Price = Utils.RoundX(manual_values.Price * PriceFactor, 100);
                car.Group = manual_values.Group;
            }
            else
            {
#if !RELEASE
                //Utils.Alert("", $"Car not manually inputted. {car.Name}");
#endif
                car.Price = 10000;
                car.Group = CarGroup.GT;
            }




            car.Kms = 0;
            car.OwnerId = 0;
            car.ForSale = 1;
            car.Id = 0;

            /// Price cars according their type
            /// Maybe a json file with middle prices

            return car;
        }

        public static Car LoadCar(int id)
        {
            return SqliteDataAccess.QuerySingleById<Car>(id, "garage");
        }

        public void DeleteInDB()
        {
            string cmd = $"DELETE FROM garage WHERE Id={Id}";
            SqliteDataAccess.ExecCmd(cmd);
        }


        public void InsertInDB()
        {
            // Id must be 0 to guarantee it's a new car in the db
            if (Id != 0)
            {
                throw new InvalidOperationException("Car Id is not 0");
            }

            string cmd = $"INSERT INTO garage (Name, Path, Preview, TopSpeed, Price, Kms, OwnerId, ForSale) " +
                $"VALUES ('{Name}', '{Path}', '{Preview}', {TopSpeed}, {Price}, {Kms}, {OwnerId}, {ForSale})";

            SqliteDataAccess.ExecCmd(cmd);
        }



        /// <summary>
        /// Updates price, mileage and owner. Returns the car updated
        /// </summary>
        public void UpdateInDB()
        {
            using (SQLiteConnection cnn = new(SqliteDataAccess.LoadConnectionString()))
            {

                string cmd = $"UPDATE garage SET " +
                    $"Price='{Price}', " +
                    $"Kms='{Kms}', " +
                    $"ForSale='{ForSale}', " +
                    $"OwnerId='{OwnerId}' " +
                    $"WHERE Id='{Id}'";

                SqliteDataAccess.ExecCmd(cmd);
            }
        }
    }
}
