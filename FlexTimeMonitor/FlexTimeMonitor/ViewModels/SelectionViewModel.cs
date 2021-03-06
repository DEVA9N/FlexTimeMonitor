﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using A9N.FlexTimeMonitor.Extensions;
using A9N.FlexTimeMonitor.Properties;
using A9N.FlexTimeMonitor.Work;

namespace A9N.FlexTimeMonitor.ViewModels
{
    internal class SelectionViewModel : ViewModelBase
    {
        public int DayCount { get; set; }
        public String Overall { get; set; }
        public String Intended { get; set; }
        public String Difference { get; set; }

        public SelectionViewModel(IEnumerable<WorkDay> workDays)
        {
            var workDaysList = workDays.ToList();
            var timeOverall = CalculateOverall(workDaysList);
            var timeIntended = CalculateIntended(workDaysList);

            DayCount = workDaysList.Count;
            Overall = timeOverall.ToTotalHhmmss();
            Intended = timeIntended.ToTotalHhmmss();
            Difference = (timeOverall - timeIntended).ToTotalHhmmss();
        }

        private static TimeSpan CalculateOverall(List<WorkDay> workDays)
        {
            var overallTicks = workDays.ToList().Sum(w => w.Elapsed.Ticks - Settings.Default.BreakPeriod.Ticks);

            return new TimeSpan(overallTicks);
        }

        private static TimeSpan CalculateIntended(List<WorkDay> workDays)
        {
            var intendedTicks = Settings.Default.WorkPeriod.Ticks * workDays.Count;

            return new TimeSpan(intendedTicks);
        }
    }
}