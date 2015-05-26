using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace A9N.FlexTimeMonitor.Controls.HistoryTree.TreeItems
{
    class HistoryTreeViewModel
    {
        public HistoryTreeViewModel(IEnumerable<WorkDay> days)
        {
            if (days == null)
            {
                throw new ArgumentNullException("days");
            }

            if (!days.Any())
            {
                throw new ArgumentException("days must not be empty");
            }

            this.Days = days;
            this.Name = "History Tree";
            this.Years = GetYears(days);
        }

        private static IEnumerable<YearViewModel> GetYears(IEnumerable<WorkDay> days)
        {
            var result = (from day in days
                          group day by day.Date.Year into yearGroup
                          select new YearViewModel(yearGroup)).ToList();

            return result;
        }

        public IEnumerable<WorkDay> Days { get; set; }

        public String Name { get; private set; }

        public IEnumerable<YearViewModel> Years { get; private set; }
    }
}
