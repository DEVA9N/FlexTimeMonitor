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
    /// Interaction logic for YearOverview.xaml
    /// </summary>
    public partial class YearOverview : UserControl, IDetailView
    {
        private List<WorkDay> _items;

        public IEnumerable<WorkDay> History
        {
            get { return _items; }
            set
            {
                _items = value != null ? value.ToList() : null;

                if (_items != null)
                {
                    this.dataGridWorkDays.ItemsSource = _items;
                }
                else
                {
                    this.dataGridWorkDays.ItemsSource = null;
                }
            }
        }

        public YearOverview()
        {
            InitializeComponent();

            this.AllowDrop = true;

        }

        //#region DataGrid Events
        ///// <summary>
        ///// Display selection results
        ///// </summary>
        ///// <param name="sender">The source of the event.</param>
        ///// <param name="e">The <see cref="SelectionChangedEventArgs" /> instance containing the event data.</param>
        //private void dataGridWorkDays_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    TimeSpan timeOverall = TimeSpan.Zero;
        //    TimeSpan timeIntended = TimeSpan.Zero;

        //    try
        //    {
        //        foreach (Object item in dataGridWorkDays.SelectedItems)
        //        {
        //            if (item is WorkDay)
        //            {
        //                timeOverall += ((WorkDay)item).Elapsed - Settings.Default.BreakPeriod;
        //                timeIntended += Settings.Default.WorkPeriod;
        //            }
        //        }
        //    }
        //    catch (InvalidCastException x)
        //    {
        //        System.Diagnostics.Debug.WriteLine("Unsupported new-line-select in datagridview" + x);
        //    }

        //    // Display results in status bar
        //    statusBarDayCountValue.Text = dataGridWorkDays.SelectedItems.Count.ToString();
        //    statusBarOverallValue.Text = TimeSpanHelper.ToHhmmss(timeOverall);
        //    statusBarIntendedValue.Text = TimeSpanHelper.ToHhmmss(timeIntended);
        //    statusBarDifferenceValue.Text = TimeSpanHelper.ToHhmmss(timeOverall - timeIntended);
        //}
        //#endregion
    }
}
