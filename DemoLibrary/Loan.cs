using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Numerics;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace DBLink
{
    // Loans are generated each day randomly and stored to db only once the player selected it
    
    public class Loan
    {
        public int Id { get; set; }
        public int OwnerId { get; set; }
        public int AmountLeft { get; set; }
        public double InterestRate { get; set; }

        // Dates in DB are stored as UNIX (int) and converted to string YYYY-MM-DD
        private long _LastPaid { get; set; }
        public DateTime LastPaid
        {
            get { return Utils.UnixTimeStampToDateTime(_LastPaid); }
            set { _LastPaid = ((DateTimeOffset)value).ToUnixTimeSeconds(); }
        }
        public int BillingInterval { get; set; }
        public int Installment { get; set; }
        

        /// <summary>
        /// Creates a random loan, depending on the day. Used to create daily loans not stored in db
        /// Loan value is between 10.000 and 3.000.000
        /// </summary>
        public Loan(int seed)
        {
            int today_seed = Utils.TodaysSeed() + seed;
            Random rd = new Random(today_seed);

            Id = 0; 
            OwnerId = 0;
            AmountLeft = Utils.RoundX(rd.Next(10000, 3000000), 1000);
            InterestRate = Utils.RoundX(Utils.GetRandomNumber(5, 45, today_seed),5);
            LastPaid = DateTime.Today;
            BillingInterval = rd.Next(3, 10);
            Installment = Utils.RoundX(AmountLeft * 0.05, 1);
        }



        /// <summary>
        /// Loads all loans from DB given an owner
        /// </summary>
        public static List<Loan> GetPlayerLoans(Player player)
        {
            List<Loan> loans = new();

            using (SQLiteConnection cnn = new(SqliteDataAccess.LoadConnectionString()))
            {
                IEnumerable<Loan> output = cnn.Query<Loan>($"SELECT * FROM loans where OwnerId={player.Id}", new DynamicParameters());

                return output.ToList();
            }
        }


        public void PayInstallment(Loan loan, Player player)
        {
            if (player.Money >= loan.Installment)
            {
                player.Money -= loan.Installment;
                loan.AmountLeft -= loan.Installment;

                Player.UpdatePlayer(player);
                
            }
        }


        public void UpdateLoan(Loan loan)
        {
            using (SQLiteConnection cnn = new(SqliteDataAccess.LoadConnectionString()))
            {
                cnn.Open();
                string update_record = ($"UPDATE players SET " +
                    $"AmountLeft='{loan.AmountLeft}', " +
                    $"_LastPaid='{loan._LastPaid}' " +
                    $"WHERE Id='{loan.Id}'");

                SQLiteCommand command = new(update_record, cnn);
                command.ExecuteNonQuery();
                cnn.Close();
            }
            return;
        }

        public bool IsInstallmentDue(Loan loan)
        {
            DateTime today = DateTime.Today;

            // ie 25th - 13th = 12 days
            // loan interval must be smaller than 12
            int interval = (int)(today - loan.LastPaid).TotalDays;


            if (loan.BillingInterval < interval)
            {
                return true;
            }
            return false;
        }



        public void ExecuteLoan(Loan loan, Player player)
        {
            int interest = Convert.ToInt32(loan.AmountLeft * (loan.InterestRate / 100));
            loan.AmountLeft += interest;
            loan.OwnerId = player.Id;
            InsertLoan(loan);
        }

        // Inserts the loan executed by the player into db
        public void InsertLoan(Loan loan) 
        {
            using (SQLiteConnection cnn = new(SqliteDataAccess.LoadConnectionString()))
            {
                string cmd = "INSERT INTO LOANS (" +
                                "OwnerId, " +
                                "AmountLeft, " +
                                "InterestRate, " +
                                "_LastPaid, " +
                                "BillingInterval, " +
                                "Installment" +
                                ") VALUES (" +
                                "@OwnerId, " +
                                "@AmountLeft, " +
                                "@InterestRate, " +
                                "@_LastPaid, " +
                                "@BillingInterval, " +
                                "@Installment)";

                cnn.Open();
                cnn.Execute(cmd, loan);
                long loan_id = cnn.LastInsertRowId;
                cnn.Close();
            }
        }

    }
}
