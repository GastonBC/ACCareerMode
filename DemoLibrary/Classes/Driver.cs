using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Diagnostics;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DBLink.Classes
{
    public class Driver
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int XP { get; set; }
        public int KmsDriven { get; set; }
        public int Races { get; set; }
        public int RaceWins { get; set; }
        public int RacePodiums { get; set; }
        public int EquippedCarId { get; set; }
        protected int _IsAI { get; set; }
        public bool IsAI
        {
            get { return Convert.ToBoolean(_IsAI); }
            set { Convert.ToInt32(value); }
        }


        


        protected void UpdateInDB()
        {
            string update_record = $"UPDATE drivers SET " +
                $"Races='{Races}', " +
                $"RaceWins='{RaceWins}', " +
                $"RacePodiums='{RacePodiums}', " +
                $"KmsDriven='{KmsDriven}', " +
                $"XP='{XP}', " +
                $"EquippedCarId='{EquippedCarId}' " +
                $"WHERE Id='{Id}'";

            SqliteDataAccess.ExecCmd(update_record);
            return;
        }



    }


}
