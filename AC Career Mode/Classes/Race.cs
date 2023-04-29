using DemoLibrary;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
#pragma warning disable CS8605 // Unboxing a possibly null value.

namespace AC_Career_Mode
{
    [Serializable]
    public class Race
    {
        public int Laps { get; set; }
        public bool Completed { get; set; }
        public Car Car { get; set; }

        public Track Track { get; set; }
        public int Prize { get; set; }
        public RaceLength RaceType { get; set; }
        public int Seed { get; set; }
        public double LengthKm { get; set; }
        public int LengthMinutes { get; set; }
        public double OneLapTimeMinutes { get; set; }
        public string Description { get; set; }

        /// <summary>
        /// Creates a race of variable length given a car, track and a length type
        /// </summary>
        public Race(Car car, Track track, RaceLength raceLengthType, int seed_modifier = 0)
        {
            Completed = false;
            Car = car;
            Track = track;
            RaceType = raceLengthType;
            OneLapTimeMinutes = (Track.LengthKm / (Car.TopSpeed * 0.70))*60;

            Seed = Utils.TodaysSeed() + (int)raceLengthType + (int)Car.Group + seed_modifier;

            #region RACE LENGTH

            int factor_seed = Seed + (int)RaceType;
            Laps = 0;

            switch (RaceType)
            {
                case RaceLength.Short:
                    LengthMinutes = Utils.RoundX(Utils.GetRandomNumber(5, 15, factor_seed), 5);
                    break;

                case RaceLength.Medium:
                    LengthMinutes = Utils.RoundX(Utils.GetRandomNumber(15, 30, factor_seed), 5);
                    break;

                case RaceLength.Long:
                    LengthMinutes = Utils.RoundX(Utils.GetRandomNumber(30, 60, factor_seed), 5);
                    break;

                case RaceLength.Endurance:
                    LengthMinutes = Utils.RoundX(Utils.GetRandomNumber(60, 120, factor_seed), 5);
                    break;

                default:
                    throw new Exception("No race length defined");
            };

            #endregion

            // Race length and laps calculations are fuzzy (not all cars have top speed and not all tracks have length)
            // But it's used to define the prize based on expected travel distance
            // Rounded up to have 1 lap races (ie nordschleife)
            Laps = Convert.ToInt32(Math.Ceiling(LengthMinutes / OneLapTimeMinutes));
            LengthKm = Convert.ToInt32(Track.LengthKm * Laps);
            switch (Car.Group)
            {
                case CarGroup.F1:
                    Prize = Utils.RoundX(LengthKm, 10) * 2400;
                    break;

                case CarGroup.Formula:
                    Prize = Utils.RoundX(LengthKm, 10) * 1400;
                    break;

                case CarGroup.GT:
                    Prize = Utils.RoundX(LengthKm, 10) * 600;
                    break;
                
                default:
                    Prize = Utils.RoundX(LengthKm, 10) * 400;
                    break;
            }


            Description = Track.Name + "\n" +
              "Track Length: " + Math.Round(Track.LengthKm, 1) + " km" + "\n" +
              Car.Name + "\n" +
              "Laps: " + Laps.ToString() + "\n" +
              "Race Lenght: " + LengthKm + " km" + "\n" +
              "Top Speed: " + Car.TopSpeed + "\n" +
              "Race Time: " + LengthMinutes + " minutes" + "\n" +
              "Prize: " + Prize;

        }

        /// <summary>
        /// Creates a random race given a list of available tracks, cars and a length type
        /// </summary>
        public static Race RaceFromList(List<Track> tracks, List<Car> cars, int seed_modifier = 0)
        {
            Random random = new Random(Utils.TodaysSeed() + seed_modifier);

            Array length_types = Enum.GetValues(typeof(RaceLength));

            RaceLength raceLengthType = (RaceLength)length_types.GetValue(random.Next(length_types.Length));


            Car car = cars[random.Next(cars.Count)];
            Track track = tracks[random.Next(tracks.Count)];


            // Force track to be suitable for car (ie no small tracks for F1 cars)
            // This may change to a list of tracks and allowed groups
            switch (car.Group)
            {
                case CarGroup.F1:
                    {
                        string[] DissallowedTracks = new string[] { "Donington", "Short", "Mallory", "Atlanta", "Victor Borrat", "Oval", "Indianapolis", "Magione", "Nordschleife", "Phoenix", "Tsukuba", };

                        while (DissallowedTracks.Any(disall_tracks => track.Name.ToLower().Contains(disall_tracks.ToLower())))
                        {
                            track = tracks[random.Next(tracks.Count)];
                        }
                        break;
                    }
                case CarGroup.Formula:
                    {

                        string[] DissallowedTracks = new string[] { "Donington", "Short", "Mallory", "Atlanta", "Victor Borrat", "Oval", "Indianapolis", "Magione", "Nordschleife", "Phoenix", "Tsukuba", };


                        while (DissallowedTracks.Any(disall_tracks => track.Name.ToLower().Contains(disall_tracks.ToLower())))
                        {
                            track = tracks[random.Next(tracks.Count)];
                        }
                        break;
                    }
                case CarGroup.GT:
                    {
                        {
                            string[] DissallowedTracks = new string[] { "Atlanta", "Oval", "Indianapolis", "Phoenix" };

                            while (DissallowedTracks.Any(disall_tracks => track.Name.ToLower().Contains(disall_tracks.ToLower())))
                            {
                                track = tracks[random.Next(tracks.Count)];
                            }

                            break;
                        }
                    }
                case CarGroup.Oval:
                    {
                        string[] AllowedTracks = new string[] { "Atlanta", "Oval", "Indy 500", "Phoenix" };


                        while (!AllowedTracks.Any(disall_tracks => track.Name.ToLower().Contains(disall_tracks.ToLower())))
                        {
                            track = tracks[random.Next(tracks.Count)];
                        }
                        break;
                    }
                default:
                    { break; }
            }


            return new Race(car, track, raceLengthType, seed_modifier);
        }


    }
}
