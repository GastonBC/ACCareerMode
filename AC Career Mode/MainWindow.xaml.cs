using DemoLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

#pragma warning disable CS8604 // Possible null reference argument.
#pragma warning disable CS8602 // Dereference of a possibly null reference.

/// TODO: Ideas
/// Have a track section where you can buy tracks and they generate profit over time, they require maintenance and improvements to generate profit
/// Have a "team" section, where you can hire drivers based on their skill. 
/// The more skill, the more races they'll win. You need to give them a car of your ownership. Races are time based (Next race in x hours)
/// Finance section, where you can take loans and repay them, view your spendings and earnings.
/// "History" section, with a log of your races, position and the races of your teammates

/// BUG: Some races require 1000nds of laps. This might be due to lacking information from AC (track length, car top speed)



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
                return;
            }

            b_GoRacing.IsEnabled = true;
            Race rc = lv_RaceLst.SelectedItem as Race;


            track_background.Source = Utils.RetriveImage(rc.Track.PreviewPath);

            track_preview.Source = Utils.RetriveImage(rc.Track.OutlinePath);
            car_preview.Source = Utils.RetriveImage(rc.Car.Preview);
        }

        private void b_GoRacing_Click(object sender, RoutedEventArgs e)
        {
            
            Race race = lv_RaceLst.SelectedItem as Race;

            List<Car> PlayerCars = SqliteDataAccess.GetPlayerCars(CurrentUser);


            // Player doesn't have the needed car
            if (!PlayerCars.Any(pCar => pCar.Name == race.Car.Name))
            {
                MessageBox.Show("You don't have the required car!");
                return;
            }

            this.Hide();

            FinishedRace race_dialog = new(race);

            race_dialog.ShowDialog();

            if (race_dialog.Result != null)
            {
                // Add statistics to player
                CurrentUser.Money += race_dialog.Result.PrizeAwarded;
                CurrentUser.Races++;
                CurrentUser.KmsDriven += Convert.ToInt32(race_dialog.Result.KmsDriven);
                
                if (race_dialog.Result.Position <= 3)
                {
                    CurrentUser.RacePodiums++;
                    if (race_dialog.Result.Position == 1)
                    {
                        CurrentUser.RaceWins++;
                    }
                }

                // Update race completed status and save to bin file
                int idx = AllRaces.IndexOf(race);
                race.Completed = true;
                AllRaces[idx] = race;

                Utils.Serialize(AllRaces, GlobalVars.SavedRaces);

                // TODO: Add mileage to car

                RefreshRaceList();
                UpdateAndRefreshPlayer(CurrentUser);
            }

            
            this.ShowDialog();
        }

        private void chk_FilterRaces_Click(object sender, RoutedEventArgs e)
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


        #endregion




        #region MARKET TAB
        private void MarketLv_SelChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lv_car_sale.SelectedItem != null)
            {
                b_BuyCar.IsEnabled = true;
                Car car = lv_car_sale.SelectedItem as Car;
                Img_ForSaleCar.Source = Utils.RetriveImage(car.Preview);
            }
            else
            {
                b_BuyCar.IsEnabled = false;
            }
        }
        private void b_BuyCar_Click(object sender, RoutedEventArgs e)
        {
            if (lv_car_sale.SelectedItem != null)
            {
                Car selected_car = lv_car_sale.SelectedItem as Car;

                if (HasPlayerEnoughMoney(CurrentUser, selected_car.Price))
                {
                    

                    CurrentUser.Money -= selected_car.Price;

                    // Car comes from DailyCar list (bin object)
                    // Remove the car from DailyCar, serialize the list
                    // Insert to database
                    if (selected_car.Id == null)
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
                }
                RefreshMarket();
                UpdateAndRefreshPlayer(CurrentUser);
            }
        }

        #endregion



        #region PROFILE TAB

        private void OwnedCars_SelChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lv_owned_cars.SelectedItem != null)
            {
                b_SellCar.IsEnabled = true;
                Car car = lv_owned_cars.SelectedItem as Car;
                img_OwnedCar.Source = Utils.RetriveImage(car.Preview);
            }
            else
            {
                b_SellCar.IsEnabled = false;
            }
        }

        private void b_SellCar_Click(object sender, RoutedEventArgs e)
        {
            if (lv_owned_cars.SelectedItem != null)
            {
                Car selected_car = lv_owned_cars.SelectedItem as Car;

                selected_car.Owner = null;
                selected_car.ForSale = 1;
                CurrentUser.Money += selected_car.Price;


                SqliteDataAccess.UpdateCar(selected_car);
                RefreshMarket();
                UpdateAndRefreshPlayer(CurrentUser);
            }
                
            
        }

        #endregion


    }
}
