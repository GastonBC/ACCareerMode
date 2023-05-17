using DBLink.Classes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace AC_Career_Mode.controls
{
    public partial class RaceItem : UserControl
    {

        public Race race
        {
            get { return (Race)GetValue(RaceProperty); }
            set { SetValue(RaceProperty, value); }
        }

        public static readonly DependencyProperty RaceProperty = DependencyProperty.Register(
                                                                                            "race",
                                                                                            typeof(Race),
                                                                                            typeof(RaceItem),
                                                                                            new PropertyMetadata(null, OnRacePropertyChanged));

        public RaceItem()
        {
            InitializeComponent();
        }


        private static void OnRacePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RaceItem raceItem = (RaceItem)d;
            Race newRace = (Race)e.NewValue;

            if (newRace != null)
            {
                raceItem.lb_Car.Content = newRace.Car.Name;
                raceItem.lb_Track.Content = newRace.Track.Name;
                raceItem.lb_Prize.Content = newRace.Prize;
                raceItem.lb_Laps.Content = newRace.Laps + " laps";
            }
        }
    }
}
