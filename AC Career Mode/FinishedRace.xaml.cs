using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace AC_Career_Mode
{
    /// <summary>
    /// Interaction logic for FinishedRace.xaml
    /// </summary>
    public partial class FinishedRace : Window
    {
        public RaceResult? Result { get; set; }

        Race race_;
        internal FinishedRace(Race race)
        {
            InitializeComponent();

            race_ = race;
            label_racedescr.Content = race.Description;
            tb_laps.Text = race.Laps.ToString();
            tb_position.Text = "1";
            Result = null;
        }

        private void b_race_finish(object sender, RoutedEventArgs e)
        {
            Result = get_results();
            this.Close();
        }

        private void b_cancel_Click(object sender, RoutedEventArgs e)
        {
            Result = null;
            this.Close();
        }

        private void pos_txt_changed(object sender, TextChangedEventArgs e)
        {
            is_race_valid();
        }

        private void laps_txt_changed(object sender, TextChangedEventArgs e)
        {
            is_race_valid();
        }

        private bool is_race_valid()
        {
            if (string.IsNullOrEmpty(tb_laps.Text) || string.IsNullOrEmpty(tb_position.Text))
            {
                b_finished.IsEnabled = false;
                return false;
            }

            if (get_results == null)
            {
                return false;
            }

            b_finished.IsEnabled = true;
            return true;
        }

        private RaceResult? get_results()
        {
            int laps = 0;
            int position = 0;
            if (int.TryParse(tb_laps.Text, out laps) && int.TryParse(tb_laps.Text, out position))
            {
                if (position > 0)
                {
                    RaceResult raceResult = new(race_, position, laps);
                    return raceResult;
                }
            }
            return null;
        }
    }
}
