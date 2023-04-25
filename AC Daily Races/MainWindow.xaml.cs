
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace AC_Daily_Races
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        const string TRACKS_PATH = @"C:\Program Files (x86)\Steam\steamapps\common\assettocorsa\content\tracks";
        const string CARS_PATH = @"C:\Program Files (x86)\Steam\steamapps\common\assettocorsa\content\cars";
        List<Track> AvailableTracks = new List<Track>();
        List<Car> AvailableCars = new List<Car>();

        const string SavedTracks = "Tracks.bin";
        const string SavedCars = "Cars.bin";


        public MainWindow()
        {

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

            /// GET 4 RANDOM AND DIFFERENT TRACKS AND CARS
            /// CREATE 4 RACES, ONE OF EACH TYPE WITH THE DAY AS THE SEED

            Race Race1 = new Race(RaceType.Short, AvailableTracks, AvailableCars, RaceGroup.GT);

            Race Race2 = new Race(RaceType.Medium, AvailableTracks, AvailableCars, RaceGroup.GT);

            Race Race3 = new Race(RaceType.Medium, AvailableTracks, AvailableCars, RaceGroup.Formula);

            Race Race4 = new Race(RaceType.Medium, AvailableTracks, AvailableCars, RaceGroup.F1);


            tb_1.Text = Race1.Description;
            tb_2.Text = Race2.Description;
            tb_3.Text = Race3.Description;
            tb_4.Text = Race4.Description;

            img_1.Source = Utils.RetriveImage(Race1.Track.OutlinePath);
            img_2.Source = Utils.RetriveImage(Race2.Track.OutlinePath);
            img_3.Source = Utils.RetriveImage(Race3.Track.OutlinePath);
            img_4.Source = Utils.RetriveImage(Race4.Track.OutlinePath);

            preview_1.Source = Utils.RetriveImage(Race1.Track.PreviewPath);
            preview_2.Source = Utils.RetriveImage(Race2.Track.PreviewPath);
            preview_3.Source = Utils.RetriveImage(Race3.Track.PreviewPath);
            preview_4.Source = Utils.RetriveImage(Race4.Track.PreviewPath);

            car_preview_1.Source = Utils.RetriveImage(Race1.Car.Preview);
            car_preview_2.Source = Utils.RetriveImage(Race2.Car.Preview);
            car_preview_3.Source = Utils.RetriveImage(Race3.Car.Preview);
            car_preview_4.Source = Utils.RetriveImage(Race4.Car.Preview);

            foreach (int i in Enum.GetValues(typeof(RaceGroup)))
            {
                cb_type_1.Items.Add((RaceGroup)i);
                cb_type_2.Items.Add((RaceGroup)i);
                cb_type_3.Items.Add((RaceGroup)i);
                cb_type_4.Items.Add((RaceGroup)i);
            }

            cb_type_1.SelectedIndex = (int)Race1.Group;
            cb_type_2.SelectedIndex = (int)Race2.Group;
            cb_type_3.SelectedIndex = (int)Race3.Group;
            cb_type_4.SelectedIndex = (int)Race4.Group;
        }



        private void cb_type_1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            ComboBox cb = (ComboBox)sender;
            Race NewRace = new Race(RaceType.Short, AvailableTracks, AvailableCars, (RaceGroup)cb.SelectedItem);

            tb_1.Text = NewRace.Description;
            img_1.Source = Utils.RetriveImage(NewRace.Track.OutlinePath);
            preview_1.Source = Utils.RetriveImage(NewRace.Track.PreviewPath);
            car_preview_1.Source = Utils.RetriveImage(NewRace.Car.Preview);
        }

        private void cb_type_2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cb = (ComboBox)sender;
            Race NewRace = new Race(RaceType.Medium, AvailableTracks, AvailableCars, (RaceGroup)cb.SelectedItem);

            tb_2.Text = NewRace.Description;
            img_2.Source = Utils.RetriveImage(NewRace.Track.OutlinePath);
            preview_2.Source = Utils.RetriveImage(NewRace.Track.PreviewPath);
            car_preview_2.Source = Utils.RetriveImage(NewRace.Car.Preview);
        }

        private void cb_type_3_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cb = (ComboBox)sender;
            Race NewRace = new Race(RaceType.Medium, AvailableTracks, AvailableCars, (RaceGroup)cb.SelectedItem);

            tb_3.Text = NewRace.Description;
            img_3.Source = Utils.RetriveImage(NewRace.Track.OutlinePath);
            preview_3.Source = Utils.RetriveImage(NewRace.Track.PreviewPath);
            car_preview_3.Source = Utils.RetriveImage(NewRace.Car.Preview);
        }

        private void cb_type_4_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cb = (ComboBox)sender;
            Race NewRace = new Race(RaceType.Medium, AvailableTracks, AvailableCars, (RaceGroup)cb.SelectedItem);

            tb_4.Text = NewRace.Description;
            img_4.Source = Utils.RetriveImage(NewRace.Track.OutlinePath);
            preview_4.Source = Utils.RetriveImage(NewRace.Track.PreviewPath);
            car_preview_4.Source = Utils.RetriveImage(NewRace.Car.Preview);
        }



    }
}
