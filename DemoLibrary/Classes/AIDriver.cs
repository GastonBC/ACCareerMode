using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace DBLink.Classes
{
    public class AIDriver : Driver
    {
        public int Wage { get; set; }
        private long _LastPaid { get; set; }
        public DateTime LastPaid
        {
            get { return Utils.UnixTimeStampToDateTime(_LastPaid); }
            set { _LastPaid = ((DateTimeOffset)value).ToUnixTimeSeconds(); }
        }

        public AIDriver(string name) 
        { 
            Name = name;
            LastPaid = DateTime.Today;
            Wage = 5000;
            XP = 0;
            IsAI = true;
        }

        // Load player from DB given an Id
        public static AIDriver LoadFromDB(int id)
        {
            return SqliteDataAccess.QuerySingleById<AIDriver>(id, "drivers"); ;
        }


        public AIDriver InsertInDB()
        {
            string cmd = $"INSERT INTO drivers (Name, _LastPaid, Wage, _IsAI) VALUES ('{Name}', {_LastPaid}, {Wage}, {_IsAI})";
            int id = SqliteDataAccess.ExecCmd(cmd);

            return LoadFromDB(id) as AIDriver;
        }

        // Keeping this method in case because I will add properties exclusive to AIDriver
        public void UpdateInDB()
        {            
            base.UpdateInDB();

            string update_record = $"UPDATE drivers SET _LastPaid='{_LastPaid}' WHERE Id='{Id}'";

            SqliteDataAccess.ExecCmd(update_record);
            
            return;
        }
    }
}
