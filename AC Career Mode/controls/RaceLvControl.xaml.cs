using DBLink.Classes;
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

        public static readonly DependencyProperty RaceListProperty = DependencyProperty.Register("RaceList", typeof(List<Race>),
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

        public static readonly DependencyProperty CurrentUserProperty = DependencyProperty.Register("CurrentUser", typeof(Player),
                                                                                                            typeof(RaceLvControl),
                                                                                                            new PropertyMetadata(null));


        public RaceLvControl()
        {
            InitializeComponent();
        }

        private void RaceLv_SelChanged(object sender, SelectionChangedEventArgs e)
        {
            Race SelectedRace = lv_Races.SelectedItem as Race;
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

        //void lv_RacesHeader_Click(object sender, RoutedEventArgs e)
        //{
        //    Utils.HeaderClickedHandler(sender, e, lv_Races);
        //}

    }
}
