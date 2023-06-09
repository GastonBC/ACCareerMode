﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AC_Career_Mode
{
    public class RaceResult
    {
        public Race Race { get; set; }
        public int LapsDriven { get; set; }
        public double KmsDriven { get; set; }
        public int PrizeAwarded { get; set; }
        public int Position { get; set; }

        public RaceResult(Race race, int position, int laps)
        {
            Race = race;
            LapsDriven = laps;
            KmsDriven = laps * race.Track.LengthKm;
            Position = position;

            /// Prize tiers
            /// 1st -           Full prize
            /// 2nd -           70%
            /// 3rd -           50%
            /// 4th to 10 -     20%
            /// 11 to last -    5%


            if (Position == 1)
            {
                PrizeAwarded = Race.Prize;
            }
            else if (Position == 2)
            {
                PrizeAwarded = Convert.ToInt32(Race.Prize * 0.7);
            }
            else if (position == 3)
            {
                PrizeAwarded = Convert.ToInt32(Race.Prize * 0.5);
            }
            else if (position >= 4 && position <= 10)
            {
                PrizeAwarded = Convert.ToInt32(race.Prize * 0.2);
            }
            else
            {
                PrizeAwarded = Convert.ToInt32(Race.Prize * 0.05);
            }
        }
    }
}
