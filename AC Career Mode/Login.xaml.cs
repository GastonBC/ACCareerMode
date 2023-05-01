using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using DBLink;

namespace AC_Career_Mode
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        List<Player> players = new();
        public Login()
        {
            InitializeComponent();
            LoadPlayerList();
        }

        private void LoadPlayerList()
        {
            players = Player.LoadAllPlayers();
            WireUpPlayerList();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Player p = new();
            p.Name = tb_Name.Text;

            p = Player.Insert(p);

            Record.RecordRegister(p);

            tb_Name.Text = "";

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            LoadPlayerList();
        }

        private void WireUpPlayerList()
        {
            lb_Players.ItemsSource = null;
            lb_Players.ItemsSource = players;
            lb_Players.DisplayMemberPath = "Name";

        }

        private void double_click_player(object sender, MouseButtonEventArgs e)
        {
            if (lb_Players.SelectedItem != null)
            {
                Player profile = lb_Players.SelectedItem as Player;
                MainWindow wn = new(profile);
                this.Close();

                wn.Show();
            }
        }
    }
}
