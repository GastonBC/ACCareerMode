using DBLink;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Utilities;

namespace AC_Career_Mode.controls
{
    /// <summary>
    /// Interaction logic for CarsControl.xaml
    /// </summary>
    public partial class CarsControl : UserControl
    {
        public object ButtonContent
        {
            get { return b_BuySell.Content; }
            set { b_BuySell.Content = value; }
        }

        public List<Car> CarList
        {
            get { return (List<Car>)GetValue(CarListProperty); }
            set { SetValue(CarListProperty, value); }
        }

        public static readonly DependencyProperty CarListProperty = DependencyProperty.Register("CarListProperty", typeof(List<Car>),
                                                                                                            typeof(CarsControl),
                                                                                                            new PropertyMetadata(null, new PropertyChangedCallback(OnCarListChanged)));

        private static void OnCarListChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CarsControl control = (CarsControl)d;
            control.lv_CarMarket.ItemsSource = (List<Car>)e.NewValue;
        }


        public CarsControl()
        {
            InitializeComponent();
        }

        public event RoutedEventHandler BuySell_Click;
        private void b_BuySell_Click(object sender, RoutedEventArgs e)
        {
            if (lv_CarMarket.SelectedItem != null)
            {
                Car car = (Car)lv_CarMarket.SelectedItem;
                this.BuySell_Click(car, e);
            }
        }

        private void MarketLv_SelChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lv_CarMarket.SelectedItem == null)
            {
                b_BuySell.IsEnabled = false;
                img_Car.Source = null;
            }
            else
            {
                b_BuySell.IsEnabled = true;
                Car car = lv_CarMarket.SelectedItem as Car;
                img_Car.Source = Utils.RetriveImage(car.Preview);
            }
        }

        public event RoutedEventHandler ListItem_DoubleClick;
        private void lv_CarMarket_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (lv_CarMarket.SelectedItem != null)
            {
                Car car = (Car)lv_CarMarket.SelectedItem;
                this.ListItem_DoubleClick(car, e);
            }
        }

        void lv_CarsHeader_Click(object sender, RoutedEventArgs e)
        {
            Utils.HeaderClickedHandler(sender, e, lv_CarMarket);
        }
    }
}
