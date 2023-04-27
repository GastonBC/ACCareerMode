
using DemoLibrary;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;


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
        private GridViewColumnHeader listViewSortCol;
        private SortAdorner listViewSortAdorner;

        List<Car> ForSale_Cars = new();

        List<Track> AvailableTracks = new();
        List<Car> AvailableCars = new();
        
        Player Profile;

        Random random = new(Utils.TodaysSeed());

        public MainWindow(Player profile)
        {
            Profile = profile;

            InitializeComponent();

            GetAvailableCarsAndTracks();
            LoadDialogUserDetails();
            PopulateRaceList();
            PopulateMarketList();
        }

        #region RACE TAB
        private void selection_changed(object sender, SelectionChangedEventArgs e)
        {
            b_goracing.IsEnabled = true;
            Race rc = lv_RaceLst.SelectedItem as Race;

            track_background.Source = Utils.RetriveImage(rc.Track.PreviewPath);
            track_preview.Source = Utils.RetriveImage(rc.Track.OutlinePath);
            car_preview.Source = Utils.RetriveImage(rc.Car.Preview);

        }

        private void b_goracing_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            Race race = lv_RaceLst.SelectedItem as Race;
            FinishedRace race_dialog = new(race);
            race_dialog.ShowDialog();

            if (race_dialog.Result != null)
            {
                // Add statistics to player
                Profile.Money += race_dialog.Result.PrizeAwarded;
                Profile.Races++;
                Profile.KmsDriven += Convert.ToInt32(race_dialog.Result.KmsDriven);
                
                if (race_dialog.Result.Position <= 3)
                {
                    Profile.RacePodiums++;
                    if (race_dialog.Result.Position == 1)
                    {
                        Profile.RaceWins++;
                    }
                }
                SqliteDataAccess.UpdatePlayer(Profile);
                LoadDialogUserDetails();
            }

            
            this.ShowDialog();
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


        #endregion

        private bool HasPlayerEnoughMoney(Player player, int price)
        {
            if (player.Money >= price)
            {
                return true;
            }
            return false;
        }

        private void b_BuyCar_Click(object sender, RoutedEventArgs e)
        {
            if (lv_car_sale.SelectedItem != null)
            {
                Car selected_car = lv_car_sale.SelectedItem as Car;

                if (HasPlayerEnoughMoney(Profile, selected_car.Price))
                {
                    selected_car.Owner = Profile.Id;
                    selected_car.ForSale = false;
                    Profile.Money -= selected_car.Price;
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
                SqliteDataAccess.UpdatePlayer(Profile);
                LoadDialogUserDetails();
            }
        }
    }
}
