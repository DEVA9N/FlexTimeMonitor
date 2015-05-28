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

            this.Days = new ObservableCollection<WorkDayViewModel>(from day in days select new WorkDayViewModel(day));
            this.Date = days.First().Date;
            this.Name = this.Date.Year.ToString();

            this.Months = GetMonths(this.Days);

            this.IsExpanded = DateTime.Now.Year == this.Date.Year;
        }

        private static IEnumerable<MonthViewModel> GetMonths(IEnumerable<WorkDayViewModel> days)
        {
            var result = (from day in days
                          group day by day.Date.Month into monthGroup
                          select new MonthViewModel(monthGroup)).ToList();

            return result;
        }

        private DateTime Date { get; set; }

        public ObservableCollection<WorkDayViewModel> Days { get; private set; }

        public IEnumerable<MonthViewModel> Months { get; private set; }

        public String Name { get; private set; }

        public bool IsExpanded { get; set; }
    }
}
