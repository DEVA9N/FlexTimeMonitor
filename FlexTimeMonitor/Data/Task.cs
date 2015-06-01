using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace A9N.FlexTimeMonitor.Data
{
    public class Task
    {
        public Project Project { get; set; }

        public DateTime Date { get; set; }
    
        public DateTime Start { get; set; }
        
        public DateTime End { get; set; }

        public override string ToString()
        {
            return Project.ToString();
        }
    }
}
