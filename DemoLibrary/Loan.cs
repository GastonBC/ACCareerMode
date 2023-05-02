﻿using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Numerics;
using System.Runtime.ConstrainedExecution;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
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
            BillingInterval = 5;
            Installment = Utils.RoundX(AmountLeft * 0.05, 1);
        }

        public Loan LoadLoan(int id)
        {
            if (id != 0)
            {
                using (SQLiteConnection cnn = new(SqliteDataAccess.LoadConnectionString()))
                {
                    Loan output = cnn.QuerySingleOrDefault<Loan>($"SELECT * FROM loans WHERE Id={id}", new DynamicParameters());
                    return output;
                }
            }
            else
            {
                throw new Exception("No id provided");
            }
        }

        public void PayInstallment(Player player)
        {
            if (player.Money >= Installment)
            {
                player.Money -= Installment;
                AmountLeft -= Installment;
                LastPaid = DateTime.Today;

                player.UpdateInDB();
                UpdateInDB();


            }
        }

        /// <summary>
        /// Updates the loan in the database
        /// </summary>
        public void UpdateInDB()
        {
            using (SQLiteConnection cnn = new(SqliteDataAccess.LoadConnectionString()))
            {
                cnn.Open();
                string update_record = ($"UPDATE loans SET " +
                    $"AmountLeft={AmountLeft}, " +
                    $"_LastPaid={_LastPaid} " +
                    $"WHERE Id={Id}");

                cnn.Execute(update_record);
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

            Id = InsertInDB().Id;
        }

        // Inserts the loan executed by the player into db. Only allowed if Id = 0 (non existent in db)
        public Loan InsertInDB()
        {
            if (this.Id != 0)
            {
                throw new InvalidOperationException("New loan alredy has an id");
            }

            using (SQLiteConnection cnn = new(SqliteDataAccess.LoadConnectionString()))
            {
                cnn.Open();
                string update_record = $"INSERT INTO loans (OwnerId, AmountLeft, InterestRate, _LastPaid, BillingInterval, Installment) VALUES ({OwnerId}, {AmountLeft}, {InterestRate}, {_LastPaid}, {BillingInterval}, {Installment})";

                cnn.Execute(update_record);
                int id = (int)cnn.LastInsertRowId;
                cnn.Close();

                return LoadLoan(id);

            }
        }

    }
}