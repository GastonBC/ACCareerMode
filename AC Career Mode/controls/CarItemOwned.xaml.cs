using DBLink.Classes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Utilities;

namespace AC_Career_Mode.controls
{
    /// <summary>
    /// Interaction logic for OwnedCarsControl.xaml
    /// </summary>
    public partial class OwnedCarItem : UserControl
    {
        public Car car
        {
            get { return (Car)GetValue(CarProperty); }
            set { SetValue(CarProperty, value); }
        }

        public static readonly DependencyProperty CarProperty = DependencyProperty.Register("car",
                                                                                            typeof(Car),
                                                                                            typeof(OwnedCarItem),
                                                                                            new PropertyMetadata(null, OnCarPropertyChanged));


        public ObservableCollection<Driver?> Drivers
        {
            get { return (ObservableCollection<Driver?>)GetValue(DriversProperty); }
            set { SetValue(DriversProperty, value); }
        }

        public static readonly DependencyProperty DriversProperty = DependencyProperty.Register("Drivers", typeof(ObservableCollection<Driver?>),
                                                                                                            typeof(OwnedCarItem),
                                                                                                            new PropertyMetadata());
        public string ButtonContent
        {
            get { return (string)GetValue(ButtonContentProperty); }
            set { SetValue(ButtonContentProperty, value); }
        }

        public static readonly DependencyProperty ButtonContentProperty = DependencyProperty.Register("ButtonContent", typeof(string),
                                                                                                            typeof(OwnedCarItem),
                                                                                                            new PropertyMetadata(null, OnButtonContentPropertyChanged));



        public OwnedCarItem()
        {
            InitializeComponent();

        }

        private static void OnButtonContentPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            OwnedCarItem CarItem = (OwnedCarItem)d;
            CarItem.b_Button.Content = (string)e.NewValue;

        }

        private static void OnCarPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            OwnedCarItem CarItem = (OwnedCarItem)d;
            Car newCar = (Car)e.NewValue;

            if (newCar != null)
            {
                CarItem.lb_Car.Content = newCar.Name;
                CarItem.img_CarPreview.Source = Utils.RetriveImage(newCar.Preview);
            }
        }

        public event EventHandler ButtonClicked;
        private void b_Button_Click(object sender, RoutedEventArgs e)
        {
            ButtonClicked?.Invoke(this, EventArgs.Empty);
        }
    }
}
