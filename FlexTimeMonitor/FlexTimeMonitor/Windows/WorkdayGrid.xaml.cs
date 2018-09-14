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
using A9N.FlexTimeMonitor.Properties;
using A9N.FlexTimeMonitor.ViewModels;
using A9N.FlexTimeMonitor.Work;

namespace A9N.FlexTimeMonitor.Windows
{
    /// <summary>
    /// Interaction logic for WorkdayGrid.xaml
    /// </summary>
    public partial class WorkdayGrid : UserControl
    {
        public WorkdayGrid()
        {
            InitializeComponent();
        }

        public void CommitChanges()
        {
            DataGridWorkDays.CommitEdit(DataGridEditingUnit.Row, true);
        }

        /// <summary>
        /// Display selection results
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="SelectionChangedEventArgs" /> instance containing the event data.</param>
        private void DataGridWorkDays_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!(DataContext is MainViewModel mainViewModel))
            {
                return;
            }

            var selection = new SelectionViewModel(DataGridWorkDays.SelectedItems.OfType<WorkDay>());

            mainViewModel.Selection = selection;
            mainViewModel.SelectionPopupVisible = selection.DayCount > 1;
        }
    }
}
