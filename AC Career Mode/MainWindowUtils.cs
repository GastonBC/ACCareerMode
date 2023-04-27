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

/// This file contains some population functions for the main window
/// Originally it was all in the same file but it cluttered reading
/// So now instead, it calls these functions
/// Most don't have a return because they interact with variables directly in the other file (at the top)

namespace AC_Career_Mode
{


    public partial class MainWindow : Window
    {


        private void PopulateMarketList()
        {

            // Using a seed so cars are changed daily

            for (int i = 0; i < 5; i++)
            {
                int index = random.Next(AvailableCars.Count);
                
                ForSale_Cars.Add(AvailableCars[index]);
            }

            lv_car_sale.ItemsSource = ForSale_Cars;
        }

        private void PopulateRaceList()
        {
            List<Race> races = new();
            // Create 100 random races each day

            for (int i = 0; i < 100; i++)
            {
                Array race_groups = Enum.GetValues(typeof(RaceGroup));
                Array race_types = Enum.GetValues(typeof(RaceLength));

                RaceGroup random_race_group = (RaceGroup)race_groups.GetValue(random.Next(race_groups.Length));
                RaceLength random_race_type = (RaceLength)race_types.GetValue(random.Next(race_types.Length));


                Race Race = new(random_race_type, AvailableTracks, AvailableCars, random_race_group, i);

                races.Add(Race);
            }

            lv_RaceLst.ItemsSource = null;
            lv_RaceLst.ItemsSource = races;
            lv_RaceLst.DisplayMemberPath = "DisplayName";
        }


        private void LoadDialogUserDetails(Player profile)
        {
            Player profile_ = SqliteDataAccess.LoadPlayer(profile.Id);
            toplabel_User.Content = profile_.Name;
            toplabel_Money.Content = $"${profile_.Money}";
            toplabel_Wins.Content = $"🏆 {profile_.RaceWins}";
            toplabel_Races.Content = $"Races: {profile_.Races}";


            List<Car> owned_cars = SqliteDataAccess.GetPlayerCars(CurrentUser);
            lv_owned_cars.ItemsSource = null;
            lv_owned_cars.ItemsSource = owned_cars;
        }

        private void ColHeader_Click(object sender, RoutedEventArgs e)
        {
            GridViewColumnHeader column = (sender as GridViewColumnHeader);
            string sortBy = column.Tag.ToString();
            if (listViewSortCol != null)
            {
                AdornerLayer.GetAdornerLayer(listViewSortCol).Remove(listViewSortAdorner);
                lv_RaceLst.Items.SortDescriptions.Clear();
            }

            ListSortDirection newDir = ListSortDirection.Ascending;
            if (listViewSortCol == column && listViewSortAdorner.Direction == newDir)
                newDir = ListSortDirection.Descending;

            listViewSortCol = column;
            listViewSortAdorner = new SortAdorner(listViewSortCol, newDir);
            AdornerLayer.GetAdornerLayer(listViewSortCol).Add(listViewSortAdorner);
            lv_RaceLst.Items.SortDescriptions.Add(new SortDescription(sortBy, newDir));
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
