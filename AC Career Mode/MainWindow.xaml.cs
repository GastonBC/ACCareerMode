using DemoLibrary;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;

#pragma warning disable CS8604 // Possible null reference argument.
#pragma warning disable CS8602 // Dereference of a possibly null reference.

/// TODO: Ideas
/// Have a track section where you can buy tracks and they generate profit over time, they require maintenance and improvements to generate profit
/// Have a "team" section, where you can hire drivers based on their skill. 
/// The more skill, the more races they'll win. You need to give them a car of your ownership. Races are time based (Next race in x hours)
/// Finance section, where you can take loans and repay them, view your spendings and earnings.
/// "History" section, with a log of your races, position and the races of your teammates

/// TODO: marketplace cars need to be serialized so when you buy one it disappears from that list
/// TODO: prices per car type, maybe a json file with middle prices. ie Formula Hybrid price is a random around 1.500.000
/// TODO: Format numbers in tables
/// BUG: Pressing a column header to sort will crash the window (only columns with numbers work)
/// BUG: Some races require 1000nds of laps. This might be due to lacking information from AC (track length, car top speed)



namespace AC_Career_Mode
{
    public partial class MainWindow : Window
    {
        List<Car> ForSale_Cars = new();

        List<Track> AvailableTracks = new();
        List<Car> AvailableCars = new();

        List<Race> AllRaces = new();
        

        Player CurrentUser;

        Random random = new(Utils.TodaysSeed());

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
        private void selection_changed(object sender, SelectionChangedEventArgs e)
        {
            if (lv_RaceLst.SelectedItem == null)
            {
                return;
            }
                b_goracing.IsEnabled = true;
                Race rc = lv_RaceLst.SelectedItem as Race;


                track_background.Source = Utils.RetriveImage(rc.Track.PreviewPath);

                track_preview.Source = Utils.RetriveImage(rc.Track.OutlinePath);
                car_preview.Source = Utils.RetriveImage(rc.Car.Preview);
        }

        private void b_goracing_Click(object sender, RoutedEventArgs e)
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

                RefreshRaceList();


                UpdateAndRefreshPlayer(CurrentUser);
            }

            
            this.ShowDialog();
        }

        private void chk_FilterRaces_click(object sender, RoutedEventArgs e)
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
        private void market_selection_changed(object sender, SelectionChangedEventArgs e)
        {
            if (lv_car_sale.SelectedItem != null)
            {
                b_BuyCar.IsEnabled = true;
                Car car = lv_car_sale.SelectedItem as Car;
                img_forsalecar.Source = Utils.RetriveImage(car.Preview);
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
                    selected_car.Owner = CurrentUser.Id;
                    selected_car.ForSale = 0;
                    CurrentUser.Money -= selected_car.Price;
                    // not yet in the database, insert new car in
                    if (selected_car.Id == null)
                    {
                        SqliteDataAccess.InsertCar(selected_car);
                    }

                    // else search the database and update the entry
                    else
                    {
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
