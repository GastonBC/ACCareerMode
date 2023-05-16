using Dapper;
using System.Data.SQLite;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using Utilities;

namespace DBLink.Classes
{
    public class Player : Driver
    {
        
        public int Money { get; set; }


        public Player() { }

        public Player(string name)
        {
            Name = name;
            Money = 50000000;
#if RELEASE
            Money = 50000;
#endif
            IsAI = false;

            Id = this.InsertInDB().Id;
        }


        // Load player from DB given an Id
        public static Player LoadFromDB(int id)
        {
            return SqliteDataAccess.QuerySingleById<Player>(id, "drivers"); ;
        }

        public Player InsertInDB()
        {
            string cmd = $"INSERT INTO drivers (Name, Money, _IsAI) VALUES ('{Name}', {Money}, {_IsAI})";
            int id = SqliteDataAccess.ExecCmd(cmd);

            return LoadFromDB(id) as Player;
        }

        public static List<Player> LoadAllPlayers()
        {
            using (SQLiteConnection cnn = new(SqliteDataAccess.LoadConnectionString()))
            {
                List<Player> output = cnn.Query<Player>($"SELECT * FROM drivers WHERE _IsAI=0").ToList();
                return output;
            }
        }

        public void PayDueLoans()
        {
            int paid = 0;
            foreach (Loan loan in GetPlayerLoans())
            {
                int multiplier = Utils.IsPaymentDue(loan.LastPaid, loan.BillingInterval);
                if (multiplier > 0)
                {
                    paid += loan.Installment * multiplier;
                    loan.PayInstallment(this, multiplier);
                }
            }

            if (paid > 0)
            {
                Utils.Alert("Loans", $"Paid {paid} in loans");
            }
        }

        public void PayRevenue()
        {
            int paid = 0;
            foreach (Track track in GetPlayerTracks())
            {
                int multiplier = Utils.IsPaymentDue(track.LastPaid, track.RevenueInterval);
                if (multiplier > 0)
                {
                    paid += track.Revenue * multiplier;

                    track.PayRevenue(this, multiplier);
                }
            }

            if (paid > 0)
            {
                Utils.Alert("Loans", $"Received {paid} in track revenue");
            }
        }



        public void UpdateInDB()
        {
            base.UpdateInDB();

            string update_record = $"UPDATE drivers SET Money='{Money}' WHERE Id='{Id}'";

            SqliteDataAccess.ExecCmd(update_record);
            return;
        }

        public List<Car> GetPlayerCars()
        {
            return SqliteDataAccess.QueryMultipleByOwnerId<Car>("garage", Id);
        }

        public List<Loan> GetPlayerLoans()
        {
            return SqliteDataAccess.QueryMultipleByOwnerId<Loan>("loans", Id);
        }

        public List<Track> GetPlayerTracks()
        {
            return SqliteDataAccess.QueryMultipleByOwnerId<Track>("tracks", Id);
        }


    }
}