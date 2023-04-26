using System;
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
        public double PrizeAwarded { get; set; }
        public int Position { get; set; }

        public RaceResult(Race race, int position, int laps)
        {
            Race = race;
            LapsDriven = laps;
            KmsDriven = laps*race.Track.LengthKm;
            Position = position;

        /// Prize tiers
        /// 1st -           Full prize
        /// 2nd -           70%
        /// 3rd -           50%
        /// 4th to 10 -     20%
        /// 11 to last -    5%

            switch (Position)
                {
                case 1:
                    PrizeAwarded = race.Prize;
                    break;
                case 2:
                    PrizeAwarded = Math.Round(race.Prize*0.7,1);
                    break;
                case 3:
                    Math.Round(race.Prize * 0.5, 1);
                    break;
                case var _ when (Position >= 3 && Position < 10):
                    Math.Round(race.Prize * 0.2, 1);
                    break;
                case var _ when (Position >= 11):
                    Math.Round(race.Prize * 0.05, 1);
                    break;



            }


        }




    }
}
