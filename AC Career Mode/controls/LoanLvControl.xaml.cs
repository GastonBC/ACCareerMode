using DBLink.Classes;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Utilities;

namespace AC_Career_Mode.controls
{
    /// <summary>
    /// Interaction logic for LoanLvControl.xaml
    /// </summary>
    public partial class LoanLvControl : UserControl
    {


        public List<Loan> LoansLst
        {
            get { return (List<Loan>)GetValue(LoansLstProperty); }
            set { SetValue(LoansLstProperty, value); }
        }

        public static readonly DependencyProperty LoansLstProperty = DependencyProperty.Register("LoansLst", typeof(List<Loan>),
                                                                                                            typeof(LoanLvControl),
                                                                                                            new PropertyMetadata(null, new PropertyChangedCallback(OnLoansLstChanged)));

        private static void OnLoansLstChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            LoanLvControl control = (LoanLvControl)d;
            control.lv_Loans.ItemsSource = (List<Loan>)e.NewValue;
        }

        public LoanLvControl()
        {
            InitializeComponent();
        }


        public event RoutedEventHandler Loan_DoubleClick;
        private void lv_Loans_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (lv_Loans.SelectedItem != null)
            {
                Loan loan = (Loan)lv_Loans.SelectedItem;
                this.Loan_DoubleClick(loan, e);
            }
        }

        void lv_LoansHeader_Click(object sender, RoutedEventArgs e)
        {
            Utils.HeaderClickedHandler(sender, e, lv_Loans);
        }
    }
}
