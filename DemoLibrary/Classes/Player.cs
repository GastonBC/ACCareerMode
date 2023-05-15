using Dapper;
using System.Data.SQLite;
using System.Windows;
using Utilities;

namespace DBLink.Classes
{
    public class Player : Driver
    {
        public int Id { get; set; }
        public int Money { get; set; }
        public int Races { get; set; }
        public int RaceWins { get; set; }
        public int RacePodiums { get; set; }
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

        public static Player InsertInDB(Player pName)
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
                string update_record = $"UPDATE players SET " +
                    $"Money='{Money}', " +
                    $"Races='{Races}', " +
                    $"RaceWins='{RaceWins}', " +
                    $"RacePodiums='{RacePodiums}', " +
                    $"KmsDriven='{KmsDriven}', " +
                    $"EquippedCarId='{EquippedCarId}' " +
                    $"WHERE Id='{Id}'";

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