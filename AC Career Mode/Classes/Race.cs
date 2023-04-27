using DemoLibrary;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace AC_Career_Mode
{
    public class Race
    {
        public uint Laps { get; set; }
        public Car Car { get; set; }
        public string DisplayName { get; set; }
        public Track Track { get; set; }
        public uint Prize { get; set; }
        public RaceLength RaceType { get; set; }
        public int Seed { get; set; }
        public string Description { get; set; }
        public double TotalLength { get; set; }
        public RaceGroup Group { get; set; }
        public TimeSpan RaceTime { get; set; }
        public double OneLapTime { get; set; }

        ///  Race is determined by type short to endurance
        ///  Race length is determined first by the 70% the top speed of the car. To make the race short in time
        ///  If car has no top speed attribute, it's a random between a distance in kms and a factor between 60% and 100%
        ///  Seed changes with the day
        public Race(RaceLength raceType, List<Track> tracks, List<Car> cars, RaceGroup group, int seed_modifier = 0)
        {
            RaceType = raceType;
            Group = group;

            // TodaysSeed makes sure the race is changed daily
            // RaceType and group flavour the seed according to the type of race
            Seed = Utils.TodaysSeed() + (int)raceType + (int)group + seed_modifier;

#if DEBUG
//  Different seeds for testing
            Random random = new();
#endif
#if RELEASE
            Random random = new Random(Seed);
#endif

            Car = cars[random.Next(cars.Count)];
            Track = tracks[random.Next(tracks.Count)];


            OneLapTime = Track.LengthKm / (Car.TopSpeed * 0.70);

            #region RACE GROUP



            if (Group == RaceGroup.F1)
            {
                string[] AllowedCars = new string[] { "Americas 2020\0", "Hybrid 2022", "RSS 1990 V12", "RSS 2000 V10", "RSS 2013 V8", "312/67", "SF15-T", "SF70H" };

                string[] DissallowedTracks = new string[] { "Donington", "Short", "Mallory", "Atlanta", "Victor Borrat", "Oval", "Indianapolis", "Magione", "Nordschleife", "Phoenix", "Tsukuba", };

                while (!AllowedCars.Any(all_car => Car.Name.ToLower().Contains(all_car.ToLower())))
                {
                    Car = cars[random.Next(cars.Count)];
                }

                while (DissallowedTracks.Any(disall_tracks => Track.Name.ToLower().Contains(disall_tracks.ToLower())))
                {
                    Track = tracks[random.Next(tracks.Count)];
                }
            }

            else if (Group == RaceGroup.Formula)
            {
                string[] AllowedCars = new string[] { "RSS 2", "RSS 4", "F312", "Type 25", "Type 49", "Maserati 250F", "X2010", "kart" };

                string[] DissallowedTracks = new string[] { "Donington", "Short", "Mallory", "Atlanta", "Victor Borrat", "Oval", "Indianapolis", "Magione", "Nordschleife", "Phoenix", "Tsukuba", };

                while (!AllowedCars.Any(all_car => Car.Name.ToLower().Contains(all_car.ToLower())))
                {
                    Car = cars[random.Next(cars.Count)];
                }

                while (DissallowedTracks.Any(disall_tracks => Track.Name.ToLower().Contains(disall_tracks.ToLower())))
                {
                    Track = tracks[random.Next(tracks.Count)];
                }
            }

            else if (Group == RaceGroup.GT)
            {
                string[] DisallowedCars = new string[] { "Formula", "F312", "312/67", "SF15-T", "SF70H", "Type 25", "Type 49", "Maserati 250F", "X2010", "Kart" };

                string[] DissallowedTracks = new string[] { "Atlanta", "Oval", "Indianapolis", "Phoenix" };

                while (DisallowedCars.Any(all_car => Car.Name.ToLower().Contains(all_car.ToLower())))
                {
                    Car = cars[random.Next(cars.Count)];
                }

                while (DissallowedTracks.Any(disall_tracks => Track.Name.ToLower().Contains(disall_tracks.ToLower())))
                {
                    Track = tracks[random.Next(tracks.Count)];
                }

            }

            else if (Group == RaceGroup.Oval)
            {
                string[] AllowedCars = new string[] { "NASCAR", "Formula Americas 2020 Oval" };

                string[] AllowedTracks = new string[] { "Atlanta", "Oval", "Indy 500", "Phoenix" };

                while (!AllowedCars.Any(all_car => Car.Name.ToLower().Contains(all_car.ToLower())))
                {
                    Car = cars[random.Next(cars.Count)];
                }

                while (!AllowedTracks.Any(disall_tracks => Track.Name.ToLower().Contains(disall_tracks.ToLower())))
                {
                    Track = tracks[random.Next(tracks.Count)];
                }
            }


            #endregion

            #region RACE LENGTH

            int factor_seed = Seed + (int)RaceType;

            if (RaceType == RaceLength.Short)
            {
                Laps = (uint)TotalLaps(0.15, 30, factor_seed);
            }
            else if (RaceType == RaceLength.Medium)
            {
                Laps = (uint)TotalLaps(0.50, 70, factor_seed);
            }

            else if (RaceType == RaceLength.Long)
            {
                Laps = (uint)TotalLaps(1.00, 150, factor_seed);
            }
            else if (RaceType == RaceLength.Endurance)
            {
                Laps = (uint)TotalLaps(2.00, 350, factor_seed);
            }
            else
            {
                throw (new Exception("Need a RaceLength"));
            }


            try
            {                
                RaceTime = TimeSpan.FromHours(OneLapTime * Laps);
            }

            // TimeSpan OverflowException with Formula Indy even tough it has a Top Speed value
            catch (OverflowException ex)
            {
                Trace.WriteLine(Car.Name);
                Trace.WriteLine(Track.Name);
                Trace.WriteLine(ex);
                RaceTime = TimeSpan.FromHours(1);
            }


            #endregion

            DisplayName = Car.Name + " - " + Track.Name;
            TotalLength = Math.Round(Track.LengthKm * Laps, 1);
            Prize = Utils.RoundTen(TotalLength) * 600;

            Description = Track.Name + "\n" +
                          "Track Length: " + Math.Round(Track.LengthKm, 1) + " km" + "\n" +
                          Car.Name + "\n" +
                          "Laps: " + Laps.ToString() + "\n" +
                          "Race Lenght: " + TotalLength + " km" + "\n" +
                          "Top Speed: " + Car.TopSpeed + "\n" +
                          "Race Time: " + Math.Round(RaceTime.TotalMinutes) + " minutes" + "\n" +
                          "Prize: " + Prize;

        }



        internal int TotalLaps(double TopRaceTime, double TopRaceLength, int seed)
        {
            double factor = Utils.GetRandomNumber(0.60, 1.00, seed);
            double OneLapTime = Track.LengthKm / (Car.TopSpeed * 0.60);

            // Not working because not all cars have top speed
            // and because it's not a perfect approach
            //// Car has top speed in json, calculate laps with race time
            //if (Car.TopSpeed > 0)
            //{
            //    double RaceTimeRandomized = TopRaceTime * factor;
            //    return (int)Math.Round(RaceTimeRandomized / OneLapTime);
            //}

            //// Calculate laps with race length
            //else
            //{
            //    double LengthRandomized= TopRaceLength * factor;
            //    return (int)Math.Round(LengthRandomized / Track.LengthKm);

            //}

            double LengthRandomized = TopRaceLength * factor;
            return Convert.ToInt32(LengthRandomized / Track.LengthKm);
        }

    }
}
