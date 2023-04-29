using DemoLibrary;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

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
        public string Description { get; set; }
        public double RaceLengthKm { get; set; }
        public RaceGroup Group { get; set; }
        public int RaceLengthMinutes { get; set; }
        public double OneLapTime { get; set; }

        ///  Race is determined by type short to endurance
        ///  Race length is determined first by the 70% the top speed of the car. To make the race short in time
        ///  If car has no top speed attribute, it's a random between a distance in kms and a factor between 60% and 100%
        ///  Seed changes with the day
        public Race(RaceLength raceLengthType, List<Track> tracks, List<Car> cars, RaceGroup group, int seed_modifier = 0)
        {
            RaceType = raceLengthType;
            Group = group;

            // TodaysSeed makes sure the race is changed daily
            // RaceType and group flavour the seed according to the type of race
            Seed = Utils.TodaysSeed() + (int)raceLengthType + (int)group + seed_modifier;

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
                

                string[] DissallowedTracks = new string[] { "Donington", "Short", "Mallory", "Atlanta", "Victor Borrat", "Oval", "Indianapolis", "Magione", "Nordschleife", "Phoenix", "Tsukuba", };

                while (Car.Group != CarGroup.F1)
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

                string[] DissallowedTracks = new string[] { "Donington", "Short", "Mallory", "Atlanta", "Victor Borrat", "Oval", "Indianapolis", "Magione", "Nordschleife", "Phoenix", "Tsukuba", };

                while (Car.Group != CarGroup.Formula)
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
                string[] DissallowedTracks = new string[] { "Atlanta", "Oval", "Indianapolis", "Phoenix" };

                while (Car.Group != CarGroup.GT)
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
                string[] AllowedTracks = new string[] { "Atlanta", "Oval", "Indy 500", "Phoenix" };

                while (Car.Group != CarGroup.Oval)
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
            Laps = 0;

            switch (RaceType)
            {
                case RaceLength.Short:
                    {
                        RaceLengthMinutes = Utils.RoundX(Utils.GetRandomNumber(5, 15, factor_seed), 5);
                        Laps = TotalLaps(30, factor_seed);
                        break;
                    }

                case RaceLength.Medium:
                    {
                        RaceLengthMinutes = Utils.RoundX(Utils.GetRandomNumber(15, 30, factor_seed), 5);
                        Laps = TotalLaps(70, factor_seed);
                        break;
                    }

                case RaceLength.Long:
                    {
                        RaceLengthMinutes = Utils.RoundX(Utils.GetRandomNumber(30, 60, factor_seed), 5);
                        Laps = TotalLaps(150, factor_seed);
                        break;
                    }

                case RaceLength.Endurance:
                    {
                        RaceLengthMinutes = Utils.RoundX(Utils.GetRandomNumber(60, 120, factor_seed), 5);
                        Laps = TotalLaps(350, factor_seed);
                        break;
                    }
                default:
                    {
                        throw new Exception("No race length defined");
                        
                    }
            };

            #endregion

            RaceLengthKm = Convert.ToInt32(Track.LengthKm * Laps);


            switch (Group)
            {
                case RaceGroup.F1:
                    Prize = Utils.RoundX(RaceLengthKm, 10) * 600;
                    break;
                case RaceGroup.GT:
                    Prize = Utils.RoundX(RaceLengthKm, 10) * 600;
                    break;
                default:
                    Prize = Utils.RoundX(RaceLengthKm, 10) * 400;
                    break;
            }    


            Completed = false;

            Description = Track.Name + "\n" +
                          "Track Length: " + Math.Round(Track.LengthKm, 1) + " km" + "\n" +
                          Car.Name + "\n" +
                          "Laps: " + Laps.ToString() + "\n" +
                          "Race Lenght: " + RaceLengthKm + " km" + "\n" +
                          "Top Speed: " + Car.TopSpeed + "\n" +
                          "Race Time: " + RaceLengthMinutes + " minutes" + "\n" +
                          "Prize: " + Prize;

        }


        /// <summary>
        /// Total laps defined by a max race running length in Kms divided the length of the track.
        /// If no length is provided, returns.
        /// </summary>
        internal int TotalLaps(double TopRaceLength, int seed)
        {
            if (Track.LengthKm > 0)
            {
                double factor = Utils.GetRandomNumber(0.60, 1.00, seed);

                double LengthRandomized = TopRaceLength * factor;
                return Convert.ToInt32(LengthRandomized / Track.LengthKm);
            }
            else return 0;
        }

    }
}
