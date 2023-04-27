
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
            UpdateDialogUserDetails();
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
                Profile.Money += (int)race_dialog.Result.PrizeAwarded;
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
                UpdateDialogUserDetails();
            }

            
            this.ShowDialog();
        }



        #endregion

        


        #region MARKET TAB
        private void market_selection_changed(object sender, SelectionChangedEventArgs e)
        {

        }
        #endregion


    }
}
