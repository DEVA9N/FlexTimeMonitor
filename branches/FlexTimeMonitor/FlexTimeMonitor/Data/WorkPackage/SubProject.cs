using System;

namespace A9N.FlexTimeMonitor.Data.Tasks
{
    public class SubProject
    {
        public String Name { get; set; }

        public String Identifier { get; set; }

        public String Description { get; set; }

        public String CostCenter { get; set; }

        // Valid period from ... to

        public override string ToString()
        {
            return Name;
        }
    }
}
