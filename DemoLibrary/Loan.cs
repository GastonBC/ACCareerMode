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


        public Loan() { }

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






        public void PayInstallment(Player player)
        {
            if (player.Money >= Installment)
            {
                player.Money -= Installment;
                AmountLeft -= Installment;
                LastPaid = DateTime.Today;

                Player.UpdatePlayer(player);
                UpdateLoan();


            }
        }

        /// <summary>
        /// Updates the loan in the database
        /// </summary>
        public void UpdateLoan()
        {
            using (SQLiteConnection cnn = new(SqliteDataAccess.LoadConnectionString()))
            {
                cnn.Open();
                string update_record = ($"UPDATE loans SET " +
                    $"AmountLeft={AmountLeft}, " +
                    $"_LastPaid={_LastPaid} " +
                    $"WHERE Id={Id}");

                SQLiteCommand command = new(update_record, cnn);
                command.ExecuteNonQuery();
                cnn.Close();
            }
            return;
        }

        public bool IsInstallmentDue()
        {
            DateTime today = DateTime.Today;

            // ie 25th - 13th = 12 days
            // loan interval must be smaller than 12
            int interval = (int)(today - this.LastPaid).TotalDays;


            if (this.BillingInterval < interval)
            {
                return true;
            }
            return false;
        }



        public void ExecuteLoan(Player player)
        {
            if (this.Id != 0)
            {
                throw new InvalidOperationException("New loan alredy has an id");
            }

            player.Money += AmountLeft;

            int interest = Convert.ToInt32(AmountLeft * (InterestRate / 100));
            AmountLeft += interest;
            OwnerId = player.Id;


            this.InsertLoan();
        }

        // Inserts the loan executed by the player into db. Only allowed if Id = 0 (non existent in db)
        public void InsertLoan()
        {
            if (this.Id != 0)
            {
                throw new InvalidOperationException("New loan alredy has an id");
            }

            using (SQLiteConnection cnn = new(SqliteDataAccess.LoadConnectionString()))
            {
                cnn.Open();
                string update_record = $"INSERT INTO loans (OwnerId, AmountLeft, InterestRate, _LastPaid, BillingInterval, Installment) VALUES ({OwnerId}, {AmountLeft}, {InterestRate}, {_LastPaid}, {BillingInterval}, {Installment})";

                SQLiteCommand command = new(update_record, cnn);
                command.ExecuteNonQuery();
                cnn.Close();

            }
        }

    }
}
