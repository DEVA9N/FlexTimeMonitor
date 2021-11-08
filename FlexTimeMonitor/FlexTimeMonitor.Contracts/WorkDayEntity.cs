using System;

namespace A9N.FlexTimeMonitor.Contracts
{
    public class WorkDayEntity
    {
        public DateTime Start { get; set; }

        public DateTime End { get; set; }

        /// <summary>
        /// Gets or sets the discrepancy. Discrepancy is a positive or negative time offset that is taken into account
        /// when calculating the total workday time. For example a skipped lunch break can be set by +1h or a doctor's
        /// appointment can be set by -1h.
        /// </summary>
        /// <value>The discrepancy.</value>
        public DateTime Discrepancy { get; set; }

        public String Note { get; set; }
    }
}
