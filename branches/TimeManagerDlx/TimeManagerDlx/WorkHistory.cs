using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TimeManagerDlx
{
	public class WorkHistory : List<WorkDay>
	{
		/// <summary>
		/// Return current work day
		/// </summary>
		public WorkDay Today
		{
			get 
			{
				if (this.Count > 0)
				{
					WorkDay lastEntry = this.Last();

					if (lastEntry != null)
					{
						// Check if last entry is from today
						if (lastEntry.Start.Date == DateTime.Now.Date)
						{
							lastEntry.End = default(DateTime);
							// This one is already in the list
							return lastEntry;
						}
					}
				}

				// If a new day or no history or so...
				WorkDay aNewDay = new WorkDay();
				this.Add(aNewDay);
				return aNewDay;
			}
		}

	}
}
