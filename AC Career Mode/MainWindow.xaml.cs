using AC_Career_Mode.controls;
using DBLink.Classes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Numerics;
using System.Windows;
using System.Windows.Controls;
using Utilities;
#pragma warning disable CS8604 // Possible null reference argument.
#pragma warning disable CS8602 // Dereference of a possibly null reference.

/// TODO: Ideas
/// Have a track section where you can buy tracks and they generate profit over time, they require maintenance and improvements to generate profit
/// Have a "team" section, where you can hire drivers based on their skill. 
/// The more skill, the more races they'll win. You need to give them a car of your ownership. Races are time based (Next race in x hours)
/// Finance section, where you can take loans and repay them, view your spendings and earnings.
/// "History" section, with a log of your races, position and the races of your teammates

namespace AC_Career_Mode
{
    public partial class MainWindow : Window
    {

        public List<Track> TracksSource { get; set; }
        public List<Car> CarsSource { get; set; }
        public List<Race> RaceSource { get; set; }
        public List<Loan> LoanSource { get; set; }

        public List<Car> MarketCars { get; set; }
        public List<Track> MarketTracks { get; set; }

        public Player CurrentUser { get; set; }
        public Random RandomDaily { get; set; }


        public MainWindow(Player profile)
        {
            TracksSource = new List<Track>();
            CarsSource = new List<Car>();
            RaceSource = new List<Race>();
            LoanSource = new List<Loan>();

            MarketCars = new List<Car>();
            MarketTracks = new List<Track>();

            CurrentUser = profile;
            RandomDaily = new Random(Utils.TodaysSeed());


            InitializeComponent();


            uc_AvailableLoans.Loan_DoubleClick += new RoutedEventHandler(TakeLoan);
            uc_PlayerLoans.Loan_DoubleClick += new RoutedEventHandler(PayLoan);

            uc_MarketCars.BuySell_Click += new RoutedEventHandler(BuyCar);
            uc_MarketTracks.BuySell_Click += new RoutedEventHandler(BuyTrack);

            uc_PlayerTracks.Upgrade_Click += new RoutedEventHandler(UpgradeTrack);
            uc_PlayerTracks.BuySell_Click += new RoutedEventHandler(SellTrack);

            GetAvailableCarsAndTracks();
            UpdateAndRefreshPlayer(CurrentUser);
            PopulateRaceList(false);
            PopulateCarMarket(false);
            PopulateTrackMarket(false);
            PopulateLoans(false);

            
        }


        void UpgradeTrack(object sender, RoutedEventArgs e)
        {
            Track track = (Track)sender;
            if (track.Tier < 5)
            {
                Record.RecordUpgradeTrack(CurrentUser, track, track.GetUpgradeCost());
                track.Upgrade(CurrentUser);

                UpdateAndRefreshPlayer(CurrentUser);
            }
            else
            {
                Utils.Alert("", "Track fully upgraded");
            }
        }

        void SellTrack(object sender, RoutedEventArgs e)
        {
            Track track = (Track)sender;
            CurrentUser.Money += track.Price;

            Record.RecordSellTrack(CurrentUser, track);
            track.DeleteInDB();
            UpdateAndRefreshPlayer(CurrentUser);
        }

        void PlayerEquipCar(object sender, RoutedEventArgs e)
        {
            Car car = (Car)sender;
            CurrentUser.EquippedCarId = car.Id;
            UpdateAndRefreshPlayer(CurrentUser);
        }

        void GoRacing(object sender, EventArgs e)
        {
            Race race = sender as Race;

            if (CurrentUser.EquippedCarId == 0)
            {
                Utils.Alert("Warning!", "You don't have the required car equipped!");
                return;
            }

            Car EquippedCar = Car.LoadCar(CurrentUser.EquippedCarId);

            // Player doesn't have the needed car
            if (EquippedCar.Name != race.Car.Name)
            {
                Utils.Alert("Warning!", "You don't have the required car equipped!");
                return;
            }

            this.Hide();

            FinishedRace RaceResult = new(race);

            RaceResult.ShowDialog();

            if (RaceResult.Result != null)
            {
                // Add statistics to player
                CurrentUser.Money += RaceResult.Result.PrizeAwarded;
                CurrentUser.Races++;
                CurrentUser.KmsDriven += Convert.ToInt32(RaceResult.Result.KmsDriven);
                Car carUsed = Car.LoadCar(CurrentUser.EquippedCarId);
                carUsed.Kms += Convert.ToInt32(RaceResult.Result.KmsDriven);

                carUsed.UpdateInDB();

                if (RaceResult.Result.Position <= 3)
                {
                    CurrentUser.RacePodiums++;
                    if (RaceResult.Result.Position == 1)
                    {
                        CurrentUser.RaceWins++;
                    }
                }

                // Update race completed status and save to bin file
                int idx = RaceSource.IndexOf(race);
                race.Completed = true;
                RaceSource[idx] = race;

                Record.RecordRace(CurrentUser, RaceResult.Result);

                PopulateRaceList(true);
                UpdateAndRefreshPlayer(CurrentUser);
            }

            this.ShowDialog();

        }

        void TakeLoan(object sender, EventArgs e)
        {
            Loan loan = (Loan)sender;

            int idx = LoanSource.IndexOf(loan);
            LoanSource.RemoveAt(idx);

            loan.ExecuteLoan(CurrentUser);
            Record.RecordLoanExecute(CurrentUser, loan);

            PopulateLoans(true);
            UpdateAndRefreshPlayer(CurrentUser);
        }

        void PayLoan(object sender, EventArgs e)
        {
                Loan loan = (Loan)sender;

                loan.PayInstallment(CurrentUser);

                Record.RecordLoanPaid(CurrentUser, loan);
                UpdateAndRefreshPlayer(CurrentUser);
        }

        void BuyCar(object sender, RoutedEventArgs e)
        {
            Car car = (Car)sender;

            if (CurrentUser.Money >= car.Price)
            {
                CurrentUser.Money -= car.Price;

                int idx = MarketCars.IndexOf(car);
                MarketCars.RemoveAt(idx);

                car.OwnerId = CurrentUser.Id;
                car.ForSale = 0;
                car.InsertInDB();


                Record.RecordBuyCar(CurrentUser, car);
            }
            PopulateCarMarket(true);
            UpdateAndRefreshPlayer(CurrentUser);

        }

        void BuyTrack(object sender, RoutedEventArgs e)
        {
            Track track = (Track)sender;

            if (CurrentUser.Money >= track.Price)
            {
                CurrentUser.Money -= track.Price;

                int idx = MarketTracks.IndexOf(track);
                MarketTracks.RemoveAt(idx);

                track.OwnerId = CurrentUser.Id;
                track.InsertInDB();

                Record.RecordBuyTrack(CurrentUser, track);
            }
            PopulateTrackMarket(true);
            UpdateAndRefreshPlayer(CurrentUser);

        }

        void SellCar(object sender, RoutedEventArgs e)
        {
            Car car = (Car)sender;
            CurrentUser.Money += car.Price;
            car.DeleteInDB();

            Record.RecordSell(CurrentUser, car);

            PopulateCarMarket(true);
            UpdateAndRefreshPlayer(CurrentUser);
        }



    }
}
