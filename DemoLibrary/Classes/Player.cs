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

            this.InsertInDB();
        }

        public Player InsertInDB()
        {
            string cmd = $"INSERT INTO drivers (Name, Money, _IsAI) VALUES ('{Name}', {Money}, {_IsAI})";
            int id = SqliteDataAccess.ExecCmd(cmd);

            return LoadDriver(id) as Player;
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

            using (SQLiteConnection cnn = new(SqliteDataAccess.LoadConnectionString()))
            {
                cnn.Open();
                string update_record = $"UPDATE drivers SET Money='{Money}' WHERE Id='{Id}'";

                SQLiteCommand command = new(update_record, cnn);
                command.ExecuteNonQuery();
                cnn.Close();
            }
            return;
        }

        public List<Car> GetPlayerCars()
        {
            return SqliteDataAccess.QueryByOwnerId<Car>("garage", Id);
        }

        public List<Loan> GetPlayerLoans()
        {
            return SqliteDataAccess.QueryByOwnerId<Loan>("loans", Id);
        }

        public List<Track> GetPlayerTracks()
        {
            return SqliteDataAccess.QueryByOwnerId<Track>("tracks", Id);
        }


    }
}