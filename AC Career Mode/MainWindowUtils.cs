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

/// This file contains some population functions for the main window
/// Originally it was all in the same file but it cluttered reading
/// So now instead, it calls these functions
/// Most don't have a return because they interact with variables directly in the other file (at the top)

namespace AC_Career_Mode
{


    public partial class MainWindow : Window
    {
        /// <summary>
        /// Serializes the dailycar list, loads it and joins with DB cars for sale, then 
        /// </summary>
        public void RefreshMarket()
        {
            Utils.Serialize(DailyCars, GlobalVars.SavedMarket);
            PopulateMarketList();
        }

        private void PopulateMarketList()
        {
            lv_car_sale.ItemsSource = null;
            DailyCars.Clear();
            // GET FROM CACHE IF FILE WAS MODIFIED TODAY
            if (File.Exists(GlobalVars.SavedMarket) && DateTime.Today == File.GetLastWriteTime(GlobalVars.SavedMarket).Date)
            {
                DailyCars = (List<Car>)Utils.Deserialize(GlobalVars.SavedMarket);
            }

            // CREATE CACHE FILE WITH AVAILABLE RACES
            else
            {
                // Using a seed so cars are changed daily
                for (int i = 0; i < 25; i++)
                {
                    int index = RandomDaily.Next(AvailableCars.Count);
                    DailyCars.Add(AvailableCars[index]);
                }

                Utils.Serialize(DailyCars, GlobalVars.SavedMarket);
            }

            List<Car> CarsForSale = SqliteDataAccess.LoadForSaleCars();

            lv_car_sale.ItemsSource = CarsForSale.Concat(DailyCars);
        }

        private void PopulateRaceList()
        {

            // GET FROM CACHE IF FILE WAS MODIFIED TODAY
            if (File.Exists(GlobalVars.SavedRaces) && DateTime.Today == File.GetLastWriteTime(GlobalVars.SavedRaces).Date)
            {
                AllRaces = (List<Race>)Utils.Deserialize(GlobalVars.SavedRaces);
            }

            // CREATE CACHE FILE WITH AVAILABLE RACES
            else
            {
                // Create 100 random races each day
                for (int i = 0; i < 400; i++)
                {
                    Array race_groups = Enum.GetValues(typeof(RaceGroup));
                    Array race_types = Enum.GetValues(typeof(RaceLength));

                    RaceGroup random_race_group = (RaceGroup)race_groups.GetValue(RandomDaily.Next(race_groups.Length));
                    RaceLength random_race_type = (RaceLength)race_types.GetValue(RandomDaily.Next(race_types.Length));


                    Race Race = new(random_race_type, AvailableTracks, AvailableCars, random_race_group, i);

                    AllRaces.Add(Race);
                }

                Utils.Serialize(AllRaces, GlobalVars.SavedRaces);
            }
            RefreshRaceList();
        }
        private void RefreshRaceList()
        {
            lv_RaceLst.ItemsSource = null;
            //lv_RaceLst.ItemsSource = AllRaces.Where(rc => rc.Completed == false);
            lv_RaceLst.ItemsSource = AllRaces;
        }

        private void LoadDialogUserDetails(Player profile)
        {
            Player profile_ = SqliteDataAccess.LoadPlayer(profile.Id);
            toplabel_User.Content = profile_.Name;
            toplabel_Money.Content = profile_.Money.ToString("##,#");
            toplabel_Wins.Content = $"🏆 {profile_.RaceWins}";
            toplabel_Races.Content = $"Races: {profile_.Races}";

            // DB returns null as 0
            if (profile_.EquippedCarId != 0)
            {
                toplabel_EquippedCar.Content = SqliteDataAccess.LoadCar(profile_.EquippedCarId).Name;
            }
            else
            {
                toplabel_EquippedCar.Content = "";
            }

            List<Car> owned_cars = SqliteDataAccess.GetPlayerCars(CurrentUser);
            lv_OwnedCar.ItemsSource = null;
            lv_OwnedCar.ItemsSource = owned_cars;
        }


        GridViewColumnHeader _lastHeaderClicked = null;
        ListSortDirection _lastDirection = ListSortDirection.Ascending;

        void GridViewColumnHeaderClickedHandler(object sender, RoutedEventArgs e)
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

                    Sort(sortBy, direction);

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

        private void Sort(string sortBy, ListSortDirection direction)
        {
            ICollectionView dataView =
              CollectionViewSource.GetDefaultView(lv_RaceLst.ItemsSource);

            dataView.SortDescriptions.Clear();
            SortDescription sd = new SortDescription(sortBy, direction);
            dataView.SortDescriptions.Add(sd);
            dataView.Refresh();
        }

        private void UpdateAndRefreshPlayer(Player profile)
        {
            SqliteDataAccess.UpdatePlayer(profile);
            LoadDialogUserDetails(profile);
        }

        private void GetAvailableCarsAndTracks()
        {

            /// Race track exceptions
            string[] InvalidTracks = { "drag", "drift", "indoor karting", "yatabe" };

            #region GET CARS AND TRACKS
#if RELEASE
            // GET ALL TRACKS AND CARS FROM CACHE IF FILE WAS MODIFIED TODAY
            if (File.Exists(GlobalVars.SavedTracks) && DateTime.Today == File.GetLastWriteTime(GlobalVars.SavedTracks).Date)
            {
                AvailableTracks = (List<Track>)Utils.Deserialize(GlobalVars.SavedTracks);
                AvailableCars = (List<Car>)Utils.Deserialize(GlobalVars.SavedCars);
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
                    AvailableCars.Add(car);
                }

                Utils.Serialize(AvailableTracks, GlobalVars.SavedTracks);
                Utils.Serialize(AvailableCars, GlobalVars.SavedCars);
            }

            #endregion
        }


        private bool HasPlayerEnoughMoney(Player player, int price)
        {
            if (player.Money >= price)
            {
                return true;
            }
            return false;
        }
    }
}
