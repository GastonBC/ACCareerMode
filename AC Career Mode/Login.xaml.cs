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
using DemoLibrary;

namespace AC_Career_Mode
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        List<Player> players = new List<Player>();
        public Login()
        {
            InitializeComponent();
            LoadPlayerList();
        }

        private void LoadPlayerList()
        {
            players = SqliteDataAccess.LoadPlayers();
            WireUpPlayerList();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Player p = new Player();
            p.Name = tb_Name.Text;
            p.Money = 10000;

            SqliteDataAccess.SavePlayer(p);

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
    }
}
