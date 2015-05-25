using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace A9N.FlexTimeMonitor.Controls.HistoryTree.TreeItems
{
    class YearTreeItem : TreeViewItem
    {
        private const double YearFontSize = 20;

        public IEnumerable<WorkDay> History { get; private set; }

        private static IEnumerable<MonthTreeItem> GetMonths(IEnumerable<WorkDay> days)
        {
            List<MonthTreeItem> result = new List<MonthTreeItem>();

            var months = (from item in days
                          select item.Date.Month).Distinct();

            foreach (var month in months)
            {
                var monthDays = from day in days
                                where day.Date.Month == month
                                select day;

                var monthItem = new MonthTreeItem(month, monthDays);

                result.Add(monthItem);
            }

            return result;
        }

        public YearTreeItem(int year, IEnumerable<WorkDay> days)
        {
            if (days == null)
            {
                throw new ArgumentNullException("days");
            }

            this.History = days;
            this.FontSize = YearFontSize;
            this.ItemsSource = GetMonths(days);
            this.Header = year.ToString();
        }

    }
}
