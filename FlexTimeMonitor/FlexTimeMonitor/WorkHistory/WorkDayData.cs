using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace A9N.FlexTimeMonitor
{
    /// <summary>
    /// Class WorkDayData is a serializable class that contains all the workday related data. WorkDayData separates the
    /// data from the view logic.
    /// </summary>
    public class WorkDayData
    {
        /// <summary>
        /// Gets or sets the date of the WorkDay. This date will determine the date of the workday regardless its Start
        /// or End date.
        /// </summary>
        /// <value>The date.</value>
        [XmlElement("Date")]
        public DateTime Date { get; set; }

        /// <summary>
        /// Gets or sets the start of the workday.
        /// </summary>
        /// <remarks>
        /// The date component of this object will be ignored. Instead Date is used. The mere reason to use DateTime 
        /// instead of TimeSpan is that DateTime is serializable.
        /// </remarks>
        /// <value>The start.</value>
        [XmlElement("Start")]
        public DateTime Start { get; set; }

        /// <summary>
        /// Gets or sets the end of the workday.
        /// </summary>
        /// <remarks>
        /// The date component of this object will be ignored. Instead Date is used. The mere reason to use DateTime 
        /// instead of TimeSpan is that DateTime is serializable.
        /// </remarks>
        /// <value>The end.</value>
        [XmlElement("End")]
        public DateTime End { get; set; }

        /// <summary>
        /// Gets or sets the discrepancy. Discrepancy is a positive or negative time offset that is taken into account
        /// when calculating the total workday time. For example a skipped lunch break can be set by +1h or a doctor's
        /// appointment can be set by -1h.
        /// </summary>
        /// <value>The discrepancy.</value>
        [XmlElement("Discrepancy")]
        public DateTime Discrepancy { get; set; }

        /// <summary>
        /// Gets or sets the note. The note can be used to store information about the workday.
        /// </summary>
        /// <value>The note.</value>
        [XmlElement("Note")]
        public String Note { get; set; }
    }
}
