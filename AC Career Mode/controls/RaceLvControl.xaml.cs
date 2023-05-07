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

        public ObservableCollection<Race> RaceList
        {
            get { return (ObservableCollection<Race>)GetValue(RaceListProperty); }
            set { SetValue(RaceListProperty, value); }
        }

        public static readonly DependencyProperty RaceListProperty = DependencyProperty.Register("RaceListProperty", typeof(ObservableCollection<Race>),
                                                                                                            typeof(RaceLvControl),
                                                                                                            new PropertyMetadata(null));

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
            if (lv_RaceLst.SelectedItem == null)
            {
                b_GoRacing.IsEnabled = false;
                track_background.Source = null;
                track_preview.Source = null;
                car_preview.Source = null;
                return;
            }

            b_GoRacing.IsEnabled = true;
            Race SelectedRace = lv_RaceLst.SelectedItem as Race;

            track_background.Source = RetriveImage(SelectedRace.Track.PreviewPath);
            track_preview.Source = RetriveImage(SelectedRace.Track.OutlinePath);
            car_preview.Source = RetriveImage(SelectedRace.Car.Preview);
        }


        public event RoutedEventHandler GoRacing_Click;
        public void b_GoRacing_Click(object sender, RoutedEventArgs e)
        {
            if (this.GoRacing_Click != null)
            {
                Race race = lv_RaceLst.SelectedItem as Race;
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

                lv_RaceLst.ItemsSource = FilteredList;
            }
            else
            {
                lv_RaceLst.ItemsSource = RaceList;
            }
        }

        void RaceLvHeader_Click(object sender, RoutedEventArgs e)
        {
            HeaderClickedHandler(sender, e, lv_RaceLst);
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
    }
}
