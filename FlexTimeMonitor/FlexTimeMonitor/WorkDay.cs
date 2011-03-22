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

        #region Helper

        /// <summary>
        /// Puts the TimeSpan in [-]hh:mm (hh=00-23, mm=00-59) format. Supports negative TimeSpans
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public String TimeSpanToString(TimeSpan t)
        {
            String sign = t < TimeSpan.Zero ? "-" : "";
            String hours = Math.Abs((int)t.TotalHours).ToString("00");
            String minutes = Math.Abs(t.Minutes).ToString("00");
            return String.Format("{0}{1}:{2}", sign, hours, minutes);
        }

        #endregion

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

        #region Studpid input hacks

        // Note: there is something like Dependency Property which may or may not be suitable to handle the input data. But I have no time to check that further.
        // Non the less: there has to be a way to handle the input, maybe with a direct data access (like WCF: Message Handling).

        /// <summary>
        /// Temporarily used to access Start.
        /// Will prevent a unwanted date change. You can't just simply replace "Start" 
        /// since there would be no nice way to set the date (well you could create
        /// another date property but that is even worse).
        /// </summary>
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
        [XmlIgnore]
        public DateTime EndHack { get { return End; } set { End = new DateTime(Start.Year, Start.Month, Start.Day, value.Hour, value.Minute, value.Second); } }

        // This line will prevent unwanted date changes. With this version it is possible to end on another date than start.
        //public DateTime EndHack { get { return End; } set { End = new DateTime(End.Year, End.Month, End.Day, value.Hour, value.Minute, value.Second); } }

        #endregion

        /// <summary>
        /// The difference between Difference and the complete workday (including break period)
        /// </summary>
        public TimeSpan OverTime
        {
            get { return Elapsed - (Properties.Settings.Default.WorkPeriod + Properties.Settings.Default.BreakPeriod); }
        }

        /// <summary>
        /// This is a workaround for the missing capability of TimeSpan formating to handle negative values.
        /// This property is used as a binding in the xaml code.
        /// </summary>
        public String OverTimeString { get { return TimeSpanHelper.TimeSpanToString(OverTime); } }

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
        public TimeSpan Estimated
        {
            get { return (Start + (Properties.Settings.Default.WorkPeriod + Properties.Settings.Default.BreakPeriod)).TimeOfDay; }
        }

        /// <summary>
        /// Remaining time - oppoosite of OverTime (5min Remaining == -5min OverTime)
        /// </summary>
        public TimeSpan Remaining
        {
            get { return -OverTime; }
        }

        /// <summary>
        /// Additional note
        /// </summary>
        public String Note { get; set; }
    }
}
