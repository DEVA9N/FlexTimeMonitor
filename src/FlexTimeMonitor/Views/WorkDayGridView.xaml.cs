using System.ComponentModel;
using System.Linq;
using System.Windows.Controls;

namespace A9N.FlexTimeMonitor.Views
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

        public void SortByDateDescending()
        {
            SortDataGrid(DataGridWorkDays, 1, ListSortDirection.Descending);
        }

        public static void SortDataGrid(DataGrid dataGrid, int columnIndex = 0, ListSortDirection sortDirection = ListSortDirection.Ascending)
        {
            var column = dataGrid.Columns[columnIndex];

            // Clear current sort descriptions
            dataGrid.Items.SortDescriptions.Clear();

            // Add the new sort description
            dataGrid.Items.SortDescriptions.Add(new SortDescription(column.SortMemberPath, sortDirection));

            // Apply sort
            foreach (var col in dataGrid.Columns)
            {
                col.SortDirection = null;
            }
            column.SortDirection = sortDirection;

            // Refresh items to display sort
            dataGrid.Items.Refresh();
        }

        /// <summary>
        /// Display selection results
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="SelectionChangedEventArgs" /> instance containing the event data.</param>
        private void DataGridWorkDays_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!(DataContext is WorkDayGridViewModel viewModel))
            {
                return;
            }

            var selection = new SelectionViewModel(DataGridWorkDays.SelectedItems.OfType<WorkDayGridItemViewModel>().Select(i => i.ToEntity()));

            viewModel.Selection = selection;
            viewModel.SelectionPopupVisible = selection.DayCount > 1;
        }
    }
}
