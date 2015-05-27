using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace A9N.FlexTimeMonitor.Data.Tasks
{
    public class Task
    {
        public Project Project { get; set; }

        public DateTime Date { get; set; }
    
        public DateTime Start { get; set; }
        
        public DateTime End { get; set; }
    
    }
}
