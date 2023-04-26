
using DemoLibrary;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;


namespace AC_Career_Mode
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        const string TRACKS_PATH = @"C:\Program Files (x86)\Steam\steamapps\common\assettocorsa\content\tracks";
        const string CARS_PATH = @"C:\Program Files (x86)\Steam\steamapps\common\assettocorsa\content\cars";
        List<Track> AvailableTracks = new();
        List<Car> AvailableCars = new();
        List<Race> races = new();

        const string SavedTracks = "Tracks.bin";
        const string SavedCars = "Cars.bin";
        Player Profile = null;

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
            for (int i = 0; i < 5; i++)
            {
                Array race_types = Enum.GetValues(typeof(RaceGroup));

                RaceGroup random_race_group = (RaceGroup)race_types.GetValue(random.Next(race_types.Length));

                Race Race = new(RaceType.Short, AvailableTracks, AvailableCars, random_race_group);

                races.Add(Race);
                WireUpRaceList();
            }

            #endregion

            #region Profile Tab

            tbl_ProfileDescription.Text = $"Name: {profile.Name}\n" +
                                            $"Money: {profile.Money}\n" +
                                            $"Kilometers Driven: {profile.KmsDriven}\n";



            #endregion
        }



        private void WireUpRaceList()
        {
            lb_RaceLst.ItemsSource = null;
            lb_RaceLst.ItemsSource = races;
            lb_RaceLst.DisplayMemberPath = "DisplayName";

        }


        #region RACE TAB
        private void selection_changed(object sender, SelectionChangedEventArgs e)
        {
            b_goracing.IsEnabled = true;
            Race rc = lb_RaceLst.SelectedItem as Race;
            tb_RaceDetails.Text = rc.Description;

        }

        private void b_goracing_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            Race race = lb_RaceLst.SelectedItem as Race;
            FinishedRace race_dialog = new(race);
            race_dialog.ShowDialog();

            if (race_dialog.Result != null)
            {
                // Add statistics to player
                Profile.Money += (int)race_dialog.Result.PrizeAwarded;
                Profile.Races++;
                Profile.KmsDriven += (int)race_dialog.Result.KmsDriven;
                
                if (race_dialog.Result.Position <= 3)
                {
                    Profile.RacePodiums++;
                    if (race_dialog.Result.Position == 1)
                    {
                        Profile.RaceWins++;
                    }
                }
            }

            SqliteDataAccess.UpdatePlayer(Profile);

            tbl_ProfileDescription.Text = $"Name: {Profile.Name}\n" +
                                $"Money: {Profile.Money}\n" +
                                $"Kilometers Driven: {Profile.KmsDriven}\n";
            this.ShowDialog();

        }

        #endregion


    }
}
