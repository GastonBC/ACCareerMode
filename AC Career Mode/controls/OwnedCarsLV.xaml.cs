﻿using DBLink.Classes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic;
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
        public ObservableCollection<AIDriver> AIDrivers
        {
            get { return (ObservableCollection<AIDriver>)GetValue(AIDriversProperty); }
            set { SetValue(AIDriversProperty, value); }
        }

        public static readonly DependencyProperty AIDriversProperty = DependencyProperty.Register("AIDrivers", typeof(ObservableCollection<AIDriver>),
                                                                                                            typeof(OwnedCarsLV),
                                                                                                            new PropertyMetadata());

        public Player Player
        {
            get { return (Player)GetValue(PlayerProperty); }
            set { SetValue(PlayerProperty, value); }
        }

        public static readonly DependencyProperty PlayerProperty = DependencyProperty.Register("Player", typeof(Player),
                                                                                                            typeof(OwnedCarsLV),
                                                                                                            new PropertyMetadata());


        public ObservableCollection<string> Drivers
        {
            get { return (ObservableCollection<string>)GetValue(DriversProperty); }
            set { SetValue(DriversProperty, value); }
        }

        public static readonly DependencyProperty DriversProperty = DependencyProperty.Register("Drivers", typeof(ObservableCollection<string>),
                                                                                                            typeof(OwnedCarsLV),
                                                                                                            new PropertyMetadata());

        public OwnedCarsLV()
        {
            InitializeComponent();
            
        }
    }
}
