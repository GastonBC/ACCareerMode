using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBLink.Classes
{
    public class Driver
    {
        public string Name { get; set; }
        public int XP { get; set; }
        public int KmsDriven { get; set; }
        public int _IsAI { get; set; }
        public bool IsAI
        {
            get { return Convert.ToBoolean(_IsAI); }
            set { Convert.ToInt32(value); }
        }
    }
}
