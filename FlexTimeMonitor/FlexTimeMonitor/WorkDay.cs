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
        /// Converts the ToString result to 
        /// 9:47 format.
        /// 
        /// The reason I put this in here and use all the "xxString" 
        /// methods is caused by the ItemSource format issues.
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        private String TimeSpanTotalToString(TimeSpan t)
        {
            if (t != null)
            {
                bool isNegative = t.Ticks < 0;
                String prefix = isNegative ? "-" : "";

                return prefix + t.ToString(@"hh\:mm");
            }
            return "";
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
        /// Difference between start and end time
        /// </summary>
        //public TimeSpan Difference
        //{
        //    get
        //    {
        //        if (End == null || End.TimeOfDay == TimeSpan.Zero)
        //        {
        //            return TimeSpan.Zero;
        //        }
        //        return End - Start;
        //    }
        //}

        /// <summary>
        /// The difference between Difference and the complete workday (including break period)
        /// </summary>
        public TimeSpan OverTime
        {
            get
            {
                return Elapsed - (Properties.Settings.Default.WorkPeriod + Properties.Settings.Default.BreakPeriod);
            }
        }

        /// <summary>
        /// This method is neccessary since the TimeSpan formating will
        /// remove the negative prefix (the TimeSpan still might be negative). 
        /// The String will include display the correct TimeSpan with prefix.
        /// 
        /// This is mainly a fix for the DataGrid display.
        /// </summary>
        public String OverTimeString
        {
            get { return TimeSpanTotalToString(OverTime); }
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
        /// Remaining time
        /// </summary>
        public TimeSpan Remaining
        {
            get
            {
                TimeSpan remaining = Elapsed - (Properties.Settings.Default.WorkPeriod + Properties.Settings.Default.BreakPeriod);

                return remaining > TimeSpan.Zero ? TimeSpan.Zero : remaining;
            }
        }

        /// <summary>
        /// Additional note
        /// </summary>
        public String Note { get; set; }



    }
}
