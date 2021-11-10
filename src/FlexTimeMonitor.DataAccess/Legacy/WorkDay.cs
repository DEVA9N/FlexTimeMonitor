using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace A9N.FlexTimeMonitor.DataAccess.Legacy
{
    [XmlType("WorkDay")]
    public class WorkDay
    {
        public WorkDayData Data { get; set; }
    }

    [XmlType("WorkDayData")]
    public class WorkDayData
    {
        [XmlElement("Start")]
        public DateTime Start { get; set; }

        [XmlElement("End")]
        public DateTime End { get; set; }

        [XmlElement("Discrepancy")]
        public DateTime Discrepancy { get; set; }

        [XmlElement("Note")]
        public String Note { get; set; }
    }
}
