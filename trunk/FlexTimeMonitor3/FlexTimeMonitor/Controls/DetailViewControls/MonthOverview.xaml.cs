using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace A9N.FlexTimeMonitor.Controls.DetailViewControls
{
    /// <summary>
    /// Interaction logic for MonthOverview.xaml
    /// </summary>
    public partial class MonthOverview : UserControl, IDetailView
    {
        public IEnumerable<WorkDay> History
        {
            get { return this.dataGridWorkDays.ItemsSource as IEnumerable<WorkDay>; }
            set { this.dataGridWorkDays.ItemsSource = value; }
        }

        public MonthOverview()
        {
            InitializeComponent();

        }

    }
}
