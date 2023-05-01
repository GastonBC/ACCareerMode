using DBLink;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Utils = Utilities.Utilities;
using GlobalVars = Utilities.GlobalVariables;

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
        List<Car> DailyCars = new();

        List<Track> AvailableTracks = new();
        List<Car> AvailableCars = new();

        List<Race> AllRaces = new();

        Player CurrentUser;

        Random RandomDaily = new(Utils.TodaysSeed());

        public MainWindow(Player profile)
        {
            CurrentUser = profile;

            InitializeComponent();

            GetAvailableCarsAndTracks();
            LoadDialogUserDetails(profile);
            PopulateRaceList();
            PopulateMarketList();
        }

        #region RACE TAB
        private void RaceLv_SelChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lv_RaceLst.SelectedItem == null)
            {
                b_GoRacing.IsEnabled = false;
                track_background.Source = null;
                track_preview.Source = null;
                car_preview.Source = null;
                return;
            }

            b_GoRacing.IsEnabled = true;
            Race SelectedRace = lv_RaceLst.SelectedItem as Race;

            track_background.Source = Utils.RetriveImage(SelectedRace.Track.PreviewPath);
            track_preview.Source = Utils.RetriveImage(SelectedRace.Track.OutlinePath);
            car_preview.Source = Utils.RetriveImage(SelectedRace.Car.Preview);
        }


        private void b_GoRacing_Click(object sender, RoutedEventArgs e)
        {
            Race race = lv_RaceLst.SelectedItem as Race;

            Car EquippedCar = SqliteDataAccess.LoadCar(CurrentUser.EquippedCarId);

            // Player doesn't have the needed car
            if (EquippedCar == null || EquippedCar.Name != race.Car.Name)
            {
                MessageBox.Show("You don't have the required car equipped!");
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
                Car carUsed = SqliteDataAccess.LoadCar(CurrentUser.EquippedCarId);
                carUsed.Kms += Convert.ToInt32(RaceResult.Result.KmsDriven);

                SqliteDataAccess.UpdateCar(carUsed);

                if (RaceResult.Result.Position <= 3)
                {
                    CurrentUser.RacePodiums++;
                    if (RaceResult.Result.Position == 1)
                    {
                        CurrentUser.RaceWins++;
                    }
                }

                // Update race completed status and save to bin file
                int idx = AllRaces.IndexOf(race);
                race.Completed = true;
                AllRaces[idx] = race;

                Utils.Serialize(AllRaces, GlobalVars.RacesBin);

                Record.RecordRace(CurrentUser, RaceResult.Result);

                RefreshRaceList();
                UpdateAndRefreshPlayer(CurrentUser);
            }

            this.ShowDialog();

        }

        private void chk_FilterRaces_Click(object sender, RoutedEventArgs e)
        {
            FilterRaces();
        }

        #endregion

        private void FilterRaces()
        {
            List<Race> FilteredRaces = new();

            if (chk_FilterRaces.IsChecked == true)
            {
                List<Car> owned_cars = SqliteDataAccess.GetPlayerCars(CurrentUser);

                IEnumerable<string>? names = AllRaces.Select(r => r.Car.Name).Intersect(owned_cars.Select(c => c.Name));
                List<Race> FilteredList = AllRaces.Where(r => names.Contains(r.Car.Name)).ToList();

                lv_RaceLst.ItemsSource = FilteredList;
            }
            else
            {
                lv_RaceLst.ItemsSource = AllRaces;
            }
        }


        #region MARKET TAB
        private void MarketLv_SelChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lv_CarMarket.SelectedItem == null)
            {
                b_BuyCar.IsEnabled = false;
                Img_ForSaleCar.Source = null;
                return;
            }

            b_BuyCar.IsEnabled = true;
            Car car = lv_CarMarket.SelectedItem as Car;
            Img_ForSaleCar.Source = Utilities.Utilities.RetriveImage(car.Preview);
        }

        private void b_BuyCar_Click(object sender, RoutedEventArgs e)
        {
            if (lv_CarMarket.SelectedItem != null)
            {
                Car selected_car = lv_CarMarket.SelectedItem as Car;

                if (HasPlayerEnoughMoney(CurrentUser, selected_car.Price))
                {
                    CurrentUser.Money -= selected_car.Price;

                    // Car comes from DailyCar list (bin object)
                    // Remove the car from DailyCar, serialize the list
                    // Insert to database
                    if (selected_car.Id == 0)
                    {
                        int idx = DailyCars.IndexOf(selected_car);
                        DailyCars.RemoveAt(idx);

                        selected_car.Owner = CurrentUser.Id;
                        selected_car.ForSale = 0;
                        SqliteDataAccess.InsertCar(selected_car);
                    }

                    // else search the database and update the entry
                    else
                    {
                        selected_car.Owner = CurrentUser.Id;
                        selected_car.ForSale = 0;
                        SqliteDataAccess.UpdateCar(selected_car);
                    }

                    Record.RecordBuy(CurrentUser, selected_car);
                }
                RefreshMarket();
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
                img_OwnedCar.Source = Utilities.Utilities.RetriveImage(car.Preview);
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

                selected_car.Owner = null;
                selected_car.ForSale = 1;
                CurrentUser.Money += selected_car.Price;
                SqliteDataAccess.UpdateCar(selected_car);

                Record.RecordSell(CurrentUser, selected_car);
                RefreshMarket();
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
