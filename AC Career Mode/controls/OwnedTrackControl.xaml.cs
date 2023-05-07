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

namespace AC_Career_Mode.controls
{
    /// <summary>
    /// Interaction logic for OwnedTrackControl.xaml
    /// </summary>
    public partial class OwnedTrackControl : UserControl
    {
        public string name_display { get; set; }
        public OwnedTrackControl()
        {
            InitializeComponent();
            DataContext = this;
        }
    }
}
