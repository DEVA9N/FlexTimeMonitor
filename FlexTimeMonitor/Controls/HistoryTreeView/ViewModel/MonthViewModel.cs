using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using A9N.FlexTimeMonitor.Data;

namespace A9N.FlexTimeMonitor.Controls.HistoryTree.TreeItems
{
    class MonthViewModel : List<WorkDay>
    {
        public MonthViewModel(IEnumerable<WorkDayViewModel> days)
        {
            if (days == null)
            {
                throw new ArgumentNullException("days");
            }

            if (!days.Any())
            {
                throw new ArgumentException("days must not be empty");
            }

            this.Days = new ObservableCollection<WorkDayViewModel>(days);
            this.Date = days.First().Date;
            this.Name = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(this.Date.Month);

            this.IsSelected = DateTime.Now.Year == this.Date.Year && DateTime.Now.Month == this.Date.Month;
        }

        private DateTime Date { get; set; }

        public ObservableCollection<WorkDayViewModel> Days { get; private set; }

        public String Name { get; private set; }

        public bool IsSelected { get; set; }

    }
}
