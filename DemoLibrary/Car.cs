using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace DemoLibrary
{
    [Serializable]
    public class Car
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        public bool ForSale { get; set; }
        public string Description { get; set; }
        public string Year { get; set; }
        public string Class { get; set; }
        public List<string> Tags { get; set; }
        public string Path { get; set; }
        public string Preview { get; set; }
        public int TopSpeed { get; set; }
        public Dictionary<string, string> Specs { get; set; }
        public int Mileage { get; set; }
        public int Price { get; set; }
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
            
            car.Mileage = 0;
            car.Owner = null;
            car.ForSale = true;
            car.Id = null;

            /// Price cars according their type
            /// Maybe a json file with middle prices

            car.Price = 10000;

            return car;
        }
    }
}
