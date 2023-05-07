using Dapper;
using ProtoBuf;
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

    [ProtoContract]
    public class Loan
    {
        [ProtoMember(1)]
        public int Id { get; set; }
        [ProtoMember(2)]
        public int OwnerId { get; set; }
        [ProtoMember(3)]
        public int AmountLeft { get; set; }
        [ProtoMember(4)]
        public double InterestRate { get; set; }

        // Dates in DB are stored as UNIX (int) and converted to string YYYY-MM-DD
        [ProtoMember(5)]
        private long _LastPaid { get; set; }
        [ProtoMember(6)]
        public DateTime LastPaid
        {
            get { return Utils.UnixTimeStampToDateTime(_LastPaid); }
            set { _LastPaid = ((DateTimeOffset)value).ToUnixTimeSeconds(); }
        }
        public int BillingInterval { get; set; }
        [ProtoMember(7)]
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




        public void PayInstallment(Player player, int multiplier = 1)
        {
            int amount = Installment * multiplier;
            if (player.Money >= amount)
            {
                player.Money -= amount;
                AmountLeft -= amount;
                LastPaid = DateTime.Today;

                player.UpdateInDB();

                // Loan fully paid. Delete from DB
                if(AmountLeft <= 0)
                {
                    DeleteInDB();
                    return;
                }
                
                // Loan is almost paid in full
                else if (AmountLeft < Installment)
                {
                    Installment = AmountLeft;
                }

                UpdateInDB();
            }


        }

        /// <summary>
        /// Updates the loan in the database
        /// </summary>
        public void UpdateInDB()
        {
            string cmd = ($"UPDATE loans SET " +
                $"AmountLeft={AmountLeft}, " +
                $"_LastPaid={_LastPaid} " +
                $"WHERE Id={Id}");

            SqliteDataAccess.ExecCmd(cmd);
        }

        public void DeleteInDB()
        {
            string cmd = ($"DELETE FROM loans WHERE Id={Id}");
            SqliteDataAccess.ExecCmd(cmd);
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
            string cmd = $"INSERT INTO loans (OwnerId, AmountLeft, InterestRate, _LastPaid, BillingInterval, Installment) VALUES " +
                                        $"({OwnerId}, {AmountLeft}, {InterestRate}, {_LastPaid}, {BillingInterval}, {Installment})";

            int id = SqliteDataAccess.ExecCmd(cmd);
            return LoadLoan(id);
        }

    }
}
