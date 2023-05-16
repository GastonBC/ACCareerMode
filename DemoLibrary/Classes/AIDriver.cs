using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBLink.Classes
{
    public class AIDriver : Driver
    {
        public AIDriver(string name) 
        { 
            Name = name;
            KmsDriven = 0;
            XP = 0;
            IsAI = true;
        }
    }
}
