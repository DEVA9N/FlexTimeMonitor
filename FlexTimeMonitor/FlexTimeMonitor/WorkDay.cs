using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
        public DateTime Start { get; set; }

        /// <summary>
        /// End time 
        /// </summary>
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

        /// <summary>
        /// The difference between Difference and the complete workday (including break period)
        /// </summary>
        public TimeSpan OverTime
        {
            get
            {
                // NOTE: this is a complicated implementation - but neccessary for the propper ToString (-hh:mm:ss)
                // Until I find a propper way to display the correct format in the main window's datagrid I will stick with it...
                TimeSpan overtime = Elapsed - (Properties.Settings.Default.WorkPeriod + Properties.Settings.Default.BreakPeriod);
                return new TimeSpan(overtime.Hours, overtime.Minutes, overtime.Seconds);
            }
            // This is nice but won't work unless I get the StringFormat in xaml to display a negative time
            //get { return Elapsed - (Properties.Settings.Default.WorkPeriod + Properties.Settings.Default.BreakPeriod); }
        }

        /// <summary>
        /// Difference between start and now
        /// </summary>
        public TimeSpan Elapsed
        {
            get { return End - Start; }
        }

        /// <summary>
        /// Estimated end time
        /// </summary>
        public DateTime Estimated
        {
            get { return Start + (Properties.Settings.Default.WorkPeriod + Properties.Settings.Default.BreakPeriod); }
        }

        /// <summary>
        /// Remaining time - oppoosite of OverTime (5min Remaining == -5min OverTime)
        /// </summary>
        public TimeSpan Remaining
        {
            get { return TimeSpan.Zero - OverTime; }
        }

        /// <summary>
        /// Additional note
        /// </summary>
        public String Note { get; set; }
    }
}
