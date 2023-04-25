using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace AC_Career_Mode
{
    [Serializable]
    internal class Car
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Year { get; set; }
        public string Class { get; set; }
        public List<string> Tags { get; set; }
        public string Path { get; set; }
        public string Preview { get; set; }
        public int TopSpeed { get; set; }
        public Dictionary<string, string> Specs { get; set; }


        public static Car LoadCarJson(string json_path)
        {
            Car car = new Car();

            // Json fills most of the parameters
            using (StreamReader r = new StreamReader(json_path))
            {
                string json = r.ReadToEnd();
                car = JsonConvert.DeserializeObject<Car>(json);
                JsonSerializerSettings settings = new JsonSerializerSettings();
            }

            // Link any car preview
            DirectoryInfo car_folder = Directory.GetParent(json_path).Parent;
            car.Preview = Directory.GetFiles(car_folder.ToString(), "preview.jpg", SearchOption.AllDirectories).First().ToString();


            // Try to get it's top speed for race time calculations
            // Most cars dont have it or it's badly formatted
            int top_speed = 200;


            if (!Int32.TryParse(new string(car.Specs["topspeed"].Where(char.IsDigit).ToArray()), out top_speed))
            {
                top_speed = 200;
            }
            car.TopSpeed = top_speed;

            return car;
        }
    }
}
