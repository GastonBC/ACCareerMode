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
        public uint LapsDriven { get; set; }
        public double KmsDriven { get; set; }
        public uint PrizeAwarded { get; set; }
        public uint Position { get; set; }

        public RaceResult(Race race, uint position, uint laps)
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




            switch (Position)
            {
                case 1:
                    PrizeAwarded = Race.Prize;
                    break;
                case 2:
                    PrizeAwarded = Convert.ToUInt32(Race.Prize * 0.7);
                    break;
                case 3:
                    PrizeAwarded = Convert.ToUInt32(Race.Prize * 0.5);
                    break;
                case >= 3 and < 10:
                    Convert.ToInt32(race.Prize * 0.2);
                    break;
                default:
                    PrizeAwarded = Convert.ToUInt32(Race.Prize * 0.05);
                    break;
            }
        }
    }
}
