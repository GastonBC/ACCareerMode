using DBLink;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Utilities;

namespace AC_Career_Mode.controls
{




    /// <summary>
    /// Interaction logic for RaceLvControl.xaml
    /// </summary>
    public partial class RaceLvControl : UserControl
    {

        public List<Race> RaceList
        {
            get { return (List<Race>)GetValue(RaceListProperty); }
            set { SetValue(RaceListProperty, value); }
        }

        public static readonly DependencyProperty RaceListProperty = DependencyProperty.Register("RaceListProperty", typeof(List<Race>),
                                                                                                            typeof(RaceLvControl),
                                                                                                            new PropertyMetadata(null, new PropertyChangedCallback(OnRaceListChanged)));

        private static void OnRaceListChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RaceLvControl control = (RaceLvControl)d;
            control.lv_Races.ItemsSource = (List<Race>)e.NewValue;

            if(control.chk_FilterRaces.IsChecked == true)
            {
                control.FilterRaces();
            }
        }


        public Player CurrentUser
        {
            get { return (Player)GetValue(CurrentUserProperty); }
            set { SetValue(CurrentUserProperty, value); }
        }

        public static readonly DependencyProperty CurrentUserProperty = DependencyProperty.Register("CurrentUserProperty", typeof(Player),
                                                                                                            typeof(RaceLvControl),
                                                                                                            new PropertyMetadata(null));


        public RaceLvControl()
        {
            InitializeComponent();
        }

        private void RaceLv_SelChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lv_Races.SelectedItem == null)
            {
                b_GoRacing.IsEnabled = false;
                img_track_bkg.Source = null;
                img_track_outline.Source = null;
                img_car_preview.Source = null;
                return;
            }

            b_GoRacing.IsEnabled = true;
            Race SelectedRace = lv_Races.SelectedItem as Race;

            img_track_bkg.Source = Utils.RetriveImage(SelectedRace.Track.PreviewPath);
            img_track_outline.Source = Utils.RetriveImage(SelectedRace.Track.OutlinePath);
            img_car_preview.Source = Utils.RetriveImage(SelectedRace.Car.Preview);
        }


        public event RoutedEventHandler GoRacing_Click;
        public void b_GoRacing_Click(object sender, RoutedEventArgs e)
        {
            if (this.GoRacing_Click != null)
            {
                Race race = lv_Races.SelectedItem as Race;
                this.GoRacing_Click(race, e);
            }
        }




        private void chk_FilterRaces_Click(object sender, RoutedEventArgs e)
        {
            FilterRaces();
        }


        private void FilterRaces()
        {
            List<Race> FilteredRaces = new();

            if (chk_FilterRaces.IsChecked == true)
            {
                List<Car> owned_cars = CurrentUser.GetPlayerCars();

                IEnumerable<string>? names = RaceList.Select(r => r.Car.Name).Intersect(owned_cars.Select(c => c.Name));
                List<Race> FilteredList = RaceList.Where(r => names.Contains(r.Car.Name)).ToList();

                lv_Races.ItemsSource = FilteredList;
            }
            else
            {
                lv_Races.ItemsSource = RaceList;
            }
        }

        void lv_RacesHeader_Click(object sender, RoutedEventArgs e)
        {
            Utils.HeaderClickedHandler(sender, e, lv_Races);
        }

    }
}
