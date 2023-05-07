using DBLink;
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
    /// Interaction logic for HistoryTab.xaml
    /// </summary>
    public partial class HistoryTab : UserControl
    {
        public ObservableCollection<Record> Records 
        { 
            get { return (ObservableCollection<Record>)GetValue(RecordsProperty); }
            set { SetValue(RecordsProperty, value); }
        }

        public static readonly DependencyProperty RecordsProperty = DependencyProperty.Register("Records", typeof(ObservableCollection<Record>),
                                                                                                            typeof(HistoryTab),
                                                                                                            new PropertyMetadata());

        public string Title { get; set; }

        public HistoryTab()
        {
            InitializeComponent();
            DataContext = this;
        }


    }
}
