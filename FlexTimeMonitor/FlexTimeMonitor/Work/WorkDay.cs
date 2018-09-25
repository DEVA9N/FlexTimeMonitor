using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using A9N.FlexTimeMonitor.Extensions;
using A9N.FlexTimeMonitor.Properties;

namespace A9N.FlexTimeMonitor.Work
{
    /// <summary>
    /// Represents a work day
    /// </summary>
    public sealed class WorkDay : INotifyPropertyChanged
    {
        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Creates Workday instance with start time now
        /// </summary>
        public WorkDay()
        {
            Data = new WorkDayData
            {
                Date = DateTime.Now,
                Start = DateTime.Now,
                End = DateTime.Now
            };
        }

        public void UpdateEnd()
        {
            if (!IsToday)
            {
                return;
            }

            Data.End = DateTime.Now;
        }

        /// <summary>
        /// Gets or sets the data object that stores the workday data.
        /// </summary>
        /// <value>The data.</value>
        public WorkDayData Data { get; set; }

        /// <summary>
        /// Gets or sets the date of the workday
        /// </summary>
        /// <value>The date.</value>
        [XmlIgnore]
        public DateTime Date
        {
            get => Data.Date;
            set => Data.Date = value;
        }

        /// <summary>
        /// Start time
        /// </summary>
        /// <value>The start.</value>
        [XmlIgnore]
        public TimeSpan Start
        {
            get => Data.Start.TimeOfDay;
            set => Data.Start = new DateTime(Data.Start.Year, Data.Start.Month, Data.Start.Day, value.Hours, value.Minutes, value.Seconds);
        }

        /// <summary>
        /// End time
        /// </summary>
        /// <value>The end.</value>
        [XmlIgnore]
        public TimeSpan End => IsToday ? DateTime.Now.TimeOfDay : Data.End.TimeOfDay;

        /// <summary>
        /// The difference between Difference and the complete workday (including break period)
        /// </summary>
        /// <value>The over time.</value>
        [XmlIgnore]
        public TimeSpan OverTime => Elapsed - (Settings.Default.WorkPeriod + Settings.Default.BreakPeriod);

        /// <summary>
        /// Gets or sets the discrepancy. Discrepancy is a positive or negative time offset that is taken into account
        /// when calculating the total workday time. For example a skipped lunch break can be set by +1h or a doctor's
        /// appointment can be set by -1h.
        /// </summary>
        /// <value>The discrepancy.</value>
        [XmlIgnore]
        public TimeSpan Discrepancy
        {
            get => Data.Discrepancy.TimeOfDay;
            set => Data.Discrepancy = new DateTime(Date.Year, Date.Month, Date.Day, value.Hours, value.Minutes, value.Seconds);
        }

        /// <summary>
        /// Difference between start and now also considering a possible discrepancy.
        /// </summary>
        /// <value>The elapsed.</value>
        [XmlIgnore]
        public TimeSpan Elapsed => IsToday
            ? DateTime.Now.TimeOfDay - Start + Discrepancy
            : End - Start + Discrepancy;

        /// <summary>
        /// Estimated end time
        /// </summary>
        /// <value>The estimated.</value>
        [XmlIgnore]
        public TimeSpan Estimated => Start - Discrepancy + (Settings.Default.WorkPeriod + Settings.Default.BreakPeriod);

        /// <summary>
        /// Remaining time - opposite of OverTime (5min Remaining == -5min OverTime)
        /// </summary>
        /// <value>The remaining.</value>
        [XmlIgnore]
        public TimeSpan Remaining => -OverTime;

        /// <summary>
        /// Additional note
        /// </summary>
        /// <value>The note.</value>
        [XmlIgnore]
        public String Note
        {
            get => Data.Note;
            set => Data.Note = value;
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
