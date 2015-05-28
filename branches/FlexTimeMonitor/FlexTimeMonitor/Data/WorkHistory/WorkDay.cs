using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using A9N.FlexTimeMonitor.Data.Tasks;

namespace A9N.FlexTimeMonitor
{
    /// <summary>
    /// Class WorkDayData is a serializable class that contains all the workday related data.
    /// </summary>
    public class WorkDay
    {
        public WorkDay()
        {
            this.Date = DateTime.Now;
            this.Start = DateTime.Now;
            this.End = DateTime.Now;
        }

        /// <summary>
        /// Gets or sets the date of the WorkDay. This date will determine the date of the workday regardless its Start
        /// or End date.
        /// </summary>
        /// <value>The date.</value>
        public DateTime Date { get; set; }

        /// <summary>
        /// Gets or sets the start of the workday.
        /// </summary>
        /// <remarks>
        /// The date component of this object will be ignored. Instead Date is used. The mere reason to use DateTime 
        /// instead of TimeSpan is that DateTime is serializable.
        /// </remarks>
        /// <value>The start.</value>
        public DateTime Start { get; set; }

        /// <summary>
        /// Gets or sets the end of the workday.
        /// </summary>
        /// <remarks>
        /// The date component of this object will be ignored. Instead Date is used. The mere reason to use DateTime 
        /// instead of TimeSpan is that DateTime is serializable.
        /// </remarks>
        /// <value>The end.</value>
        public DateTime End { get; set; }

        /// <summary>
        /// Gets or sets the note. The note can be used to store information about the workday.
        /// </summary>
        /// <value>The note.</value>
        public String Note { get; set; }

        /// <summary>
        /// Gets or sets the list of tasks for that workday.
        /// </summary>
        /// <value>The tasks.</value>
        public List<Task> Tasks { get; set; }
    }
}
