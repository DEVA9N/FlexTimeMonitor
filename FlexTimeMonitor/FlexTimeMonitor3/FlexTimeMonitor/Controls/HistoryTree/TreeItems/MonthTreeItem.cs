using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace A9N.FlexTimeMonitor.Controls.HistoryTree.TreeItems
{
    class MonthTreeItem : TreeViewItem
    {
        private const double MonthFontSize = 18;

        public IEnumerable<WorkDay> Days { get; private set; }

        public MonthTreeItem(int month, IEnumerable<WorkDay> monthDays)
        {
            if (month < 1 || month > 12)
            {
                throw new ArgumentException("month");
            }

            if (monthDays == null)
            {
                throw new ArgumentNullException("monthDays");
            }

            this.Days = monthDays;
            this.FontSize = MonthFontSize;
            this.ItemsSource = monthDays;
            this.Header = GetName(month);
        }

        public static String GetName(int month)
        {
            return CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month);
        }
   
    }
}
