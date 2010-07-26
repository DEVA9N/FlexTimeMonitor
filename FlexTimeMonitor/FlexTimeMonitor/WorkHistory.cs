using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using A9N.FlexTimeMonitor.Properties;

namespace A9N.FlexTimeMonitor
{
    public class WorkHistory : ObservableCollection<WorkDay>
    {
        /// <summary>
        /// Gets the history entry of today. If none is found it
        /// will return a fresh new day.
        /// </summary>
        /// <returns></returns>
        public WorkDay GetToday()
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
