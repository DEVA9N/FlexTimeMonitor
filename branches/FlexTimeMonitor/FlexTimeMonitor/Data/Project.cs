using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace A9N.FlexTimeMonitor.Data
{
    public class Project
    {
        public Task Task { get; set; }

        public String Description { get; set; }
        public String Employer { get; set; }
        public String Identifier { get; set; }
        public String Name { get; set; }

        public override string ToString()
        {
            return  Name + Task;
        }
    }
}
