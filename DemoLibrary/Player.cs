using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoLibrary
{
    public class Player
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Money { get; set; }
	    public int Loans { get; set; }
	    public int Races { get; set; }
	    public int RaceWins { get; set; }
	    public int RacePodiums { get; set; }
        public int KmsDriven { get; set; }
        public int EquippedCarId { get; set; }
    }
}
