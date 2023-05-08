using DBLink;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Utilities;

namespace AC_Career_Mode.controls
{
    /// <summary>
    /// Interaction logic for TracksControl.xaml
    /// </summary>
    public partial class TracksControl : UserControl
    {

        public object ButtonContent
        {
            get { return b_BuySell.Content; }
            set { b_BuySell.Content = value; }
        }

        public List<Track> TrackList
        {
            get { return (List<Track>)GetValue(TrackListProperty); }
            set { SetValue(TrackListProperty, value); }
        }

        public static readonly DependencyProperty TrackListProperty = DependencyProperty.Register("TrackListProperty", typeof(List<Track>),
                                                                                                            typeof(TracksControl),
                                                                                                            new PropertyMetadata(null, new PropertyChangedCallback(OnTrackListChanged)));

        private static void OnTrackListChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TracksControl control = (TracksControl)d;
            control.lv_Tracks.ItemsSource = (List<Track>)e.NewValue;
        }

        public TracksControl()
        {
            InitializeComponent();
        }

        private void lv_Tracks_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lv_Tracks.SelectedItem  != null)
            {
                Track track = (Track)lv_Tracks.SelectedItem;
                img_TrackOutline.Source = Utils.RetriveImage(track.OutlinePath);
                img_TrackPreview.Source = Utils.RetriveImage(track.PreviewPath);
                b_BuySell.IsEnabled = true;

                if (track.OwnerId != 0)
                {
                    b_Upgrade.Content = $"Upgrade: ${track.GetUpgradeCost()}";
                    b_Upgrade.IsEnabled = true; 
                }
            }
            else
            {
                img_TrackOutline.Source = null;
                img_TrackPreview.Source = null;
                b_BuySell.IsEnabled = false;
                b_Upgrade.Content = $"Upgrade";
                b_Upgrade.IsEnabled = false;
            }
        }

        public event RoutedEventHandler BuySell_Click;
        private void b_BuySell_Click(object sender, RoutedEventArgs e)
        {
            if (lv_Tracks.SelectedItem != null)
            {
                Track track = (Track)lv_Tracks.SelectedItem;
                this.BuySell_Click(track, e);
            }
        }

        public event RoutedEventHandler Upgrade_Click;
        private void b_Upgrade_Click(object sender, RoutedEventArgs e)
        {
            if (lv_Tracks.SelectedItem != null)
            {
                Track track = (Track)lv_Tracks.SelectedItem;
                this.Upgrade_Click(track, e);
            }
        }

        void lv_TracksHeader_Click(object sender, RoutedEventArgs e)
        {
            Utils.HeaderClickedHandler(sender, e, lv_Tracks);
        }


    }
}
