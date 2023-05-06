using DBLink;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Utilities;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Numerics;

#pragma warning disable CS8605 // Unboxing a possibly null value.

/// This file contains some population functions for the main window
/// Originally it was all in the same file but it cluttered reading
/// So now instead, it calls these functions
/// Most don't have a return because they interact with variables directly in the other file (at the top)

namespace AC_Career_Mode
{


    public partial class MainWindow : Window
    {
        #region POPULATE
        private void PopulateMarketList(bool SerializeCurrent)
        {
            // Called when a car was bought
            if (SerializeCurrent) Utils.ProtoSerialize(MarketCars, GlobalVars.DailyCarBin);

            lv_CarMarket.ItemsSource = null;
            MarketCars.Clear();

            // Using a seed so cars are changed daily
            for (int i = 0; i < 25; i++)
            {
                int index = RandomDaily.Next(CarsSource.Count);
                MarketCars.Add(CarsSource[index]);
            }

            CarsSource = Utils.ReadWriteBin(GlobalVars.DailyCarBin, CarsSource);

            List<Car> CarsForSale = Car.LoadForSaleCars();

            lv_CarMarket.ItemsSource = CarsForSale.Concat(MarketCars);
        }

        private void PopulateLoans(bool SerializeCurrent)
        {
            // Called when a loan was taken
            if (SerializeCurrent) Utils.ProtoSerialize(LoanSource, GlobalVars.DailyLoansBin);

            lv_LoansAvailable.ItemsSource = null;

            for (int i = 0; i < 10; i++)
            {
                LoanSource.Add(new Loan(i));
            }

            LoanSource = Utils.ReadWriteBin(GlobalVars.DailyLoansBin, LoanSource);

            lv_LoansAvailable.ItemsSource = LoanSource;
        }

        private void PopulateRaceList(bool SerializeCurrent)
        {
            // Called when a race was raced
            if (SerializeCurrent) Utils.ProtoSerialize(RaceSource, GlobalVars.RacesBin);

            lv_RaceLst.ItemsSource = null;

            // Create 200 random races each day
            for (int i = 0; i < 200; i++)
            {
                Race race = Race.RaceFromList(TracksSource, CarsSource, i);
                RaceSource.Add(race);
            }

            RaceSource = Utils.ReadWriteBin(GlobalVars.RacesBin, RaceSource);

            if (chk_FilterRaces.IsChecked == true)
            {
                FilterRaces();
            }
            else
            {
                lv_RaceLst.ItemsSource = RaceSource;
            }
        }

        private void UpdateAndRefreshPlayer(Player profile)
        {
            profile.PayDueLoans();
            profile.UpdateInDB();


            // Update UI
            profile = Player.LoadPlayer(profile.Id);
            toplabel_User.Content = profile.Name;
            toplabel_Money.Content = profile.Money.ToString("##,#");
            toplabel_Wins.Content = $"🏆 {profile.RaceWins}";
            toplabel_Races.Content = $"Races: {profile.Races}";

            lv_PlayerLoans.ItemsSource = profile.GetPlayerLoans();
            lv_HistoryRecords.ItemsSource = Record.DeserializeRecords(profile);

            // DB returns null as 0
            if (profile.EquippedCarId != 0)
            {
                toplabel_EquippedCar.Content = Car.LoadCar(profile.EquippedCarId).Name;
            }
            else
            {
                toplabel_EquippedCar.Content = "";
            }

            lv_OwnedCar.ItemsSource = null;
            lv_OwnedCar.ItemsSource = CurrentUser.GetPlayerCars();
        }
        #endregion

        #region RACES

        private void FilterRaces()
        {
            List<Race> FilteredRaces = new();

            if (chk_FilterRaces.IsChecked == true)
            {
                List<Car> owned_cars = CurrentUser.GetPlayerCars();

                IEnumerable<string>? names = RaceSource.Select(r => r.Car.Name).Intersect(owned_cars.Select(c => c.Name));
                List<Race> FilteredList = RaceSource.Where(r => names.Contains(r.Car.Name)).ToList();

                lv_RaceLst.ItemsSource = FilteredList;
            }
            else
            {
                lv_RaceLst.ItemsSource = RaceSource;
            }
        }

        private void GetAvailableCarsAndTracks()
        {
            TracksSource.Clear();
            CarsSource.Clear();

            /// Race track exceptions
            string[] InvalidTracks = { "drag", "drift", "indoor karting", "yatabe" };

            foreach (string ui_track in Directory.GetFiles(GlobalVars.TRACKS_PATH, "ui_track.json", SearchOption.AllDirectories))
            {
                Track track = Track.LoadTrackJson(ui_track);

                if (InvalidTracks.Any(InvalidTrack => track.Name.ToLower().Contains(InvalidTrack)))
                { continue; }


                DirectoryInfo ui_folder = Directory.GetParent(ui_track);
                track.OutlinePath = ui_folder.ToString() + @"\" + "outline.png";

                TracksSource.Add(track);
            }

            foreach (string ui_car in Directory.GetFiles(GlobalVars.CARS_PATH, "ui_car.json", SearchOption.AllDirectories))
            {
                Car car = Car.LoadCarJson(ui_car);
                CarsSource.Add(car);
            }

            TracksSource = Utils.ReadWriteBin(GlobalVars.TracksBin, TracksSource);
            CarsSource = Utils.ReadWriteBin(GlobalVars.CarsBin, CarsSource);
        }

        #endregion


        #region OTHER

        void RaceLv_Clicked(object sender, RoutedEventArgs e)
        {
            HeaderClickedHandler(sender, e, lv_RaceLst);
        }

        private void MarketCarsLv_Click(object sender, RoutedEventArgs e)
        {
            HeaderClickedHandler(sender, e, lv_CarMarket);
        }

        void OwnedCarsLv_Clicked(object sender, RoutedEventArgs e)
        {
            HeaderClickedHandler(sender, e, lv_OwnedCar);
        }

        GridViewColumnHeader _lastHeaderClicked = null;
        ListSortDirection _lastDirection = ListSortDirection.Ascending;

        void HeaderClickedHandler(object sender, RoutedEventArgs e, ListView lv)
        {
            GridViewColumnHeader? headerClicked = e.OriginalSource as GridViewColumnHeader;

            ListSortDirection direction;

            if (headerClicked != null)
            {
                if (headerClicked.Role != GridViewColumnHeaderRole.Padding)
                {
                    if (headerClicked != _lastHeaderClicked)
                    {
                        direction = ListSortDirection.Ascending;
                    }
                    else
                    {
                        if (_lastDirection == ListSortDirection.Ascending)
                        {
                            direction = ListSortDirection.Descending;
                        }
                        else
                        {
                            direction = ListSortDirection.Ascending;
                        }
                    }

                    var columnBinding = headerClicked.Column.DisplayMemberBinding as Binding;
                    var sortBy = columnBinding?.Path.Path ?? headerClicked.Column.Header as string;

                    Sort(sortBy, direction, lv);

                    if (direction == ListSortDirection.Ascending)
                    {
                        headerClicked.Column.HeaderTemplate =
                          Resources["HeaderTemplateArrowUp"] as DataTemplate;
                    }
                    else
                    {
                        headerClicked.Column.HeaderTemplate =
                          Resources["HeaderTemplateArrowDown"] as DataTemplate;
                    }

                    // Remove arrow from previously sorted header
                    if (_lastHeaderClicked != null && _lastHeaderClicked != headerClicked)
                    {
                        _lastHeaderClicked.Column.HeaderTemplate = null;
                    }

                    _lastHeaderClicked = headerClicked;
                    _lastDirection = direction;
                }
            }
        }

        private void Sort(string sortBy, ListSortDirection direction, ListView lv)
        {
            ICollectionView dataView =
              CollectionViewSource.GetDefaultView(lv.ItemsSource);

            dataView.SortDescriptions.Clear();
            SortDescription sd = new SortDescription(sortBy, direction);
            dataView.SortDescriptions.Add(sd);
            dataView.Refresh();
        }


        public static ImageSource? RetriveImage(string imagePath)
        {

            Uri myUri = new(imagePath, UriKind.Absolute);

            try
            {
                BitmapDecoder decoder = BitmapDecoder.Create(myUri, BitmapCreateOptions.IgnoreImageCache, BitmapCacheOption.None);
                return decoder.Frames[0];
            }
            catch (System.IO.FileNotFoundException)
            {
                return null;
            }
        }

        public static bool IsFileBelowThreshold(string filename, int hours)
        {
            var threshold = DateTime.Now.AddHours(-hours);
            return File.GetCreationTime(filename) >= threshold;
        }

        #endregion
    }

}
