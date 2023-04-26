
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

#pragma warning disable CS8605 // Unboxing a possibly null value.

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private GridViewColumnHeader listViewSortCol;
        private SortAdorner listViewSortAdorner;

        const string TRACKS_PATH = @"C:\Program Files (x86)\Steam\steamapps\common\assettocorsa\content\tracks";
        const string CARS_PATH = @"C:\Program Files (x86)\Steam\steamapps\common\assettocorsa\content\cars";
        List<Track> AvailableTracks = new();
        List<Car> AvailableCars = new();
        List<Race> races = new();

        const string SavedTracks = "Tracks.bin";
        const string SavedCars = "Cars.bin";
        Player Profile;

        public MainWindow(Player profile)
        {
            Profile = profile;
            /// Race track exceptions
            string[] InvalidTracks = { "drag", "drift", "indoor karting", "yatabe" };

            InitializeComponent();

#if RELEASE
            // GET ALL TRACKS AND CARS FROM CACHE IF FILE WAS MODIFIED TODAY
            if (File.Exists(SavedTracks) && DateTime.Today == File.GetLastWriteTime(SavedTracks).Date)
            {
                AvailableTracks = (List<Track>)Utils.Deserialize(SavedTracks);
                AvailableCars = (List<Car>)Utils.Deserialize(SavedCars);
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
                foreach (string ui_track in Directory.GetFiles(TRACKS_PATH, "ui_track.json", SearchOption.AllDirectories))
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

                foreach (string ui_car in Directory.GetFiles(CARS_PATH, "ui_car.json", SearchOption.AllDirectories))
                {
                    Car car = Car.LoadCarJson(ui_car);
                    AvailableCars.Add(car);
                }

                Utils.Serialize(AvailableTracks, SavedTracks);
                Utils.Serialize(AvailableCars, SavedCars);
            }


            #region Race Tab
            Random random = new();
            for (int i = 0; i < 25; i++)
            {
                Array race_groups = Enum.GetValues(typeof(RaceGroup));
                Array race_types = Enum.GetValues(typeof(RaceType));

                RaceGroup random_race_group = (RaceGroup)race_groups.GetValue(random.Next(race_groups.Length));
                RaceType random_race_type = (RaceType)race_types.GetValue(random.Next(race_types.Length));


                Race Race = new(random_race_type, AvailableTracks, AvailableCars, random_race_group);

                races.Add(Race);

            }
            UpdateDialogUserDetails();
            WireUpRaceList();
            #endregion

            #region Profile Tab

            // CODE


            #endregion
        }



        


        #region RACE TAB
        private void selection_changed(object sender, SelectionChangedEventArgs e)
        {
            b_goracing.IsEnabled = true;
            Race rc = lv_RaceLst.SelectedItem as Race;
            tb_RaceDetails.Text = rc.Description;

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

        private void WireUpRaceList()
        {
            lv_RaceLst.ItemsSource = null;
            lv_RaceLst.ItemsSource = races;
            lv_RaceLst.DisplayMemberPath = "DisplayName";
        }

        #endregion


        private void UpdateDialogUserDetails()
        {
            Profile = SqliteDataAccess.LoadPlayer(Profile.Id);
            toplabel_User.Content = Profile.Name;
            toplabel_Money.Content = $"${Profile.Money}";
            toplabel_Wins.Content = $"🏆 {Profile.RaceWins}";
            toplabel_Races.Content = $"Races: {Profile.Races}";


            tbl_ProfileDescription.Text = $"Name: {Profile.Name}\n" +
                                          $"Money: {Profile.Money}\n" +
                                          $"Race Wins: {Profile.RacePodiums}\n" +
                                          $"Loans: {Profile.Loans}\n" +
                                          $"Race Podiums: {Profile.RacePodiums}\n" +
                                          $"Kilometers Driven: {Profile.KmsDriven}\n";
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

    }
}
