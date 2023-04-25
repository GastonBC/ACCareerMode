
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

            lb_cars.ItemsSource = AvailableCars;
        }
    }
}
