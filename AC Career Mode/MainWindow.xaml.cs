using AC_Career_Mode.controls;
using DBLink;
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
        
        List<Track> TracksSource = new();
        List<Car> CarsSource = new();
        List<Race> RaceSource = new();
        List<Loan> LoanSource = new();

        List<Car> MarketCars = new();
        List<Track> MarketTracks = new();

        Player CurrentUser;

        Random RandomDaily = new(Utils.TodaysSeed());
        

        public MainWindow(Player profile)
        {
            CurrentUser = profile;

            InitializeComponent();
            uc_RaceTab.GoRacing_Click += new RoutedEventHandler(uc_RaceTab_GoRacing_Click);
            uc_AvailableLoans.Loan_DoubleClick += new RoutedEventHandler(uc_AvailableLoans_DoubleClick);
            uc_PlayerLoans.Loan_DoubleClick += new RoutedEventHandler(uc_PlayerLoans_DoubleClick);

            GetAvailableCarsAndTracks();
            UpdateAndRefreshPlayer(CurrentUser);
            PopulateRaceList(false);
            PopulateCarMarket(false);
            PopulateTrackMarket(false);
            PopulateLoans(false);

            

            //lb_test_itemcontrol.ItemsSource = MarketTracks;
        }



        void uc_RaceTab_GoRacing_Click(object sender, EventArgs e)
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


        // BUG: removeat is working but not updating the control
        void uc_AvailableLoans_DoubleClick(object sender, EventArgs e)
        {
            Loan loan = (Loan)sender;

            int idx = LoanSource.IndexOf(loan);
            LoanSource.RemoveAt(idx);

            loan.ExecuteLoan(CurrentUser);
            Record.RecordLoanExecute(CurrentUser, loan);

            PopulateLoans(true);
            UpdateAndRefreshPlayer(CurrentUser);
        }

        void uc_PlayerLoans_DoubleClick(object sender, EventArgs e)
        {
                Loan loan = (Loan)sender;

                loan.PayInstallment(CurrentUser);

                Record.RecordLoanPaid(CurrentUser, loan);
                UpdateAndRefreshPlayer(CurrentUser);
        }


        #region MARKET TAB
        private void MarketLv_SelChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lv_CarMarket.SelectedItem == null)
            {
                b_BuyCar.IsEnabled = false;
                Img_ForSaleCar.Source = null;
            }
            else
            {
                b_BuyCar.IsEnabled = true;
                Car car = lv_CarMarket.SelectedItem as Car;
                Img_ForSaleCar.Source = RetriveImage(car.Preview);
            }

            if (lv_TrackMarket.SelectedItem == null)
            {
                b_BuyTrack.IsEnabled = false;
                Img_ForSaleTrack.Source = null;
                Img_ForSaleTrackOutline.Source = null;
            }
            else
            {
                b_BuyTrack.IsEnabled = true;
                Track track = lv_TrackMarket.SelectedItem as Track;
                Img_ForSaleTrack.Source = RetriveImage(track.PreviewPath);
                Img_ForSaleTrackOutline.Source = RetriveImage(track.OutlinePath);
            }
        }

        private void b_BuyCar_Click(object sender, RoutedEventArgs e)
        {
            if (lv_CarMarket.SelectedItem != null)
            {
                Car car = lv_CarMarket.SelectedItem as Car;

                if (CurrentUser.HasPlayerEnoughMoney(car.Price))
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
        }

        private void b_BuyTrack_Click(object sender, RoutedEventArgs e)
        {
            if (lv_TrackMarket.SelectedItem != null)
            {
                Track track = lv_TrackMarket.SelectedItem as Track;

                if (CurrentUser.Money >= track.Price)
                {
                    CurrentUser.Money -= track.Price;

                    // Car comes from DailyCar list (bin object)
                    // Remove the car from DailyCar, serialize the list
                    // Insert to database
                    
                    int idx = MarketTracks.IndexOf(track);
                    MarketTracks.RemoveAt(idx);

                    track.OwnerId = CurrentUser.Id;
                    track.InsertInDB();

                    Record.RecordBuyTrack(CurrentUser, track);
                }
                PopulateTrackMarket(true);
                UpdateAndRefreshPlayer(CurrentUser);
            }
        }

        #endregion



        #region PROFILE TAB

        private void OwnedCars_SelChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lv_OwnedCar.SelectedItem != null)
            {
                b_SellCar.IsEnabled = true;
                Car car = lv_OwnedCar.SelectedItem as Car;
                img_OwnedCar.Source = RetriveImage(car.Preview);
            }
            else
            {
                b_SellCar.IsEnabled = false;
                img_OwnedCar.Source = null;
            }
        }

        private void b_SellCar_Click(object sender, RoutedEventArgs e)
        {
            if (lv_OwnedCar.SelectedItem != null)
            {
                Car selected_car = lv_OwnedCar.SelectedItem as Car;

                CurrentUser.Money += selected_car.Price;
                selected_car.DeleteInDB();

                Record.RecordSell(CurrentUser, selected_car);

                PopulateCarMarket(true);
                UpdateAndRefreshPlayer(CurrentUser);
            }
                
            
        }
        private void lv_OwnedCars_DoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (lv_OwnedCar.SelectedItem != null)
            {
                Car SelCar = lv_OwnedCar.SelectedItem as Car;
                CurrentUser.EquippedCarId = SelCar.Id;

                UpdateAndRefreshPlayer(CurrentUser);
            }
        }


        #endregion



    }
}
