using DBLink.Classes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace AC_Career_Mode.controls
{
    /// <summary>
    /// Interaction logic for OwnedCarsLV.xaml
    /// </summary>
    public partial class OwnedCarsLV : UserControl
    {




        public ObservableCollection<Car> Cars
        {
            get { return (ObservableCollection<Car>)GetValue(CarsProperty); }
            set { SetValue(CarsProperty, value); }
        }

        public static readonly DependencyProperty CarsProperty = DependencyProperty.Register("Cars", typeof(ObservableCollection<Car>),
                                                                                                            typeof(OwnedCarsLV),
                                                                                                            new PropertyMetadata());


        public ObservableCollection<Driver?> Drivers
        {
            get { return (ObservableCollection<Driver?>)GetValue(DriversProperty); }
            set { SetValue(DriversProperty, value); }
        }

        public static readonly DependencyProperty DriversProperty = DependencyProperty.Register("Drivers", typeof(ObservableCollection<Driver?>),
                                                                                                            typeof(OwnedCarsLV),
                                                                                                            new PropertyMetadata());

        public OwnedCarsLV()
        {
            InitializeComponent();

        }

        public void Button_Click()
        {

        }
    }
}
