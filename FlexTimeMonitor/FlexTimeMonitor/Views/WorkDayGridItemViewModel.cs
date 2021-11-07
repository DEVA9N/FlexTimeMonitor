using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using A9N.FlexTimeMonitor.Contracts;
using A9N.FlexTimeMonitor.Extensions;
using A9N.FlexTimeMonitor.Mvvm;
using A9N.FlexTimeMonitor.Properties;

namespace A9N.FlexTimeMonitor.Work
{
    internal sealed class WorkDayGridItemViewModel : ViewModel
    {
        private readonly WorkDayData _data;

        public WorkDayGridItemViewModel(WorkDayData data)
        {
            _data = data ?? throw new ArgumentNullException(nameof(data));
        }

        public DateTime Date
        {
            get => _data.Date;
            set => _data.Date = value;
        }

        public TimeSpan Start
        {
            get => _data.Start.TimeOfDay;
            set => _data.Start = value.ToDateTime(_data.Start);
        }

        public TimeSpan End
        {
            get => _data.End.TimeOfDay;
            set => _data.End = value.ToDateTime(_data.End);
        }

        /// <summary>
        /// The difference between Difference and the complete workday (including break period)
        /// </summary>
        /// <value>The over time.</value>
        public TimeSpan OverTime => Elapsed - (Settings.Default.WorkPeriod + Settings.Default.BreakPeriod);

        /// <summary>
        /// Gets or sets the discrepancy. Discrepancy is a positive or negative time offset that is taken into account
        /// when calculating the total workday time. For example a skipped lunch break can be set by +1h or a doctor's
        /// appointment can be set by -1h.
        /// </summary>
        /// <value>The discrepancy.</value>
        public TimeSpan Discrepancy
        {
            get => _data.Discrepancy.TimeOfDay;
            set => _data.Discrepancy = value.ToDateTime(Date);
        }

        /// <summary>
        /// Difference between start and now also considering a possible discrepancy.
        /// </summary>
        /// <value>The elapsed.</value>
        public TimeSpan Elapsed => IsToday
            ? DateTime.Now.TimeOfDay - Start + Discrepancy
            : End - Start + Discrepancy;

        /// <summary>
        /// Estimated end time
        /// </summary>
        /// <value>The estimated.</value>
        public TimeSpan Estimated => Start - Discrepancy + (Settings.Default.WorkPeriod + Settings.Default.BreakPeriod);

        /// <summary>
        /// Remaining time - opposite of OverTime (5min Remaining == -5min OverTime)
        /// </summary>
        /// <value>The remaining.</value>
        public TimeSpan Remaining => -OverTime;

        /// <summary>
        /// Additional note
        /// </summary>
        /// <value>The note.</value>
        public String Note
        {
            get => _data.Note;
            set => _data.Note = value;
        }

        /*
         * Instead of using enhanced WPF patterns to achieve certain benefits I took the fast approach and am using some
         * helper properties instead. These helper methods are used in the datagrid for trigger and display purposes.
         * 
         */

        /// <summary>
        /// Gets a value indicating whether this instance is odd week.
        /// </summary>
        /// <remarks>
        /// This value is used to highlight the odd weeks in the datagrid.
        /// </remarks>
        /// <value><c>true</c> if this instance is odd week; otherwise, <c>false</c>.</value>
        [XmlIgnore]
        public bool IsOddWeek
        {
            get
            {
                var calendar = System.Globalization.DateTimeFormatInfo.CurrentInfo?.Calendar;
                var weekNumber = calendar?.GetWeekOfYear(Date, System.Globalization.CalendarWeekRule.FirstDay, DayOfWeek.Monday) ?? 0;

                if (weekNumber > 0)
                {
                    return weekNumber % 2 > 0;
                }

                return true;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is today.
        /// </summary>
        /// <remarks>
        /// This value is used to highlight today in the datagrid.
        /// </remarks>
        /// <value><c>true</c> if this instance is today; otherwise, <c>false</c>.</value>
        [XmlIgnore]
        public bool IsToday => Date.Date == DateTime.Now.Date;

        /// <summary>
        /// Gets a value indicating whether this instance has discrepancy which is either Discrepancy != Zero or negative OverTim.
        /// </summary>
        /// <value><c>true</c> if this instance has discrepancy; otherwise, <c>false</c>.</value>
        [XmlIgnore]
        public bool HasNegativeOvertime => OverTime < TimeSpan.Zero && !IsToday;

        /// <summary>
        /// This is a workaround for the missing capability of TimeSpan formating to handle negative values.
        /// This property is used as a binding in the xaml code.
        /// </summary>
        /// <value>The over time string.</value>
        [XmlIgnore]
        public String OverTimeString => TimeSpanExtension.ToHhmmss(OverTime);

    }
}
