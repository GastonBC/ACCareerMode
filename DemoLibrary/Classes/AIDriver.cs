using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace DBLink.Classes
{
    public class AIDriver : Driver
    {
        public AIDriver(string name) 
        { 
            Name = name;
            XP = 0;
            IsAI = true;
        }


        public AIDriver InsertInDB()
        {
            string cmd = $"INSERT INTO drivers (Name, _IsAI) VALUES ('{Name}',{_IsAI})";
            int id = SqliteDataAccess.ExecCmd(cmd);

            return LoadDriver(id) as AIDriver;
        }

        // Keeping this method in case because I will add properties exclusive to AIDriver
        public void UpdateInDB()
        {            
            base.UpdateInDB();
            return;
        }
    }
}
