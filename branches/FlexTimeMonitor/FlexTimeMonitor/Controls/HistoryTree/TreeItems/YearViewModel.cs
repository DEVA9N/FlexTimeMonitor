using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace A9N.FlexTimeMonitor.Controls.HistoryTree.TreeItems
{
    class YearViewModel : List<WorkDay>
    {
        public YearViewModel(IEnumerable<WorkDay> days)
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

            this.Months = GetMonths(days);
        }

        private static IEnumerable<MonthViewModel> GetMonths(IEnumerable<WorkDay> days)
        {
            var result = (from day in days
                          group day by day.Date.Month into monthGroup
                          select new MonthViewModel(monthGroup)).ToList();

            return result;
        }

        private DateTime Date { get; set; }

        public IEnumerable<MonthViewModel> Months { get; set; }

        public String Name { get { return this.Date.Year.ToString(); } }

        public bool IsExpanded { get; set; }

        public bool IsSelected { get; set; }
    }
}
