using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace A9N.FlexTimeMonitor.Controls.HistoryTree.TreeItems
{
    class MonthViewModel : List<WorkDay>
    {
        public MonthViewModel(IEnumerable<WorkDay> days)
        {
            if (days == null)
            {
                throw new ArgumentNullException("days");
            }

            if (!days.Any())
            {
                throw new ArgumentException("days must not be empty");
            }

            this.Date = days.First().Date;

            this.AddRange(days.ToArray());
        }

        private DateTime Date { get; set; }

        public String Name { get { return CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(this.Date.Month); } }
    }
}
