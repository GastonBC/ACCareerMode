using Dapper;
using System.Data.SQLite;
using System.Windows;
using Utilities;

namespace DBLink
{
    public class Player
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Money { get; set; }
        public int Races { get; set; }
        public int RaceWins { get; set; }
        public int RacePodiums { get; set; }
        public int KmsDriven { get; set; }
        public int EquippedCarId { get; set; }



        // Load player from DB given an Id
        public static Player LoadPlayer(int Id)
        {
            using (SQLiteConnection cnn = new(SqliteDataAccess.LoadConnectionString()))
            {
                Player output = cnn.QuerySingleOrDefault<Player>($"select * from players where Id={Id}", new DynamicParameters());
                return output;
            }
        }

        public static List<Player> LoadAllPlayers()
        {
            return SqliteDataAccess.QueryByOwnerId<Player>("players");
        }

        public void PayDueLoans()
        {
            int paid = 0;
            foreach (Loan loan in GetPlayerLoans())
            {
                int dueInst = Utils.IsPaymentDue(loan.LastPaid, loan.BillingInterval);
                if (dueInst > 0)
                {
                    paid += loan.Installment * dueInst;
                    loan.PayInstallment(this, dueInst);
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
                int dueInst = Utils.IsPaymentDue(track.LastPaid, track.RevenueInterval);
                if (dueInst > 0)
                {
                    paid += track.Revenue * dueInst;

                    track.PayRevenue(this, dueInst);
                }
            }

            if (paid > 0)
            {
                Utils.Alert("Loans", $"Received {paid} in track revenue");
            }
        }

        public static Player Insert(Player pName)
        {
            int money = 100000000;
#if !DEBUG
            money = 50000;
#endif

            string cmd = $"INSERT INTO players (Name, Money) VALUES ('{pName.Name}', {money})";
            int id = SqliteDataAccess.ExecCmd(cmd);

            return LoadPlayer(id);

        }





        public void UpdateInDB()
        {
            using (SQLiteConnection cnn = new(SqliteDataAccess.LoadConnectionString()))
            {
                cnn.Open();
                string update_record = ($"UPDATE players SET " +
                    $"Money='{Money}', " +
                    $"Races='{Races}', " +
                    $"RaceWins='{RaceWins}', " +
                    $"RacePodiums='{RacePodiums}', " +
                    $"KmsDriven='{KmsDriven}', " +
                    $"EquippedCarId='{EquippedCarId}' " +
                    $"WHERE Id='{Id}'");

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

        public bool HasPlayerEnoughMoney(int price)
        {
            if (this.Money >= price)
            {
                return true;
            }
            return false;
        }

    }
}