using System;
using System.Collections.Generic;
using System.Linq;
using A9N.FlexTimeMonitor.Extensions;
using A9N.FlexTimeMonitor.Mvvm;
using A9N.FlexTimeMonitor.Properties;
using A9N.FlexTimeMonitor.Work;

namespace A9N.FlexTimeMonitor.Views
{
    internal class SelectionViewModel : ViewModel
    {
        public int DayCount { get; set; }
        public String Overall { get; set; }
        public String Intended { get; set; }
        public String Difference { get; set; }

        public SelectionViewModel(IEnumerable<WorkDayGridItemViewModel> workDays)
        {
            var workDaysList = workDays.ToList();
            var timeOverall = CalculateOverall(workDaysList);
            var timeIntended = CalculateIntended(workDaysList);

            DayCount = workDaysList.Count;
            Overall = timeOverall.ToTotalHhmmss();
            Intended = timeIntended.ToTotalHhmmss();
            Difference = (timeOverall - timeIntended).ToTotalHhmmss();
        }

        private static TimeSpan CalculateOverall(List<WorkDayGridItemViewModel> workDays)
        {
            var overallTicks = workDays.ToList().Sum(w => w.Elapsed.Ticks - Settings.Default.BreakPeriod.Ticks);

            return new TimeSpan(overallTicks);
        }

        private static TimeSpan CalculateIntended(List<WorkDayGridItemViewModel> workDays)
        {
            var intendedTicks = Settings.Default.WorkPeriod.Ticks * workDays.Count;

            return new TimeSpan(intendedTicks);
        }
    }
}