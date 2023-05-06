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
            if (SerializeCurrent) Utils.Serialize(DailyCars, GlobalVars.DailyCarBin);

            lv_CarMarket.ItemsSource = null;
            DailyCars.Clear();

            // Using a seed so cars are changed daily
            for (int i = 0; i < 25; i++)
            {
                int index = RandomDaily.Next(CarMarketSource.Count);
                DailyCars.Add(CarMarketSource[index]);
            }

            CarMarketSource = Utils.ReadCreateBin(GlobalVars.DailyCarBin, CarMarketSource);

            List<Car> CarsForSale = Car.LoadForSaleCars();

            lv_CarMarket.ItemsSource = CarsForSale.Concat(DailyCars);
        }

        private void PopulateLoans(bool SerializeCurrent)
        {
            // Called when a loan was taken
            if (SerializeCurrent) Utils.Serialize(LoanSource, GlobalVars.DailyLoansBin);

            lv_LoansAvailable.ItemsSource = null;

            for (int i = 0; i < 10; i++)
            {
                LoanSource.Add(new Loan(i));
            }

            LoanSource = Utils.ReadCreateBin(GlobalVars.DailyLoansBin, LoanSource);

            lv_LoansAvailable.ItemsSource = LoanSource;
        }

        private void PopulateRaceList()
        {
            lv_RaceLst.ItemsSource = null;

            // Create 200 random races each day
            for (int i = 0; i < 200; i++)
            {
                Race race = Race.RaceFromList(AvailableTracks, CarMarketSource, i);
                RaceSource.Add(race);
            }

            RaceSource = Utils.ReadCreateBin(GlobalVars.RacesBin, RaceSource);

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

            /// Race track exceptions
            string[] InvalidTracks = { "drag", "drift", "indoor karting", "yatabe" };

            #region GET CARS AND TRACKS
#if RELEASE
            // GET ALL TRACKS AND CARS FROM CACHE IF FILE WAS MODIFIED TODAY
            if (File.Exists(GlobalVars.TracksBin) && DateTime.Today == File.GetLastWriteTime(GlobalVars.TracksBin).Date)
            {
                AvailableTracks = (List<Track>)Utils.Deserialize<List<Track>>(GlobalVars.TracksBin);
                AvailableCars = (List<Car>)Utils.Deserialize<List<Car>>(GlobalVars.CarsBin);
            }
#endif

#if DEBUG
            // Jump to else statement to create new files every time
            if (false)
            {
            }
#endif


            // CREATE CACHE FILE WITH AVAILABLE CARS AND TRACKS
            else
            {
                foreach (string ui_track in Directory.GetFiles(GlobalVars.TRACKS_PATH, "ui_track.json", SearchOption.AllDirectories))
                {
                    Track track = Track.LoadTrackJson(ui_track);

                    if (InvalidTracks.Any(InvalidTrack => track.Name.ToLower().Contains(InvalidTrack)))
                    { continue; }


                    // City tracks
                    if (track.Author != null && track.Author.Contains("Soyo"))
                    { continue; }

                    DirectoryInfo ui_folder = Directory.GetParent(ui_track);
                    string map_outline = ui_folder.ToString() + @"\" + "outline.png";
                    track.OutlinePath = map_outline;

                    AvailableTracks.Add(track);
                }

                foreach (string ui_car in Directory.GetFiles(GlobalVars.CARS_PATH, "ui_car.json", SearchOption.AllDirectories))
                {
                    Car car = Car.LoadCarJson(ui_car);
                    CarMarketSource.Add(car);
                }

                Utils.Serialize(AvailableTracks, GlobalVars.TracksBin);
                Utils.Serialize(CarMarketSource, GlobalVars.CarsBin);
            }

            #endregion
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
