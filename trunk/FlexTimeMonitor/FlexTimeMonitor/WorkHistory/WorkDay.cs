using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Runtime.CompilerServices;

namespace A9N.FlexTimeMonitor
{
    /// <summary>
    /// Represents a work day
    /// </summary>
    public class WorkDay
    {
        /// <summary>
        /// Creates Workday instance with start time now
        /// </summary>
        public WorkDay()
        {
            Data = new WorkDayData();

            Data.Date = DateTime.Now;
            Data.Start = DateTime.Now;
            Data.End = DateTime.Now;
        }

        /// <summary>
        /// Gets or sets the data object that stores the workday data.
        /// </summary>
        /// <value>The data.</value>
        public WorkDayData Data { get; set; }

        /// <summary>
        /// Sets the time of day.
        /// </summary>
        /// <param name="date">The date.</param>
        /// <param name="time">The time.</param>
        /// <returns>DateTime.</returns>
        private DateTime SetTimeOfDay(DateTime date, TimeSpan time)
        {
            return new DateTime(date.Year, date.Month, date.Day, time.Hours, time.Minutes, time.Seconds);
        }

        /// <summary>
        /// Gets or sets the date of the workday
        /// </summary>
        /// <value>The date.</value>
        [XmlIgnore]
        public DateTime Date { get { return Data.Date; } set { Data.Date = value; } }

        /// <summary>
        /// Start time
        /// </summary>
        /// <value>The start.</value>
        [XmlIgnore]
        public TimeSpan Start { get { return Data.Start.TimeOfDay; } set { Data.Start = SetTimeOfDay(Data.Start, value); } }

        /// <summary>
        /// End time
        /// </summary>
        /// <value>The end.</value>
        [XmlIgnore]
        public TimeSpan End { get { return Data.End.TimeOfDay; } set { Data.End = SetTimeOfDay(Data.End, value); } }

        /// <summary>
        /// The difference between Difference and the complete workday (including break period)
        /// </summary>
        /// <value>The over time.</value>
        [XmlIgnore]
        public TimeSpan OverTime
        {
            get { return Elapsed - (Properties.Settings.Default.WorkPeriod + Properties.Settings.Default.BreakPeriod); }
        }

        /// <summary>
        /// This is a workaround for the missing capability of TimeSpan formating to handle negative values.
        /// This property is used as a binding in the xaml code.
        /// </summary>
        /// <value>The over time string.</value>
        [XmlIgnore]
        public String OverTimeString { get { return TimeSpanHelper.ToHhmmss(OverTime); } }

        /// <summary>
        /// Difference between start and now
        /// </summary>
        /// <value>The elapsed.</value>
        [XmlIgnore]
        public TimeSpan Elapsed
        {
            get { return End - Start; }
        }

        /// <summary>
        /// Estimated end time
        /// </summary>
        /// <value>The estimated.</value>
        [XmlIgnore]
        public TimeSpan Estimated
        {
            get { return Start + (Properties.Settings.Default.WorkPeriod + Properties.Settings.Default.BreakPeriod); }
        }

        /// <summary>
        /// Remaining time - opposite of OverTime (5min Remaining == -5min OverTime)
        /// </summary>
        /// <value>The remaining.</value>
        [XmlIgnore]
        public TimeSpan Remaining
        {
            get { return -OverTime; }
        }

        /// <summary>
        /// Additional note
        /// </summary>
        /// <value>The note.</value>
        [XmlIgnore]
        public String Note { get { return Data.Note; } set { Data.Note = value; } }
    }
}
