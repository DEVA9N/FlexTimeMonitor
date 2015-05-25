using A9N.FlexTimeMonitor.Controls.HistoryTree.TreeItems;
using System;
using System.Collections.Generic;
using System.Globalization;
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

namespace A9N.FlexTimeMonitor.Controls.HistoryTree
{
    /// <summary>
    /// Interaction logic for HistoryTreeControl.xaml
    /// </summary>
    public partial class HistoryTreeControl : UserControl
    {
        private const double DayFontSize = 12;

        private List<WorkDay> _items;

        public event RoutedPropertyChangedEventHandler<object> SelectedItemChanged;

        public bool ShowDays { get; set; }

        public HistoryTreeControl()
        {
            InitializeComponent();

            treeView.SelectedItemChanged += treeView_SelectedItemChanged;
        }

        public IEnumerable<WorkDay> Items
        {
            get { return _items; }
            set
            {
                _items = value == null ? null : value.ToList();

                if (_items != null)
                {
                    PopulateTree(_items);
                }
                else
                {
                    ClearTree();
                }

            }
        }

        private void PopulateTree(List<WorkDay> history)
        {
            var years = (from item in history
                         select item.Date.Year).Distinct();

            foreach (var year in years)
            {
                var yearDays = from day in history
                               where day.Date.Year == year
                               select day;

                var yearObj = new YearTreeItem(year, yearDays);

                treeView.Items.Add(yearObj);
            }


            if (this.treeView.Items.Count > 0)
            {
                var currentYear = this.treeView.Items[this.treeView.Items.Count - 1] as TreeViewItem;

                if (currentYear != null)
                {
                    currentYear.ExpandSubtree();

                    var currentMonth = currentYear.Items.Count > 0 ? currentYear.Items[currentYear.Items.Count - 1] as TreeViewItem : null;

                    if (currentMonth != null)
                    {
                        currentMonth.Focus();
                    }
                }
            }
        }

        //private void PopulateYear(TreeView parent, YearTreeItem yearObj)
        //{
        //    var yearItem = new TreeViewItem();
        //    yearItem.FontSize = YearFontSize;
        //    yearItem.DisplayMemberPath = "Number";
        //    yearItem.ItemsSource = yearObj;


        //    parent.Items.Add(yearItem);

        //    foreach (var month in yearObj.Months)
        //    {
        //        //PopulateMonth(yearItem, month);
        //    }
        //}

        //private void PopulateMonth(TreeViewItem parent, MonthTreeItem monthObj)
        //{
        //    var monthItem = new TreeViewItem();
        //    monthItem.FontSize = MonthFontSize;
        //    monthItem.Header = monthObj.DisplayName;

        //    if (ShowDays)
        //    {
        //        monthItem.ItemsSource = monthObj.Days;
        //    }

        //    parent.Items.Add(monthItem);
        //}

        private void ClearTree()
        {
            this.treeView.Items.Clear();
        }


        void treeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (SelectedItemChanged != null)
            {
                SelectedItemChanged(this, e);
            }
        }

    }
}
