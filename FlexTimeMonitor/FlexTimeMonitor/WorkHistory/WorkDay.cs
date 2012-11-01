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
        private DateTime end;

        /// <summary>
        /// Creates Workday instance with start time now
        /// </summary>
        public WorkDay()
        {
            Start = DateTime.Now;
            End = DateTime.Now;
        }

        /// <summary>
        /// Start time
        /// </summary>
        /// <value>The start.</value>
        public DateTime Start { get; set; }

        /// <summary>
        /// End time
        /// </summary>
        /// <value>The end.</value>
        public DateTime End
        {
            get
            {
                Boolean isToday = this.Start.Date == DateTime.Now.Date;

                if (isToday)
                {
                    end = DateTime.Now;
                }

                return end;
            }
            set
            {
                end = value;
            }
        }

        #region Studpid input hacks
        // Note: there is something like Dependency Property which may or may not be suitable to handle the input data. But I have no time to check that further.
        // Non the less: there has to be a way to handle the input, maybe with a direct data access (like WCF: Message Handling).

        /// <summary>
        /// Temporarily used to access Start.
        /// Will prevent a unwanted date change. You can't just simply replace "Start"
        /// since there would be no nice way to set the date (well you could create
        /// another date property but that is even worse).
        /// </summary>
        /// <value>The start hack.</value>
        [XmlIgnore]
        public DateTime StartHack { get { return Start; } set { Start = new DateTime(Start.Year, Start.Month, Start.Day, value.Hour, value.Minute, value.Second); } }

        /// <summary>
        /// Temporarily used to access End. End now always ends on the start day.
        /// This hack addresses a GUI issue. When editing a cell the GUI always
        /// provides the current date for the entered time. There is no way to
        /// set the date manually (may change in the future)
        /// Will prevent a unwanted date change. You can't just simply replace "End"
        /// since there would be no nice way to set the date (well you could create
        /// another date property but that is even worse).
        /// </summary>
        /// <value>The end hack.</value>
        [XmlIgnore]
        public DateTime EndHack { get { return End; } set { End = new DateTime(Start.Year, Start.Month, Start.Day, value.Hour, value.Minute, value.Second); } }
        #endregion

        /// <summary>
        /// The difference between Difference and the complete workday (including break period)
        /// </summary>
        /// <value>The over time.</value>
        public TimeSpan OverTime
        {
            get { return Elapsed - (Properties.Settings.Default.WorkPeriod + Properties.Settings.Default.BreakPeriod); }
        }

        /// <summary>
        /// This is a workaround for the missing capability of TimeSpan formating to handle negative values.
        /// This property is used as a binding in the xaml code.
        /// </summary>
        /// <value>The over time string.</value>
        public String OverTimeString { get { return TimeSpanHelper.ToHhmmss(OverTime); } }

        /// <summary>
        /// Difference between start and now
        /// </summary>
        /// <value>The elapsed.</value>
        public TimeSpan Elapsed
        {
            get { return End - Start; }
        }

        /// <summary>
        /// Estimated end time
        /// </summary>
        /// <value>The estimated.</value>
        public TimeSpan Estimated
        {
            get { return (Start + (Properties.Settings.Default.WorkPeriod + Properties.Settings.Default.BreakPeriod)).TimeOfDay; }
        }

        /// <summary>
        /// Remaining time - opposite of OverTime (5min Remaining == -5min OverTime)
        /// </summary>
        /// <value>The remaining.</value>
        public TimeSpan Remaining
        {
            get { return -OverTime; }
        }

        /// <summary>
        /// Additional note
        /// </summary>
        /// <value>The note.</value>
        public String Note { get; set; }
    }
}
