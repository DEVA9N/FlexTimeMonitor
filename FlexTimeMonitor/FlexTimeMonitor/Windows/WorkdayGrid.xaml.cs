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

namespace A9N.FlexTimeMonitor.Windows
{
    /// <summary>
    /// Interaction logic for WorkdayGrid.xaml
    /// </summary>
    public partial class WorkdayGrid : UserControl
    {
        private WorkHistory _dataSource;

        public WorkdayGrid()
        {
            InitializeComponent();
        }

        public WorkHistory ItemsSource
        {
            get => _dataSource;
            set
            {
                _dataSource = value;

                DataGridWorkDays.ItemsSource = value;

                // Set focus on last item
                if (DataGridWorkDays.Items.Count > 0)
                {
                    DataGridWorkDays.UpdateLayout();
                    DataGridWorkDays.ScrollIntoView(DataGridWorkDays.Items[DataGridWorkDays.Items.Count - 1]);
                }
            }
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            try
            {
                this.DataGridWorkDays.Items.Refresh();
            }
            catch (InvalidOperationException)
            {
                // This exception occurs if the cell of an edited item has not been left before putting this app to tray.
            }
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
        private void dataGridWorkDays_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //TimeSpan timeOverall = TimeSpan.Zero;
            //TimeSpan timeIntended = TimeSpan.Zero;

            //try
            //{
            //    foreach (Object item in dataGridWorkDays.SelectedItems)
            //    {
            //        if (item is WorkDay)
            //        {
            //            timeOverall += ((WorkDay)item).Elapsed - Settings.Default.BreakPeriod;
            //            timeIntended += Settings.Default.WorkPeriod;
            //        }
            //    }
            //}
            //catch (InvalidCastException x)
            //{
            //    System.Diagnostics.Debug.WriteLine("Unsupported new-line-select in datagridview" + x);
            //}

            //// Display results in status bar
            //statusBarDayCountValue.Text = dataGridWorkDays.SelectedItems.Count.ToString();
            //statusBarOverallValue.Text = TimeSpanExtension.ToHhmmss(timeOverall);
            //statusBarIntendedValue.Text = TimeSpanExtension.ToHhmmss(timeIntended);
            //statusBarDifferenceValue.Text = TimeSpanExtension.ToHhmmss(timeOverall - timeIntended);
        }
    }
}
