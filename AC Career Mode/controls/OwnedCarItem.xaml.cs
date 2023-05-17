﻿using DBLink.Classes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace AC_Career_Mode.controls
{
    /// <summary>
    /// Interaction logic for OwnedCarsControl.xaml
    /// </summary>
    public partial class OwnedCarItem : UserControl
    {
        public ObservableCollection<Driver?> Drivers
        {
            get { return (ObservableCollection<Driver?>)GetValue(DriversProperty); }
            set { SetValue(DriversProperty, value); }
        }

        public static readonly DependencyProperty DriversProperty = DependencyProperty.Register("Drivers", typeof(ObservableCollection<Driver?>),
                                                                                                            typeof(OwnedCarItem),
                                                                                                            new PropertyMetadata());

        public string CarName
        {
            get { return (string)GetValue(CarNameProperty); }
            set { SetValue(CarNameProperty, value); }
        }

        public static readonly DependencyProperty CarNameProperty = DependencyProperty.Register("CarName", typeof(string), typeof(OwnedCarItem), new PropertyMetadata(""));


        public OwnedCarItem()
        {
            InitializeComponent();

        }
    }
}