using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TimeManagerDlx
{
	public class WorkDay
	{
		public WorkDay()
		{
			Start = DateTime.Now;
		}

		public DateTime Start { get; set; }

		public DateTime End { get; set; }

		public TimeSpan Difference
		{
			get
			{
				if (End == null || End.TimeOfDay == TimeSpan.Zero)
				{
					return TimeSpan.Zero;
				}
				return End - Start;
			}
		}

		public TimeSpan Elapsed
		{
			get { return DateTime.Now - Start; }
		}

		public DateTime Estimated
		{
            get { return Start + (Properties.Settings.Default.WorkPeriod + Properties.Settings.Default.BreakPeriod); }
		}

		public TimeSpan Remaining
		{
			get 
			{
                TimeSpan remaining = Elapsed - (Properties.Settings.Default.WorkPeriod + Properties.Settings.Default.BreakPeriod);

				return remaining > TimeSpan.Zero ? TimeSpan.Zero : remaining;
			}
		}
	}
}
