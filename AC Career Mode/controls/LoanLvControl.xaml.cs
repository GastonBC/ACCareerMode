using DBLink;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Interaction logic for LoanLvControl.xaml
    /// </summary>
    public partial class LoanLvControl : UserControl
    {


        public List<Loan> LoansLst
        {
            get { return (List<Loan>)GetValue(LoansLstProperty); }
            set { SetValue(LoansLstProperty, value); }
        }

        public static readonly DependencyProperty LoansLstProperty = DependencyProperty.Register("LoansLstProperty", typeof(List<Loan>),
                                                                                                            typeof(LoanLvControl),
                                                                                                            new PropertyMetadata(null));
        
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

    }
}
