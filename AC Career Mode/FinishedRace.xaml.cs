using System.Windows;
using System.Windows.Controls;

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

        private void b_RaceFinish_Click(object sender, RoutedEventArgs e)
        {
            Result = GetRaceResults();

            if (Result != null)
            {
                string msg = $"Race finished!\n" +
                    $"You finished {Result.Position} and earned {Result.PrizeAwarded}";

                MessageBox.Show(msg);
            }
            this.Close();
        }

        private void b_RaceCancel_Click(object sender, RoutedEventArgs e)
        {
            Result = null;
            this.Close();
        }

        private void tb_Pos_Changed(object sender, TextChangedEventArgs e)
        {
            IsRaceValid();
        }

        private void tb_Laps_Changed(object sender, TextChangedEventArgs e)
        {
            IsRaceValid();
        }

        private bool IsRaceValid()
        {
            b_finished.IsEnabled = false;
            if (string.IsNullOrEmpty(tb_laps.Text) || string.IsNullOrEmpty(tb_position.Text))
            {
                return false;
            }

            // laps or position is a negative number
            if (!uint.TryParse(tb_laps.Text, out _) || !uint.TryParse(tb_position.Text, out _))
            {
                return false;
            }

            if (GetRaceResults == null)
            {
                return false;
            }

            b_finished.IsEnabled = true;
            return true;
        }

        private RaceResult? GetRaceResults()
        {
            int laps;
            int position;
            if (int.TryParse(tb_laps.Text, out laps) && int.TryParse(tb_position.Text, out position))
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
